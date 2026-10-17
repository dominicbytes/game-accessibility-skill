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
        Check(n.Definitions.Count == 77, "77 keys " + n.Definitions.Count);
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

        Console.WriteLine(passes + " passed, " + fails + " failed");
        return fails == 0 ? 0 : 1;
    }
}
