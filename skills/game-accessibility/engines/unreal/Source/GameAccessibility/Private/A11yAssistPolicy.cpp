// SPDX-License-Identifier: MIT
// game-accessibility skill: AssistPolicy. NOT COMPILED by the author; please report compile errors.

#include "A11yAssistPolicy.h"
#include "A11ySettingsSubsystem.h"

float UA11yAssistPolicy::DamageToPlayer(const UObject* WorldContextObject, float Amount, FName Source)
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	if (!Settings || Source == FName(TEXT("fall")))
	{
		return Amount;
	}
	return FMath::Max(0.f, Amount * Settings->GetFloat(TEXT("assist.damage_taken"), 1.f));
}

bool UA11yAssistPolicy::ShouldFail(const UObject* WorldContextObject, FName Reason)
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	if (!Settings)
	{
		return true;
	}
	if (Reason == FName(TEXT("fall")))
	{
		return !Settings->GetBool(TEXT("assist.no_fall_death"), false);
	}
	if (Reason == FName(TEXT("timer")))
	{
		return Settings->GetEnum(TEXT("assist.timers"), TEXT("normal")) != TEXT("off");
	}
	return true;
}

bool UA11yAssistPolicy::Consume(const UObject* WorldContextObject, FName Resource, float Amount, float& Current)
{
	if (const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject))
	{
		if ((Resource == FName(TEXT("ammo")) && Settings->GetBool(TEXT("assist.infinite_ammo"), false))
			|| (Resource == FName(TEXT("stamina")) && Settings->GetBool(TEXT("assist.infinite_stamina"), false)))
		{
			return true; // Allowed, nothing spent. Never add infinity to the counter.
		}
	}
	if (Current < Amount)
	{
		return false;
	}
	Current -= Amount;
	return true;
}

float UA11yAssistPolicy::TimerScale(const UObject* WorldContextObject)
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	if (!Settings)
	{
		return 1.f;
	}
	const FString Mode = Settings->GetEnum(TEXT("assist.timers"), TEXT("normal"));
	if (Mode == TEXT("off"))
	{
		return TNumericLimits<float>::Max();
	}
	return Mode == TEXT("extended") ? 2.f : 1.f;
}

bool UA11yAssistPolicy::AreTimersDisabled(const UObject* WorldContextObject)
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	return Settings && Settings->GetEnum(TEXT("assist.timers"), TEXT("normal")) == TEXT("off");
}

float UA11yAssistPolicy::TimeScale(const UObject* WorldContextObject)
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	return Settings ? Settings->GetFloat(TEXT("assist.game_speed"), 1.f) : 1.f;
}

float UA11yAssistPolicy::EnemyAggression(const UObject* WorldContextObject)
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	return Settings ? Settings->GetFloat(TEXT("assist.enemy_aggression"), 1.f) : 1.f;
}

float UA11yAssistPolicy::EncounterRate(const UObject* WorldContextObject)
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	return Settings ? Settings->GetFloat(TEXT("assist.encounter_rate"), 1.f) : 1.f;
}

bool UA11yAssistPolicy::IsAutoBattleOn(const UObject* WorldContextObject)
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	return Settings && Settings->GetBool(TEXT("assist.auto_battle"), false);
}

float UA11yAssistPolicy::BattleAnimationSpeed(const UObject* WorldContextObject)
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	return Settings ? Settings->GetFloat(TEXT("assist.battle_speed"), 1.f) : 1.f;
}

bool UA11yAssistPolicy::IsSkipAvailable(const UObject* WorldContextObject)
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	return Settings && Settings->HasKey(TEXT("assist.skip_available")) && Settings->GetBool(TEXT("assist.skip_available"), false);
}

bool UA11yAssistPolicy::RequestSkip(const UObject* WorldContextObject, FName ChallengeId)
{
	UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	if (!Settings || !IsSkipAvailable(WorldContextObject))
	{
		return false;
	}
	return Settings->SubmitSkipRequest(ChallengeId);
}

bool UA11yAssistPolicy::IsSceneSkipAvailable(const UObject* WorldContextObject)
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	return Settings && Settings->HasKey(TEXT("content.scene_skip")) && Settings->GetBool(TEXT("content.scene_skip"), false);
}

bool UA11yAssistPolicy::RequestSceneSkip(const UObject* WorldContextObject, FName SceneId)
{
	UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	if (!Settings || !IsSceneSkipAvailable(WorldContextObject))
	{
		return false;
	}
	return Settings->SubmitSkipRequest(SceneId);
}

void UA11yAssistPolicy::ClearSkipRequest(const UObject* WorldContextObject, FName Id)
{
	if (UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject))
	{
		Settings->ClearSkipRequest(Id);
	}
}

bool UA11yAssistPolicy::AreJumpScaresReduced(const UObject* WorldContextObject)
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	return Settings && Settings->GetEnum(TEXT("content.jump_scares"), TEXT("full")) == TEXT("reduced");
}

bool UA11yAssistPolicy::AreScareWarningsOn(const UObject* WorldContextObject)
{
	const UA11ySettingsSubsystem* Settings = UA11ySettingsSubsystem::Get(WorldContextObject);
	return Settings && Settings->GetBool(TEXT("content.scare_warnings"), false);
}
