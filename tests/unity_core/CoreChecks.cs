// Behaviour checks for the Unity sample's pure C# Core (skills/game-accessibility/engines/unity/Runtime/Core).
// Runs on plain .NET without Unity. Build and run with tests/unity_core/run_unity_core_tests.sh.
// Usage: dotnet CoreChecks.dll <path to settings-registry.json>

using System;
using System.Collections.Generic;
using System.IO;
using GameAccessibility;

static class CoreChecks
{
    static int fails = 0, passes = 0;

    static void Check(bool c, string name)
    {
        if (c) passes++;
        else { fails++; Console.WriteLine("FAIL " + name); }
    }

    static int Main(string[] args)
    {
        if (args.Length < 1) { Console.WriteLine("usage: CoreChecks <settings-registry.json>"); return 2; }
        string json = File.ReadAllText(args[0]);

        // ---- store: load, types, validation
        var s = A11ySettingsStore.FromRegistryJson(json);
        Check(s.Definitions.Count > 50, "loaded defs");
        Check(s.Presets.Count == 7, "presets " + s.Presets.Count);
        Check(Math.Abs(s.Get<float>("text.scale") - 1f) < 1e-6, "default text.scale");
        Check(s.Get<string>("input.qte_mode") == "normal", "enum default");
        Check(s.Get<int>("content.spiders") == 0, "int default");
        var changes = new List<string>();
        s.SettingChanged += (k, v) => changes.Add(k);
        Check(s.Set("text.scale", 5.0), "clamp set returns changed");
        Check(s.Get<double>("text.scale") == 2.0, "clamped to max");
        Check(!s.Set("text.scale", 2.0), "same value no change");
        Check(!s.Set("input.qte_mode", "bogus"), "invalid enum rejected");
        Check(!s.Set("text.scale", double.NaN), "NaN rejected");
        Check(!s.Set("subtitles.dialogue", 1.0), "bool type strict");
        Check(changes.Count == 1 && changes[0] == "text.scale", "only own key changed");
        Check(s.Set("content.spiders", 2.6) && s.Get<int>("content.spiders") == 3, "int rounding");

        // ---- OS preferences never override explicit choices
        s.ImportPlatformPreference("text_scale", 1.25f);
        Check(s.Get<double>("text.scale") == 2.0, "explicit text.scale not overridden by OS");
        s.ImportPlatformPreference("reduce_motion", true);
        Check(s.Get<double>("motion.camera_shake") == 0.0, "reduce motion -> shake 0");
        Check(s.Get<bool>("motion.motion_blur") == false, "reduce motion -> blur off");
        Check(s.Get<bool>("motion.reduce_flashing") == true, "reduce motion -> reduce flashing");
        Check(!s.IsExplicit("motion.camera_shake"), "OS import not explicit");
        s.ImportPlatformPreference("captions_enabled", false);
        Check(s.Get<bool>("subtitles.dialogue") == true, "captions off keeps subtitle default on");
        s.ImportPlatformPreference("reduce_motion", false);
        Check(s.Get<double>("motion.camera_shake") == 0.5, "OS revert restores default");
        s.ImportPlatformPreference("reduce_motion", true);
        s.Set("motion.camera_shake", 0.75);
        s.ImportPlatformPreference("reduce_motion", true);
        Check(s.Get<double>("motion.camera_shake") == 0.75, "explicit wins over OS");
        Check(s.Reset("motion.camera_shake") && s.Get<double>("motion.camera_shake") == 0.0, "reset returns to OS initial");

        // ---- presets: one batch, undoable, later hand changes survive undo
        s.Set("assist.damage_taken", 0.25);
        Check(s.ApplyPreset("lower_combat_pressure"), "apply preset");
        Check(s.Get<double>("assist.damage_taken") == 0.5 && s.Get<bool>("assist.infinite_ammo"), "preset values");
        s.Set("assist.enemy_aggression", 0.75);
        Check(s.UndoLastPreset(), "undo");
        Check(s.Get<double>("assist.damage_taken") == 0.25, "undo restores prior explicit");
        Check(!s.Get<bool>("assist.infinite_ammo") && !s.IsExplicit("assist.infinite_ammo"), "undo restores non-explicit");
        Check(s.Get<double>("assist.enemy_aggression") == 0.75, "undo keeps later hand change");
        Check(!s.UndoLastPreset(), "undo only once");
        bool consistent = true;
        s.SettingChanged += (k, v) => { if (k == "input.hold_mode" && s.Get<string>("input.qte_mode") != "auto") consistent = false; };
        s.ApplyPreset("reduced_physical_effort");
        Check(consistent, "preset events after whole batch stored");
        Check(s.ResetCategory("input") > 0 && s.Get<string>("input.hold_mode") == "hold", "reset category");

        // ---- persistence, repair, migration
        s.Set("audio.master", 0.3);
        string file = s.Serialize();
        var s2 = A11ySettingsStore.FromRegistryJson(json);
        Check(s2.Deserialize(file), "deserialize");
        Check(s2.Get<double>("audio.master") == 0.3 && s2.IsExplicit("audio.master"), "device key round trip");
        Check(s2.Get<double>("text.scale") == 2.0, "profile key round trip");
        Check(!s2.IsExplicit("subtitles.dialogue"), "non-explicit not saved");
        var s3 = A11ySettingsStore.FromRegistryJson(json);
        s3.Migration = (from, root) =>
        {
            if (root.TryGetValue("shake_off", out var v) && v is bool off && off)
            {
                var p = (Dictionary<string, object>)root["profile"]; p["motion.camera_shake"] = 0.0; root.Remove("shake_off");
            }
        };
        Check(s3.Deserialize("{\"schema_version\":0,\"shake_off\":true,\"profile\":{\"text.scale\":\"huge\",\"text.font\":\"readable\",\"mod.thing\":3}}"), "repair load");
        Check(s3.Get<double>("motion.camera_shake") == 0.0, "migration applied");
        Check(s3.Get<double>("text.scale") == 1.0 && s3.Get<string>("text.font") == "readable", "per-key repair");
        Check(s3.LoadWarnings.Count == 2, "warnings " + s3.LoadWarnings.Count);
        Check(s3.Serialize().Contains("mod.thing"), "unknown key preserved");
        Check(!s3.Deserialize("{not json"), "corrupt file rejected");
        Check(s3.Get<string>("text.font") == "readable", "corrupt file keeps current");
        var sub = A11ySettingsStore.FromRegistryJson(json, new HashSet<string> { "text.scale", "assist.damage_taken" });
        Check(sub.Definitions.Count == 2, "subset");

        // ---- AssistPolicy
        var pol = new AssistPolicy(sub);
        Check(pol.TimerScale == 1f && pol.TimeScale == 1f && !pol.IsInfinite("ammo") && pol.ShouldFail("fall"), "policy defaults with missing keys");
        var ps = A11ySettingsStore.FromRegistryJson(json);
        var policy = new AssistPolicy(ps);
        ps.Set("assist.damage_taken", 0.0);
        Check(policy.DamageToPlayer(10, "enemy") == 0f && policy.DamageToPlayer(10, "fall") == 10f, "damage channels");
        Check(policy.ShouldFail("fall"), "fall fails by default");
        ps.Set("assist.no_fall_death", true);
        Check(!policy.ShouldFail("fall") && policy.ShouldFail("detected"), "safe falls");
        ps.Set("assist.timers", "off");
        Check(float.IsPositiveInfinity(policy.TimerScale) && !policy.ShouldFail("timer"), "timers off");
        ps.Set("assist.infinite_ammo", true);
        int ammo = 0;
        Check(policy.Consume("ammo", 1, ref ammo) && ammo == 0, "infinite ammo not spent");
        float stamina = 0.5f;
        Check(!policy.Consume("stamina", 1f, ref stamina) && stamina == 0.5f, "stamina insufficient");
        string skipped = null; policy.SkipRequested += id => skipped = id;
        Check(policy.RequestSkip("boss1") && skipped == "boss1", "skip");

        // ---- InputAssist
        var ia = new InputAssistCore(ps);
        Check(ia.UpdateHeld("aim", true) && !ia.UpdateHeld("aim", false), "hold mode");
        ps.Set("input.hold_mode", "toggle");
        Check(ia.UpdateHeld("aim", true) && ia.UpdateHeld("aim", false) && ia.UpdateHeld("aim", false), "toggle latches");
        Check(!ia.UpdateHeld("aim", true), "second press releases");
        ia.UpdateHeld("aim", false);
        ia.UpdateHeld("aim", true);
        ia.ClearLatches();
        Check(!ia.IsHeld("aim"), "latch cleared");
        Check(!ia.UpdateHeld("aim", true), "still-held button does not re-latch");
        var m = ia.StartMash(10, 2f); for (int i = 0; i < 10; i++) m.Press(); Check(m.IsComplete, "mash");
        ps.Set("input.mash_mode", "hold"); var mh = ia.StartMash(10, 2f); mh.Press(); Check(!mh.IsComplete, "press no-op in hold");
        for (int i = 0; i < 21; i++) mh.Tick(0.1f, true); Check(mh.IsComplete, "hold fills");
        ps.Set("input.mash_mode", "single"); var ms = ia.StartMash(10, 2f); ms.Press(); Check(ms.IsComplete, "single");
        var q = ia.StartQte(1f); Check(q.WindowSeconds == 1f, "normal qte");
        ps.Set("input.qte_mode", "slow"); Check(ia.StartQte(1f).WindowSeconds == 3f, "slow 3x");
        Check(float.IsInfinity(ia.StartQte(1f, true).WindowSeconds), "slow indefinite");
        ps.Set("input.qte_mode", "auto"); var qa = ia.StartQte(1f); qa.Tick(0.3f); Check(qa.State == QteState.Pending, "auto beat"); qa.Tick(0.4f); Check(qa.State == QteState.Success, "auto success");
        ps.Set("input.timing_window_scale", 2.0); Check(ia.TimingWindow(0.2f) == 0.4f, "timing window");

        // ---- FlashGuard
        var fg = new FlashGuardCore(ps);
        int allowed = 0;
        for (int i = 0; i < 10; i++) { var d = fg.RequestFlash(i * 0.1, 1f, 1f); if (d.Output == FlashOutput.Flash) allowed++; }
        Check(allowed == 3, "3 per second " + allowed);
        Check(fg.RequestFlash(1.05, 1f, 1f).Output == FlashOutput.Flash, "window slides");
        Check(fg.RequestFlash(1.06, 0.05f, 0.01f).Output == FlashOutput.Flash, "small dim flash always ok");
        ps.Set("motion.reduce_flashing", true);
        var r = fg.RequestFlash(10, 1f, 1f, true); Check(r.Output == FlashOutput.SteadyFade && r.Intensity <= 0.08f && r.Desaturate, "reduce converts");

        // ---- CaptionBus
        var cb = new CaptionBusCore(ps);
        int subs = 0;
        cb.SubtitleRequested += l => subs++;
        var dlg = new CaptionEvent("1", CaptionKind.Dialogue, "Hello", 2f, "mara", isVoiced: false);
        Check(cb.Emit(dlg, 0) == CaptionRoute.Subtitle && subs == 1, "dialogue subtitle");
        ps.Set("narration.dialogue", true);
        Check(cb.Emit(dlg, 0) == (CaptionRoute.Subtitle | CaptionRoute.Narration), "unvoiced narrated");
        var alarm = new CaptionEvent("2", CaptionKind.Sound, "[Alarm]", 3f, bearingDegrees: -90f, mergeKey: "alarm");
        Check(cb.Emit(alarm, 0) == CaptionRoute.None, "sound captions off by default");
        Check(cb.Emit(alarm, 1) == CaptionRoute.Merged, "merge");
        Check(cb.Emit(alarm, 10) == CaptionRoute.None, "merge expired");
        ps.Set("subtitles.sound_captions", true); ps.Set("audio.sound_visualizer", true); ps.Set("subtitles.direction", true);
        CaptionLine last = default; cb.CaptionRequested += l => last = l;
        Check(cb.Emit(alarm, 20) == (CaptionRoute.Caption | CaptionRoute.Visualizer) && last.Direction == "left", "caption+visualizer+direction");
        var hidden = new CaptionEvent("3", CaptionKind.Dialogue, "...", 1f, "villain", speakerHidden: true);
        CaptionLine hl = default; cb.SubtitleRequested += l => hl = l; cb.Emit(hidden, 30); Check(hl.Speaker == "Unknown voice", "hidden speaker");
        var pages = CaptionBusCore.Paginate("The quick brown fox jumps over the lazy dog and keeps running far away into the night sky", 40, 2);
        Check(pages.Count == 2 && pages[0].Split('\n').Length == 2, "paginate " + pages.Count);
        foreach (var pg in pages) foreach (var ln in pg.Split('\n')) Check(ln.Length <= 40, "line len");
        Check(CaptionBusCore.DirectionKey(170) == "behind" && CaptionBusCore.DirectionKey(-200) == "behind" && CaptionBusCore.DirectionKey(10) == "ahead" && CaptionBusCore.DirectionKey(90) == "right", "direction");

        // ---- ColorTokens
        var ct = new ColorTokens(ps); int changed = 0; ct.Changed += () => changed++;
        var before = ct.Get("team.enemy");
        ps.Set("visual.color_profile", "deutan");
        Check(changed == 1 && !ct.Get("team.enemy").Equals(before) && ct.Symbol("team.enemy") == "diamond", "colour profile swap");
        ps.Set("visual.color_profile", "custom");
        Check(ct.Get("team.enemy").Equals(before), "custom falls back to none");
        ct.SetCustom("team.enemy", new Rgba(1, 0, 1)); Check(ct.Get("team.enemy").Equals(new Rgba(1, 0, 1)), "custom set");

        // ---- MiniJson
        var obj = MiniJson.Parse("{\"a\":[1,2.5,-3e2,true,null,\"x\\u00e9\\n\"],\"b\":{}}");
        string back = MiniJson.Serialize(obj, false);
        Check(back == "{\"a\":[1,2.5,-300,true,null,\"x\u00e9\\n\"],\"b\":{}}", "json roundtrip: " + back);
        Check(!MiniJson.TryParse(new string('[', 100), out _, out _), "depth limit");

        // ---- registry v2: new keys and the string type
        var n = A11ySettingsStore.FromRegistryJson(json);
        Check(n.Definitions.Count == 90, "90 keys " + n.Definitions.Count);
        Check(n.GetDefinition("visual.custom_colors").Type == SettingType.String && n.Get<string>("visual.custom_colors") == "", "string type default");
        Check(n.Set("hud.hidden_elements", "minimap,damage_numbers") && n.Get<string>("hud.hidden_elements") == "minimap,damage_numbers", "string set");
        Check(!n.Set("hud.hidden_elements", 3.0) && !n.Set("hud.hidden_elements", null), "string rejects non-strings");
        Check(n.Set("hud.mode", "off"), "hud.mode off");
        Check(n.Set("content.gore", "off") && n.Get<string>("content.blood") == "full", "gore separate from blood");
        var n2 = A11ySettingsStore.FromRegistryJson(json);
        n2.Deserialize(n.Serialize());
        Check(n2.Get<string>("hud.hidden_elements") == "minimap,damage_numbers" && n2.Get<string>("hud.mode") == "off", "string round trip");

        // ---- guidance: per-channel keys with "follow"
        Check(GuidanceRules.EffectiveLevel(n, GuidanceKind.Paint) == "standard", "follow uses overall");
        n.Set("guidance.level", "subtle");
        Check(GuidanceRules.EffectiveLevel(n, GuidanceKind.Trail) == "subtle", "follow tracks overall");
        n.Set("guidance.paint", "off");
        Check(GuidanceRules.EffectiveLevel(n, GuidanceKind.Paint) == "off"
              && GuidanceRules.EffectiveLevel(n, GuidanceKind.Trail) == "subtle"
              && GuidanceRules.EffectiveLevel(n, GuidanceKind.InteractHighlight) == "subtle", "per-channel override");
        n.Set("guidance.highlights", "boosted");
        n.Set("guidance.level", "off");
        Check(GuidanceRules.EffectiveLevel(n, GuidanceKind.InteractHighlight) == "boosted"
              && GuidanceRules.EffectiveLevel(n, GuidanceKind.Trail) == "off", "override survives overall off");
        Check(GuidanceRules.EffectiveLevel(n, GuidanceKind.ObjectiveMarker) == "standard", "objective marker own key");
        bool bo;
        Check(GuidanceRules.Intensity("on_request", false, out bo) == 0f && GuidanceRules.Intensity("on_request", true, out bo) == 1f
              && GuidanceRules.Intensity("boosted", false, out bo) > 1f && bo, "guidance intensity");

        // ---- FlashGuard: <= 3 per 1 s AND <= 9 per 5 s; 2 Hz for 6 s must be capped
        var fs = A11ySettingsStore.FromRegistryJson(json);
        var fg2 = new FlashGuardCore(fs);
        var allowedTimes = new List<double>();
        for (int i = 0; i < 12; i++)
        {
            double t = i * 0.5;
            if (fg2.RequestFlash(t, 1f, 1f).Output == FlashOutput.Flash) allowedTimes.Add(t);
        }
        // t = 0..4.0 fills the 5 s budget (9); 4.5 is blocked; 5.0 and 5.5 fit once t = 0 and 0.5 age out.
        Check(allowedTimes.Count == 11, "2 Hz strobe for 6 s capped (" + allowedTimes.Count + " of 12 allowed)");
        bool ok5 = true, ok1 = true;
        foreach (double a in allowedTimes)
        {
            int in5 = 0, in1 = 0;
            foreach (double b in allowedTimes)
            {
                if (b >= a && b - a < 5.0) in5++;
                if (b >= a && b - a < 1.0) in1++;
            }
            if (in5 > 9) ok5 = false;
            if (in1 > 3) ok1 = false;
        }
        Check(ok5, "no 5 s window over 9");
        Check(ok1, "no 1 s window over 3");
        var fg3 = new FlashGuardCore(fs);
        int allowed3 = 0;
        for (int i = 0; i < 18; i++) if (fg3.RequestFlash(i / 3.0, 1f, 1f).Output == FlashOutput.Flash) allowed3++;
        // 9 allowed by t = 2.67 s, then blocked until t = 5.0 s, then 3 more: 12 of 18.
        Check(allowed3 == 12, "3 Hz strobe for 6 s capped (" + allowed3 + " of 18 allowed)");
        Check(fg2.RequestFlash(100, 0.05f, 0.01f, true).Desaturate, "red desaturated in every mode");

        // ---- timing windows: simulation time, scaled once; real = base x window_scale / time_scale
        var ts = A11ySettingsStore.FromRegistryJson(json);
        ts.ApplyPreset("more_time");
        var tia = new InputAssistCore(ts);
        var tpol = new AssistPolicy(ts);
        Check(Math.Abs(tia.TimingWindow(0.2f) - 0.4f) < 1e-6, "window in sim time = base x scale");
        Check(Math.Abs(tia.RealSecondsForWindow(0.2f, tpol.TimeScale) - 0.2f * 2f / 0.75f) < 1e-5, "real = base x scale / time_scale");

        // ---- custom colours from visual.custom_colors
        var cs = A11ySettingsStore.FromRegistryJson(json);
        cs.Set("visual.custom_colors", "team.enemy=#FF00FF; hazard = #00ff00 ;bad;pickup.rare=#12");
        var ctc = new ColorTokens(cs);
        cs.Set("visual.color_profile", "custom");
        Check(ctc.Get("team.enemy").Equals(new Rgba(1, 0, 1)) && ctc.Get("hazard").Equals(new Rgba(0, 1, 0)), "custom colours parsed");
        Check(ctc.Get("pickup.rare").Equals(Rgba.FromHex("#8E24AA")), "malformed entry falls back to designer colour");
        int cchanged = 0;
        ctc.Changed += () => cchanged++;
        ctc.SetCustom("team.ally", new Rgba(0, 0, 1));
        Check(cs.Get<string>("visual.custom_colors") == "hazard=#00FF00;team.ally=#0000FF;team.enemy=#FF00FF"
              && cs.IsExplicit("visual.custom_colors") && cchanged == 1 && ctc.Get("team.ally").Equals(new Rgba(0, 0, 1)), "SetCustom persists to visual.custom_colors");

        // ---- new policy and input accessors
        var ap = A11ySettingsStore.FromRegistryJson(json);
        var app = new AssistPolicy(ap);
        Check(app.EncounterRate == 1f && !app.AutoBattle && app.BattleSpeed == 1f && app.SceneSkipAvailable, "new policy defaults");
        ap.Set("assist.encounter_rate", 0.0);
        ap.Set("assist.battle_speed", 9.0);
        Check(app.EncounterRate == 0f && app.BattleSpeed == 4f, "encounters off, battle speed clamped");
        var apia = new InputAssistCore(ap);
        Check(!apia.TargetLock && ap.ApplyPreset("reduced_physical_effort") && apia.TargetLock, "target lock via preset");

        FlashCountingChecks(json);
        SkipChecks(json);
        ToggleAndOverrideChecks(json);
        CameraShakeChecks(json);
        NarrationChecks(json);

        Console.WriteLine(passes + " passed, " + fails + " failed");
        return fails == 0 ? 0 : 1;
    }

