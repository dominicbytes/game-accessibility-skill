// SPDX-License-Identifier: MIT
// game-accessibility skill: A11ySettings store. NOT COMPILED by the author; please report compile errors.

#include "A11ySettingsSubsystem.h"

#include "Dom/JsonObject.h"
#include "Dom/JsonValue.h"
#include "Engine/Engine.h"
#include "Engine/GameInstance.h"
#include "Engine/World.h"
#include "HAL/FileManager.h"
#include "Misc/FileHelper.h"
#include "Misc/Paths.h"
#include "Policies/PrettyJsonPrintPolicy.h"
#include "Serialization/JsonReader.h"
#include "Serialization/JsonSerializer.h"
#include "Serialization/JsonWriter.h"

namespace A11ySettingsPrivate
{
	/**
	 * OS preference -> key -> value used while the preference is ON. While it is OFF, non-explicit keys
	 * return to their registry default. Keys missing from the game's registry are skipped.
	 * Edit this table to match your game's design; it is a recommendation, not an engine rule.
	 */
	struct FBoolPrefMapping
	{
		const TCHAR* Pref;
		const TCHAR* Key;
		const TCHAR* ValueWhenOn;
	};

	static const FBoolPrefMapping BoolPrefMappings[] = {
		{ TEXT("reduce_motion"), TEXT("motion.reduce_motion"), TEXT("true") },
		{ TEXT("reduce_motion"), TEXT("motion.camera_shake"), TEXT("0") },
		{ TEXT("reduce_motion"), TEXT("motion.head_bob"), TEXT("0") },
		{ TEXT("reduce_motion"), TEXT("motion.weapon_sway"), TEXT("0") },
		{ TEXT("reduce_motion"), TEXT("motion.motion_blur"), TEXT("false") },
		{ TEXT("reduce_motion"), TEXT("motion.reduce_flashing"), TEXT("true") },
		{ TEXT("screen_reader_active"), TEXT("narration.enabled"), TEXT("true") },
		{ TEXT("increase_contrast"), TEXT("visual.high_contrast"), TEXT("true") },
		{ TEXT("captions_enabled"), TEXT("subtitles.dialogue"), TEXT("true") },
		{ TEXT("captions_enabled"), TEXT("subtitles.sound_captions"), TEXT("true") },
		{ TEXT("mono_audio"), TEXT("audio.mono"), TEXT("true") },
	};

	static bool ParseType(const FString& In, EA11ySettingType& Out)
	{
		if (In == TEXT("bool")) { Out = EA11ySettingType::Bool; return true; }
		if (In == TEXT("float")) { Out = EA11ySettingType::Float; return true; }
		if (In == TEXT("int")) { Out = EA11ySettingType::Int; return true; }
		if (In == TEXT("enum")) { Out = EA11ySettingType::Enum; return true; }
		if (In == TEXT("string")) { Out = EA11ySettingType::String; return true; }
		return false;
	}
}

UA11ySettingsSubsystem* UA11ySettingsSubsystem::Get(const UObject* WorldContextObject)
{
	if (!WorldContextObject || !GEngine)
	{
		return nullptr;
	}
	const UWorld* World = GEngine->GetWorldFromContextObject(WorldContextObject, EGetWorldErrorMode::ReturnNull);
	const UGameInstance* GameInstance = World ? World->GetGameInstance() : nullptr;
	return GameInstance ? GameInstance->GetSubsystem<UA11ySettingsSubsystem>() : nullptr;
}

FString UA11ySettingsSubsystem::GetRegistryPath()
{
	return FPaths::ProjectConfigDir() / TEXT("Accessibility/settings_registry.json");
}

FString UA11ySettingsSubsystem::GetSaveFilePath()
{
	// Desktop path. Consoles and some mobile targets require the platform save API instead; see GUIDE.md.
	return FPaths::ProjectSavedDir() / TEXT("Accessibility.json");
}

void UA11ySettingsSubsystem::Initialize(FSubsystemCollectionBase& Collection)
{
	Super::Initialize(Collection);
	LoadRegistry();
	LoadInternal(/*bBroadcast*/ false);
	QueryPlatformPreferences();
}

void UA11ySettingsSubsystem::Deinitialize()
{
	if (bDirty)
	{
		Save();
	}
	Super::Deinitialize();
}

// ------------------------------------------------------------------------------------------------
// Registry
// ------------------------------------------------------------------------------------------------

