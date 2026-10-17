// SPDX-License-Identifier: MIT
// game-accessibility skill: CameraComfort. NOT COMPILED by the author; please report compile errors.

#include "A11yCameraComfortComponent.h"
#include "A11yDepthOfFieldSubsystem.h"
#include "A11ySettingsSubsystem.h"

#include "Camera/CameraComponent.h"
#include "Camera/CameraModifier_CameraShake.h"
#include "Camera/CameraShakeBase.h"
#include "Camera/PlayerCameraManager.h"
#include "Engine/Engine.h"
#include "Engine/GameViewportClient.h"
#include "GameFramework/Pawn.h"
#include "GameFramework/PlayerController.h"

#ifndef A11Y_UE_HAS_DOF_ENABLED
#define A11Y_UE_HAS_DOF_ENABLED 0
#endif

UA11yCameraComfortComponent::UA11yCameraComfortComponent()
{
	PrimaryComponentTick.bCanEverTick = true;
	// The camera manager updates after the tick groups. Scaling in TG_PostUpdateWork applies to the same frame.
	PrimaryComponentTick.TickGroup = TG_PostUpdateWork;
}

void UA11yCameraComfortComponent::BeginPlay()
{
	Super::BeginPlay();
	if (!Camera && GetOwner())
	{
		Camera = GetOwner()->FindComponentByClass<UCameraComponent>();
	}
	if (UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this))
	{
		Settings->OnSettingChanged.AddUniqueDynamic(this, &UA11yCameraComfortComponent::HandleSettingChanged);
	}
	ApplyNow(); // late subscriber: read current values first
}

void UA11yCameraComfortComponent::EndPlay(const EEndPlayReason::Type EndPlayReason)
{
	if (UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this))
	{
		Settings->OnSettingChanged.RemoveDynamic(this, &UA11yCameraComfortComponent::HandleSettingChanged);
	}
	// Hand the global DOF override back to its persistent owner. The owner restores the authored quality
	// only when no other component still holds a request, so a level change or overlap stays balanced.
	if (UA11yDepthOfFieldSubsystem* Dof = UA11yDepthOfFieldSubsystem::Get(this))
	{
		Dof->Release(this);
	}
	ShakeScales.Reset();
	Super::EndPlay(EndPlayReason);
}

void UA11yCameraComfortComponent::HandleSettingChanged(FName Key, const FString& /*Value*/)
{
	if (Key.ToString().StartsWith(TEXT("motion.")))
	{
		ApplyNow(); // shakes are rescaled every tick
	}
}

void UA11yCameraComfortComponent::SetCamera(UCameraComponent* InCamera)
{
	Camera = InCamera;
	bCapturedBase = false;
	ApplyNow();
}

void UA11yCameraComfortComponent::ApplyNow()
{
	if (bApplyPostProcess)
	{
		ApplyPostProcess();
	}
	if (bApplyFieldOfView)
	{
		ApplyFieldOfView();
	}
}

// ---- Scales -------------------------------------------------------------------------------------

float UA11yCameraComfortComponent::GetShakeScale() const
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this);
	return Settings ? Settings->GetFloat(TEXT("motion.camera_shake"), 1.f) : 1.f;
}

float UA11yCameraComfortComponent::GetHeadBobScale() const
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this);
	return Settings ? Settings->GetFloat(TEXT("motion.head_bob"), 1.f) : 1.f;
}

float UA11yCameraComfortComponent::GetWeaponSwayScale() const
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this);
	return Settings ? Settings->GetFloat(TEXT("motion.weapon_sway"), 1.f) : 1.f;
}

float UA11yCameraComfortComponent::GetScreenEffectsScale() const
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this);
	return Settings ? Settings->GetFloat(TEXT("motion.screen_effects"), 1.f) : 1.f;
}

bool UA11yCameraComfortComponent::IsReduceMotionOn() const
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this);
	return Settings && Settings->GetBool(TEXT("motion.reduce_motion"), false);
}