    static int CountFlashes(FlashGuardCore g, double start, int n, double hz, float intensity, float area, bool red)
    {
        int allowed = 0;
        for (int i = 0; i < n; i++)
            if (g.RequestFlash(start + i / hz, intensity, area, red).Output != FlashOutput.Suppressed) allowed++;
        return allowed;
    }

    // ---- FlashGuard: counted = area >= 0.25 OR intensity >= 0.1 OR saturated red; limits cannot be loosened
    static void FlashCountingChecks(string json)
    {
        var st = A11ySettingsStore.FromRegistryJson(json);
        Check(CountFlashes(new FlashGuardCore(st), 0, 10, 10.0, 0.4f, 0.09f, false) == 3, "small-area moderate-intensity repeats capped at 3 per s");
        var g = new FlashGuardCore(st);
        int sustained = CountFlashes(g, 0, 20, 10.0 / 3.0, 0.4f, 0.09f, false); // 3.3 Hz for 6 s
        Check(sustained == 12, "small-area moderate repeats also hit the 5 s budget (" + sustained + ")");
        Check(CountFlashes(new FlashGuardCore(st), 0, 10, 10.0, 0.05f, 0.01f, true) == 3, "small saturated-red repeats capped at 3 per s");
        var gr = new FlashGuardCore(st);
        int red3 = CountFlashes(gr, 0, 18, 3.0, 0.02f, 0.02f, true);
        Check(red3 == 12, "small saturated-red 3 Hz for 6 s capped like any flash (" + red3 + ")");
        Check(gr.Counts(0f, 0f, true) && !gr.Counts(0.09f, 0.24f, false) && gr.Counts(0.1f, 0f, false) && gr.Counts(0f, 0.25f, false), "counting rule");
        Check(CountFlashes(new FlashGuardCore(st), 0, 10, 10.0, 0.05f, 0.01f, false) == 10, "small dim non-red flashes not counted");
        var loose = new FlashGuardCore(st) { LargeAreaThreshold = 0.9f, BrightIntensityThreshold = 0.9f, MaxFlashesPerWindow = 10, MaxFlashesPerLongWindow = 50 };
        Check(CountFlashes(loose, 0, 10, 10.0, 0.4f, 0.09f, false) == 3, "thresholds and budgets cannot be loosened");
        var strict = new FlashGuardCore(st) { LargeAreaThreshold = 0.05f };
        Check(CountFlashes(strict, 0, 10, 10.0, 0.05f, 0.06f, false) == 3, "stricter area threshold honoured");

        // Shortened windows cannot loosen the fixed 1 s / 5 s caps: check every rolling 1 s and 5 s interval.
        var both = new FlashGuardCore(st) { WindowSeconds = 0.1, LongWindowSeconds = 1 };
        Check(both.EffectiveWindowSeconds == 1.0 && both.EffectiveLongWindowSeconds == 5.0, "shortened windows clamped to 1 s / 5 s");
        Check(RollingWindowsHold(both, 25.0, 12.0, out string m1), "shortened windows: every rolling 1 s <= 3 and 5 s <= 9 (" + m1 + ")");
        Check(RollingWindowsHold(new FlashGuardCore(st) { WindowSeconds = 0.1, LongWindowSeconds = 1 }, 10.0 / 3.0, 12.0, out string m2),
            "shortened windows at 3.3 Hz: every rolling 1 s <= 3 and 5 s <= 9 (" + m2 + ")");
        Check(RollingWindowsHold(new FlashGuardCore(st) { WindowSeconds = 0.1 }, 25.0, 12.0, out string m3), "short window 0.1 s alone cannot loosen (" + m3 + ")");
        Check(RollingWindowsHold(new FlashGuardCore(st) { LongWindowSeconds = 1 }, 10.0 / 3.0, 12.0, out string m4), "long window 1 s alone cannot loosen (" + m4 + ")");
        Check(RollingWindowsHold(new FlashGuardCore(st) { WindowSeconds = double.NaN, LongWindowSeconds = double.NaN }, 25.0, 12.0, out string m5), "NaN windows use 1 s / 5 s (" + m5 + ")");
        var longer = new FlashGuardCore(st) { WindowSeconds = 2, LongWindowSeconds = 10 };
        Check(CountFlashes(longer, 0, 40, 4.0, 1f, 1f, false) == 9 && longer.EffectiveLongWindowSeconds == 10.0, "longer windows are stricter and honoured");

        // NaN area means unknown, so a dim, non-red request still counts (as full screen).
        Check(CountFlashes(new FlashGuardCore(st), 0, 10, 10.0, 0.01f, float.NaN, false) == 3, "dim non-red NaN-area repeats suppressed beyond the 1 s budget");
        Check(CountFlashes(new FlashGuardCore(st), 0, 10, 2.0, 0.01f, float.NaN, false) == 9, "dim non-red NaN-area repeats suppressed beyond the 5 s budget");
        Check(CountFlashes(new FlashGuardCore(st), 0, 10, 10.0, float.NaN, 0.01f, false) == 3, "NaN intensity counted as worst case");
        st.Set("motion.reduce_flashing", true);
        var gReduce = new FlashGuardCore(st);
        Check(CountFlashes(gReduce, 0, 10, 10.0, 0.05f, 0.01f, true) == 3, "red counted with reduce_flashing on");
        Check(CountFlashes(new FlashGuardCore(st), 0, 10, 10.0, 0.01f, float.NaN, false) == 3, "dim non-red NaN-area counted with reduce_flashing on");
    }

