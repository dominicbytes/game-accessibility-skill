// Game Accessibility sample: input assists that wrap logical actions (hold/toggle, mash, QTE, timing windows).
// Pure C# (netstandard2.1). No UnityEngine references. Feed it action state from your input layer
// (Unity Input System or legacy Input); it never sees physical keys.
//
// Contract: references/architecture.md section 3.

using System;
using System.Collections.Generic;

namespace GameAccessibility
{
    public enum QteMode { Normal, Slow, Auto }
    public enum MashMode { Mash, Hold, Single }
    public enum QteState { Pending, Success, Failure }

    public sealed class InputAssistCore : IDisposable
    {
        private readonly A11ySettingsStore _store;
        private readonly Dictionary<string, bool> _latched = new Dictionary<string, bool>(StringComparer.Ordinal);
        private readonly Dictionary<string, bool> _wasDown = new Dictionary<string, bool>(StringComparer.Ordinal);
        private readonly IDisposable _holdModeSubscription;

        public InputAssistCore(A11ySettingsStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            // Switching hold mode mid-action must not leave an action stuck on.
            _holdModeSubscription = _store.Observe("input.hold_mode", _ => ClearLatches());
        }

        /// <summary>Stops listening to the settings store.</summary>
        public void Dispose() => _holdModeSubscription.Dispose();

        public bool ToggleMode => string.Equals(_store.GetOr("input.hold_mode", "hold"), "toggle", StringComparison.Ordinal);

        public MashMode CurrentMashMode
        {
            get
            {
                switch (_store.GetOr("input.mash_mode", "mash"))
                {
                    case "hold": return MashMode.Hold;
                    case "single": return MashMode.Single;
                    default: return MashMode.Mash;
                }
            }
        }

        public QteMode CurrentQteMode
        {
            get
            {
                switch (_store.GetOr("input.qte_mode", "normal"))
                {
                    case "slow": return QteMode.Slow;
                    case "auto": return QteMode.Auto;
                    default: return QteMode.Normal;
                }
            }
        }

        /// <summary>
        /// Call once per frame per holdable action (aim, sprint, crouch, block) with the physical "is down" state.
        /// Returns the logical held state: the input itself in hold mode; a latch that flips on each press in toggle mode.
        /// </summary>
        public bool UpdateHeld(string action, bool isDown)
        {
            _wasDown.TryGetValue(action, out bool wasDown);
            _wasDown[action] = isDown;
            if (!ToggleMode) return isDown;
            _latched.TryGetValue(action, out bool latched);
            if (isDown && !wasDown) latched = !latched;
            _latched[action] = latched;
            return latched;
        }

        /// <summary>Last logical held state computed by <see cref="UpdateHeld"/>.</summary>
        public bool IsHeld(string action)
        {
            if (ToggleMode) return _latched.TryGetValue(action, out bool l) && l;
            return _wasDown.TryGetValue(action, out bool d) && d;
        }

        /// <summary>
        /// Releases every toggle latch. Call on pause, death, scene change, focus loss and device disconnect.
        /// </summary>
        public void ClearLatches()
        {
            _latched.Clear();
            // _wasDown is kept, so a button still held now needs a fresh press to latch again.
        }

        /// <summary>
        /// Window for parry, dodge, rhythm and perfect-hit inputs, in SIMULATION seconds:
        /// <c>base × input.timing_window_scale</c>. Count it down with the simulation delta (Time.deltaTime), never
        /// with unscaled time, and never multiply it by the game speed as well. Game speed then lengthens it in real
        /// time by itself: see <see cref="RealSecondsForWindow"/>.
        /// </summary>
        public float TimingWindow(float baseSeconds) => baseSeconds * Math.Max(1f, _store.GetOr("input.timing_window_scale", 1f));

        /// <summary>
        /// How long the window lasts in REAL seconds at <paramref name="timeScale"/> (AssistPolicy.TimeScale):
        /// <c>base × window_scale / time_scale</c>. For display and tests only; gameplay counts
        /// <see cref="TimingWindow"/> down in simulation time.
        /// </summary>
        public float RealSecondsForWindow(float baseSeconds, float timeScale) =>
            TimingWindow(baseSeconds) / Math.Max(0.01f, timeScale);

        /// <summary>input.target_lock: aim stays on the selected target until the player switches or releases it.</summary>
        public bool TargetLock => _store.GetOr("input.target_lock", false);

        /// <summary>input.aim_assist (0..1): slowdown only. Scale aim sensitivity over a target by (1 - AimSlowdown x maxSlow).</summary>
        public float AimSlowdown => Clamp01(_store.GetOr("input.aim_assist", 0f));

