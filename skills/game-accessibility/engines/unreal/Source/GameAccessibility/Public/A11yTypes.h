// SPDX-License-Identifier: MIT
// game-accessibility skill: shared types. NOT COMPILED by the author; please report compile errors.

#pragma once

#include "CoreMinimal.h"
#include "A11yTypes.generated.h"

GAMEACCESSIBILITY_API DECLARE_LOG_CATEGORY_EXTERN(LogA11y, Log, All);

/** Value type of a registry entry ("bool", "float", "int", "enum", "string" in settings_registry.json). */
UENUM(BlueprintType)
enum class EA11ySettingType : uint8
{
	Bool,
	Float,
	Int,
	Enum,
	/** Free text with a game-defined format, e.g. visual.custom_colors "role=#RRGGBB;..." or a csv list. */
	String
};

UENUM(BlueprintType)
enum class EA11yCaptionKind : uint8
{
	Dialogue,
	Sound,
	Music
};

UENUM(BlueprintType)
enum class EA11yCaptionImportance : uint8
{
	Low,
	Normal,
	Critical
};

/** What FlashGuard allows the caller to render. */
UENUM(BlueprintType)
enum class EA11yFlashStyle : uint8
{
	/** Render the flash as authored, using the returned intensity. */
	Flash,
	/** Render a steady, low-contrast fade (ramp in/out for at least MinFadeSeconds) or an icon pulse. */
	SoftFade,
	/** Do not flash. Keep the steady state; show the information another way (icon, text, sound). */
	Suppressed
};

UENUM(BlueprintType)
enum class EA11yQteState : uint8
{
	Inactive,
	Pending,
	Succeeded,
	Failed
};

UENUM(BlueprintType)
enum class EA11yNarrationPriority : uint8
{
	/** May be dropped when the queue is full. */
	Low,
	/** Menu focus speech. New focus speech replaces stale focus speech. */
	Focus,
	/** Never dropped. */
	Critical
};

/** Player-facing full-screen colour filter. Optional extra on top of semantic colour tokens. */
UENUM(BlueprintType)
enum class EA11yCvdFilterType : uint8
{
	None,
	Protan,
	Deutan,
	Tritan
};