    // Requests a full-screen flash at hz for the given seconds, then checks every rolling [t, t + 1) and
    // [t, t + 5) interval starting at an allowed flash (the worst-case alignment) holds at most 3 and 9.
    static bool RollingWindowsHold(FlashGuardCore g, double hz, double seconds, out string detail)
    {
        var allowed = new List<double>();
        int n = (int)(seconds * hz);
        for (int i = 0; i < n; i++)
        {
            double t = i / hz;
            if (g.RequestFlash(t, 1f, 1f).Output != FlashOutput.Suppressed) allowed.Add(t);
        }
        int max1 = 0, max5 = 0;
        foreach (double start in allowed)
        {
            int in1 = 0, in5 = 0;
            foreach (double t in allowed)
            {
                if (t >= start && t < start + 1.0) in1++;
                if (t >= start && t < start + 5.0) in5++;
            }
            max1 = Math.Max(max1, in1);
            max5 = Math.Max(max5, in5);
        }
        detail = "allowed " + allowed.Count + ", max per 1 s " + max1 + ", max per 5 s " + max5;
        return allowed.Count > 0 && max1 <= 3 && max5 <= 9;
    }

    // ---- skips: challenge skip and sensitive-scene skip are gated separately and share one completion
    static void SkipChecks(string json)
    {
        foreach (bool challenge in new[] { false, true })
        foreach (bool scene in new[] { false, true })
        {
            var st = A11ySettingsStore.FromRegistryJson(json);
            st.Set("assist.skip_available", challenge);
            st.Set("content.scene_skip", scene);
            var p = new AssistPolicy(st);
            var done = new List<string>();
            p.SkipRequested += id => done.Add(id);
            bool c = p.RequestSkip("puzzle1");
            bool s = p.RequestSceneSkip("scene7");
            string tag = " (skip_available=" + challenge + ", scene_skip=" + scene + ")";
            Check(c == challenge && s == scene, "skip gates" + tag);
            Check(done.Contains("puzzle1") == challenge && done.Contains("scene7") == scene && done.Count == (challenge ? 1 : 0) + (scene ? 1 : 0), "skip completion events" + tag);
        }
        var only = A11ySettingsStore.FromRegistryJson(json, new HashSet<string> { "content.scene_skip" });
        var op = new AssistPolicy(only);
        int events = 0;
        op.SkipRequested += _ => events++;
        Check(!op.RequestSkip("puzzle1") && op.RequestSceneSkip("scene7") && events == 1, "scene-only registry: scene skip works, challenge skip refused");
        Check(op.RequestSceneSkip("scene7") && events == 1 && op.WasSkipped("scene7"), "repeat scene skip is idempotent");
        op.ForgetSkip("scene7");
        Check(op.RequestSceneSkip("scene7") && events == 2, "ForgetSkip allows a replay");
        Check(!op.RequestSceneSkip("") && !op.RequestSceneSkip(null), "empty scene id refused");
        var both = A11ySettingsStore.FromRegistryJson(json);
        var bp = new AssistPolicy(both);
        int bEvents = 0;
        bp.SkipRequested += _ => bEvents++;
        Check(bp.RequestSkip("x") && bp.RequestSceneSkip("x") && bEvents == 1, "same id through both routes completes once");
    }