bool UA11ySettingsSubsystem::LoadRegistry()
{
	Definitions.Reset();
	KeyOrder.Reset();
	Presets.Reset();

	const FString Path = GetRegistryPath();
	FString Text;
	if (!FFileHelper::LoadFileToString(Text, *Path))
	{
		UE_LOG(LogA11y, Error, TEXT("Accessibility registry not found at %s. Copy engines/unreal/Resources/settings_registry.json there."), *Path);
		return false;
	}

	TSharedPtr<FJsonObject> Root;
	const TSharedRef<TJsonReader<>> Reader = TJsonReaderFactory<>::Create(Text);
	if (!FJsonSerializer::Deserialize(Reader, Root) || !Root.IsValid())
	{
		UE_LOG(LogA11y, Error, TEXT("Accessibility registry at %s is not valid JSON."), *Path);
		return false;
	}

	const TArray<TSharedPtr<FJsonValue>>* SettingsArray = nullptr;
	if (Root->TryGetArrayField(TEXT("settings"), SettingsArray) && SettingsArray)
	{
		for (const TSharedPtr<FJsonValue>& Entry : *SettingsArray)
		{
			const TSharedPtr<FJsonObject>* ObjPtr = nullptr;
			if (!Entry.IsValid() || !Entry->TryGetObject(ObjPtr) || !ObjPtr || !ObjPtr->IsValid())
			{
				continue;
			}
			const FJsonObject& Obj = **ObjPtr;

			FA11ySettingDefinition Def;
			FString IdString, CategoryString, TypeString, ScopeString;
			if (!Obj.TryGetStringField(TEXT("id"), IdString) || !Obj.TryGetStringField(TEXT("type"), TypeString)
				|| !A11ySettingsPrivate::ParseType(TypeString, Def.Type))
			{
				UE_LOG(LogA11y, Warning, TEXT("Registry entry skipped: missing id or unknown type."));
				continue;
			}
			Def.Id = FName(*IdString);
			Obj.TryGetStringField(TEXT("category"), CategoryString);
			Def.Category = FName(*CategoryString);
			Obj.TryGetStringField(TEXT("scope"), ScopeString);
			Def.Scope = FName(*ScopeString);
			Obj.TryGetStringField(TEXT("label"), Def.Label);
			Obj.TryGetStringField(TEXT("description"), Def.Description);
			Obj.TryGetStringField(TEXT("platform_pref"), Def.PlatformPref);

			double Number = 0.0;
			if (Obj.TryGetNumberField(TEXT("min"), Number)) { Def.Min = static_cast<float>(Number); }
			if (Obj.TryGetNumberField(TEXT("max"), Number)) { Def.Max = static_cast<float>(Number); }
			if (Obj.TryGetNumberField(TEXT("step"), Number)) { Def.Step = static_cast<float>(Number); }

			const TArray<TSharedPtr<FJsonValue>>* EnumValues = nullptr;
			if (Obj.TryGetArrayField(TEXT("values"), EnumValues) && EnumValues)
			{
				for (const TSharedPtr<FJsonValue>& V : *EnumValues)
				{
					FString S;
					if (V.IsValid() && V->TryGetString(S))
					{
						Def.Values.Add(S);
					}
				}
			}

			FA11yStoredValue Parsed;
			bool bClamped = false;
			if (!JsonToCanonical(Obj.TryGetField(TEXT("default")), Def.DefaultValue)
				|| !ParseValue(Def, Def.DefaultValue, Parsed, bClamped) || bClamped)
			{
				UE_LOG(LogA11y, Warning, TEXT("Registry entry %s skipped: default is missing or illegal."), *IdString);
				continue;
			}

			const FName NewId = Def.Id;
			if (Definitions.Contains(NewId))
			{
				UE_LOG(LogA11y, Warning, TEXT("Registry entry %s is duplicated; keeping the first."), *IdString);
				continue;
			}
			KeyOrder.Add(NewId);
			Definitions.Add(NewId, MoveTemp(Def));
		}
	}

	const TArray<TSharedPtr<FJsonValue>>* PresetArray = nullptr;
	if (Root->TryGetArrayField(TEXT("presets"), PresetArray) && PresetArray)
	{
		for (const TSharedPtr<FJsonValue>& Entry : *PresetArray)
		{
			const TSharedPtr<FJsonObject>* ObjPtr = nullptr;
			if (!Entry.IsValid() || !Entry->TryGetObject(ObjPtr) || !ObjPtr || !ObjPtr->IsValid())
			{
				continue;
			}
			FA11yPresetDefinition Preset;
			FString IdString;
			(*ObjPtr)->TryGetStringField(TEXT("id"), IdString);
			(*ObjPtr)->TryGetStringField(TEXT("label"), Preset.Label);
			Preset.Id = FName(*IdString);

			const TSharedPtr<FJsonObject>* ChangesPtr = nullptr;
			if ((*ObjPtr)->TryGetObjectField(TEXT("changes"), ChangesPtr) && ChangesPtr && ChangesPtr->IsValid())
			{
				for (const TPair<FString, TSharedPtr<FJsonValue>>& Pair : (*ChangesPtr)->Values)
				{
					FString Canonical;
					if (JsonToCanonical(Pair.Value, Canonical))
					{
						Preset.Changes.Add(FName(*Pair.Key), Canonical);
					}
				}
			}
			Presets.Add(MoveTemp(Preset));
		}
	}

	UE_LOG(LogA11y, Log, TEXT("Accessibility registry: %d settings, %d presets."), KeyOrder.Num(), Presets.Num());
	return KeyOrder.Num() > 0;
}

