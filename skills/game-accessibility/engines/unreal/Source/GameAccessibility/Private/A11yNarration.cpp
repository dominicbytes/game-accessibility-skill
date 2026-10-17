// SPDX-License-Identifier: MIT
// game-accessibility skill: Narrator queue. NOT COMPILED by the author; please report compile errors.

#include "A11yNarration.h"
#include "A11yCaptionSubsystem.h"
#include "A11ySettingsSubsystem.h"
#include "Engine/GameInstance.h"

void UA11yNarrationSubsystem::Initialize(FSubsystemCollectionBase& Collection)
{
	Collection.InitializeDependency(UA11ySettingsSubsystem::StaticClass());
	Collection.InitializeDependency(UA11yCaptionSubsystem::StaticClass());
	Super::Initialize(Collection);

	if (UA11ySettingsSubsystem* Settings = GetGameInstance()->GetSubsystem<UA11ySettingsSubsystem>())
	{
		Settings->OnSettingChanged.AddUniqueDynamic(this, &UA11yNarrationSubsystem::HandleSettingChanged);
	}
	if (UA11yCaptionSubsystem* Captions = GetGameInstance()->GetSubsystem<UA11yCaptionSubsystem>())
	{
		Captions->OnCaption.AddUniqueDynamic(this, &UA11yNarrationSubsystem::HandleCaption);
	}
	// Core ticker runs while the game is paused, so menus keep speaking.
	TickerHandle = FTSTicker::GetCoreTicker().AddTicker(FTickerDelegate::CreateUObject(this, &UA11yNarrationSubsystem::TickQueue), 0.05f);
}

void UA11yNarrationSubsystem::Deinitialize()
{
	FTSTicker::GetCoreTicker().RemoveTicker(TickerHandle);
	if (UA11ySettingsSubsystem* Settings = GetGameInstance()->GetSubsystem<UA11ySettingsSubsystem>())
	{
		Settings->OnSettingChanged.RemoveDynamic(this, &UA11yNarrationSubsystem::HandleSettingChanged);
	}
	if (UA11yCaptionSubsystem* Captions = GetGameInstance()->GetSubsystem<UA11yCaptionSubsystem>())
	{
		Captions->OnCaption.RemoveDynamic(this, &UA11yNarrationSubsystem::HandleCaption);
	}
	StopAll();
	Backend = nullptr;
	Super::Deinitialize();
}

void UA11yNarrationSubsystem::SetBackend(UObject* InBackend)
{
	if (InBackend && !InBackend->GetClass()->ImplementsInterface(UA11yNarrationBackend::StaticClass()))
	{
		UE_LOG(LogA11y, Warning, TEXT("%s does not implement IA11yNarrationBackend."), *InBackend->GetName());
		return;
	}
	StopAll();
	Backend = InBackend;
}

bool UA11yNarrationSubsystem::BackendAvailable()
{
	return Backend && IA11yNarrationBackend::Execute_IsAvailable(Backend);
}

float UA11yNarrationSubsystem::GetRate() const
{
	const UA11ySettingsSubsystem* Settings = GetGameInstance()->GetSubsystem<UA11ySettingsSubsystem>();
	return Settings ? Settings->GetFloat(TEXT("narration.rate"), 1.f) : 1.f;
}

bool UA11yNarrationSubsystem::IsStillWanted(EA11ySpeechCategory Category, EA11yNarrationPriority Priority) const
{
	const UA11ySettingsSubsystem* Settings = GetGameInstance() ? GetGameInstance()->GetSubsystem<UA11ySettingsSubsystem>() : nullptr;
	if (!Settings)
	{
		return false;
	}
	if (Category == EA11ySpeechCategory::Dialogue)
	{
		// Dialogue reading is its own option: independent of menu narration and of the native reader.
		return Settings->GetBool(TEXT("narration.dialogue"), false);
	}
	if (!Settings->GetBool(TEXT("narration.enabled"), false))
	{
		return false;
	}
	// One route per announcement: the native reader already speaks focus.
	return !(bNativeScreenReaderActive && Priority == EA11yNarrationPriority::Focus);
}

void UA11yNarrationSubsystem::DiscardWhere(TFunctionRef<bool(EA11ySpeechCategory, EA11yNarrationPriority)> Pred)
{
	Queue.RemoveAll([&Pred](const FQueuedSpeech& Item) { return Pred(Item.Category, Item.Priority); });
	if (bHasCurrent && Backend && Pred(CurrentCategory, CurrentPriority))
	{
		if (IA11yNarrationBackend::Execute_IsSpeaking(Backend))
		{
			IA11yNarrationBackend::Execute_StopSpeaking(Backend);
		}
		bHasCurrent = false;
	}
}

void UA11yNarrationSubsystem::HandleSettingChanged(FName Key, const FString& /*Value*/)
{
	if (Key == FName(TEXT("narration.enabled")) || Key == FName(TEXT("narration.dialogue")))
	{
		// Turning a category off stops and discards only that category's speech.
		DiscardWhere([this](EA11ySpeechCategory Category, EA11yNarrationPriority Priority)
		{
			return !IsStillWanted(Category, Priority);
		});
	}
}

