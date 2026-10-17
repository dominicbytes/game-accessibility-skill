// SPDX-License-Identifier: MIT
// game-accessibility skill: owner of the global depth-of-field fallback (used by CameraComfort).
// NOT COMPILED by the author; please report compile errors.
//
// Only used when FPostProcessSettings::DepthOfFieldEnabled is unavailable (A11Y_UE_HAS_DOF_ENABLED=0).
// r.DepthOfFieldQuality is global, so its override has ONE owner that outlives level changes: this
// GameInstance subsystem. Each requester (a UA11yCameraComfortComponent) holds at most one
// "depth of field off" request. The authored quality is captured when the first request arrives and
// restored when the last one is released, so:
//  - Off -> level change -> On restores the authored quality, not the 0 the override wrote;
//  - overlapping components are balanced: the override stays until every requester has released it.
// Requesters that are destroyed without releasing are pruned (weak pointers) on the next change.
// Limitation: several Play-In-Editor clients in one process share the cvar; each instance restores the
// value it captured. Test the fallback in a standalone or packaged build.

#pragma once

#include "CoreMinimal.h"
#include "Subsystems/GameInstanceSubsystem.h"
#include "UObject/WeakObjectPtrTemplates.h"
#include "A11yDepthOfFieldSubsystem.generated.h"

UCLASS()
class GAMEACCESSIBILITY_API UA11yDepthOfFieldSubsystem : public UGameInstanceSubsystem
{
	GENERATED_BODY()

public:
	/** Returns null outside a game world. */
	static UA11yDepthOfFieldSubsystem* Get(const UObject* WorldContextObject);

	/** Restores the authored quality if the override is still active (game shutdown, end of PIE). */
	virtual void Deinitialize() override;

	/** Adds (true) or removes (false) Requester's "depth of field off" request. Idempotent per requester. */
	void SetDisabledBy(const UObject* Requester, bool bDisabled);

	/** Drops Requester's request. Call from EndPlay. Same as SetDisabledBy(Requester, false). */
	void Release(const UObject* Requester);

	/** True while r.DepthOfFieldQuality is held at 0 by at least one live requester. */
	UFUNCTION(BlueprintPure, Category = "Accessibility|Camera")
	bool IsOverrideActive() const { return bOverrideActive; }

	/**
	 * Route your graphics menu's depth-of-field quality through here. While the override is active it only
	 * updates the value that will be restored; otherwise it sets the cvar now (at code priority).
	 */
	UFUNCTION(BlueprintCallable, Category = "Accessibility|Camera")
	void SetAuthoredQuality(int32 Quality);

private:
	void Reevaluate();

	TSet<TWeakObjectPtr<const UObject>> Requesters;
	/** Authored r.DepthOfFieldQuality captured when the override began. -1 while inactive. */
	int32 BaselineQuality = -1;
	bool bOverrideActive = false;
};
