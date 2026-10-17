// SPDX-License-Identifier: MIT
// game-accessibility skill: GuidanceLayer. NOT COMPILED by the author; please report compile errors.

#include "A11yGuidanceComponent.h"
#include "A11ySettingsSubsystem.h"

#include "Components/MeshComponent.h"
#include "Components/PrimitiveComponent.h"
#include "Engine/Engine.h"
#include "Engine/World.h"
#include "TimerManager.h"
#include "UObject/UObjectIterator.h"

void UA11yGuidanceComponent::BeginPlay()
{
	Super::BeginPlay();
	if (UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this))
	{
		Settings->OnSettingChanged.AddUniqueDynamic(this, &UA11yGuidanceComponent::HandleSettingChanged);
	}
	Refresh();
}

void UA11yGuidanceComponent::EndPlay(const EEndPlayReason::Type EndPlayReason)
{
	if (UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this))
	{
		Settings->OnSettingChanged.RemoveDynamic(this, &UA11yGuidanceComponent::HandleSettingChanged);
	}
	if (UWorld* World = GetWorld())
	{
		World->GetTimerManager().ClearTimer(HintTimer);
	}
	Super::EndPlay(EndPlayReason);
}

void UA11yGuidanceComponent::HandleSettingChanged(FName Key, const FString& /*Value*/)
{
	if (Key == FName(TEXT("guidance.level")) || Key == Channel)
	{
		Refresh();
	}
}

void UA11yGuidanceComponent::RequestHint(const UObject* WorldContextObject, float Seconds)
{
	const UWorld* World = GEngine ? GEngine->GetWorldFromContextObject(WorldContextObject, EGetWorldErrorMode::ReturnNull) : nullptr;
	if (!World)
	{
		return;
	}
	// Hints are rare player actions, so a filtered object iteration is acceptable here.
	for (TObjectIterator<UA11yGuidanceComponent> It; It; ++It)
	{
		if (!It->IsTemplate() && It->GetWorld() == World && It->HasBegunPlay())
		{
			It->ShowHint(Seconds);
		}
	}
}

void UA11yGuidanceComponent::ShowHint(float Seconds)
{
	bHintActive = true;
	if (UWorld* World = GetWorld())
	{
		World->GetTimerManager().SetTimer(HintTimer, this, &UA11yGuidanceComponent::EndHint, FMath::Max(0.5f, Seconds), false);
	}
	Refresh();
}

void UA11yGuidanceComponent::EndHint()
{
	bHintActive = false;
	Refresh();
}

void UA11yGuidanceComponent::Refresh()
{
	// Effective level: the channel's own key unless it is "follow" (or absent), then guidance.level.
	FString Level = TEXT("standard");
	if (const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this))
	{
		const FString Own = Settings->HasKey(Channel) ? Settings->GetEnum(Channel, TEXT("follow")) : FString(TEXT("follow"));
		Level = (Own != TEXT("follow")) ? Own : Settings->GetEnum(TEXT("guidance.level"), TEXT("standard"));
	}

	bool bVisible = true;
	float Intensity = 1.f;
	if (!bIsHazardCue)
	{
		if (Level == TEXT("off")) { bVisible = false; }
		else if (Level == TEXT("on_request")) { bVisible = bHintActive; }
		else if (Level == TEXT("subtle")) { Intensity = SubtleIntensity; }
		else if (Level == TEXT("boosted")) { Intensity = BoostedIntensity; }
	}
	const bool bOutline = !bIsHazardCue && bOutlineWhenBoosted && Level == TEXT("boosted");

	SetVisibility(bVisible, /*bPropagateToChildren*/ true);

	TArray<USceneComponent*> Children;
	GetChildrenComponents(/*bIncludeAllDescendants*/ true, Children);
	for (USceneComponent* Child : Children)
	{
		if (UMeshComponent* Mesh = Cast<UMeshComponent>(Child))
		{
			Mesh->SetScalarParameterValueOnMaterials(IntensityParameterName, Intensity);
		}
		if (UPrimitiveComponent* Primitive = Cast<UPrimitiveComponent>(Child))
		{
			Primitive->SetRenderCustomDepth(bOutline);
		}
	}
	OnGuidanceChanged.Broadcast(bVisible, Intensity);
}