// ------------------------------------------------------------------------------------------------
// Parsing and validation
// ------------------------------------------------------------------------------------------------

bool UA11ySettingsSubsystem::JsonToCanonical(const TSharedPtr<FJsonValue>& JsonValue, FString& Out)
{
	if (!JsonValue.IsValid())
	{
		return false;
	}
	switch (JsonValue->Type)
	{
	case EJson::Boolean:
		Out = JsonValue->AsBool() ? TEXT("true") : TEXT("false");
		return true;
	case EJson::Number:
		Out = FString::SanitizeFloat(JsonValue->AsNumber());
		return true;
	case EJson::String:
		Out = JsonValue->AsString();
		return true;
	default:
		return false;
	}
}

bool UA11ySettingsSubsystem::ParseValue(const FA11ySettingDefinition& Def, const FString& Raw, FA11yStoredValue& Out, bool& bOutClamped) const
{
	bOutClamped = false;
	const FString Trimmed = Raw.TrimStartAndEnd();
	switch (Def.Type)
	{
	case EA11ySettingType::Bool:
		if (Trimmed.Equals(TEXT("true"), ESearchCase::IgnoreCase) || Trimmed == TEXT("1")) { Out.bBool = true; return true; }
		if (Trimmed.Equals(TEXT("false"), ESearchCase::IgnoreCase) || Trimmed == TEXT("0")) { Out.bBool = false; return true; }
		return false;

	case EA11ySettingType::Float:
	{
		if (!Trimmed.IsNumeric()) { return false; }
		const float Parsed = FCString::Atof(*Trimmed);
		Out.Float = FMath::Clamp(Parsed, Def.Min, Def.Max);
		bOutClamped = !FMath::IsNearlyEqual(Parsed, Out.Float);
		return true;
	}

	case EA11ySettingType::Int:
	{
		if (!Trimmed.IsNumeric()) { return false; }
		const int32 Parsed = FMath::RoundToInt(FCString::Atof(*Trimmed));
		Out.Int = FMath::Clamp(Parsed, FMath::RoundToInt(Def.Min), FMath::RoundToInt(Def.Max));
		bOutClamped = Parsed != Out.Int;
		return true;
	}

	case EA11ySettingType::Enum:
		if (!Def.Values.Contains(Trimmed)) { return false; }
		Out.Str = Trimmed;
		return true;

	case EA11ySettingType::String:
		// Format is game-defined (visual.custom_colors, hud.hidden_elements). Consumers parse defensively.
		if (Raw.Len() > MaxStringLength) { return false; }
		Out.Str = Raw;
		return true;
	}
	return false;
}

FString UA11ySettingsSubsystem::ToCanonical(const FA11ySettingDefinition& Def, const FA11yStoredValue& Value) const
{
	switch (Def.Type)
	{
	case EA11ySettingType::Bool: return Value.bBool ? TEXT("true") : TEXT("false");
	case EA11ySettingType::Float: return FString::SanitizeFloat(Value.Float);
	case EA11ySettingType::Int: return FString::FromInt(Value.Int);
	case EA11ySettingType::Enum: return Value.Str;
	case EA11ySettingType::String: return Value.Str;
	}
	return FString();
}

