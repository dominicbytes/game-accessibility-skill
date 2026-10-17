// SPDX-License-Identifier: MIT
// game-accessibility skill: FlashGuard. NOT COMPILED by the author; please report compile errors.

#include "A11yFlashGuardSubsystem.h"
#include "A11ySettingsSubsystem.h"
#include "Engine/GameInstance.h"
#include "HAL/PlatformTime.h"

namespace A11yFlashGuardPrivate
{
	static constexpr double ShortWindowSeconds = 1.0;
	static constexpr double LongWindowSeconds = 5.0;
}

void UA11yFlashGuardSubsystem::Initialize(FSubsystemCollectionBase& Collection)
{
	Collection.InitializeDependency(UA11ySettingsSubsystem::StaticClass());
	Super::Initialize(Collection);
}

void UA11yFlashGuardSubsystem::Prune(double Now) const
{
	// Real time, not game time: slow motion or pause does not make flashes safer for the viewer.
	CountedFlashTimes.RemoveAll([Now](double T) { return Now - T >= A11yFlashGuardPrivate::LongWindowSeconds; });
}

int32 UA11yFlashGuardSubsystem::CountSince(double Now, double WindowSeconds) const
{
	int32 Count = 0;
	for (const double T : CountedFlashTimes)
	{
		Count += (Now - T < WindowSeconds) ? 1 : 0;
	}
	return Count;
}

int32 UA11yFlashGuardSubsystem::GetCountedFlashesInLastSecond() const
{
	const double Now = FPlatformTime::Seconds();
	Prune(Now);
	return CountSince(Now, A11yFlashGuardPrivate::ShortWindowSeconds);
}

int32 UA11yFlashGuardSubsystem::GetCountedFlashesInLastFiveSeconds() const
{
	const double Now = FPlatformTime::Seconds();
	Prune(Now);
	return CountSince(Now, A11yFlashGuardPrivate::LongWindowSeconds);
}

bool UA11yFlashGuardSubsystem::IsSaturatedRed(const FLinearColor& LinearColor)
{
	// Linear channels (FLinearColor). Negative values from HDR maths are treated as 0.
	const float R = FMath::Max(0.f, LinearColor.R);
	const float G = FMath::Max(0.f, LinearColor.G);
	const float B = FMath::Max(0.f, LinearColor.B);
	const float Sum = R + G + B;
	return Sum > KINDA_SMALL_NUMBER && R / Sum >= 0.8f;
}

bool UA11yFlashGuardSubsystem::CountsTowardBudget(float Intensity, float ScreenArea, bool bSaturatedRed) const
{
	// Conservative: ANY condition counts. The requested intensity is used, not the reduced-mode cap, so a
	// softened flash still spends budget.
	return bSaturatedRed
		|| FMath::Clamp(ScreenArea, 0.f, 1.f) >= FMath::Clamp(LargeAreaFraction, 0.f, 1.f)
		|| FMath::Clamp(Intensity, 0.f, 1.f) >= FMath::Clamp(BrightIntensityThreshold, 0.f, 1.f);
}

FA11yFlashResult UA11yFlashGuardSubsystem::RequestFlash(float Intensity, float ScreenArea, bool bSaturatedRed)
{
	const double Now = FPlatformTime::Seconds();
	Prune(Now);

	FA11yFlashResult Result;
	Result.Style = EA11yFlashStyle::Flash;
	Result.Intensity = FMath::Clamp(Intensity, 0.f, 1.f);
	Result.bDesaturate = bSaturatedRed; // always, in every mode, whether or not the flash is allowed

	const UA11ySettingsSubsystem* Settings = GetGameInstance() ? GetGameInstance()->GetSubsystem<UA11ySettingsSubsystem>() : nullptr;
	if (Settings && Settings->GetBool(TEXT("motion.reduce_flashing"), false))
	{
		Result.Style = EA11yFlashStyle::SoftFade;
		Result.Intensity = FMath::Min(Result.Intensity, FMath::Max(0.f, ReducedIntensityCap));
		Result.MinFadeSeconds = FMath::Max(0.f, ReducedMinFadeSeconds);
	}

	if (!CountsTowardBudget(Intensity, ScreenArea, bSaturatedRed))
	{
		return Result;
	}

	// Hard budgets: never above 3 per 1 s or 9 per 5 s, whatever the configuration.
	const int32 ShortLimit = FMath::Clamp(MaxCountedFlashesPerSecond, 0, 3);
	const int32 LongLimit = FMath::Clamp(MaxCountedFlashesPerFiveSeconds, 0, 9);
	const int32 InShort = CountSince(Now, A11yFlashGuardPrivate::ShortWindowSeconds);
	const int32 InLong = CountSince(Now, A11yFlashGuardPrivate::LongWindowSeconds);
	if (InShort >= ShortLimit || InLong >= LongLimit)
	{
		Result.Style = EA11yFlashStyle::Suppressed;
		Result.Intensity = 0.f;
		Result.MinFadeSeconds = 0.f;
		UE_LOG(LogA11y, Verbose, TEXT("FlashGuard suppressed a flash (%d in 1 s, %d in 5 s)."), InShort, InLong);
		return Result;
	}
	CountedFlashTimes.Add(Now);
	return Result;
}