    // ---- ResolveToggle, outline classes, motion overrides, scares, aim keys
    static void ToggleAndOverrideChecks(string json)
    {
        var st = A11ySettingsStore.FromRegistryJson(json);
        Check(st.ResolveToggle("visual.outline_enemies", true) && !st.ResolveToggle("visual.outline_enemies", false), "follow returns followValue");
        st.Set("visual.outline_enemies", "on");
        Check(st.ResolveToggle("visual.outline_enemies", false), "on returns true");
        st.Set("visual.outline_enemies", "off");
        Check(!st.ResolveToggle("visual.outline_enemies", true), "off returns false");
        Check(st.ResolveToggle("no.such_key", true) && !st.ResolveToggle("no.such_key", false) && st.ResolveToggle(null, true), "unregistered key returns followValue");
        var sub = A11ySettingsStore.FromRegistryJson(json, new HashSet<string> { "visual.high_contrast" });
        Check(sub.ResolveToggle("visual.outline_allies", true) && !sub.ResolveToggle("visual.outline_allies", false), "pruned override returns followValue");

        // outlines follow visual.high_contrast per class
        var o = A11ySettingsStore.FromRegistryJson(json);
        bool allOff = true;
        foreach (OutlineClass c in (OutlineClass[])Enum.GetValues(typeof(OutlineClass))) allOff &= !PresentationRules.OutlineEnabled(o, c);
        Check(allOff, "outlines off by default");
        o.Set("visual.high_contrast", true);
        o.Set("visual.outline_allies", "off");
        Check(PresentationRules.OutlineEnabled(o, OutlineClass.Enemies) && !PresentationRules.OutlineEnabled(o, OutlineClass.Allies)
              && PresentationRules.OutlineEnabled(o, OutlineClass.Interactables) && PresentationRules.OutlineEnabled(o, OutlineClass.Hazards), "outline class override under high contrast");
        o.Set("visual.high_contrast", false);
        o.Set("visual.outline_hazards", "on");
        Check(PresentationRules.OutlineEnabled(o, OutlineClass.Hazards) && !PresentationRules.OutlineEnabled(o, OutlineClass.Enemies), "single outline class without high contrast");
        Check(PresentationRules.OutlineEnabled(sub, OutlineClass.Enemies) == false && sub.Set("visual.high_contrast", true) && PresentationRules.OutlineEnabled(sub, OutlineClass.Enemies), "outlines follow umbrella when class keys pruned");

        // motion overrides resolve with followValue = !reduce_motion
        var m = A11ySettingsStore.FromRegistryJson(json);
        bool allOn = true;
        foreach (MotionEffect e in (MotionEffect[])Enum.GetValues(typeof(MotionEffect))) allOn &= PresentationRules.MotionEffectEnabled(m, e);
        Check(allOn, "motion effects on by default");
        m.Set("motion.reduce_motion", true);
        m.Set("motion.auto_camera", "on");
        Check(PresentationRules.MotionEffectEnabled(m, MotionEffect.AutoCamera) && !PresentationRules.MotionEffectEnabled(m, MotionEffect.Parallax)
              && !PresentationRules.MotionEffectEnabled(m, MotionEffect.MenuAnimation) && !PresentationRules.MotionEffectEnabled(m, MotionEffect.SpeedLines)
              && !PresentationRules.MotionEffectEnabled(m, MotionEffect.FovEffects), "reduce motion with one effect kept on");
        m.Set("motion.reduce_motion", false);
        m.Set("motion.parallax", "off");
        Check(!PresentationRules.MotionEffectEnabled(m, MotionEffect.Parallax) && PresentationRules.MotionEffectEnabled(m, MotionEffect.SpeedLines), "single effect off without reduce motion");

        // OS reduce-motion preference: floats to 0 (shake, bob, weapon sway), overrides follow the umbrella
        var os = A11ySettingsStore.FromRegistryJson(json);
        os.ImportPlatformPreference("reduce_motion", true);
        Check(os.Get<double>("motion.weapon_sway") == 0.0 && os.Get<double>("motion.head_bob") == 0.0 && os.Get<bool>("motion.reduce_motion"), "OS reduce motion -> sway, bob off, umbrella on");
        Check(os.Get<string>("motion.parallax") == "follow" && !PresentationRules.MotionEffectEnabled(os, MotionEffect.Parallax), "OS reduce motion reaches follow overrides");
        os.Set("motion.menu_animation", "on");
        os.ImportPlatformPreference("reduce_motion", true);
        Check(PresentationRules.MotionEffectEnabled(os, MotionEffect.MenuAnimation), "explicit override survives OS import");
        os.ImportPlatformPreference("reduce_motion", false);
        Check(os.Get<double>("motion.weapon_sway") == 0.5 && PresentationRules.MotionEffectEnabled(os, MotionEffect.Parallax), "OS revert restores sway and effects");

        // weapon sway is independent of head bob
        var wb = A11ySettingsStore.FromRegistryJson(json);
        wb.Set("motion.head_bob", 0.0);
        Check(wb.Get<double>("motion.weapon_sway") == 0.5, "head bob does not change weapon sway");
        wb.Set("motion.weapon_sway", 1.0);
        Check(wb.Get<double>("motion.head_bob") == 0.0, "weapon sway does not change head bob");

        // scares: intensity and warnings are separate
        var sc = A11ySettingsStore.FromRegistryJson(json);
        Check(!PresentationRules.JumpScaresReduced(sc) && !PresentationRules.ScareWarnings(sc), "scare defaults");
        Check(!sc.Set("content.jump_scares", "warned"), "old 'warned' value rejected");
        sc.Set("content.scare_warnings", true);
        Check(!PresentationRules.JumpScaresReduced(sc) && PresentationRules.ScareWarnings(sc), "full scares with warnings");
        sc.Set("content.jump_scares", "reduced");
        sc.Set("content.scare_warnings", false);
        Check(PresentationRules.JumpScaresReduced(sc) && !PresentationRules.ScareWarnings(sc), "reduced scares without warnings");

        // aim: slowdown, pull and stabilisation are separate
        var aim = A11ySettingsStore.FromRegistryJson(json);
        var ia = new InputAssistCore(aim);
        Check(ia.AimSlowdown == 0f && ia.AimPull == 0f && ia.AimStabilization == 0f, "aim defaults off");
        aim.Set("input.aim_pull", 0.75);
        Check(ia.AimPull == 0.75f && ia.AimSlowdown == 0f && ia.AimStabilization == 0f, "pull without slowdown");
        aim.ApplyPreset("reduced_physical_effort");
        Check(ia.AimSlowdown == 0.75f && ia.AimPull == 0.5f && ia.AimStabilization == 0.5f, "preset sets each aim key");
        var noAim = new InputAssistCore(A11ySettingsStore.FromRegistryJson(json, new HashSet<string> { "input.aim_assist" }));
        Check(noAim.AimPull == 0f && noAim.AimStabilization == 0f, "pruned aim keys mean no change");
        ia.Dispose(); noAim.Dispose();
    }