bool UA11ySettingsSubsystem::ValuesEqual(EA11ySettingType Type, const FA11yStoredValue& A, const FA11yStoredValue& B)
{
	switch (Type)
	{
	case EA11ySettingType::Bool: return A.bBool == B.bBool;
	case EA11ySettingType::Float: return FMath::IsNearlyEqual(A.Float, B.Float, 1.e-4f);
	case EA11ySettingType::Int: return A.Int == B.Int;
	case EA11ySettingType::Enum: return A.Str == B.Str;
	case EA11ySettingType::String: return A.Str.Equals(B.Str, ESearchCase::CaseSensitive);
	}
	return false;
}

// ------------------------------------------------------------------------------------------------
// Queries
// ------------------------------------------------------------------------------------------------

bool UA11ySettingsSubsystem::HasKey(FName Key) const
{
	return Definitions.Contains(Key);
}

bool UA11ySettingsSubsystem::GetBool(FName Key, bool bFallback) const
{
	const FA11ySettingDefinition* Def = Definitions.Find(Key);
	const FA11yStoredValue* V = Values.Find(Key);
	return (Def && V && Def->Type == EA11ySettingType::Bool) ? V->bBool : bFallback;
}

float UA11ySettingsSubsystem::GetFloat(FName Key, float Fallback) const
{
	const FA11ySettingDefinition* Def = Definitions.Find(Key);
	const FA11yStoredValue* V = Values.Find(Key);
	if (!Def || !V) { return Fallback; }
	if (Def->Type == EA11ySettingType::Float) { return V->Float; }
	if (Def->Type == EA11ySettingType::Int) { return static_cast<float>(V->Int); }
	return Fallback;
}

int32 UA11ySettingsSubsystem::GetInt(FName Key, int32 Fallback) const
{
	const FA11ySettingDefinition* Def = Definitions.Find(Key);
	const FA11yStoredValue* V = Values.Find(Key);
	return (Def && V && Def->Type == EA11ySettingType::Int) ? V->Int : Fallback;
}

FString UA11ySettingsSubsystem::GetEnum(FName Key, FString Fallback) const
{
	const FA11ySettingDefinition* Def = Definitions.Find(Key);
	const FA11yStoredValue* V = Values.Find(Key);
	return (Def && V && Def->Type == EA11ySettingType::Enum) ? V->Str : Fallback;
}

FString UA11ySettingsSubsystem::GetString(FName Key, FString Fallback) const
{
	const FA11ySettingDefinition* Def = Definitions.Find(Key);
	const FA11yStoredValue* V = Values.Find(Key);
	return (Def && V && (Def->Type == EA11ySettingType::String || Def->Type == EA11ySettingType::Enum)) ? V->Str : Fallback;
}

FString UA11ySettingsSubsystem::GetValueAsString(FName Key) const
{
	const FA11ySettingDefinition* Def = Definitions.Find(Key);
	const FA11yStoredValue* V = Values.Find(Key);
	return (Def && V) ? ToCanonical(*Def, *V) : FString();
}

bool UA11ySettingsSubsystem::ResolveToggle(FName Key, bool bFollowValue) const
{
	const FA11ySettingDefinition* Def = Definitions.Find(Key);
	const FA11yStoredValue* V = Values.Find(Key);
	if (!Def || !V)
	{
		return bFollowValue; // the game pruned this override: the umbrella decides
	}
	if (Def->Type == EA11ySettingType::Bool)
	{
		return V->bBool;
	}
	if (Def->Type == EA11ySettingType::Enum || Def->Type == EA11ySettingType::String)
	{
		if (V->Str.Equals(TEXT("on"), ESearchCase::IgnoreCase))
		{
			return true;
		}
		if (V->Str.Equals(TEXT("off"), ESearchCase::IgnoreCase))
		{
			return false;
		}
	}
	return bFollowValue; // "follow" (or an unexpected value)
}

bool UA11ySettingsSubsystem::IsExplicitlySet(FName Key) const
{
	return ExplicitKeys.Contains(Key);
}

bool UA11ySettingsSubsystem::GetDefinition(FName Key, FA11ySettingDefinition& OutDefinition) const
{
	if (const FA11ySettingDefinition* Def = Definitions.Find(Key))
	{
		OutDefinition = *Def;
		return true;
	}
	return false;
}