void UA11yNarrationSubsystem::SetNativeScreenReaderActive(bool bActive)
{
	bNativeScreenReaderActive = bActive;
	if (bActive)
	{
		// The native reader now owns focus speech: drop self-voiced focus speech, current and queued.
		DiscardWhere([this](EA11ySpeechCategory Category, EA11yNarrationPriority Priority)
		{
			return !IsStillWanted(Category, Priority);
		});
	}
}

void UA11yNarrationSubsystem::Speak(const FString& Text, EA11yNarrationPriority Priority, bool bInterrupt)
{
	if (!IsStillWanted(EA11ySpeechCategory::Menu, Priority))
	{
		return; // narration.enabled is off, or the native reader owns this focus announcement
	}
	Enqueue(Text, Priority, EA11ySpeechCategory::Menu, bInterrupt);
}

void UA11yNarrationSubsystem::HandleCaption(const FA11yCaptionEvent& Event, const FA11yCaptionRouting& Routing)
{
	// Unvoiced dialogue, already gated by narration.dialogue in the caption routing.
	if (!Routing.bNarrate)
	{
		return;
	}
	const FString Line = (Routing.bShowSpeakerName && !Event.Speaker.IsEmpty())
		? FString::Printf(TEXT("%s: %s"), *Event.Speaker.ToString(), *Event.Text.ToString())
		: Event.Text.ToString();
	const EA11yNarrationPriority Priority = Event.Importance == EA11yCaptionImportance::Critical
		? EA11yNarrationPriority::Critical
		: EA11yNarrationPriority::Low;
	Enqueue(Line, Priority, EA11ySpeechCategory::Dialogue, /*bInterrupt*/ false);
}

void UA11yNarrationSubsystem::Enqueue(const FString& Text, EA11yNarrationPriority Priority, EA11ySpeechCategory Category, bool bInterrupt)
{
	if (Text.IsEmpty() || !BackendAvailable())
	{
		return;
	}

	if (bInterrupt && Priority != EA11yNarrationPriority::Critical)
	{
		// Within this category only: stale speech is dropped and the current utterance is stopped.
		// Critical announcements are never dropped. Speech of the other category, current or queued, is
		// left alone: a menu focus line waits for an in-progress dialogue line to finish (TickQueue
		// dispatches it once the backend stops speaking) instead of cutting it off.
		Queue.RemoveAll([Category](const FQueuedSpeech& Item)
		{
			return Item.Priority != EA11yNarrationPriority::Critical && Item.Category == Category;
		});
		if (bHasCurrent && CurrentCategory == Category && CurrentPriority != EA11yNarrationPriority::Critical)
		{
			IA11yNarrationBackend::Execute_StopSpeaking(Backend);
			bHasCurrent = false;
		}
	}

	FQueuedSpeech Item;
	Item.Text = Text;
	Item.Priority = Priority;
	Item.Category = Category;
	if (bInterrupt)
	{
		// Behind any queued critical items, ahead of everything else.
		int32 InsertAt = 0;
		while (InsertAt < Queue.Num() && Queue[InsertAt].Priority == EA11yNarrationPriority::Critical)
		{
			++InsertAt;
		}
		Queue.Insert(Item, InsertAt);
	}
	else
	{
		Queue.Add(Item);
	}

	// Bound the queue: drop the oldest non-critical item first.
	while (Queue.Num() > FMath::Max(1, MaxQueueLength))
	{
		const int32 DropIndex = Queue.IndexOfByPredicate([](const FQueuedSpeech& Q) { return Q.Priority != EA11yNarrationPriority::Critical; });
		if (DropIndex == INDEX_NONE)
		{
			break;
		}
		Queue.RemoveAt(DropIndex);
	}

	TickQueue(0.f);
}

bool UA11yNarrationSubsystem::TickQueue(float /*DeltaTime*/)
{
	if (Queue.Num() == 0 || !Backend)
	{
		return true; // keep ticking
	}
	if (IA11yNarrationBackend::Execute_IsSpeaking(Backend))
	{
		return true;
	}
	if (!BackendAvailable())
	{
		Queue.Reset(); // speech failure is silent; never replay stale lines later
		bHasCurrent = false;
		return true;
	}
	while (Queue.Num() > 0)
	{
		const FQueuedSpeech Next = Queue[0];
		Queue.RemoveAt(0);
		// Re-check at dispatch: the setting may have been turned off, or the native reader may now own it.
		if (!IsStillWanted(Next.Category, Next.Priority))
		{
			continue;
		}
		CurrentPriority = Next.Priority;
		CurrentCategory = Next.Category;
		bHasCurrent = true;
		IA11yNarrationBackend::Execute_Speak(Backend, Next.Text, GetRate());
		break;
	}
	return true;
}

void UA11yNarrationSubsystem::StopAll()
{
	Queue.Reset();
	CurrentPriority = EA11yNarrationPriority::Low;
	CurrentCategory = EA11ySpeechCategory::Menu;
	bHasCurrent = false;
	if (Backend)
	{
		IA11yNarrationBackend::Execute_StopSpeaking(Backend);
	}
}
