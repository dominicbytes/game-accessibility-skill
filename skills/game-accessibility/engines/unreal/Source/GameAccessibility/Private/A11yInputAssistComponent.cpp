// SPDX-License-Identifier: MIT
// game-accessibility skill: InputAssist. NOT COMPILED by the author; please report compile errors.

#include "A11yInputAssistComponent.h"
#include "A11ySettingsSubsystem.h"

#include "Framework/Application/SlateApplication.h"
#include "GenericPlatform/GenericPlatformInputDeviceMapper.h"

UA11yInputAssistComponent::UA11yInputAssistComponent()
{
	PrimaryComponentTick.bCanEverTick = false;
}

void UA11yInputAssistComponent::BeginPlay()
{
	Super::BeginPlay();

	if (UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this))
	{
		Settings->OnSettingChanged.AddUniqueDynamic(this, &UA11yInputAssistComponent::HandleSettingChanged);
	}

	// Focus loss: a latched action must not stay on while the player is in another window.
	// UNVERIFIED signature: FSlateApplication::OnApplicationActivationStateChanged() event with (const bool IsActive).
	if (FSlateApplication::IsInitialized())
	{
		AppActivationHandle = FSlateApplication::Get().OnApplicationActivationStateChanged().AddWeakLambda(
			this, [this](const bool bIsActive)
			{
				if (!bIsActive)
				{
					ClearAllLatches();
				}
			});
	}

	// Device disconnect (UE 5.1+): FOnUserInputDeviceConnectionChange(EInputDeviceConnectionState, FPlatformUserId, FInputDeviceId).
	DeviceConnectionHandle = IPlatformInputDeviceMapper::Get().GetOnInputDeviceConnectionChange().AddWeakLambda(
		this, [this](EInputDeviceConnectionState NewState, FPlatformUserId /*UserId*/, FInputDeviceId /*DeviceId*/)
		{
			if (NewState == EInputDeviceConnectionState::Disconnected)
			{
				ClearAllLatches();
			}
		});
}

void UA11yInputAssistComponent::EndPlay(const EEndPlayReason::Type EndPlayReason)
{
	ClearAllLatches();
	if (UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this))
	{
		Settings->OnSettingChanged.RemoveDynamic(this, &UA11yInputAssistComponent::HandleSettingChanged);
	}
	if (FSlateApplication::IsInitialized() && AppActivationHandle.IsValid())
	{
		FSlateApplication::Get().OnApplicationActivationStateChanged().Remove(AppActivationHandle);
	}
	if (DeviceConnectionHandle.IsValid())
	{
		IPlatformInputDeviceMapper::Get().GetOnInputDeviceConnectionChange().Remove(DeviceConnectionHandle);
	}
	Super::EndPlay(EndPlayReason);
}

void UA11yInputAssistComponent::HandleSettingChanged(FName Key, const FString& /*Value*/)
{
	if (Key == FName(TEXT("input.hold_mode")))
	{
		// Switching between hold and toggle must not leave an action stuck on.
		ClearAllLatches();
	}
}

FString UA11yInputAssistComponent::GetMode(FName Key, const TCHAR* Fallback) const
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this);
	return Settings ? Settings->GetEnum(Key, Fallback) : FString(Fallback);
}

bool UA11yInputAssistComponent::IsToggleMode() const
{
	return GetMode(TEXT("input.hold_mode"), TEXT("hold")) == TEXT("toggle");
}

// ---- Hold / toggle ------------------------------------------------------------------------------

bool UA11yInputAssistComponent::HandleHoldInput(FName Action, bool bPressed)
{
	const bool bToggle = IsToggleMode();
	FHoldState& State = HoldStates.FindOrAdd(Action);
	const bool bWasHeld = bToggle ? State.bLatched : State.bPhysical;

	if (bToggle && bPressed && !State.bPhysical)
	{
		State.bLatched = !State.bLatched; // rising edge flips the latch
	}
	State.bPhysical = bPressed;

	const bool bNowHeld = bToggle ? State.bLatched : State.bPhysical;
	if (bNowHeld != bWasHeld)
	{
		OnHeldChanged.Broadcast(Action, bNowHeld);
	}
	return bNowHeld;
}

bool UA11yInputAssistComponent::IsHeld(FName Action) const
{
	const FHoldState* State = HoldStates.Find(Action);
	if (!State)
	{
		return false;
	}
	return IsToggleMode() ? State->bLatched : State->bPhysical;
}

void UA11yInputAssistComponent::ClearAllLatches()
{
	const bool bToggle = IsToggleMode();
	for (TPair<FName, FHoldState>& Pair : HoldStates)
	{
		const bool bWasHeld = bToggle ? Pair.Value.bLatched : Pair.Value.bPhysical;
		Pair.Value.bLatched = false;
		Pair.Value.bPhysical = false; // release events may be lost while unfocused or disconnected
		if (bWasHeld)
		{
			OnHeldChanged.Broadcast(Pair.Key, false);
		}
	}
}

// ---- Repeated presses ---------------------------------------------------------------------------

