// SPDX-License-Identifier: MIT
// game-accessibility skill: Narrator (architecture.md section 5).
// NOT COMPILED by the author; please report compile errors.
//
// Route choice:
//  1. Native screen readers (preferred where supported): Slate/UMG accessibility, EXPERIMENTAL, documented
//     for Windows (NVDA, JAWS) and iOS (VoiceOver). Enable with Accessibility.Enable=1 and fill each
//     widget's Accessibility section (Override Accessible Defaults, Accessible Behavior). The player's own
//     screen reader then speaks menu focus; call SetNativeScreenReaderActive(true) so this queue does not
//     double-speak focus changes.
//  2. Self-voicing TTS (fallback/complement): implement IA11yNarrationBackend in YOUR game module.
//     This module deliberately has no dependency on the experimental TextToSpeech plugin.
//
// TODO (bind TTS): enable the "Text To Speech" plugin, add "TextToSpeech" to your game module's
// dependencies, and implement the backend with UTextToSpeechEngineSubsystem (an engine subsystem;
// header TextToSpeechEngineSubsystem.h; API names checked against Epic's API reference, not compiled):
//     AddDefaultChannel(ChannelId); ActivateChannel(ChannelId);         // once
//     SpeakOnChannel(ChannelId, Text);   // interrupts speech already on that channel
//     StopSpeakingOnChannel(ChannelId); IsSpeakingOnChannel(ChannelId);
//     SetRateOnChannel(ChannelId, Rate); // documented clamp 0..1; map narration.rate (0.5-2.5x) yourself
// Epic's overview states the TTS plugin supports English only. Test packaged builds on each target:
// TTS behaviour has differed between Play In Editor and Standalone in past engine versions.

#pragma once

#include "CoreMinimal.h"
#include "Subsystems/GameInstanceSubsystem.h"
#include "UObject/Interface.h"
#include "Containers/Ticker.h"
#include "A11yTypes.h"
#include "A11yCaptionSubsystem.h"
#include "A11yNarration.generated.h"

UINTERFACE(MinimalAPI, Blueprintable)
class UA11yNarrationBackend : public UInterface
{
	GENERATED_BODY()
};

/** Speech output. Implement in C++ or Blueprint. Keep localisation outside the backend. */
class GAMEACCESSIBILITY_API IA11yNarrationBackend
{
	GENERATED_BODY()

public:
	UFUNCTION(BlueprintNativeEvent, BlueprintCallable, Category = "Accessibility|Narration")
	bool IsAvailable();

	/** RateMultiplier is narration.rate (1.0 = normal). Map it to your backend's range. */
	UFUNCTION(BlueprintNativeEvent, BlueprintCallable, Category = "Accessibility|Narration")
	void Speak(const FString& Text, float RateMultiplier);

	UFUNCTION(BlueprintNativeEvent, BlueprintCallable, Category = "Accessibility|Narration")
	void StopSpeaking();

	UFUNCTION(BlueprintNativeEvent, BlueprintCallable, Category = "Accessibility|Narration")
	bool IsSpeaking();
};

/** Bounded priority queue in front of one backend. Never blocks input; speech failure is silent and safe. */
UCLASS()
class GAMEACCESSIBILITY_API UA11yNarrationSubsystem : public UGameInstanceSubsystem
{
	GENERATED_BODY()

public:
	virtual void Initialize(FSubsystemCollectionBase& Collection) override;
	virtual void Deinitialize() override;

	/** Object must implement IA11yNarrationBackend. Pass null to disable self-voicing. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Narration")
	void SetBackend(UObject* InBackend);

	/** Menu and UI speech. Gated by narration.enabled. Focus speech replaces stale focus speech. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Narration")
	void Speak(const FString& Text, EA11yNarrationPriority Priority = EA11yNarrationPriority::Focus, bool bInterrupt = true);

	UFUNCTION(BlueprintCallable, Category = "Accessibility|Narration")
	void StopAll();

	/** When a native screen reader is speaking the UI, skip Focus speech to avoid double-speaking. */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Narration")
	void SetNativeScreenReaderActive(bool bActive) { bNativeScreenReaderActive = bActive; }

	UPROPERTY(BlueprintReadWrite, Category = "Accessibility|Narration")
	int32 MaxQueueLength = 8;

protected:
	UFUNCTION()
	void HandleCaption(const FA11yCaptionEvent& Event, const FA11yCaptionRouting& Routing);

private:
	struct FQueuedSpeech
	{
		FString Text;
		EA11yNarrationPriority Priority = EA11yNarrationPriority::Low;
	};

	void Enqueue(const FString& Text, EA11yNarrationPriority Priority, bool bInterrupt);
	bool TickQueue(float DeltaTime);
	bool BackendAvailable();
	float GetRate() const;

	UPROPERTY(Transient)
	TObjectPtr<UObject> Backend;

	TArray<FQueuedSpeech> Queue;
	EA11yNarrationPriority CurrentPriority = EA11yNarrationPriority::Low;
	bool bNativeScreenReaderActive = false;
	FTSTicker::FDelegateHandle TickerHandle;
};
