// SPDX-License-Identifier: MIT
// game-accessibility skill: AssistPolicy (architecture.md section 2).
// NOT COMPILED by the author; please report compile errors.
//
// Gameplay asks the policy at the moment of decision. Achievement and save code must never call this
// library or read assist settings (core-policy.md section 2).

#pragma once

#include "CoreMinimal.h"
#include "Kismet/BlueprintFunctionLibrary.h"
#include "A11yAssistPolicy.generated.h"

UCLASS()
class GAMEACCESSIBILITY_API UA11yAssistPolicy : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:
	/**
	 * Scales damage dealt to the player by assist.damage_taken. Source "fall" is NOT scaled: falls follow
	 * assist.no_fall_death through ShouldFail("fall"). Other sources: "enemy", "hazard", "self", ...
	 */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Assist", meta = (WorldContext = "WorldContextObject"))
	static float DamageToPlayer(const UObject* WorldContextObject, float Amount, FName Source);

	/**
	 * False when an assist turns this failure off. "fall" -> assist.no_fall_death (caller returns the player
	 * to the last safe spot), "timer" -> assist.timers == "off". Unknown reasons fail as authored.
	 */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Assist", meta = (WorldContext = "WorldContextObject"))
	static bool ShouldFail(const UObject* WorldContextObject, FName Reason);

	/**
	 * Intercepts spending. "ammo" -> assist.infinite_ammo, "stamina" -> assist.infinite_stamina: returns true
	 * without spending. Otherwise spends from Current if there is enough. Keep quest items and currency out.
	 */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Assist", meta = (WorldContext = "WorldContextObject"))
	static bool Consume(const UObject* WorldContextObject, FName Resource, float Amount, UPARAM(ref) float& Current);

	/** Multiplier for non-race countdowns: 1 (normal), 2 (extended), or TNumericLimits<float>::Max() when off. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Assist", meta = (WorldContext = "WorldContextObject"))
	static float TimerScale(const UObject* WorldContextObject);

	/** True when assist.timers == "off": do not count non-race timers down at all. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Assist", meta = (WorldContext = "WorldContextObject"))
	static bool AreTimersDisabled(const UObject* WorldContextObject);

	/** assist.game_speed (0.5-1.0). Apply to simulation time only (see GUIDE.md); never to menus or audio-timed subtitles. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Assist", meta = (WorldContext = "WorldContextObject"))
	static float TimeScale(const UObject* WorldContextObject);

	/** assist.enemy_aggression (0-1). Multiply attack frequency and detection speed. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Assist", meta = (WorldContext = "WorldContextObject"))
	static float EnemyAggression(const UObject* WorldContextObject);

	/** assist.encounter_rate (0-1). Multiply the random-encounter chance; story battles always happen. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Assist", meta = (WorldContext = "WorldContextObject"))
	static float EncounterRate(const UObject* WorldContextObject);

	/** assist.auto_battle. The player can take control at any time. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Assist", meta = (WorldContext = "WorldContextObject"))
	static bool IsAutoBattleOn(const UObject* WorldContextObject);

	/** assist.battle_speed (1-4). Battle animation playback only; decision time is unaffected. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Assist", meta = (WorldContext = "WorldContextObject"))
	static float BattleAnimationSpeed(const UObject* WorldContextObject);

	/** assist.skip_available: puzzle, chase and challenge skips. False if the game did not register the key. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Assist", meta = (WorldContext = "WorldContextObject"))
	static bool IsSkipAvailable(const UObject* WorldContextObject);

	/**
	 * Challenge skip, gated ONLY by assist.skip_available. Broadcasts UA11ySettingsSubsystem::
	 * OnChallengeSkipRequested; the owner completes through its normal success path (same rewards, flags,
	 * checkpoint, next-scene state). Returns false if challenge skipping is not offered. Idempotent: a
	 * repeat for the same Id returns true without a second broadcast (see ClearSkipRequest).
	 */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Assist", meta = (WorldContext = "WorldContextObject"))
	static bool RequestSkip(const UObject* WorldContextObject, FName ChallengeId);

	/** content.scene_skip: listed sensitive scenes offer a skip. Independent of assist.skip_available and content.warnings. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Content", meta = (WorldContext = "WorldContextObject"))
	static bool IsSceneSkipAvailable(const UObject* WorldContextObject);

	/**
	 * Sensitive-scene skip, gated ONLY by content.scene_skip (works with assist.skip_available off or
	 * absent). Reaches the same OnChallengeSkipRequested delegate and completion path as RequestSkip, with
	 * the same idempotency. Show the optional factual recap from the completion path.
	 */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Content", meta = (WorldContext = "WorldContextObject"))
	static bool RequestSceneSkip(const UObject* WorldContextObject, FName SceneId);

	/** Re-arms a skip Id when the challenge or scene can be played again (restart, chapter replay). */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Assist", meta = (WorldContext = "WorldContextObject"))
	static void ClearSkipRequest(const UObject* WorldContextObject, FName Id);

	/** content.jump_scares == "reduced": soften stings, camera snaps and sudden spawns. Independent of warnings. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Content", meta = (WorldContext = "WorldContextObject"))
	static bool AreJumpScaresReduced(const UObject* WorldContextObject);

	/** content.scare_warnings: show and announce a consistent heads-up before scripted scares, full or reduced. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Content", meta = (WorldContext = "WorldContextObject"))
	static bool AreScareWarningsOn(const UObject* WorldContextObject);
};
