// Game Accessibility sample: the camera owner scales every shake, bob, sway and screen effect here.
// Contract: references/architecture.md section 8. Applies to gameplay AND cutscene/scripted shake.
//
// NOT COMPILED IN THIS REPOSITORY: no Unity Editor was available. See engines/unity/GUIDE.md.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameAccessibility.UnityBridge
{
    [Serializable] public sealed class FloatEvent : UnityEvent<float> { }
    [Serializable] public sealed class BoolEvent : UnityEvent<bool> { }

    /// <summary>
    /// Put on the object that owns the camera transform (or a child "shake" pivot under it). Shake sources call
    /// <see cref="AddTrauma"/>; bob code multiplies by <see cref="BobScale"/>, held-item sway by
    /// <see cref="WeaponSwayScale"/>. Automatic and decorative motion asks <see cref="IsMotionEffectEnabled"/>.
    /// Post effects listen to the events and set their own volume/override weights (render-pipeline specific).
    /// The shake scale is applied at final output, so lowering motion.camera_shake during a shake takes effect at
    /// once, and Off clears the shake (see <see cref="CameraShakeCore"/>).
    /// </summary>
    public sealed class CameraComfort : MonoBehaviour
    {
        [SerializeField] private A11yBootstrap bootstrap;

        [Tooltip("Camera whose vertical field of view follows motion.fov. Leave empty for 2D or fixed-FOV games.")]
        [SerializeField] private Camera fovCamera;

        [Tooltip("Transform that receives the shake offset (a pivot under the camera rig, not the rig itself).")]
        [SerializeField] private Transform shakePivot;

        [SerializeField] private float maxShakeOffset = 0.3f;
        [SerializeField] private float maxShakeRollDegrees = 3f;
        [SerializeField] private float traumaDecayPerSecond = 1.5f;
        [SerializeField] private float shakeFrequency = 18f;

        [Tooltip("Invoked with motion.screen_effects (0..1). Wire to vignette/chromatic aberration/damage overlay weights.")]
        public FloatEvent onScreenEffectsScale = new FloatEvent();

        [Tooltip("Invoked with whether motion blur may be enabled. Wire to your pipeline's motion blur override.")]
        public BoolEvent onMotionBlurEnabled = new BoolEvent();

        [Tooltip("Invoked with whether chromatic aberration may be enabled (motion.chromatic_aberration).")]
        public BoolEvent onChromaticAberrationEnabled = new BoolEvent();

        [Tooltip("Invoked with whether depth of field may be enabled (motion.depth_of_field).")]
        public BoolEvent onDepthOfFieldEnabled = new BoolEvent();

        [Tooltip("Invoked with motion.reduce_motion (the umbrella). Prefer onMotionEffectsChanged with IsMotionEffectEnabled.")]
        public BoolEvent onReduceMotion = new BoolEvent();

        [Tooltip("Invoked when motion.reduce_motion or a per-effect override (auto camera, FOV effects, speed lines, " +
                 "parallax, menu animation) changes. Re-read IsMotionEffectEnabled for each effect you own.")]
        public UnityEvent onMotionEffectsChanged = new UnityEvent();

        private A11ySettingsStore _store;
        private CameraShakeCore _shake;
        private IDisposable[] _subs = Array.Empty<IDisposable>();
        private float _seed;

        /// <summary>
        /// motion.camera_shake (0..1). AddTrauma and the pivot apply it already. Multiply shake you compute yourself
        /// (Cinemachine impulses, Timeline) by this every frame, including cutscenes.
        /// </summary>
        public float ShakeScale => _shake?.Scale ?? (_store?.GetOr("motion.camera_shake", 1f) ?? 1f);
        /// <summary>motion.head_bob (0..1). Multiply walk/run camera bob amplitude by this.</summary>
        public float BobScale => _store?.GetOr("motion.head_bob", 1f) ?? 1f;
        /// <summary>motion.weapon_sway (0..1), separate from head bob. Multiply held-item sway and breathing by this. Visual only: never change accuracy.</summary>
        public float WeaponSwayScale => _store?.GetOr("motion.weapon_sway", 1f) ?? 1f;
        public float ScreenEffectsScale => _store?.GetOr("motion.screen_effects", 1f) ?? 1f;
        public bool ReduceMotion => _store?.GetOr("motion.reduce_motion", false) ?? false;

        /// <summary>
        /// Whether an automatic or decorative motion effect may play: its own override ("on"/"off") or, on "follow",
        /// !motion.reduce_motion. Without a store the authored effect plays.
        /// </summary>
        public bool IsMotionEffectEnabled(MotionEffect effect) =>
            _store == null || PresentationRules.MotionEffectEnabled(_store, effect);

        private void Awake() => _seed = UnityEngine.Random.value * 100f;

        private void OnEnable()
        {
            var boot = A11yBootstrap.Resolve(bootstrap);
            _store = boot != null ? boot.Store : null;
            if (_store == null) return;
            _shake = new CameraShakeCore(_store) { DecayPerSecond = traumaDecayPerSecond };
            // Observe = read the current value now, then follow changes (late spawns are covered).
            var subs = new List<IDisposable>
            {
                _store.Observe("motion.fov", v => ApplyFov(Convert.ToSingle(v))),
                _store.Observe("motion.screen_effects", v => onScreenEffectsScale.Invoke(Convert.ToSingle(v))),
                _store.Observe("motion.motion_blur", v => onMotionBlurEnabled.Invoke((bool)v)),
                _store.Observe("motion.chromatic_aberration", v => onChromaticAberrationEnabled.Invoke((bool)v)),
                _store.Observe("motion.depth_of_field", v => onDepthOfFieldEnabled.Invoke((bool)v)),
                _store.Observe("motion.reduce_motion", v => { onReduceMotion.Invoke((bool)v); onMotionEffectsChanged.Invoke(); }),
            };
            foreach (MotionEffect e in (MotionEffect[])Enum.GetValues(typeof(MotionEffect)))
                subs.Add(_store.Observe(PresentationRules.MotionKey(e), _ => onMotionEffectsChanged.Invoke()));
            _subs = subs.ToArray();
        }

        private void OnDisable()
        {
            foreach (var s in _subs) s.Dispose();
            _subs = Array.Empty<IDisposable>();
            _shake?.Dispose();
            _shake = null; // trauma is discarded: re-enabling never resumes an old shake
            ResetPivot();
        }

        private void ResetPivot()
        {
            if (shakePivot == null) return;
            shakePivot.localPosition = Vector3.zero;
            shakePivot.localRotation = Quaternion.identity;
        }

        private void ApplyFov(float verticalDegrees)
        {
            // Camera.fieldOfView is vertical by default. If your rig zooms (aim, sprint), apply zoom as a
            // multiplier on top of this base value instead of overwriting it; skip that zoom when
            // IsMotionEffectEnabled(MotionEffect.FovEffects) is false.
            if (fovCamera != null && !fovCamera.orthographic) fovCamera.fieldOfView = verticalDegrees;
        }

        /// <summary>
        /// Adds shake. <paramref name="amount"/> 0..1 as authored. The player's scale is applied at output every
        /// frame; while motion.camera_shake is 0 the request is ignored.
        /// </summary>
        public void AddTrauma(float amount) => _shake?.AddTrauma(amount);

        /// <summary>For code that computes its own offset (procedural animation, Cinemachine impulses): scale it here every frame.</summary>
        public Vector3 ScaleShakeOffset(Vector3 authoredOffset) => authoredOffset * ShakeScale;

        private void LateUpdate()
        {
            if (_shake == null) return;
            _shake.DecayPerSecond = traumaDecayPerSecond;
            // Current scale x squared trauma (a gentle tail), read every frame: Off mid-shake rests the camera now.
            float k = _shake.Amplitude;
            if (shakePivot == null || k <= 0f)
            {
                ResetPivot();
                _shake.Tick(Time.unscaledDeltaTime);
                return;
            }
            // Unscaled time: shake reads the same at any game speed.
            float t = Time.unscaledTime * shakeFrequency;
            shakePivot.localPosition = new Vector3(
                (Mathf.PerlinNoise(_seed, t) * 2f - 1f) * maxShakeOffset * k,
                (Mathf.PerlinNoise(_seed + 1f, t) * 2f - 1f) * maxShakeOffset * k,
                0f);
            shakePivot.localRotation = Quaternion.Euler(0f, 0f,
                (Mathf.PerlinNoise(_seed + 2f, t) * 2f - 1f) * maxShakeRollDegrees * k);
            _shake.Tick(Time.unscaledDeltaTime);
        }
    }
}