TArray<FName> UA11ySettingsSubsystem::GetKeysInCategory(FName Category) const
{
	TArray<FName> Result;
	for (const FName& Key : KeyOrder)
	{
		if (Definitions[Key].Category == Category)
		{
			Result.Add(Key);
		}
	}
	return Result;
}

TArray<FA11yPresetDefinition> UA11ySettingsSubsystem::GetPresets() const
{
	return Presets;
}

// ------------------------------------------------------------------------------------------------
// Mutation
// ------------------------------------------------------------------------------------------------

bool UA11ySettingsSubsystem::SetInternal(FName Key, const FString& RawValue, bool bMarkExplicit, bool bSaveNow)
{
	const FA11ySettingDefinition* Def = Definitions.Find(Key);
	if (!Def)
	{
		UE_LOG(LogA11y, Warning, TEXT("Unknown accessibility key '%s'. Copy it from settings-registry.json first."), *Key.ToString());
		return false;
	}

	FA11yStoredValue Parsed;
	bool bClamped = false;
	if (!ParseValue(*Def, RawValue, Parsed, bClamped))
	{
		UE_LOG(LogA11y, Warning, TEXT("Rejected value '%s' for '%s'."), *RawValue, *Key.ToString());
		return false;
	}

	if (bMarkExplicit && !ExplicitKeys.Contains(Key))
	{
		ExplicitKeys.Add(Key);
		bDirty = true;
	}

	FA11yStoredValue& Current = Values.FindOrAdd(Key);
	const bool bChanged = !ValuesEqual(Def->Type, Current, Parsed);
	if (bChanged)
	{
		Current = Parsed;
		bDirty = true;
		OnSettingChanged.Broadcast(Key, ToCanonical(*Def, Current));
	}

	if (bSaveNow && bAutoSave && bDirty)
	{
		Save();
	}
	return bChanged;
}

bool UA11ySettingsSubsystem::SetBool(FName Key, bool bValue)
{
	return SetInternal(Key, bValue ? TEXT("true") : TEXT("false"), true, true);
}

bool UA11ySettingsSubsystem::SetFloat(FName Key, float Value)
{
	return SetInternal(Key, FString::SanitizeFloat(Value), true, true);
}

bool UA11ySettingsSubsystem::SetInt(FName Key, int32 Value)
{
	return SetInternal(Key, FString::FromInt(Value), true, true);
}

bool UA11ySettingsSubsystem::SetEnum(FName Key, const FString& Value)
{
	return SetInternal(Key, Value, true, true);
}

bool UA11ySettingsSubsystem::SetString(FName Key, const FString& Value)
{
	return SetInternal(Key, Value, true, true);
}

bool UA11ySettingsSubsystem::SetFromString(FName Key, const FString& Value)
{
	return SetInternal(Key, Value, true, true);
}

bool UA11ySettingsSubsystem::ResetKey(FName Key)
{
	const FA11ySettingDefinition* Def = Definitions.Find(Key);
	if (!Def)
	{
		return false;
	}
	if (ExplicitKeys.Remove(Key) > 0)
	{
		bDirty = true;
	}
	return SetInternal(Key, Def->DefaultValue, false, true);
}

int32 UA11ySettingsSubsystem::ResetCategory(FName Category)
{
	int32 Changed = 0;
	for (const FName& Key : KeyOrder)
	{
		const FA11ySettingDefinition& Def = Definitions[Key];
		if (Def.Category != Category)
		{
			continue;
		}
		if (ExplicitKeys.Remove(Key) > 0)
		{
			bDirty = true;
		}
		Changed += SetInternal(Key, Def.DefaultValue, false, false) ? 1 : 0;
	}
	if (bAutoSave && bDirty)
	{
		Save();
	}
	return Changed;
}

int32 UA11ySettingsSubsystem::ApplyPreset(FName PresetId)
{
	const FA11yPresetDefinition* Preset = Presets.FindByPredicate([PresetId](const FA11yPresetDefinition& P) { return P.Id == PresetId; });
	if (!Preset)
	{
		UE_LOG(LogA11y, Warning, TEXT("Unknown preset '%s'."), *PresetId.ToString());
		return 0;
	}

	LastPresetUndo.Reset();
	int32 Changed = 0;
	for (const TPair<FName, FString>& Change : Preset->Changes)
	{
		if (!Definitions.Contains(Change.Key))
		{
			continue; // The game did not include this option; nothing to change.
		}
		FPresetUndoEntry Entry;
		Entry.Key = Change.Key;
		Entry.PreviousValue = GetValueAsString(Change.Key);
		Entry.bWasExplicit = ExplicitKeys.Contains(Change.Key);
		Changed += SetInternal(Change.Key, Change.Value, /*bMarkExplicit*/ true, /*bSaveNow*/ false) ? 1 : 0;
		Entry.AppliedValue = GetValueAsString(Change.Key);
		LastPresetUndo.Add(MoveTemp(Entry));
	}
	if (bAutoSave && bDirty)
	{
		Save();
	}
	return Changed;
}