        /// <summary>input.aim_pull (0..1): strength of the gentle pull toward a nearby target. Independent of slowdown.</summary>
        public float AimPull => Clamp01(_store.GetOr("input.aim_pull", 0f));

        /// <summary>input.aim_stabilization (0..1): smoothing of small unintended movement, and recoil/sway reduction while aiming.</summary>
        public float AimStabilization => Clamp01(_store.GetOr("input.aim_stabilization", 0f));

        private static float Clamp01(float v) => v < 0f ? 0f : (v > 1f ? 1f : v);

        /// <summary>Starts a repeated-press challenge in the player's current mash mode.</summary>
        /// <param name="requiredPresses">Presses needed in mash mode.</param>
        /// <param name="holdSecondsToFill">Seconds of holding that fill progress in hold mode (author it to feel fair).</param>
        public MashChallenge StartMash(int requiredPresses, float holdSecondsToFill) =>
            new MashChallenge(this, Math.Max(1, requiredPresses), Math.Max(0.05f, holdSecondsToFill));

        /// <summary>
        /// Starts a quick-time event. Normal: authored window. Slow: window at least 3x (and at least the timing
        /// scale), or no limit when <paramref name="canWaitIndefinitely"/> (nothing else moves). Auto: succeeds after
        /// <paramref name="autoBeatSeconds"/> through the same success result as a player press.
        /// The mode is captured at start; a QTE never changes rules halfway through.
        /// </summary>
        public QteSession StartQte(float baseWindowSeconds, bool canWaitIndefinitely = false, float autoBeatSeconds = 0.6f)
        {
            float window;
            QteMode mode = CurrentQteMode;
            switch (mode)
            {
                case QteMode.Slow:
                    window = canWaitIndefinitely
                        ? float.PositiveInfinity
                        : baseWindowSeconds * Math.Max(3f, _store.GetOr("input.timing_window_scale", 1f));
                    break;
                case QteMode.Auto:
                    window = Math.Max(0.1f, autoBeatSeconds);
                    break;
                default:
                    window = baseWindowSeconds;
                    break;
            }
            return new QteSession(mode, window);
        }

        /// <summary>Repeated-press challenge whose completion rule follows input.mash_mode (read live).</summary>
        public sealed class MashChallenge
        {
            private readonly InputAssistCore _owner;
            private readonly int _required;
            private readonly float _holdSeconds;
            private float _progress;

            internal MashChallenge(InputAssistCore owner, int required, float holdSeconds)
            {
                _owner = owner;
                _required = required;
                _holdSeconds = holdSeconds;
            }

            /// <summary>0..1.</summary>
            public float Progress => _progress;
            public bool IsComplete => _progress >= 1f;

            /// <summary>Call on each press edge of the action.</summary>
            public void Press()
            {
                if (IsComplete) return;
                switch (_owner.CurrentMashMode)
                {
                    case MashMode.Single: _progress = 1f; break;
                    case MashMode.Mash: _progress = Math.Min(1f, _progress + 1f / _required); break;
                    case MashMode.Hold: break; // holding is handled in Tick
                }
            }

            /// <summary>Call every frame with the simulation delta and whether the action is down.</summary>
            public void Tick(float deltaSeconds, bool isDown)
            {
                if (IsComplete || !isDown || _owner.CurrentMashMode != MashMode.Hold) return;
                _progress = Math.Min(1f, _progress + deltaSeconds / _holdSeconds);
            }
        }

        /// <summary>One quick-time event. Feed it time and presses; read <see cref="State"/>.</summary>
        public sealed class QteSession
        {
            public QteMode Mode { get; }
            /// <summary>Seconds allowed. +infinity means it waits for the player.</summary>
            public float WindowSeconds { get; }
            public float Elapsed { get; private set; }
            public QteState State { get; private set; } = QteState.Pending;

            internal QteSession(QteMode mode, float window)
            {
                Mode = mode;
                WindowSeconds = window;
            }

            /// <summary>Remaining fraction 1..0 for a timer bar; 1 while waiting indefinitely.</summary>
            public float RemainingFraction =>
                float.IsInfinity(WindowSeconds) ? 1f : Math.Max(0f, 1f - Elapsed / WindowSeconds);

            public QteState Tick(float deltaSeconds)
            {
                if (State != QteState.Pending) return State;
                Elapsed += deltaSeconds;
                if (Elapsed >= WindowSeconds)
                    State = Mode == QteMode.Auto ? QteState.Success : QteState.Failure;
                return State;
            }

            /// <summary>Player input. A correct press succeeds in every mode; a wrong press fails only in Normal mode.</summary>
            public QteState Press(bool correct)
            {
                if (State != QteState.Pending) return State;
                if (correct) State = QteState.Success;
                else if (Mode == QteMode.Normal) State = QteState.Failure;
                return State;
            }
        }
    }
}
