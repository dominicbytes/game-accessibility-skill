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

void UA11yNarrationSubsystem::Speak(const FString& Text, EA11yNarrationPriority Priority, bool bInterrupt)
{
	const UA11ySettingsSubsystem* Settings = GetGameInstance()->GetSubsystem<UA11ySettingsSubsystem>();
	if (!Settings || !Settings->GetBool(TEXT("narration.enabled"), false))
	{
		return;
	}
	if (bNativeScreenReaderActive && Priority == EA11yNarrationPriority::Focus)
	{
		return; // one route per announcement: the native reader already speaks focus
	}
	Enqueue(Text, Priority, bInterrupt);
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
	Enqueue(Line, Priority, /*bInterrupt*/ false);
}

void UA11yNarrationSubsystem::Enqueue(const FString& Text, EA11yNarrationPriority Priority, bool bInterrupt)
{
	if (Text.IsEmpty() || !BackendAvailable())
	{
		return;
	}

	if (bInterrupt && Priority != EA11yNarrationPriority::Critical)
	{
		// Stale focus/low speech is dropped; critical announcements are never dropped.
		Queue.RemoveAll([](const FQueuedSpeech& Item) { return Item.Priority != EA11yNarrationPriority::Critical; });
		if (CurrentPriority != EA11yNarrationPriority::Critical)
		{
			IA11yNarrationBackend::Execute_StopSpeaking(Backend);
		}
	}

	FQueuedSpeech Item;
	Item.Text = Text;
	Item.Priority = Priority;
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
	const FQueuedSpeech Next = Queue[0];
	Queue.RemoveAt(0);
	CurrentPriority = Next.Priority;
	IA11yNarrationBackend::Execute_Speak(Backend, Next.Text, GetRate());
	return true;
}

void UA11yNarrationSubsystem::StopAll()
{
	Queue.Reset();
	CurrentPriority = EA11yNarrationPriority::Low;
	if (Backend)
	{
		IA11yNarrationBackend::Execute_StopSpeaking(Backend);
	}
}
