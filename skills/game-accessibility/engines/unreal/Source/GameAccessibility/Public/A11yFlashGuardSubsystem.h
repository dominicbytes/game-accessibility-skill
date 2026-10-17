// SPDX-License-Identifier: MIT
// game-accessibility skill: FlashGuard (architecture.md section 8).
// NOT COMPILED by the author; please report compile errors.
//
// Every full-screen flash, strobe, lightning strike, hit flash and muzzle flash asks RequestFlash first.
// In EVERY mode, large or bright flashes are limited to no more than 3 in any 1-second window AND no
// more than 9 in any 5-second window (real time). The second limit blocks a sustained 2-3 Hz strobe.
// Saturated red flashes are desaturated in every mode. With motion.reduce_flashing on, flashes become
// capped, slow fades. This is one prevention layer, not a photosensitivity proof: it cannot see flashes
// baked into video, textures or overlapping shaders. Analyse captured footage (for example EA IRIS) too.

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
	 * Intensity: 0..1, the size of the luminance change the effect would produce.
	 * ScreenArea: 0..1, the fraction of the screen it covers.
	 * bSaturatedRed: the flash is a saturated red (counted and desaturated).
	 * One call = one flash (one on-off pair). Ask again for every repeat of a strobe.
	 */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Flash")
	FA11yFlashResult RequestFlash(float Intensity, float ScreenArea, bool bSaturatedRed);

	UFUNCTION(BlueprintPure, Category = "Accessibility|Flash")
	int32 GetCountedFlashesInLastSecond() const;

	UFUNCTION(BlueprintPure, Category = "Accessibility|Flash")
	int32 GetCountedFlashesInLastFiveSeconds() const;

	/** Heuristic thresholds (UNVERIFIED as a substitute for a standard's definitions). Counted if any is met. */
	UPROPERTY(BlueprintReadWrite, Category = "Accessibility|Flash")
	float LargeAreaFraction = 0.25f;

	UPROPERTY(BlueprintReadWrite, Category = "Accessibility|Flash")
	float BrightIntensityThreshold = 0.5f;

	/** Values above 3 are clamped to 3: the hard limit applies in every mode. */
	UPROPERTY(BlueprintReadWrite, Category = "Accessibility|Flash")
	int32 MaxCountedFlashesPerSecond = 3;

	/** Values above 9 are clamped to 9: the sustained-strobe limit applies in every mode. */
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