int32 UA11ySettingsSubsystem::UndoLastPreset()
{
	int32 Restored = 0;
	for (const FPresetUndoEntry& Entry : LastPresetUndo)
	{
		if (GetValueAsString(Entry.Key) != Entry.AppliedValue)
		{
			continue; // The player changed this key by hand after the preset. Keep their choice.
		}
		SetInternal(Entry.Key, Entry.PreviousValue, false, false);
		if (Entry.bWasExplicit)
		{
			ExplicitKeys.Add(Entry.Key);
		}
		else
		{
			ExplicitKeys.Remove(Entry.Key);
		}
		bDirty = true;
		++Restored;
	}
	LastPresetUndo.Reset();
	if (bAutoSave && bDirty)
	{
		Save();
	}
	return Restored;
}

// ------------------------------------------------------------------------------------------------
// Skip requests (shared by challenge skips and sensitive-scene skips)
// ------------------------------------------------------------------------------------------------

bool UA11ySettingsSubsystem::SubmitSkipRequest(FName Id)
{
	if (SubmittedSkipIds.Contains(Id))
	{
		return true; // already accepted: the owner is completing it; never complete twice
	}
	// Mark before broadcasting, so a listener that re-enters RequestSkip cannot complete twice.
	SubmittedSkipIds.Add(Id);
	OnChallengeSkipRequested.Broadcast(Id);
	return true;
}

void UA11ySettingsSubsystem::ClearSkipRequest(FName Id)
{
	SubmittedSkipIds.Remove(Id);
}

bool UA11ySettingsSubsystem::IsSkipRequested(FName Id) const
{
	return SubmittedSkipIds.Contains(Id);
}

// ------------------------------------------------------------------------------------------------
// Persistence
// ------------------------------------------------------------------------------------------------

bool UA11ySettingsSubsystem::Save()
{
	const TSharedRef<FJsonObject> Root = MakeShared<FJsonObject>();
	Root->SetNumberField(TEXT("version"), SaveFormatVersion);

	const TSharedRef<FJsonObject> ValuesObject = MakeShared<FJsonObject>();
	TArray<TSharedPtr<FJsonValue>> ExplicitArray;
	for (const FName& Key : KeyOrder)
	{
		const FA11ySettingDefinition& Def = Definitions[Key];
		const FA11yStoredValue* V = Values.Find(Key);
		if (!V)
		{
			continue;
		}
		const FString KeyString = Key.ToString();
		switch (Def.Type)
		{
		case EA11ySettingType::Bool: ValuesObject->SetBoolField(KeyString, V->bBool); break;
		case EA11ySettingType::Float: ValuesObject->SetNumberField(KeyString, V->Float); break;
		case EA11ySettingType::Int: ValuesObject->SetNumberField(KeyString, V->Int); break;
		case EA11ySettingType::Enum: ValuesObject->SetStringField(KeyString, V->Str); break;
		case EA11ySettingType::String: ValuesObject->SetStringField(KeyString, V->Str); break;
		}
		if (ExplicitKeys.Contains(Key))
		{
			ExplicitArray.Add(MakeShareable(new FJsonValueString(KeyString)));
		}
	}
	Root->SetObjectField(TEXT("values"), ValuesObject);
	Root->SetArrayField(TEXT("explicit"), ExplicitArray);

	FString Output;
	const TSharedRef<TJsonWriter<TCHAR, TPrettyJsonPrintPolicy<TCHAR>>> Writer =
		TJsonWriterFactory<TCHAR, TPrettyJsonPrintPolicy<TCHAR>>::Create(&Output);
	if (!FJsonSerializer::Serialize(Root, Writer))
	{
		UE_LOG(LogA11y, Error, TEXT("Could not serialise accessibility settings."));
		return false;
	}

	const FString Path = GetSaveFilePath();
	const FString TempPath = Path + TEXT(".tmp");
	IFileManager::Get().MakeDirectory(*FPaths::GetPath(Path), /*Tree*/ true);
	if (!FFileHelper::SaveStringToFile(Output, *TempPath, FFileHelper::EEncodingOptions::ForceUTF8WithoutBOM))
	{
		UE_LOG(LogA11y, Error, TEXT("Could not write %s."), *TempPath);
		return false;
	}
	// Move with Replace narrows the window for a half-written file. It is not guaranteed atomic on every platform.
	if (!IFileManager::Get().Move(*Path, *TempPath, /*Replace*/ true, /*EvenIfReadOnly*/ true))
	{
		UE_LOG(LogA11y, Error, TEXT("Could not move %s to %s."), *TempPath, *Path);
		return false;
	}
	bDirty = false;
	return true;
}

