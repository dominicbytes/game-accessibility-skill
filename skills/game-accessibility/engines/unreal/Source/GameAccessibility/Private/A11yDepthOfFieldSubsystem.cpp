// SPDX-License-Identifier: MIT
// game-accessibility skill: depth-of-field fallback owner. NOT COMPILED by the author; please report compile errors.

#include "A11yDepthOfFieldSubsystem.h"
#include "A11yTypes.h"

#include "Engine/Engine.h"
#include "Engine/GameInstance.h"
#include "Engine/World.h"
#include "HAL/IConsoleManager.h"

namespace A11yDepthOfFieldPrivate
{
	static IConsoleVariable* FindQualityCvar()
	{
		return IConsoleManager::Get().FindConsoleVariable(TEXT("r.DepthOfFieldQuality"));
	}
}

UA11yDepthOfFieldSubsystem* UA11yDepthOfFieldSubsystem::Get(const UObject* WorldContextObject)
{
	if (!WorldContextObject || !GEngine)
	{
		return nullptr;
	}
	const UWorld* World = GEngine->GetWorldFromContextObject(WorldContextObject, EGetWorldErrorMode::ReturnNull);
	const UGameInstance* GameInstance = World ? World->GetGameInstance() : nullptr;
	return GameInstance ? GameInstance->GetSubsystem<UA11yDepthOfFieldSubsystem>() : nullptr;
}

void UA11yDepthOfFieldSubsystem::Deinitialize()
{
	Requesters.Reset();
	Reevaluate(); // restores the authored quality if the override was active
	Super::Deinitialize();
}

void UA11yDepthOfFieldSubsystem::SetDisabledBy(const UObject* Requester, bool bDisabled)
{
	if (!Requester)
	{
		return;
	}
	const TWeakObjectPtr<const UObject> Key(Requester);
	if (bDisabled)
	{
		Requesters.Add(Key);
	}
	else
	{
		Requesters.Remove(Key);
	}
	Reevaluate();
}

void UA11yDepthOfFieldSubsystem::Release(const UObject* Requester)
{
	SetDisabledBy(Requester, false);
}

void UA11yDepthOfFieldSubsystem::SetAuthoredQuality(int32 Quality)
{
	const int32 Clamped = FMath::Max(0, Quality);
	if (bOverrideActive)
	{
		BaselineQuality = Clamped; // restored when the last requester releases
		return;
	}
	if (IConsoleVariable* DofQuality = A11yDepthOfFieldPrivate::FindQualityCvar())
	{
		DofQuality->Set(Clamped, ECVF_SetByCode);
	}
}

void UA11yDepthOfFieldSubsystem::Reevaluate()
{
	for (auto It = Requesters.CreateIterator(); It; ++It)
	{
		if (!It->IsValid())
		{
			It.RemoveCurrent(); // destroyed without EndPlay
		}
	}

	IConsoleVariable* DofQuality = A11yDepthOfFieldPrivate::FindQualityCvar();
	if (!DofQuality)
	{
		return;
	}

	// ECVF_SetByCode outranks scalability, so after a restore the value stays pinned at code priority.
	// If your options menu changes DOF quality, route it through SetAuthoredQuality.
	const bool bWantOverride = Requesters.Num() > 0;
	if (bWantOverride && !bOverrideActive)
	{
		BaselineQuality = DofQuality->GetInt(); // captured once, by the single owner
		bOverrideActive = true;
		DofQuality->Set(0, ECVF_SetByCode);
	}
	else if (bWantOverride && DofQuality->GetInt() != 0)
	{
		DofQuality->Set(0, ECVF_SetByCode); // something else changed it while the player has DOF off
	}
	else if (!bWantOverride && bOverrideActive)
	{
		DofQuality->Set(FMath::Max(0, BaselineQuality), ECVF_SetByCode);
		bOverrideActive = false;
		BaselineQuality = -1;
		UE_LOG(LogA11y, Verbose, TEXT("Depth of field fallback released; authored quality restored."));
	}
}