    // ---- camera shake: scale applied at output; Off clears active shake
    static void CameraShakeChecks(string json)
    {
        var st = A11ySettingsStore.FromRegistryJson(json);
        st.Set("motion.camera_shake", 1.0);
        var sh = new CameraShakeCore(st) { DecayPerSecond = 0f };
        sh.AddTrauma(1f);
        Check(Math.Abs(sh.Amplitude - 1f) < 1e-6, "full shake amplitude");
        st.Set("motion.camera_shake", 0.5);
        Check(Math.Abs(sh.Amplitude - 0.5f) < 1e-6, "lowering scale mid-shake lowers output at once");
        st.Set("motion.camera_shake", 0.0);
        Check(sh.Amplitude == 0f && sh.Trauma == 0f, "Off during an active shake stops it");
        sh.Tick(1f);
        st.Set("motion.camera_shake", 1.0);
        Check(sh.Amplitude == 0f, "old shake does not resume when re-enabled (zero decay)");
        st.Set("motion.camera_shake", 0.0);
        sh.AddTrauma(1f);
        Check(sh.Trauma == 0f, "new shake ignored while Off");
        st.Set("motion.camera_shake", 1.0);
        sh.DecayPerSecond = 1f;
        sh.AddTrauma(0.5f);
        sh.Tick(0.25f);
        Check(Math.Abs(sh.Trauma - 0.25f) < 1e-6, "decay in real seconds");
        sh.Dispose();
        st.Set("motion.camera_shake", 0.0);
        Check(Math.Abs(sh.Trauma - 0.25f) < 1e-6, "disposed core stops listening");
        var none = new CameraShakeCore(A11ySettingsStore.FromRegistryJson(json, new HashSet<string> { "text.scale" }));
        none.AddTrauma(1f);
        Check(none.Scale == 1f && none.Amplitude == 1f, "unregistered shake key keeps authored shake");
    }

