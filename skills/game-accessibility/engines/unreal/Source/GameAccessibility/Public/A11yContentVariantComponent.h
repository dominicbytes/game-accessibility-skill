// SPDX-License-Identifier: MIT
// game-accessibility skill: ContentFilter (architecture.md section 7).
// NOT COMPILED by the author; please report compile errors.
//
// Put presentation variants (meshes, sprites, decals) as direct children of this scene component and
// give each a component tag "A11yVariant.<value>", for example "A11yVariant.0" ... "A11yVariant.3" for
// content.spiders, or "A11yVariant.full" / "A11yVariant.reduced" / "A11yVariant.off" for content.blood.
// Exactly one variant is visible. Collision, hitboxes, AI, damage and rewards stay on the actor and are
// never part of a variant. Applied in OnRegister so the original is not rendered for even one frame.
// If the exact variant is missing, the next MORE filtered authored variant is shown (higher int, or a
// later value in the registry's enum order); if none exists, all tagged variants are hidden.

#pragma once

#include "CoreMinimal.h"
#include "Components/SceneComponent.h"
#include "A11yContentVariantComponent.generated.h"

DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FA11yVariantChangedSignature, const FString&, ActiveVariant);

UCLASS(ClassGroup = (Accessibility), meta = (BlueprintSpawnableComponent))
class GAMEACCESSIBILITY_API UA11yContentVariantComponent : public USceneComponent
{
	GENERATED_BODY()

public:
	/** content.spiders, content.blood, or a project-specific content key. */
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Accessibility|Content")
	FName SettingKey = TEXT("content.spiders");

	/** Swap sound sets, animation sets, icons and portraits here. Empty string = nothing shown. */
	UPROPERTY(BlueprintAssignable, Category = "Accessibility|Content")
	FA11yVariantChangedSignature OnVariantChanged;

	UFUNCTION(BlueprintCallable, Category = "Accessibility|Content")
	void Refresh();

	UFUNCTION(BlueprintPure, Category = "Accessibility|Content")
	FString GetActiveVariant() const { return ActiveVariant; }

protected:
	virtual void OnRegister() override;
	virtual void BeginPlay() override;
	virtual void EndPlay(const EEndPlayReason::Type EndPlayReason) override;

	UFUNCTION()
	void HandleSettingChanged(FName Key, const FString& Value);

private:
	static bool GetVariantTag(const USceneComponent* Component, FString& OutValue);
	FString ChooseVariant(const FString& Requested, const TArray<FString>& Authored) const;

	FString ActiveVariant;
	bool bHasApplied = false;
};
