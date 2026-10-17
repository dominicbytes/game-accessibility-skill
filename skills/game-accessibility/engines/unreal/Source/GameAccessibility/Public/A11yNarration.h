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

/** Which setting owns an utterance. Internal; not reflected. */
enum class EA11ySpeechCategory : uint8
{
	/** Speak(): menus, buttons, values, focus. Owned by narration.enabled. */
	Menu,
	/** Unvoiced dialogue from the caption bus. Owned by narration.dialogue. */
	Dialogue
};

/**
 * Bounded priority queue in front of one backend. Never blocks input; speech failure is silent and safe.
 * Two independent categories: menu/UI speech (Speak, gated by narration.enabled) and dialogue reading
 * (unvoiced lines from the caption bus, gated by narration.dialogue). Turning a category's setting off
 * stops its current utterance and discards its queued items; the other category is untouched. An
 * interrupting Speak() likewise only stops and drops speech of its own category. Every queued item is
 * re-checked (setting and route ownership) just before it is dispatched.
 */
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

	/**
	 * Menu and UI speech. Gated by narration.enabled. With bInterrupt, stale non-critical menu speech
	 * (current and queued) is replaced; a critical utterance is never cut off. Interruption never crosses
	 * categories: while a dialogue line is being read, this waits for it to finish, then speaks.
	 */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Narration")
	void Speak(const FString& Text, EA11yNarrationPriority Priority = EA11yNarrationPriority::Focus, bool bInterrupt = true);

	UFUNCTION(BlueprintCallable, Category = "Accessibility|Narration")
	void StopAll();

	/**
	 * When a native screen reader is speaking the UI, skip Focus speech to avoid double-speaking. Turning it
	 * on also stops and discards self-voiced focus speech that is current or queued.
	 */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Narration")
	void SetNativeScreenReaderActive(bool bActive);

	UPROPERTY(BlueprintReadWrite, Category = "Accessibility|Narration")
	int32 MaxQueueLength = 8;

protected:
	UFUNCTION()
	void HandleCaption(const FA11yCaptionEvent& Event, const FA11yCaptionRouting& Routing);

	UFUNCTION()
	void HandleSettingChanged(FName Key, const FString& Value);

private:
	struct FQueuedSpeech
	{
		FString Text;
		EA11yNarrationPriority Priority = EA11yNarrationPriority::Low;
		EA11ySpeechCategory Category = EA11ySpeechCategory::Menu;
	};

	void Enqueue(const FString& Text, EA11yNarrationPriority Priority, EA11ySpeechCategory Category, bool bInterrupt);
	bool TickQueue(float DeltaTime);
	bool BackendAvailable();
	float GetRate() const;
	/** Setting still on, and this queue (not the native reader) still owns the route. */
	bool IsStillWanted(EA11ySpeechCategory Category, EA11yNarrationPriority Priority) const;
	/** Stops the current utterance if Pred matches it, and removes matching queued items. */
	void DiscardWhere(TFunctionRef<bool(EA11ySpeechCategory, EA11yNarrationPriority)> Pred);

	UPROPERTY(Transient)
	TObjectPtr<UObject> Backend;

	TArray<FQueuedSpeech> Queue;
	/** The last utterance handed to the backend. Only meaningful while the backend reports IsSpeaking. */
	EA11yNarrationPriority CurrentPriority = EA11yNarrationPriority::Low;
	EA11ySpeechCategory CurrentCategory = EA11ySpeechCategory::Menu;
	bool bHasCurrent = false;
	bool bNativeScreenReaderActive = false;
	FTSTicker::FDelegateHandle TickerHandle;
};