    sealed class FakeBackend : INarrationBackend
    {
        public bool IsAvailable { get; set; } = true;
        public bool IsSpeaking { get; set; }
        public readonly List<string> Spoken = new List<string>();
        public int Stops;
        public void Speak(string text, bool interrupt) { Spoken.Add(text); IsSpeaking = true; }
        public void Stop() { Stops++; IsSpeaking = false; }
        public void SetRate(float rate) { }
        public void Finish() => IsSpeaking = false;
    }

    // ---- narration handover with a busy backend: content preserved, dispatches never overlap
    static void HandoverChecks(A11ySettingsStore st)
    {
        var be = new FakeBackend();
        bool native = false;
        var log = new List<string>(); // native dispatches, in order
        int overlaps = 0;
        var q = new NarrationQueue(st)
        {
            Backend = be, NativeRouteActive = () => native,
            Announce = t => { if (be.IsSpeaking) overlaps++; log.Add(t); },
        };

        // Active critical self-voiced utterance interrupted by handover.
        const string ledge = "Hold the ledge: press A twice to climb";
        q.Speak(ledge, NarrationPriority.Critical); q.Tick();
        q.Speak("menu x");
        q.Speak("line q", NarrationPriority.Normal, NarrationChannel.Dialogue);
        q.Speak("crit 2", NarrationPriority.Critical);
        Check(be.IsSpeaking && be.Spoken.Count == 1 && q.PendingCount == 3, "handover/critical: busy backend speaking critical, rest queued");
        native = true;
        q.Tick();
        Check(be.Stops == 1 && !be.IsSpeaking, "handover/critical: self-voicing stopped first");
        Check(log.Count == 3 && log[0] == ledge && log[1] == "crit 2" && log[2] == "line q",
            "handover/critical: active critical transferred before queued entries, menu speech dropped");
        q.Tick(); q.Tick();
        Check(log.Count == 3 && q.PendingCount == 0 && be.Spoken.Count == 1, "handover/critical: transferred once, nothing self-voiced afterwards");
        Check(overlaps == 0, "handover/critical: no overlapping dispatch");
        native = false;

        // Active self-voiced dialogue when the reader starts.
        log.Clear();
        q.Speak("line A", NarrationPriority.Normal, NarrationChannel.Dialogue); q.Tick();
        q.Speak("menu y");
        q.Speak("line B", NarrationPriority.Normal, NarrationChannel.Dialogue);
        Check(be.IsSpeaking && be.Spoken[be.Spoken.Count - 1] == "line A" && q.PendingCount == 2, "handover/dialogue: busy backend speaking dialogue, rest queued");
        int stops = be.Stops;
        native = true;
        q.Tick();
        Check(be.Stops == stops + 1 && !be.IsSpeaking, "handover/dialogue: fallback dialogue stopped before native output");
        Check(log.Count == 2 && log[0] == "line A" && log[1] == "line B", "handover/dialogue: active then queued dialogue transferred in order");
        q.Speak("checkpoint reached");
        Check(log.Count == 3 && log[2] == "checkpoint reached", "handover/dialogue: later announcements follow the transferred dialogue");
        Check(overlaps == 0, "handover/dialogue: no overlapping dispatch");
        native = false;

        // The reader starts between Ticks and a new announcement arrives first: still no overlap, nothing lost.
        log.Clear();
        q.Speak("line E", NarrationPriority.Normal, NarrationChannel.Dialogue); q.Tick();
        q.Speak("line F", NarrationPriority.Normal, NarrationChannel.Dialogue);
        stops = be.Stops;
        native = true;
        q.Speak("objective updated");
        Check(be.Stops == stops + 1 && !be.IsSpeaking && overlaps == 0, "handover/speak: self-voicing stopped before a native announcement");
        Check(log.Count == 3 && log[0] == "line E" && log[1] == "line F" && log[2] == "objective updated",
            "handover/speak: active and queued dialogue announced before the new announcement");

        // An idle backend has nothing to transfer: finished speech is not repeated.
        native = false;
        log.Clear();
        q.Speak("done line", NarrationPriority.Normal, NarrationChannel.Dialogue); q.Tick();
        be.Finish();
        stops = be.Stops;
        native = true;
        q.Tick();
        Check(log.Count == 0 && be.Stops == stops, "handover/idle: finished utterance not repeated");
        q.Dispose();
    }

