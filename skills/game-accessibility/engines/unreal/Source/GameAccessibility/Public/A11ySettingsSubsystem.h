// SPDX-License-Identifier: MIT
// game-accessibility skill: A11ySettings store (architecture.md section 1).
// NOT COMPILED by the author (no Unreal Editor / C++ toolchain). Please report compile errors.
//
// Registry:    <Project>/Config/Accessibility/settings_registry.json (copy from engines/unreal/Resources/).
//              Staged into packaged builds by GameAccessibility.Build.cs (RuntimeDependencies, NonUFS).
// Player file: FPaths::ProjectSavedDir()/Accessibility.json. Separate from story saves. Human-readable.
//              Written to Accessibility.json.tmp, then moved over the old file.

#pragma once

#include "CoreMinimal.h"
#include "Subsystems/GameInstanceSubsystem.h"
#include "A11yTypes.h"
#include "A11ySettingsSubsystem.generated.h"

class FJsonObject;
class FJsonValue;

/** One entry of settings_registry.json. Labels are English source strings; localise them in your project. */
USTRUCT(BlueprintType)
struct GAMEACCESSIBILITY_API FA11ySettingDefinition
{
	GENERATED_BODY()

	UPROPERTY(BlueprintReadOnly, Category = "Accessibility") FName Id;
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility") FName Category;
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility") EA11ySettingType Type = EA11ySettingType::Bool;
	/** Default in canonical string form: "true"/"false", "1.5", "2", or an enum value. */
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility") FString DefaultValue;
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility") float Min = 0.f;
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility") float Max = 1.f;
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility") float Step = 0.f;
	/** Legal values for Enum entries, in registry order. */
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility") TArray<FString> Values;
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility") FString Label;
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility") FString Description;
	/** "profile" follows the player; "device" stays with the machine. */
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility") FName Scope;
	/** OS preference that may provide the initial value (reduce_motion, text_scale, ...). Empty if none. */
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility") FString PlatformPref;
};

/** A preset is an editable patch: it lists exactly which keys it changes. */
USTRUCT(BlueprintType)
struct GAMEACCESSIBILITY_API FA11yPresetDefinition
{
	GENERATED_BODY()

	UPROPERTY(BlueprintReadOnly, Category = "Accessibility") FName Id;
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility") FString Label;
	/** Key -> canonical value string. Keys missing from the game's registry are skipped when applied. */
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility") TMap<FName, FString> Changes;
};

/** Typed storage for one value. Internal; not reflected. */
struct FA11yStoredValue
{
	bool bBool = false;
	float Float = 0.f;
	int32 Int = 0;
	/** Enum and String values. */
	FString Str;
};

/** Key, new value in canonical string form. Read typed values with GetBool/GetFloat/GetInt/GetEnum. */
DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FA11ySettingChangedSignature, FName, Key, const FString&, Value);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FA11yChallengeSkipSignature, FName, ChallengeId);

/**
 * Single source of truth for accessibility settings. Gameplay code should ask UA11yAssistPolicy and the
 * other services instead of reading raw keys. Late subscribers: read the current value, then bind
 * OnSettingChanged. Never rely on having seen an earlier change event.
 */
UCLASS()
class GAMEACCESSIBILITY_API UA11ySettingsSubsystem : public UGameInstanceSubsystem
{
	GENERATED_BODY()

public:
	/** Convenience accessor. Returns null outside a game world (for example in editor previews). */
	static UA11ySettingsSubsystem* Get(const UObject* WorldContextObject);

	virtual void Initialize(FSubsystemCollectionBase& Collection) override;
	virtual void Deinitialize() override;

	static FString GetRegistryPath();
	static FString GetSaveFilePath();

	// ---- Queries -------------------------------------------------------------------------------

	UFUNCTION(BlueprintPure, Category = "Accessibility|Settings")
	bool HasKey(FName Key) const;