void UA11ySettingsSubsystem::Load()
{
	LoadInternal(/*bBroadcast*/ true);
}

void UA11ySettingsSubsystem::MigrateValues(int32 FromVersion, const TSharedPtr<FJsonObject>& ValuesObject)
{
	// Migrate by meaning when a key changes. EXAMPLE ONLY: a hypothetical pre-v1 boolean
	// "motion.shake_off": true becomes "motion.camera_shake": 0.0. Add real steps here when you rename keys.
	if (FromVersion < 1 && ValuesObject.IsValid())
	{
		bool bShakeOff = false;
		if (ValuesObject->TryGetBoolField(TEXT("motion.shake_off"), bShakeOff))
		{
			ValuesObject->RemoveField(TEXT("motion.shake_off"));
			if (bShakeOff && !ValuesObject->HasField(TEXT("motion.camera_shake")))
			{
				ValuesObject->SetNumberField(TEXT("motion.camera_shake"), 0.0);
			}
		}
	}
}

void UA11ySettingsSubsystem::LoadInternal(bool bBroadcast)
{
	TMap<FName, FString> Before;
	if (bBroadcast)
	{
		for (const FName& Key : KeyOrder)
		{
			Before.Add(Key, GetValueAsString(Key));
		}
	}

	// Start from registry defaults so every key has a valid value.
	Values.Reset();
	ExplicitKeys.Reset();
	for (const FName& Key : KeyOrder)
	{
		FA11yStoredValue V;
		bool bClamped = false;
		ParseValue(Definitions[Key], Definitions[Key].DefaultValue, V, bClamped);
		Values.Add(Key, V);
	}

	bool bNeedsResave = false;
	const FString Path = GetSaveFilePath();
	FString Text;
	if (FPaths::FileExists(Path) && FFileHelper::LoadFileToString(Text, *Path))
	{
		TSharedPtr<FJsonObject> Root;
		const TSharedRef<TJsonReader<>> Reader = TJsonReaderFactory<>::Create(Text);
		if (!FJsonSerializer::Deserialize(Reader, Root) || !Root.IsValid())
		{
			// Keep the player's file for inspection instead of silently discarding it.
			const FString BadPath = Path + TEXT(".bad");
			IFileManager::Get().Copy(*BadPath, *Path);
			UE_LOG(LogA11y, Warning, TEXT("%s is not valid JSON. Kept a copy at %s and used defaults."), *Path, *BadPath);
			bNeedsResave = true;
		}
		else
		{
			double VersionNumber = 0.0;
			const int32 FileVersion = Root->TryGetNumberField(TEXT("version"), VersionNumber) ? static_cast<int32>(VersionNumber) : 0;

			const TSharedPtr<FJsonObject>* ValuesObjectPtr = nullptr;
			if (Root->TryGetObjectField(TEXT("values"), ValuesObjectPtr) && ValuesObjectPtr && ValuesObjectPtr->IsValid())
			{
				if (FileVersion < SaveFormatVersion)
				{
					MigrateValues(FileVersion, *ValuesObjectPtr);
					bNeedsResave = true;
				}
				for (const TPair<FString, TSharedPtr<FJsonValue>>& Pair : (*ValuesObjectPtr)->Values)
				{
					const FName Key(*Pair.Key);
					const FA11ySettingDefinition* Def = Definitions.Find(Key);
					if (!Def)
					{
						UE_LOG(LogA11y, Verbose, TEXT("Ignoring unknown key '%s' in %s."), *Pair.Key, *Path);
						continue;
					}
					FString Canonical;
					FA11yStoredValue Parsed;
					bool bClamped = false;
					if (!JsonToCanonical(Pair.Value, Canonical) || !ParseValue(*Def, Canonical, Parsed, bClamped))
					{
						// Per-key repair: this key returns to its default; every other key is kept.
						UE_LOG(LogA11y, Warning, TEXT("Invalid value for '%s' in %s; using the default."), *Pair.Key, *Path);
						bNeedsResave = true;
						continue;
					}
					if (bClamped)
					{
						UE_LOG(LogA11y, Warning, TEXT("Out-of-range value for '%s' clamped."), *Pair.Key);
						bNeedsResave = true;
					}
					Values.Add(Key, Parsed);
				}
			}

			const TArray<TSharedPtr<FJsonValue>>* ExplicitArray = nullptr;
			if (Root->TryGetArrayField(TEXT("explicit"), ExplicitArray) && ExplicitArray)
			{
				for (const TSharedPtr<FJsonValue>& V : *ExplicitArray)
				{
					FString KeyString;
					if (V.IsValid() && V->TryGetString(KeyString) && Definitions.Contains(FName(*KeyString)))
					{
						ExplicitKeys.Add(FName(*KeyString));
					}
				}
			}
		}
	}

	if (bBroadcast)
	{
		for (const FName& Key : KeyOrder)
		{
			const FString Now = GetValueAsString(Key);
			const FString* Was = Before.Find(Key);
			if (!Was || *Was != Now)
			{
				OnSettingChanged.Broadcast(Key, Now);
			}
		}
	}

	bDirty = bNeedsResave;
	if (bNeedsResave)
	{
		Save();
	}
}

