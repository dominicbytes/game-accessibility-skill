// SPDX-License-Identifier: MIT
// game-accessibility skill: GuidanceLayer (architecture.md section 6).
// NOT COMPILED by the author; please report compile errors.
//
// Attach guidance visuals (painted-ledge decals, trail meshes, highlight meshes, markers) as children of
// this scene component. Channel is the per-channel setting key (guidance.paint, guidance.trail,
// guidance.highlights, guidance.objective_marker). The effective level is that key's value unless it is
// "follow" (the default) or the key is absent, in which case guidance.level applies. So a player can keep
// trails and turn paint off. Levels:
//   off -> hidden, on_request -> hidden until RequestHint, subtle -> SubtleIntensity,
//   standard -> 1.0, boosted -> BoostedIntensity plus custom-depth outline.
// bIsHazardCue: lethal-edge markers, attack telegraphs and interaction prompts. Always visible.

#pragma once

#include "CoreMinimal.h"
#include "Components/SceneComponent.h"
#include "TimerManager.h"
#include "A11yGuidanceComponent.generated.h"

DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FA11yGuidanceChangedSignature, bool, bVisible, float, Intensity);

UCLASS(ClassGroup = (Accessibility), meta = (BlueprintSpawnableComponent))
class GAMEACCESSIBILITY_API UA11yGuidanceComponent : public USceneComponent
{
	GENERATED_BODY()

public:
	/** Per-channel key: guidance.paint, guidance.trail, guidance.highlights, guidance.objective_marker, or your own. */
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Accessibility|Guidance")
	FName Channel = TEXT("guidance.paint");

	/** Hazard warnings are not guidance: never hidden, never dimmed. */
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Accessibility|Guidance")
	bool bIsHazardCue = false;

	/** Scalar parameter set on child mesh materials. Decals and Niagara: bind OnGuidanceChanged. */
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Accessibility|Guidance")
	FName IntensityParameterName = TEXT("GuidanceIntensity");

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Accessibility|Guidance")
	float SubtleIntensity = 0.4f;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Accessibility|Guidance")
	float BoostedIntensity = 1.5f;

	/** Turns on Render CustomDepth for child primitives at "boosted" (pair with an outline post-process material). */
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Accessibility|Guidance")
	bool bOutlineWhenBoosted = true;

	UPROPERTY(BlueprintAssignable, Category = "Accessibility|Guidance")
	FA11yGuidanceChangedSignature OnGuidanceChanged;

	/** Hint action handler: shows "on_request" guidance in this world for Seconds. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Guidance", meta = (WorldContext = "WorldContextObject"))
	static void RequestHint(const UObject* WorldContextObject, float Seconds = 5.f);

	UFUNCTION(BlueprintCallable, Category = "Accessibility|Guidance")
	void ShowHint(float Seconds);

	UFUNCTION(BlueprintCallable, Category = "Accessibility|Guidance")
	void Refresh();

protected:
	virtual void BeginPlay() override;
	virtual void EndPlay(const EEndPlayReason::Type EndPlayReason) override;

	UFUNCTION()
	void HandleSettingChanged(FName Key, const FString& Value);

private:
	void EndHint();

	bool bHintActive = false;
	FTimerHandle HintTimer;
};