	UFUNCTION(BlueprintPure, Category = "Accessibility|Settings")
	bool GetBool(FName Key, bool bFallback = false) const;

	UFUNCTION(BlueprintPure, Category = "Accessibility|Settings")
	float GetFloat(FName Key, float Fallback = 0.f) const;

	UFUNCTION(BlueprintPure, Category = "Accessibility|Settings")
	int32 GetInt(FName Key, int32 Fallback = 0) const;

	UFUNCTION(BlueprintPure, Category = "Accessibility|Settings")
	FString GetEnum(FName Key, FString Fallback = FString(TEXT(""))) const;

	/** String keys (and enum keys) as text, e.g. visual.custom_colors or hud.hidden_elements. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Settings")
	FString GetString(FName Key, FString Fallback = FString(TEXT(""))) const;

	/** Canonical string form of any value. Empty if the key is not in the registry. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Settings")
	FString GetValueAsString(FName Key) const;

	/**
	 * Resolves a per-effect override key whose values are "follow"/"off"/"on" (visual.outline_*,
	 * motion.auto_camera, motion.fov_effects, ...). "on" -> true, "off" -> false. "follow", an unregistered
	 * key or any other value -> bFollowValue, which the caller computes from the umbrella key, for example
	 * GetBool("visual.high_contrast") or !GetBool("motion.reduce_motion"). A project that registered the key
	 * as a plain bool gets that bool.
	 */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Settings")
	bool ResolveToggle(FName Key, bool bFollowValue) const;

	/** True when the player chose this value (directly or through a preset). OS preferences never override it. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Settings")
	bool IsExplicitlySet(FName Key) const;

	UFUNCTION(BlueprintPure, Category = "Accessibility|Settings")
	bool GetDefinition(FName Key, FA11ySettingDefinition& OutDefinition) const;

	/** Keys in registry order, for building a settings page. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Settings")
	TArray<FName> GetKeysInCategory(FName Category) const;

	UFUNCTION(BlueprintPure, Category = "Accessibility|Settings")
	TArray<FA11yPresetDefinition> GetPresets() const;

	// ---- Player changes (validate, clamp, mark explicit, broadcast, auto-save) -----------------
	// Each setter changes only its own key. Returns true if the stored value changed.

	UFUNCTION(BlueprintCallable, Category = "Accessibility|Settings")
	bool SetBool(FName Key, bool bValue);

	UFUNCTION(BlueprintCallable, Category = "Accessibility|Settings")
	bool SetFloat(FName Key, float Value);

	UFUNCTION(BlueprintCallable, Category = "Accessibility|Settings")
	bool SetInt(FName Key, int32 Value);

	UFUNCTION(BlueprintCallable, Category = "Accessibility|Settings")
	bool SetEnum(FName Key, const FString& Value);

	UFUNCTION(BlueprintCallable, Category = "Accessibility|Settings")
	bool SetString(FName Key, const FString& Value);

	/** Parses "true", "1.5", "2", an enum value or free text according to the key's type. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Settings")
	bool SetFromString(FName Key, const FString& Value);

	/** Back to the registry default. Clears the explicit flag, so OS preferences apply again. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Settings")
	bool ResetKey(FName Key);

	/** Returns the number of keys whose value changed. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Settings")
	int32 ResetCategory(FName Category);

	/** One batch, one save, undoable. Later hand edits are never overwritten by UndoLastPreset. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Settings")
	int32 ApplyPreset(FName PresetId);

	/** Restores keys from the last preset that the player has not changed since. Returns keys restored. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Settings")
	int32 UndoLastPreset();

	UFUNCTION(BlueprintPure, Category = "Accessibility|Settings")
	bool CanUndoPreset() const { return LastPresetUndo.Num() > 0; }

	// ---- Persistence ---------------------------------------------------------------------------

	UFUNCTION(BlueprintCallable, Category = "Accessibility|Settings")
	bool Save();

	/** Reloads the player file, repairing invalid values one key at a time. Broadcasts changed keys. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Settings")
	void Load();

	/** When true (default), every player change is written immediately. Values are step-bounded, so writes are few. */
	UPROPERTY(BlueprintReadWrite, Category = "Accessibility|Settings")
	bool bAutoSave = true;

