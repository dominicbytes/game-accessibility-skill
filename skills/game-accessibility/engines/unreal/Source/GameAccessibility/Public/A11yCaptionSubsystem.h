// SPDX-License-Identifier: MIT
// game-accessibility skill: CaptionBus (architecture.md section 4).
// NOT COMPILED by the author; please report compile errors.
//
// Author semantic audio events, not just audio files. One event routes to subtitles, sound captions,
// the sound visualiser and narration according to the player's settings. Captions are emitted from the
// event even when audio is muted or the volume is zero; this subsystem never reads audio levels.
// UE's built-in USoundWave subtitles already cover voiced speech; use this bus for non-speech captions,
// direction indicators and unvoiced lines, or route all dialogue through it for one consistent view.

#pragma once

#include "CoreMinimal.h"
#include "Subsystems/GameInstanceSubsystem.h"
#include "A11yTypes.h"
#include "A11yCaptionSubsystem.generated.h"

USTRUCT(BlueprintType)
struct GAMEACCESSIBILITY_API FA11yCaptionEvent
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Accessibility|Captions")
	FName Id;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Accessibility|Captions")
	EA11yCaptionKind Kind = EA11yCaptionKind::Sound;

	/** Localised line, or a caption such as "[Alarm blaring]". */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Accessibility|Captions")
	FText Text;

	/** Speaker name. Use "Unknown voice" for a hidden speaker; never reveal more than a hearing player gets. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Accessibility|Captions")
	FText Speaker;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Accessibility|Captions")
	bool bHasSourceLocation = false;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Accessibility|Captions")
	FVector SourceLocation = FVector::ZeroVector;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Accessibility|Captions")
	EA11yCaptionImportance Importance = EA11yCaptionImportance::Normal;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Accessibility|Captions", meta = (ClampMin = "0.1"))
	float DurationSeconds = 3.f;

	/** Looping or repeated sounds share a merge key and are captioned once while active. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Accessibility|Captions")
	FName MergeKey;

	/** False for unvoiced dialogue, which narration.dialogue may read aloud. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Accessibility|Captions")
	bool bIsVoiced = true;
};

/** Where an event should be shown, resolved from settings at post time. */
USTRUCT(BlueprintType)
struct GAMEACCESSIBILITY_API FA11yCaptionRouting
{
	GENERATED_BODY()

	UPROPERTY(BlueprintReadOnly, Category = "Accessibility|Captions") bool bShowSubtitle = false;
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility|Captions") bool bShowCaption = false;
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility|Captions") bool bShowVisualizer = false;
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility|Captions") bool bNarrate = false;
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility|Captions") bool bShowSpeakerName = true;
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility|Captions") bool bShowDirection = false;
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility|Captions") float TextScale = 1.f;
	UPROPERTY(BlueprintReadOnly, Category = "Accessibility|Captions") float BackgroundOpacity = 0.75f;
};

DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FA11yCaptionSignature, const FA11yCaptionEvent&, Event, const FA11yCaptionRouting&, Routing);

UCLASS()
class GAMEACCESSIBILITY_API UA11yCaptionSubsystem : public UGameInstanceSubsystem
{
	GENERATED_BODY()

public:
	virtual void Initialize(FSubsystemCollectionBase& Collection) override;

	/** Returns true if any output received the event. Merged repeats return false. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Captions")
	bool PostCaption(const FA11yCaptionEvent& Event);

	/** Ends a merged loop early (for example when the alarm stops). */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Captions")
	void EndMerge(FName MergeKey);

	UFUNCTION(BlueprintPure, Category = "Accessibility|Captions")
	FA11yCaptionRouting ResolveRouting(const FA11yCaptionEvent& Event) const;

	/** UMG subtitle view, caption view and sound visualiser bind here. Narration binds here too. */
	UPROPERTY(BlueprintAssignable, Category = "Accessibility|Captions")
	FA11yCaptionSignature OnCaption;

private:
	/** MergeKey -> real-time expiry. */
	TMap<FName, double> ActiveMerges;
};
