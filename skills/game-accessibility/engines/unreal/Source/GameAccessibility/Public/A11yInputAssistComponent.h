// SPDX-License-Identifier: MIT
// game-accessibility skill: InputAssist (architecture.md section 3).
// NOT COMPILED by the author; please report compile errors.
//
// Works on logical action names. Game code forwards Enhanced Input events (Started/Completed) or any
// other logical input into these functions; this component never reads physical keys.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "A11yTypes.h"
#include "A11yInputAssistComponent.generated.h"

DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FA11yHeldChangedSignature, FName, Action, bool, bHeld);

UCLASS(ClassGroup = (Accessibility), meta = (BlueprintSpawnableComponent))
class GAMEACCESSIBILITY_API UA11yInputAssistComponent : public UActorComponent
{
	GENERATED_BODY()

public:
	UA11yInputAssistComponent();

	// ---- Hold / toggle (input.hold_mode) -------------------------------------------------------

	/**
	 * Forward every press (true) and release (false) of a holdable action. Returns the logical held state.
	 * hold: held while pressed. toggle: first press latches, second press releases.
	 */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Input")
	bool HandleHoldInput(FName Action, bool bPressed);

	UFUNCTION(BlueprintPure, Category = "Accessibility|Input")
	bool IsHeld(FName Action) const;

	/** Call on pause, death, scene change. Focus loss and device disconnect are handled automatically. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Input")
	void ClearAllLatches();

	UPROPERTY(BlueprintAssignable, Category = "Accessibility|Input")
	FA11yHeldChangedSignature OnHeldChanged;

	// ---- Repeated presses (input.mash_mode) ----------------------------------------------------

	/** HoldSecondsForFull: how long a hold fills the bar in "hold" mode. "single" mode completes on one press. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Input")
	void BeginMash(FName ChallengeId, int32 RequiredPresses, float HoldSecondsForFull = 2.f);

	/** Call on each press. Returns progress 0..1. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Input")
	float RegisterMashPress(FName ChallengeId);

	/** Call every frame with the held state; only "hold" mode uses it. Returns progress 0..1. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Input")
	float TickMashHold(FName ChallengeId, bool bHeld, float DeltaSeconds);

	UFUNCTION(BlueprintPure, Category = "Accessibility|Input")
	float GetMashProgress(FName ChallengeId) const;

	UFUNCTION(BlueprintCallable, Category = "Accessibility|Input")
	void EndMash(FName ChallengeId);

	// ---- Quick-time events (input.qte_mode) ----------------------------------------------------

	/**
	 * normal: authored window. slow: window x SlowQteScale (>= 3), or no limit when bNothingElseMoves.
	 * auto: succeeds after AutoQteBeatSeconds through the normal success branch.
	 */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Input")
	void BeginQte(FName QteId, float AuthoredWindowSeconds, bool bNothingElseMoves);

	/** Call every frame with the simulation-time delta (World->GetDeltaSeconds()). On Succeeded run the same success branch as a real press. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Input")
	EA11yQteState TickQte(FName QteId, float DeltaSeconds, bool bCorrectInputThisFrame);

	/** Window for the current mode. Returns a negative value for "no time limit". */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Input")
	float GetQteWindowSeconds(float AuthoredWindowSeconds, bool bNothingElseMoves) const;

	UFUNCTION(BlueprintCallable, Category = "Accessibility|Input")
	void EndQte(FName QteId);

	// ---- Timing windows (input.timing_window_scale) --------------------------------------------

	/**
	 * base x input.timing_window_scale, measured in SIMULATION time (the dilated world clock) for parry,
	 * dodge, rhythm and perfect-hit windows. In real time it then lasts base x window_scale / time_scale,
	 * so game speed and window scale combine once each. Never also multiply by the time scale, and never
	 * measure the window with an undilated real-time clock.
	 */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Input")
	float TimingWindow(float BaseSeconds) const;

	// ---- Aim assistance (three independent keys) ----------------------------------------------

	/** input.aim_assist (0..1): SLOWDOWN only. Reduce look sensitivity while the reticle is over a target. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Input")
	float GetAimSlowdownStrength() const;

	/** input.aim_pull (0..1): gentle pull toward a nearby target. Independent of slowdown. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Input")
	float GetAimPullStrength() const;

	/** input.aim_stabilization (0..1): smooth small unintended input and reduce recoil/sway while aiming. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Input")
	float GetAimStabilizationStrength() const;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Accessibility|Input", meta = (ClampMin = "3.0"))
	float SlowQteScale = 3.f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Accessibility|Input", meta = (ClampMin = "0.0"))
	float AutoQteBeatSeconds = 0.6f;

protected:
	virtual void BeginPlay() override;
	virtual void EndPlay(const EEndPlayReason::Type EndPlayReason) override;

	UFUNCTION()
	void HandleSettingChanged(FName Key, const FString& Value);

private:
	struct FHoldState
	{
		bool bPhysical = false;
		bool bLatched = false;
	};
	struct FMashState
	{
		int32 RequiredPresses = 1;
		float HoldSecondsForFull = 2.f;
		float Progress = 0.f;
	};
	struct FQteRuntime
	{
		float WindowSeconds = 0.f; // negative = no limit
		float Elapsed = 0.f;
		bool bAuto = false;
		EA11yQteState State = EA11yQteState::Inactive;
	};

	bool IsToggleMode() const;
	FString GetMode(FName Key, const TCHAR* Fallback) const;
	float GetUnitFloat(FName Key) const;

	TMap<FName, FHoldState> HoldStates;
	TMap<FName, FMashState> Mashes;
	TMap<FName, FQteRuntime> Qtes;
	FDelegateHandle AppActivationHandle;
	FDelegateHandle DeviceConnectionHandle;
};