	// ---- OS preferences ------------------------------------------------------------------------
	// Imported as the initial value only for keys the player has not set explicitly. Never marks explicit.

	/** PrefName: reduce_motion, screen_reader_active, increase_contrast, captions_enabled, mono_audio. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Settings")
	int32 ImportPlatformPreferenceBool(const FString& PrefName, bool bEnabled);

	/** PrefName: text_scale, caption_scale. Applies to float keys whose platform_pref matches. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Settings")
	int32 ImportPlatformPreferenceFloat(const FString& PrefName, float Value);

	// ---- Events --------------------------------------------------------------------------------

	UPROPERTY(BlueprintAssignable, Category = "Accessibility|Settings")
	FA11ySettingChangedSignature OnSettingChanged;

	/**
	 * Raised by UA11yAssistPolicy::RequestSkip (challenge, gated by assist.skip_available) and
	 * UA11yAssistPolicy::RequestSceneSkip (sensitive scene, gated by content.scene_skip). Both use this one
	 * delegate, so the owner completes through its normal success path either way. Raised at most once per
	 * Id until ClearSkipRequest(Id).
	 */
	UPROPERTY(BlueprintAssignable, Category = "Accessibility|Assist")
	FA11yChallengeSkipSignature OnChallengeSkipRequested;

	/**
	 * Shared, idempotent skip path. The FIRST request for Id broadcasts OnChallengeSkipRequested; repeats
	 * (double press, or both a scene skip and a challenge skip for the same Id) return true without
	 * broadcasting again. Gate checks happen in UA11yAssistPolicy; call that instead of this.
	 */
	bool SubmitSkipRequest(FName Id);

	/** Re-arms Id. Call when the challenge or scene can be played again (restart, chapter replay). */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Assist")
	void ClearSkipRequest(FName Id);

	UFUNCTION(BlueprintPure, Category = "Accessibility|Assist")
	bool IsSkipRequested(FName Id) const;

	static constexpr int32 SaveFormatVersion = 1;
	/** Upper bound for "string" values so a hand-edited file cannot load megabytes of text. */
	static constexpr int32 MaxStringLength = 4096;

private:
	struct FPresetUndoEntry
	{
		FName Key;
		FString PreviousValue;
		bool bWasExplicit = false;
		FString AppliedValue;
	};

	bool LoadRegistry();
	void LoadInternal(bool bBroadcast);
	void QueryPlatformPreferences();
	static void MigrateValues(int32 FromVersion, const TSharedPtr<FJsonObject>& ValuesObject);

	bool SetInternal(FName Key, const FString& RawValue, bool bMarkExplicit, bool bSaveNow);
	bool ParseValue(const FA11ySettingDefinition& Def, const FString& Raw, FA11yStoredValue& Out, bool& bOutClamped) const;
	FString ToCanonical(const FA11ySettingDefinition& Def, const FA11yStoredValue& Value) const;
	static bool ValuesEqual(EA11ySettingType Type, const FA11yStoredValue& A, const FA11yStoredValue& B);
	static bool JsonToCanonical(const TSharedPtr<FJsonValue>& JsonValue, FString& Out);

	TMap<FName, FA11ySettingDefinition> Definitions;
	TArray<FName> KeyOrder;
	TMap<FName, FA11yStoredValue> Values;
	TSet<FName> ExplicitKeys;
	TArray<FA11yPresetDefinition> Presets;
	TArray<FPresetUndoEntry> LastPresetUndo;
	/** Ids whose skip has been broadcast and not yet re-armed. Session only; never saved. */
	TSet<FName> SubmittedSkipIds;
	bool bDirty = false;
};
