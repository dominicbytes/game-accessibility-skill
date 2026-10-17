// SPDX-License-Identifier: MIT
// game-accessibility skill: ContentFilter. NOT COMPILED by the author; please report compile errors.

#include "A11yContentVariantComponent.h"
#include "A11ySettingsSubsystem.h"
#include "Engine/World.h"

namespace A11yContentVariantPrivate
{
	static const TCHAR* TagPrefix = TEXT("A11yVariant.");
}

void UA11yContentVariantComponent::OnRegister()
{
	Super::OnRegister();
	Refresh(); // before the first rendered frame
}

void UA11yContentVariantComponent::BeginPlay()
{
	Super::BeginPlay();
	if (UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this))
	{
		Settings->OnSettingChanged.AddUniqueDynamic(this, &UA11yContentVariantComponent::HandleSettingChanged);
	}
	// Late subscribers: Blueprint listeners bound after this point should read GetActiveVariant().
	bHasApplied = false;
	Refresh();
}

void UA11yContentVariantComponent::EndPlay(const EEndPlayReason::Type EndPlayReason)
{
	if (UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this))
	{
		Settings->OnSettingChanged.RemoveDynamic(this, &UA11yContentVariantComponent::HandleSettingChanged);
	}
	Super::EndPlay(EndPlayReason);
}

void UA11yContentVariantComponent::HandleSettingChanged(FName Key, const FString& /*Value*/)
{
	if (Key == SettingKey)
	{
		Refresh(); // live swap, same frame
	}
}

bool UA11yContentVariantComponent::GetVariantTag(const USceneComponent* Component, FString& OutValue)
{
	const FString Prefix(A11yContentVariantPrivate::TagPrefix);
	for (const FName& Tag : Component->ComponentTags)
	{
		const FString TagString = Tag.ToString();
		if (TagString.StartsWith(Prefix, ESearchCase::IgnoreCase))
		{
			OutValue = TagString.RightChop(Prefix.Len());
			return true;
		}
	}
	return false;
}

FString UA11yContentVariantComponent::ChooseVariant(const FString& Requested, const TArray<FString>& Authored) const
{
	for (const FString& A : Authored)
	{
		if (A.Equals(Requested, ESearchCase::IgnoreCase))
		{
			return A;
		}
	}

	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this);
	FA11ySettingDefinition Def;
	if (!Settings || !Settings->GetDefinition(SettingKey, Def))
	{
		return FString();
	}

	if (Def.Type == EA11ySettingType::Int)
	{
		// Smallest authored level that is at least as filtered as requested.
		const int32 RequestedLevel = FCString::Atoi(*Requested);
		FString Best;
		int32 BestLevel = MAX_int32;
		for (const FString& A : Authored)
		{
			if (!A.IsNumeric())
			{
				continue;
			}
			const int32 Level = FCString::Atoi(*A);
			if (Level >= RequestedLevel && Level < BestLevel)
			{
				BestLevel = Level;
				Best = A;
			}
		}
		return Best;
	}

	if (Def.Type == EA11ySettingType::Enum)
	{
		// Assumes registry order goes from original to most filtered (full, reduced, off).
		const int32 Index = Def.Values.IndexOfByKey(Requested);
		for (int32 i = Index + 1; Index != INDEX_NONE && i < Def.Values.Num(); ++i)
		{
			for (const FString& A : Authored)
			{
				if (A.Equals(Def.Values[i], ESearchCase::IgnoreCase))
				{
					return A;
				}
			}
		}
	}
	return FString();
}

void UA11yContentVariantComponent::Refresh()
{
	const UWorld* World = GetWorld();
	if (!World || !World->IsGameWorld())
	{
		return; // leave editor previews alone
	}
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this);
	if (!Settings || !Settings->HasKey(SettingKey))
	{
		UE_LOG(LogA11y, Verbose, TEXT("%s: key %s is not in the registry; variants unchanged."), *GetPathName(), *SettingKey.ToString());
		return;
	}

	TArray<USceneComponent*> Children;
	GetChildrenComponents(/*bIncludeAllDescendants*/ false, Children);

	TArray<FString> Authored;
	for (const USceneComponent* Child : Children)
	{
		FString Value;
		if (Child && GetVariantTag(Child, Value))
		{
			Authored.AddUnique(Value);
		}
	}

	const FString Requested = Settings->GetValueAsString(SettingKey);
	const FString Chosen = ChooseVariant(Requested, Authored);
	if (Chosen.IsEmpty() && Authored.Num() > 0)
	{
		// Never fall back below the requested level: keep the presentation hidden and report the gap.
		UE_LOG(LogA11y, Warning, TEXT("%s: no variant at or above %s=%s is authored; all variants hidden."),
			*GetPathName(), *SettingKey.ToString(), *Requested);
	}
	for (USceneComponent* Child : Children)
	{
		FString Value;
		if (Child && GetVariantTag(Child, Value))
		{
			Child->SetVisibility(!Chosen.IsEmpty() && Value.Equals(Chosen, ESearchCase::IgnoreCase), /*bPropagateToChildren*/ true);
		}
	}

	if (!bHasApplied || Chosen != ActiveVariant)
	{
		ActiveVariant = Chosen;
		bHasApplied = true;
		OnVariantChanged.Broadcast(ActiveVariant);
	}
}