// ------------------------------------------------------------------------------------------------
// OS preferences
// ------------------------------------------------------------------------------------------------

int32 UA11ySettingsSubsystem::ImportPlatformPreferenceBool(const FString& PrefName, bool bEnabled)
{
	int32 Changed = 0;
	for (const A11ySettingsPrivate::FBoolPrefMapping& Mapping : A11ySettingsPrivate::BoolPrefMappings)
	{
		const FName Key(Mapping.Key);
		const FA11ySettingDefinition* Def = Definitions.Find(Key);
		if (PrefName != Mapping.Pref || !Def || ExplicitKeys.Contains(Key))
		{
			continue;
		}
		const FString Value = bEnabled ? FString(Mapping.ValueWhenOn) : Def->DefaultValue;
		Changed += SetInternal(Key, Value, /*bMarkExplicit*/ false, /*bSaveNow*/ false) ? 1 : 0;
	}
	if (Changed > 0 && bAutoSave)
	{
		Save();
	}
	return Changed;
}

int32 UA11ySettingsSubsystem::ImportPlatformPreferenceFloat(const FString& PrefName, float Value)
{
	int32 Changed = 0;
	for (const FName& Key : KeyOrder)
	{
		const FA11ySettingDefinition& Def = Definitions[Key];
		if (Def.PlatformPref != PrefName || Def.Type != EA11ySettingType::Float || ExplicitKeys.Contains(Key))
		{
			continue;
		}
		Changed += SetInternal(Key, FString::SanitizeFloat(Value), false, false) ? 1 : 0;
	}
	if (Changed > 0 && bAutoSave)
	{
		Save();
	}
	return Changed;
}

void UA11ySettingsSubsystem::QueryPlatformPreferences()
{
	// STUB. No cross-platform Unreal API for reduce-motion, contrast or caption preferences was verified.
	// Platform code can call ImportPlatformPreferenceBool/Float with values from these sources
	// (check each against your engine version and platform SDK before use):
	//  - Screen reader (Windows/iOS, experimental, needs Accessibility.Enable=1 and WITH_ACCESSIBILITY):
	//      FGenericAccessibleMessageHandler::IsActive(), reached through the platform application's
	//      GetAccessibleMessageHandler(). Documented as "usually when screen-reading software is turned on".
	//  - Windows "Show animations": SystemParametersInfoW(SPI_GETCLIENTAREAANIMATION, ...).
	//  - iOS: UIAccessibilityIsReduceMotionEnabled(), UIAccessibilityIsVoiceOverRunning().
	//  - Android: Settings.Global ANIMATOR_DURATION_SCALE == 0 (through JNI).
	// Example: ImportPlatformPreferenceBool(TEXT("reduce_motion"), bOsReduceMotion);
}
