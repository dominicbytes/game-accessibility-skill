// SPDX-License-Identifier: MIT
// game-accessibility skill: CameraComfort (architecture.md section 8).
// NOT COMPILED by the author; please report compile errors.
//
// Add to the player pawn (or the PlayerController). It:
//  - scales EVERY active camera shake on the owning PlayerCameraManager each frame by motion.camera_shake
//    (or motion.head_bob for classes listed in HeadBobShakeClasses), using UCameraShakeBase::ShakeScale on
//    instances read from UCameraModifier_CameraShake::GetActiveCameraShakes. This also covers shakes started
//    elsewhere (UCameraShakeSourceComponent, PlayWorldCameraShake, Sequencer) as long as they run through
//    the camera manager. Verify Sequencer shakes in your engine version.
//  - drives camera post-process overrides from the authored values, each from its own key:
//    MotionBlurAmount (motion.motion_blur), SceneFringeIntensity (motion.chromatic_aberration),
//    VignetteIntensity (motion.screen_effects) and depth of field (motion.depth_of_field).
//    Depth of field: define A11Y_UE_HAS_DOF_ENABLED=1 if your FPostProcessSettings has DepthOfFieldEnabled
//    (listed in current API docs; absent from the 5.0-5.4 Python docs). Otherwise the component sets the
//    global r.DepthOfFieldQuality cvar to 0 and restores the previous value when the option is turned on.
//  - sets the camera FOV from motion.fov (vertical degrees in the registry; UE FieldOfView is horizontal
//    under the default AspectRatio_MaintainXFOV, so it converts using the viewport aspect).

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "Camera/CameraShakeBase.h"
#include "Templates/SubclassOf.h"
#include "A11yCameraComfortComponent.generated.h"

class UCameraComponent;
class UCameraShakeBase;
class APlayerCameraManager;

UCLASS(ClassGroup = (Accessibility), meta = (BlueprintSpawnableComponent))
class GAMEACCESSIBILITY_API UA11yCameraComfortComponent : public UActorComponent
{
	GENERATED_BODY()

public:
	UA11yCameraComfortComponent();

	/** Starts a shake with AuthoredScale x motion.camera_shake (or head_bob). Returns null when scaled to 0. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Camera")
	UCameraShakeBase* PlayCameraShake(TSubclassOf<UCameraShakeBase> ShakeClass, float AuthoredScale = 1.f);

	/** For procedural shake/bob code that does not use UCameraShakeBase. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Camera")
	float GetShakeScale() const;

	UFUNCTION(BlueprintPure, Category = "Accessibility|Camera")
	float GetHeadBobScale() const;

	/** Multiply damage overlays, distortion and other UMG/material screen effects by this. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Camera")
	float GetScreenEffectsScale() const;

	/** Stop automatic camera swings, zoom pulses, parallax and animated menus when true. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Camera")
	bool IsReduceMotionOn() const;

	UFUNCTION(BlueprintCallable, Category = "Accessibility|Camera")
	void SetCamera(UCameraComponent* InCamera);

	/** Re-applies post-process and FOV. Call after a viewport resize or a camera swap. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Camera")
	void ApplyNow();

	/** Shakes of these classes count as head bob and use motion.head_bob. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Accessibility|Camera")
	TArray<TSubclassOf<UCameraShakeBase>> HeadBobShakeClasses;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Accessibility|Camera")
	bool bScaleAllActiveShakes = true;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Accessibility|Camera")
	bool bApplyPostProcess = true;

	/** Disable for fixed-camera or 2D games that do not offer motion.fov. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Accessibility|Camera")
	bool bApplyFieldOfView = true;

	/** True: motion.fov is vertical degrees and is converted to UE's horizontal FieldOfView. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Accessibility|Camera")
	bool bFovSettingIsVertical = true;

	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

protected:
	virtual void BeginPlay() override;
	virtual void EndPlay(const EEndPlayReason::Type EndPlayReason) override;

	UFUNCTION()
	void HandleSettingChanged(FName Key, const FString& Value);

private:
	struct FShakeScaleRecord
	{
		float Authored = 1.f;
		float Written = 1.f;
	};

	APlayerCameraManager* GetCameraManager() const;
	float ScaleForShake(const UCameraShakeBase* Shake) const;
	void ScaleActiveShakes();
	void ApplyPostProcess();
	void ApplyFieldOfView();

	void ApplyDepthOfField(bool bEnabled);

	UPROPERTY(Transient)
	TObjectPtr<UCameraComponent> Camera;

	int32 SavedDofQuality = -1;

	TMap<TWeakObjectPtr<UCameraShakeBase>, FShakeScaleRecord> ShakeScales;

	bool bCapturedBase = false;
	float BaseMotionBlurAmount = 0.f;
	float BaseSceneFringeIntensity = 0.f;
	float BaseVignetteIntensity = 0.f;
};
