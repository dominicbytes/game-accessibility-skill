// SPDX-License-Identifier: MIT
// game-accessibility skill: ColorTokens. NOT COMPILED by the author; please report compile errors.

#include "A11yColorTokens.h"
#include "A11ySettingsSubsystem.h"
#include "Blueprint/WidgetBlueprintLibrary.h"

FLinearColor UA11yColorTokens::GetColor(FName Token, FName Profile) const
{
	const FA11yColorToken* Entry = Tokens.Find(Token);
	if (!Entry)
	{
		UE_LOG(LogA11y, Warning, TEXT("Colour token '%s' is missing."), *Token.ToString());
		return FLinearColor(1.f, 0.f, 1.f, 1.f);
	}
	if (!Profile.IsNone() && Profile != FName(TEXT("none")))
	{
		if (const FLinearColor* ProfileColor = Entry->ProfileColors.Find(Profile))
		{
			return *ProfileColor;
		}
	}
	return Entry->Default;
}

bool UA11yColorTokens::GetToken(FName Token, FA11yColorToken& OutToken) const
{
	if (const FA11yColorToken* Entry = Tokens.Find(Token))
	{
		OutToken = *Entry;
		return true;
	}
	return false;
}

bool UA11yColorLibrary::TryGetCustomColor(const FString& CustomColorsSpec, FName Role, FLinearColor& OutColor)
{
	TArray<FString> Pairs;
	CustomColorsSpec.ParseIntoArray(Pairs, TEXT(";"), /*InCullEmpty*/ true);
	for (const FString& Pair : Pairs)
	{
		FString Left, Right;
		if (!Pair.Split(TEXT("="), &Left, &Right))
		{
			continue;
		}
		Left.TrimStartAndEndInline();
		Right.TrimStartAndEndInline();
		if (FName(*Left) != Role)
		{
			continue;
		}
		if (Right.StartsWith(TEXT("#")))
		{
			Right = Right.RightChop(1);
		}
		if (Right.Len() != 6)
		{
			return false;
		}
		for (const TCHAR C : Right)
		{
			if (!FChar::IsHexDigit(C))
			{
				return false;
			}
		}
		OutColor = FLinearColor(FColor::FromHex(Right)); // sRGB hex -> linear
		return true;
	}
	return false;
}

FLinearColor UA11yColorLibrary::ResolveTokenColor(const UObject* WorldContextObject, const UA11yColorTokens* Palette, FName Token)
{
	if (!Palette)
	{
		return FLinearColor(1.f, 0.f, 1.f, 1.f);
	}
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	const FString Profile = Settings ? Settings->GetEnum(TEXT("visual.color_profile"), TEXT("none")) : FString(TEXT("none"));
	if (Settings && Profile == TEXT("custom"))
	{
		FLinearColor Custom;
		if (TryGetCustomColor(Settings->GetString(TEXT("visual.custom_colors")), Token, Custom))
		{
			return Custom;
		}
	}
	return Palette->GetColor(Token, FName(*Profile));
}

bool UA11yColorLibrary::ShouldOutline(const UObject* WorldContextObject, FName OutlineKey)
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	if (!Settings)
	{
		return false; // no store: as authored (outlines are an option, off by default)
	}
	return Settings->ResolveToggle(OutlineKey, Settings->GetBool(TEXT("visual.high_contrast"), false));
}

void UA11yColorLibrary::ApplyOptionalCvdFilter(EA11yCvdFilterType Type, float Strength01, float EngineSeverityMax)
{
	EColorVisionDeficiency EngineType = EColorVisionDeficiency::NormalVision;
	switch (Type)
	{
	case EA11yCvdFilterType::Protan: EngineType = EColorVisionDeficiency::Protanope; break;
	case EA11yCvdFilterType::Deutan: EngineType = EColorVisionDeficiency::Deuteranope; break;
	case EA11yCvdFilterType::Tritan: EngineType = EColorVisionDeficiency::Tritanope; break;
	default: break;
	}

	const float Severity = FMath::Clamp(Strength01, 0.f, 1.f) * FMath::Max(0.f, EngineSeverityMax);
	const bool bCorrect = EngineType != EColorVisionDeficiency::NormalVision && Severity > 0.f;

	// CorrectDeficiency = true: shift colours for the player (per the Blueprint reference). A deficiency type
	// with CorrectDeficiency = false is expected to SIMULATE the deficiency (UNVERIFIED here): a developer
	// tool, never a player option. So "None" always passes NormalVision.
	// If this call fails to link in your engine version, call the Blueprint node
	// "Set Color Vision Deficiency Type" from your options widget instead.
	UWidgetBlueprintLibrary::SetColorVisionDeficiencyType(
		bCorrect ? EngineType : EColorVisionDeficiency::NormalVision,
		bCorrect ? Severity : 0.f,
		/*CorrectDeficiency*/ bCorrect,
		/*ShowCorrectionWithDeficiency*/ false);
}