    // ---- narration: settings and route ownership re-checked; menu off stops menu speech only
    static void NarrationChecks(string json)
    {
        var st = A11ySettingsStore.FromRegistryJson(json);
        st.Set("narration.enabled", true);
        st.Set("narration.dialogue", true);
        var be = new FakeBackend();
        bool native = false;
        var announced = new List<string>();
        int overlaps = 0;
        var q = new NarrationQueue(st)
        {
            Backend = be, NativeRouteActive = () => native,
            Announce = t => { if (be.IsSpeaking) overlaps++; announced.Add(t); },
        };

        q.Speak("menu 1"); q.Speak("menu 2"); q.Speak("line 1", NarrationPriority.Normal, NarrationChannel.Dialogue);
        q.Tick();
        Check(be.Spoken.Count == 1 && be.Spoken[0] == "menu 1" && be.IsSpeaking && q.PendingCount == 2, "busy backend: first utterance playing, rest queued");
        st.Set("narration.enabled", false);
        Check(be.Stops == 1 && !be.IsSpeaking && q.PendingCount == 1, "menu narration off stops current and discards queued menu speech");
        q.Tick();
        Check(be.Spoken.Count == 2 && be.Spoken[1] == "line 1", "dialogue reading continues independently");
        q.Speak("menu 3");
        q.Speak("Volume, slider", NarrationPriority.Focus);
        Check(q.PendingCount == 0, "menu speech refused while off");
        be.Finish(); q.Tick();
        Check(be.Spoken.Count == 2, "nothing menu-related dispatched after off");

        // dialogue off leaves menu speech alone
        st.Set("narration.enabled", true);
        q.Speak("line 2", NarrationPriority.Normal, NarrationChannel.Dialogue);
        q.Tick();
        q.Speak("menu 4");
        int stops = be.Stops;
        st.Set("narration.dialogue", false);
        Check(be.Stops == stops + 1 && q.PendingCount == 1, "dialogue off stops dialogue only");
        q.Tick();
        Check(be.Spoken[be.Spoken.Count - 1] == "menu 4", "menu speech survives dialogue off");

        // route ownership re-checked before dispatch
        st.Set("narration.dialogue", true);
        be.Finish(); q.Tick();
        q.Speak("menu 5"); q.Tick();                         // playing
        q.Speak("menu 6");
        q.Speak("line 3", NarrationPriority.Normal, NarrationChannel.Dialogue);
        q.Speak("alert", NarrationPriority.Critical);        // interrupts menu 5
        Check(be.Spoken[be.Spoken.Count - 1] == "alert", "critical interrupts");
        q.Speak("menu 7"); // queued behind
        int spokenBefore = be.Spoken.Count;
        stops = be.Stops;
        native = true;
        q.Tick();
        Check(q.PendingCount == 0 && be.Spoken.Count == spokenBefore, "native route: queued self-voicing dropped, nothing dispatched");
        Check(be.Stops == stops + 1 && !be.IsSpeaking, "native route: current self-voiced speech stopped");
        Check(announced.Count == 2 && announced[0] == "alert" && announced[1] == "line 3",
            "native route: interrupted critical first, then queued dialogue; menu speech dropped");
        Check(overlaps == 0, "native route: nothing announced over self-voicing");
        q.Speak("focus", NarrationPriority.Focus);
        q.Speak("notice");
        Check(!announced.Contains("focus") && announced.Contains("notice") && q.PendingCount == 0, "native route: focus left to reader, announcements sent natively");
        native = false;
        HandoverChecks(st);

        // focus replacement and queue cap still hold
        be.Finish(); q.Tick();
        q.Speak("f1", NarrationPriority.Focus); q.Tick();
        q.Speak("f2", NarrationPriority.Focus);
        Check(be.Spoken[be.Spoken.Count - 1] == "f2", "newer focus interrupts focus speech");
        var cap = new NarrationQueue(st, 2) { Backend = new FakeBackend { IsSpeaking = true } };
        cap.Speak("a"); cap.Speak("b"); cap.Speak("c");
        Check(cap.PendingCount == 2, "queue capped");
        int stopsBeforeDispose = be.Stops;
        q.Dispose();
        st.Set("narration.enabled", false);
        Check(be.Stops == stopsBeforeDispose && be.IsSpeaking, "disposed queue stops listening to settings");
        var noKeys = new NarrationQueue(A11ySettingsStore.FromRegistryJson(json, new HashSet<string> { "text.scale" })) { Backend = new FakeBackend() };
        noKeys.Speak("x"); noKeys.Tick();
        Check(noKeys.PendingCount == 0, "unregistered narration keys mean no self-voicing");
    }
}

