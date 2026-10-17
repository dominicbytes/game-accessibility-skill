// SPDX-License-Identifier: MIT
// game-accessibility skill: ColorTokens (architecture.md section 9).
// NOT COMPILED by the author; please report compile errors.
//
// UI and gameplay colours are semantic tokens (team.ally, hazard, pickup.rare), each with a symbol that is
// always shown. visual.color_profile (none/protan/deutan/tritan/custom) picks a per-token palette.
// "custom" reads the player's own colours from visual.custom_colors ("role=#RRGGBB;..."), where role is
// the token name; tokens the player did not set fall back to ProfileColors["custom"], then Default.
// The engine's full-window colour-vision correction is an OPTIONAL EXTRA, never the main solution.
// Per-class outlines (visual.outline_enemies/allies/interactables/hazards) resolve through ShouldOutline:
// each class key is "follow" by default, which uses the visual.high_contrast umbrella.

#pragma once

#include "CoreMinimal.h"
#include "Engine/DataAsset.h"
#include "Kismet/BlueprintFunctionLibrary.h"
#include "UObject/SoftObjectPtr.h"
#include "A11yTypes.h"
#include "A11yColorTokens.generated.h"

class UTexture2D;

USTRUCT(BlueprintType)
struct GAMEACCESSIBILITY_API FA11yColorToken
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Accessibility|Color")
	FLinearColor Default = FLinearColor::White;

	/** Keyed by visual.color_profile value: protan, deutan, tritan, custom. Missing entries use Default. */
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Accessibility|Color")
	TMap<FName, FLinearColor> ProfileColors;

	/** Paired shape, icon or pattern. Always shown, whatever the palette. */
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Accessibility|Color")
	TSoftObjectPtr<UTexture2D> Symbol;

	/** Text alternative for the symbol (for narration and tooltips). */
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Accessibility|Color")
	FText SymbolLabel;
};

UCLASS(BlueprintType)
class GAMEACCESSIBILITY_API UA11yColorTokens : public UDataAsset
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Accessibility|Color")
	TMap<FName, FA11yColorToken> Tokens;

	UFUNCTION(BlueprintPure, Category = "Accessibility|Color")
	FLinearColor GetColor(FName Token, FName Profile) const;

	UFUNCTION(BlueprintPure, Category = "Accessibility|Color")
	bool GetToken(FName Token, FA11yColorToken& OutToken) const;
};

UCLASS()
class GAMEACCESSIBILITY_API UA11yColorLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:
	/** Parses "role=#RRGGBB;role2=#RRGGBB" and returns the colour for Role. False if absent or malformed. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Color")
	static bool TryGetCustomColor(const FString& CustomColorsSpec, FName Role, FLinearColor& OutColor);

	/** Token colour for the player's current visual.color_profile. Magenta if the token is missing. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Color", meta = (WorldContext = "WorldContextObject"))
	static FLinearColor ResolveTokenColor(const UObject* WorldContextObject, const UA11yColorTokens* Palette, FName Token);

	/**
	 * Outline for one class: OutlineKey is visual.outline_enemies, visual.outline_allies,
	 * visual.outline_interactables or visual.outline_hazards. "on"/"off" win; "follow" or an unregistered key
	 * uses the visual.high_contrast umbrella. Drive SetRenderCustomDepth (or your outline material) with it,
	 * and refresh when OnSettingChanged reports the class key or visual.high_contrast.
	 */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Color", meta = (WorldContext = "WorldContextObject"))
	static bool ShouldOutline(const UObject* WorldContextObject, FName OutlineKey);

	/**
	 * OPTIONAL full-window correction through UWidgetBlueprintLibrary::SetColorVisionDeficiencyType.
	 * Strength01 is the player's 0..1 strength. EngineSeverityMax maps it onto the engine's Severity range.
	 *
	 * SEVERITY RANGE TRAP: the Blueprint reference documents Severity as 0..1, while Lyra's options UI exposes
	 * a 0..10 "strength". Before shipping, open WidgetBlueprintLibrary.cpp in YOUR engine version, check how
	 * Severity is clamped, and set EngineSeverityMax to match (1.0 for a 0..1 clamp, 10.0 for 0..10).
	 * Never pass a menu value straight through. Never enable ShowCorrectionWithDeficiency for players: that is
	 * a developer simulation preview.
	 */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Color")
	static void ApplyOptionalCvdFilter(EA11yCvdFilterType Type, float Strength01, float EngineSeverityMax = 1.f);
};
