// SPDX-License-Identifier: MIT
// game-accessibility skill: CaptionBus. NOT COMPILED by the author; please report compile errors.

#include "A11yCaptionSubsystem.h"
#include "A11ySettingsSubsystem.h"
#include "Engine/GameInstance.h"
#include "HAL/PlatformTime.h"

void UA11yCaptionSubsystem::Initialize(FSubsystemCollectionBase& Collection)
{
	Collection.InitializeDependency(UA11ySettingsSubsystem::StaticClass());
	Super::Initialize(Collection);
}

FA11yCaptionRouting UA11yCaptionSubsystem::ResolveRouting(const FA11yCaptionEvent& Event) const
{
	FA11yCaptionRouting Routing;
	const UA11ySettingsSubsystem* Settings = GetGameInstance() ? GetGameInstance()->GetSubsystem<UA11ySettingsSubsystem>() : nullptr;
	if (!Settings)
	{
		// No store: fall back to the safe default of showing dialogue text.
		Routing.bShowSubtitle = Event.Kind == EA11yCaptionKind::Dialogue;
		return Routing;
	}

	const bool bDialogue = Event.Kind == EA11yCaptionKind::Dialogue;
	Routing.bShowSubtitle = bDialogue && Settings->GetBool(TEXT("subtitles.dialogue"), true);
	Routing.bShowCaption = !bDialogue && Settings->GetBool(TEXT("subtitles.sound_captions"), false);
	Routing.bShowVisualizer = Event.bHasSourceLocation && Settings->GetBool(TEXT("audio.sound_visualizer"), false);
	Routing.bNarrate = bDialogue && !Event.bIsVoiced && Settings->GetBool(TEXT("narration.dialogue"), false);
	Routing.bShowSpeakerName = !Event.Speaker.IsEmpty() && Settings->GetBool(TEXT("subtitles.speaker_names"), true);
	Routing.bShowDirection = Event.bHasSourceLocation && Settings->GetBool(TEXT("subtitles.direction"), false);
	Routing.TextScale = Settings->GetFloat(TEXT("subtitles.size"), 1.f);
	Routing.BackgroundOpacity = Settings->GetFloat(TEXT("subtitles.background_opacity"), 0.75f);
	return Routing;
}

bool UA11yCaptionSubsystem::PostCaption(const FA11yCaptionEvent& Event)
{
	// Real time: captions are not slowed by assist.game_speed.
	const double Now = FPlatformTime::Seconds();
	if (!Event.MergeKey.IsNone())
	{
		const double Expiry = Now + FMath::Max(0.1f, Event.DurationSeconds);
		if (double* Existing = ActiveMerges.Find(Event.MergeKey))
		{
			if (Now < *Existing)
			{
				*Existing = Expiry; // still sounding: extend, do not repeat the caption
				return false;
			}
		}
		ActiveMerges.Add(Event.MergeKey, Expiry);
	}

	const FA11yCaptionRouting Routing = ResolveRouting(Event);
	if (!Routing.bShowSubtitle && !Routing.bShowCaption && !Routing.bShowVisualizer && !Routing.bNarrate)
	{
		return false;
	}
	OnCaption.Broadcast(Event, Routing);
	return true;
}

void UA11yCaptionSubsystem::EndMerge(FName MergeKey)
{
	ActiveMerges.Remove(MergeKey);
}