void UA11yInputAssistComponent::BeginMash(FName ChallengeId, int32 RequiredPresses, float HoldSecondsForFull)
{
	FMashState& State = Mashes.FindOrAdd(ChallengeId);
	State.RequiredPresses = FMath::Max(1, RequiredPresses);
	State.HoldSecondsForFull = FMath::Max(0.05f, HoldSecondsForFull);
	State.Progress = 0.f;
}

float UA11yInputAssistComponent::RegisterMashPress(FName ChallengeId)
{
	FMashState* State = Mashes.Find(ChallengeId);
	if (!State)
	{
		return 0.f;
	}
	const FString Mode = GetMode(TEXT("input.mash_mode"), TEXT("mash"));
	if (Mode == TEXT("single"))
	{
		State->Progress = 1.f;
	}
	else if (Mode == TEXT("mash"))
	{
		State->Progress = FMath::Min(1.f, State->Progress + 1.f / static_cast<float>(State->RequiredPresses));
	}
	// "hold": presses do nothing by themselves; TickMashHold fills the bar while held.
	return State->Progress;
}

float UA11yInputAssistComponent::TickMashHold(FName ChallengeId, bool bHeld, float DeltaSeconds)
{
	FMashState* State = Mashes.Find(ChallengeId);
	if (!State)
	{
		return 0.f;
	}
	if (bHeld && GetMode(TEXT("input.mash_mode"), TEXT("mash")) == TEXT("hold"))
	{
		State->Progress = FMath::Min(1.f, State->Progress + FMath::Max(0.f, DeltaSeconds) / State->HoldSecondsForFull);
	}
	return State->Progress;
}

float UA11yInputAssistComponent::GetMashProgress(FName ChallengeId) const
{
	const FMashState* State = Mashes.Find(ChallengeId);
	return State ? State->Progress : 0.f;
}

void UA11yInputAssistComponent::EndMash(FName ChallengeId)
{
	Mashes.Remove(ChallengeId);
}

// ---- Quick-time events --------------------------------------------------------------------------

float UA11yInputAssistComponent::GetQteWindowSeconds(float AuthoredWindowSeconds, bool bNothingElseMoves) const
{
	const FString Mode = GetMode(TEXT("input.qte_mode"), TEXT("normal"));
	if (Mode == TEXT("auto"))
	{
		return AutoQteBeatSeconds;
	}
	if (Mode == TEXT("slow"))
	{
		return bNothingElseMoves ? -1.f : AuthoredWindowSeconds * FMath::Max(3.f, SlowQteScale);
	}
	return AuthoredWindowSeconds;
}

void UA11yInputAssistComponent::BeginQte(FName QteId, float AuthoredWindowSeconds, bool bNothingElseMoves)
{
	FQteRuntime& Qte = Qtes.FindOrAdd(QteId);
	Qte.bAuto = GetMode(TEXT("input.qte_mode"), TEXT("normal")) == TEXT("auto");
	Qte.WindowSeconds = GetQteWindowSeconds(AuthoredWindowSeconds, bNothingElseMoves);
	Qte.Elapsed = 0.f;
	Qte.State = EA11yQteState::Pending;
}

EA11yQteState UA11yInputAssistComponent::TickQte(FName QteId, float DeltaSeconds, bool bCorrectInputThisFrame)
{
	FQteRuntime* Qte = Qtes.Find(QteId);
	if (!Qte)
	{
		return EA11yQteState::Inactive;
	}
	if (Qte->State != EA11yQteState::Pending)
	{
		return Qte->State;
	}
	Qte->Elapsed += FMath::Max(0.f, DeltaSeconds);

	if (bCorrectInputThisFrame)
	{
		Qte->State = EA11yQteState::Succeeded;
	}
	else if (Qte->bAuto && Qte->Elapsed >= Qte->WindowSeconds)
	{
		Qte->State = EA11yQteState::Succeeded; // auto mode: short readable beat, then the normal success branch
	}
	else if (!Qte->bAuto && Qte->WindowSeconds >= 0.f && Qte->Elapsed >= Qte->WindowSeconds)
	{
		Qte->State = EA11yQteState::Failed;
	}
	return Qte->State;
}

void UA11yInputAssistComponent::EndQte(FName QteId)
{
	Qtes.Remove(QteId);
}

// ---- Aim assistance -----------------------------------------------------------------------------

float UA11yInputAssistComponent::GetUnitFloat(FName Key) const
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this);
	return Settings ? FMath::Clamp(Settings->GetFloat(Key, 0.f), 0.f, 1.f) : 0.f;
}

float UA11yInputAssistComponent::GetAimSlowdownStrength() const
{
	return GetUnitFloat(TEXT("input.aim_assist"));
}

float UA11yInputAssistComponent::GetAimPullStrength() const
{
	return GetUnitFloat(TEXT("input.aim_pull"));
}

float UA11yInputAssistComponent::GetAimStabilizationStrength() const
{
	return GetUnitFloat(TEXT("input.aim_stabilization"));
}

// ---- Timing windows -----------------------------------------------------------------------------

float UA11yInputAssistComponent::TimingWindow(float BaseSeconds) const
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this);
	return BaseSeconds * (Settings ? Settings->GetFloat(TEXT("input.timing_window_scale"), 1.f) : 1.f);
}