bool UA11yCameraComfortComponent::IsMotionEffectEnabled(const UObject* WorldContextObject, FName OverrideKey)
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	if (!Settings)
	{
		return true; // no store: as authored
	}
	return Settings->ResolveToggle(OverrideKey, !Settings->GetBool(TEXT("motion.reduce_motion"), false));
}

float UA11yCameraComfortComponent::ScaleForShake(const UCameraShakeBase* Shake) const
{
	for (const TSubclassOf<UCameraShakeBase>& BobClass : HeadBobShakeClasses)
	{
		if (Shake && *BobClass && Shake->IsA(BobClass))
		{
			return GetHeadBobScale();
		}
	}
	for (const TSubclassOf<UCameraShakeBase>& SwayClass : WeaponSwayShakeClasses)
	{
		if (Shake && *SwayClass && Shake->IsA(SwayClass))
		{
			return GetWeaponSwayScale();
		}
	}
	return GetShakeScale();
}

// ---- Shakes -------------------------------------------------------------------------------------

APlayerCameraManager* UA11yCameraComfortComponent::GetCameraManager() const
{
	APlayerController* PC = Cast<APlayerController>(GetOwner());
	if (!PC)
	{
		if (const APawn* Pawn = Cast<APawn>(GetOwner()))
		{
			PC = Cast<APlayerController>(Pawn->GetController());
		}
	}
	return PC ? PC->PlayerCameraManager.Get() : nullptr;
}

UCameraShakeBase* UA11yCameraComfortComponent::PlayCameraShake(TSubclassOf<UCameraShakeBase> ShakeClass, float AuthoredScale)
{
	APlayerCameraManager* CameraManager = GetCameraManager();
	if (!CameraManager || !*ShakeClass)
	{
		return nullptr;
	}
	const UCameraShakeBase* ShakeCDO = ShakeClass->GetDefaultObject<UCameraShakeBase>();
	const float Scale = ScaleForShake(ShakeCDO);
	if (Scale <= KINDA_SMALL_NUMBER)
	{
		return nullptr; // 0% means removed completely
	}
	UCameraShakeBase* Shake = CameraManager->StartCameraShake(ShakeClass, AuthoredScale * Scale, ECameraShakePlaySpace::CameraLocal, FRotator::ZeroRotator);
	if (Shake)
	{
		FShakeScaleRecord Record;
		Record.Authored = AuthoredScale;
		Record.Written = Shake->ShakeScale;
		ShakeScales.Add(TWeakObjectPtr<UCameraShakeBase>(Shake), Record);
	}
	return Shake;
}

void UA11yCameraComfortComponent::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);
	if (bScaleAllActiveShakes)
	{
		ScaleActiveShakes();
	}
}

void UA11yCameraComfortComponent::ScaleActiveShakes()
{
	APlayerCameraManager* CameraManager = GetCameraManager();
	UCameraModifier_CameraShake* ShakeModifier = CameraManager
		? Cast<UCameraModifier_CameraShake>(CameraManager->FindCameraModifierByClass(UCameraModifier_CameraShake::StaticClass()))
		: nullptr;
	if (!ShakeModifier)
	{
		return;
	}

	TArray<FActiveCameraShakeInfo> ActiveShakes;
	ShakeModifier->GetActiveCameraShakes(ActiveShakes);
	for (const FActiveCameraShakeInfo& Info : ActiveShakes)
	{
		UCameraShakeBase* Shake = Info.ShakeInstance.Get();
		if (!Shake)
		{
			continue;
		}
		const TWeakObjectPtr<UCameraShakeBase> Key(Shake);
		FShakeScaleRecord* Record = ShakeScales.Find(Key);
		// New instance, or a pooled instance restarted with a new scale: remember the authored value.
		if (!Record || !FMath::IsNearlyEqual(Shake->ShakeScale, Record->Written))
		{
			FShakeScaleRecord NewRecord;
			NewRecord.Authored = Shake->ShakeScale;
			Record = &ShakeScales.Add(Key, NewRecord);
		}
		Shake->ShakeScale = Record->Authored * ScaleForShake(Shake);
		Record->Written = Shake->ShakeScale;
	}

	for (auto It = ShakeScales.CreateIterator(); It; ++It)
	{
		if (!It.Key().IsValid())
		{
			It.RemoveCurrent();
		}
	}
}

