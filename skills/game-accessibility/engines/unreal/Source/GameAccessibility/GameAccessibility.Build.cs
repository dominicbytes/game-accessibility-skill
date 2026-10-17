// SPDX-License-Identifier: MIT
// game-accessibility skill: Unreal Engine 5 runtime module.
// NOT COMPILED by the author (no Unreal Editor / C++ toolchain available). Please report compile errors.

using System.IO;
using UnrealBuildTool;

public class GameAccessibility : ModuleRules
{
	public GameAccessibility(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

		PublicDependencyModuleNames.AddRange(new string[]
		{
			"Core",
			"CoreUObject",
			"Engine"
		});

		PrivateDependencyModuleNames.AddRange(new string[]
		{
			"Json",            // FJsonSerializer for the registry and Saved/Accessibility.json
			"UMG",             // UWidgetBlueprintLibrary::SetColorVisionDeficiencyType (optional CVD filter)
			"Slate",           // FSlateApplication focus-loss event (input latch clearing)
			"SlateCore",
			"ApplicationCore"  // IPlatformInputDeviceMapper device-disconnect event (UE 5.1+)
		});

		// The settings registry lives at <Project>/Config/Accessibility/settings_registry.json.
		// Config/ only stages .ini files by default, so stage the registry as a loose (NonUFS) file.
		// The existence check keeps the build working before the file has been copied in.
		if (Target.ProjectFile != null)
		{
			string RegistryPath = Path.Combine(Target.ProjectFile.Directory.FullName, "Config", "Accessibility", "settings_registry.json");
			if (File.Exists(RegistryPath))
			{
				RuntimeDependencies.Add("$(ProjectDir)/Config/Accessibility/settings_registry.json", StagedFileType.NonUFS);
			}
		}
	}
}
