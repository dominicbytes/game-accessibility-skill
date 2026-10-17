// SPDX-License-Identifier: MIT
// game-accessibility skill: FlashGuard (architecture.md section 8).
// NOT COMPILED by the author; please report compile errors.
//
// Every full-screen flash, strobe, lightning strike, hit flash and muzzle flash asks RequestFlash first.
//
// Counting rule (conservative sample defaults; tune ONLY with captured-output analysis of your game):
// a request counts toward the budgets if ScreenArea >= 0.25 OR Intensity >= 0.1 OR bSaturatedRed.
// 0.1 is the WCAG general-flash luminance change. Small, moderately bright flashes therefore still
// count: when the real rendered area and luminance are unknown, assume the flash may be hazardous.
// Budgets, in EVERY mode and whatever the configuration: no more than 3 counted flashes in any 1-second
// window AND no more than 9 in any 5-second window (real time). The second limit blocks a sustained
// 2-3 Hz strobe. Configured budgets above 3 and 9 are clamped down at use.
// Saturated red is ALWAYS desaturated (bDesaturate), whatever the mode or count. Use IsSaturatedRed on a
// LINEAR colour to classify it. With motion.reduce_flashing on, flashes become capped, slow fades.
// This is one prevention layer, not a photosensitivity proof: it cannot see flashes baked into video,
// textures or overlapping shaders, and it cannot measure what was rendered. Analyse captured footage
// (for example EA IRIS) as a separate gate.

#pragma once

#include "CoreMinimal.h"
#include "Subsystems/GameInstanceSubsystem.h"
#include "A11yTypes.h"
#include "A11yFlashGuardSubsystem.generated.h"

USTRUCT(BlueprintType)
struct GAMEACCESSIBILITY_API FA11yFlashResult
{
	GENERATED_BODY()

	UPROPERTY(BlueprintReadOnly, Category = "Accessibility|Flash")
	EA11yFlashStyle Style = EA11yFlashStyle::Flash;

	/** Use this instead of the requested intensity (0..1). */
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility|Flash")
	float Intensity = 0.f;

	/** Ramp in and out over at least this long. 0 = as authored. */
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility|Flash")
	float MinFadeSeconds = 0.f;

	/** Replace saturated red with a desaturated colour. */
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility|Flash")
	bool bDesaturate = false;
};

UCLASS()
class GAMEACCESSIBILITY_API UA11yFlashGuardSubsystem : public UGameInstanceSubsystem
{
	GENERATED_BODY()

public:
	virtual void Initialize(FSubsystemCollectionBase& Collection) override;

	/**
	 * Intensity: 0..1, the size of the luminance change the effect would produce (requested, before any cap).
	 * ScreenArea: 0..1, the fraction of the screen it covers. Pass 1 when unknown.
	 * bSaturatedRed: the flash is a saturated red (always counted and always desaturated).
	 * One call = one flash (one on-off pair). Ask again for every repeat of a strobe.
	 */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Flash")
	FA11yFlashResult RequestFlash(float Intensity, float ScreenArea, bool bSaturatedRed);

	/**
	 * WCAG saturated red: R / (R + G + B) >= 0.8, computed on LINEAR channels. FLinearColor is linear; convert
	 * an sRGB FColor with FLinearColor(FColor) first. Never test encoded sRGB channel ratios.
	 */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Flash")
	static bool IsSaturatedRed(const FLinearColor& LinearColor);

	/** True when a request with these values counts toward the budgets (see the counting rule above). */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Flash")
	bool CountsTowardBudget(float Intensity, float ScreenArea, bool bSaturatedRed) const;

	UFUNCTION(BlueprintPure, Category = "Accessibility|Flash")
	int32 GetCountedFlashesInLastSecond() const;

	UFUNCTION(BlueprintPure, Category = "Accessibility|Flash")
	int32 GetCountedFlashesInLastFiveSeconds() const;

	/**
	 * Counting thresholds: conservative SAMPLE DEFAULTS, not a standard's definitions. A request counts if ANY
	 * is met. Raise them only with evidence from captured-output analysis of your own effects; lowering them
	 * is always safe. Clamped to 0..1 at use.
	 */
	UPROPERTY(BlueprintReadWrite, Category = "Accessibility|Flash")
	float LargeAreaFraction = 0.25f;

	UPROPERTY(BlueprintReadWrite, Category = "Accessibility|Flash")
	float BrightIntensityThreshold = 0.1f;

	/** Clamped to 0..3 at use: the hard limit applies in every mode, whatever this is set to. */
	UPROPERTY(BlueprintReadWrite, Category = "Accessibility|Flash")
	int32 MaxCountedFlashesPerSecond = 3;

	/** Clamped to 0..9 at use: the sustained-strobe limit applies in every mode, whatever this is set to. */
	UPROPERTY(BlueprintReadWrite, Category = "Accessibility|Flash")
	int32 MaxCountedFlashesPerFiveSeconds = 9;

	UPROPERTY(BlueprintReadWrite, Category = "Accessibility|Flash")
	float ReducedIntensityCap = 0.2f;

	UPROPERTY(BlueprintReadWrite, Category = "Accessibility|Flash")
	float ReducedMinFadeSeconds = 0.5f;

private:
	void Prune(double Now) const;
	int32 CountSince(double Now, double WindowSeconds) const;

	/** Real-time stamps (FPlatformTime::Seconds) of counted flashes. Mutable so const queries can prune. */
	mutable TArray<double> CountedFlashTimes;
};