// ---- Post process and FOV -----------------------------------------------------------------------

void UA11yCameraComfortComponent::ApplyPostProcess()
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this);
	if (!Camera || !Settings)
	{
		return;
	}
	FPostProcessSettings& PP = Camera->PostProcessSettings;
	if (!bCapturedBase)
	{
		// Authored strengths on the camera. Camera post-process settings blend over volumes, so
		// author these effects on the camera (or mirror this logic in your volumes).
		BaseMotionBlurAmount = PP.MotionBlurAmount;
		BaseSceneFringeIntensity = PP.SceneFringeIntensity;
		BaseVignetteIntensity = PP.VignetteIntensity;
		bCapturedBase = true;
	}

	if (Settings->HasKey(TEXT("motion.motion_blur")))
	{
		PP.bOverride_MotionBlurAmount = true;
		PP.MotionBlurAmount = Settings->GetBool(TEXT("motion.motion_blur"), true) ? BaseMotionBlurAmount : 0.f;
	}
	if (Settings->HasKey(TEXT("motion.chromatic_aberration")))
	{
		PP.bOverride_SceneFringeIntensity = true;
		PP.SceneFringeIntensity = Settings->GetBool(TEXT("motion.chromatic_aberration"), true) ? BaseSceneFringeIntensity : 0.f;
	}
	if (Settings->HasKey(TEXT("motion.screen_effects")))
	{
		PP.bOverride_VignetteIntensity = true;
		PP.VignetteIntensity = BaseVignetteIntensity * Settings->GetFloat(TEXT("motion.screen_effects"), 1.f);
	}
	if (Settings->HasKey(TEXT("motion.depth_of_field")))
	{
		ApplyDepthOfField(Settings->GetBool(TEXT("motion.depth_of_field"), true));
	}
}

void UA11yCameraComfortComponent::ApplyDepthOfField(bool bEnabled)
{
#if A11Y_UE_HAS_DOF_ENABLED
	if (Camera)
	{
		Camera->PostProcessSettings.bOverride_DepthOfFieldEnabled = true;
		Camera->PostProcessSettings.DepthOfFieldEnabled = bEnabled;
	}
#else
	// Global fallback. r.DepthOfFieldQuality is global, so this component never stores a restore value of
	// its own: the persistent GameInstance owner captures the authored quality once and reference-counts
	// requests across components and level changes.
	if (UA11yDepthOfFieldSubsystem* Dof = UA11yDepthOfFieldSubsystem::Get(this))
	{
		Dof->SetDisabledBy(this, !bEnabled);
	}
#endif
}

void UA11yCameraComfortComponent::ApplyFieldOfView()
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(this);
	if (!Camera || !Settings || !Settings->HasKey(TEXT("motion.fov")))
	{
		return;
	}
	float Fov = Settings->GetFloat(TEXT("motion.fov"), 75.f);
	if (bFovSettingIsVertical)
	{
		float Aspect = 16.f / 9.f;
		if (GEngine && GEngine->GameViewport)
		{
			FVector2D Size;
			GEngine->GameViewport->GetViewportSize(Size);
			if (Size.Y > 0.0)
			{
				Aspect = static_cast<float>(Size.X / Size.Y);
			}
		}
		// Split-screen: each player's view has its own aspect; adapt this for your layout.
		const float HalfVerticalRadians = FMath::DegreesToRadians(Fov) * 0.5f;
		Fov = FMath::RadiansToDegrees(2.f * FMath::Atan(FMath::Tan(HalfVerticalRadians) * Aspect));
	}
	Camera->SetFieldOfView(FMath::Clamp(Fov, 5.f, 170.f));
}
