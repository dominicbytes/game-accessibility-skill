// Game Accessibility sample: narration routing and the self-voicing queue, without Unity.
// Pure C# (netstandard2.1). No UnityEngine references. The Unity Narrator component drives it every frame.
//
// Contract: references/architecture.md section 5.
// - One route per utterance: the native screen reader (announcement sink) when it is active, otherwise
//   self-voicing through an INarrationBackend when the utterance's own setting is on.
// - Each utterance belongs to a channel with its own setting: Menu (focus, menu and game announcements,
//   narration.enabled) or Dialogue (unvoiced lines, narration.dialogue). Turning a channel's setting off stops
//   and discards that channel's current and queued speech at once; the other channel is untouched.
// - Route ownership and the channel's setting are checked again before every queued utterance is dispatched.
// - Handover to the native screen reader (one policy for the active utterance and the queue): self-voicing is
//   stopped first, then the still-required entries go to the reader in order, the interrupted active one first.
//   Still required = any critical entry, and dialogue that is not focus speech. Menu and focus speech is dropped
//   (the reader speaks focused controls itself). Critical information is never lost and no native announcement
//   is ever sent while self-voicing is still speaking. The backend cannot report how far it got, so an
//   interrupted utterance is re-announced in full.

using System;
using System.Collections.Generic;

namespace GameAccessibility
{
    public enum NarrationPriority
    {
        /// <summary>Focus speech: replaced by newer focus speech; skipped when a native screen reader handles focus.</summary>
        Focus,
        Normal,
        /// <summary>Never dropped from the queue; interrupts non-critical speech.</summary>
        Critical,
    }

    /// <summary>The player setting that owns an utterance.</summary>
    public enum NarrationChannel
    {
        /// <summary>Focus changes, menus and game announcements. Gated by narration.enabled.</summary>
        Menu,
        /// <summary>Unvoiced dialogue and notes read aloud. Gated by narration.dialogue.</summary>
        Dialogue,
    }

    /// <summary>A text-to-speech engine. Implement one per platform you ship.</summary>
    public interface INarrationBackend
    {
        bool IsAvailable { get; }
        bool IsSpeaking { get; }
        void Speak(string text, bool interrupt);
        /// <summary>Silences speech at once: IsSpeaking is false when it returns. Handover to the screen reader relies on it.</summary>
        void Stop();
        /// <summary>1 = the voice's normal rate. Receives narration.rate (0.5..2.5).</summary>
        void SetRate(float rate);
    }

    public sealed class NarrationQueue : IDisposable
    {
        public const string MenuKey = "narration.enabled";
        public const string DialogueKey = "narration.dialogue";

        private readonly struct Entry
        {
            public readonly string Text;
            public readonly NarrationPriority Priority;
            public readonly NarrationChannel Channel;

            public Entry(string text, NarrationPriority priority, NarrationChannel channel)
            {
                Text = text;
                Priority = priority;
                Channel = channel;
            }
        }

        private readonly A11ySettingsStore _store;
        private readonly List<Entry> _queue = new List<Entry>();
        private readonly IDisposable[] _subscriptions;
        private Entry _current;
        private bool _hasCurrent;

        public NarrationQueue(A11ySettingsStore store, int maxQueue = 8)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            MaxQueue = maxQueue;
            _subscriptions = new[]
            {
                _store.Observe(MenuKey, v => { if (!(v is bool on && on)) StopChannel(NarrationChannel.Menu); }),
                _store.Observe(DialogueKey, v => { if (!(v is bool on && on)) StopChannel(NarrationChannel.Dialogue); }),
            };
        }

        /// <summary>Self-voicing engine. Null = no self-voicing available.</summary>
        public INarrationBackend Backend { get; set; }

        /// <summary>Returns true while an OS screen reader is running and reachable natively. Null = never.</summary>
        public Func<bool> NativeRouteActive { get; set; }

        /// <summary>Sends an announcement through the native screen reader (for example SendAnnouncement).</summary>
        public Action<string> Announce { get; set; }

        /// <summary>Queued self-voicing utterances kept at most (critical ones are never dropped).</summary>
        public int MaxQueue { get; set; }

        public int PendingCount => _queue.Count;

        /// <summary>True while an utterance dispatched by this queue is still being spoken.</summary>
        public bool IsSelfVoicing => _hasCurrent && Backend != null && Backend.IsSpeaking;

        public bool IsNativeRoute => NativeRouteActive != null && NativeRouteActive();

        /// <summary>Whether the channel's own setting is on. A key the game did not register counts as off.</summary>
        public bool ChannelEnabled(NarrationChannel channel) =>
            _store.GetOr(channel == NarrationChannel.Dialogue ? DialogueKey : MenuKey, false);

        private bool CanSelfVoice(NarrationChannel channel) =>
            ChannelEnabled(channel) && Backend != null && Backend.IsAvailable;

        private NarrationPriority CurrentPriority => _hasCurrent ? _current.Priority : NarrationPriority.Normal;

        public void Speak(string text, NarrationPriority priority = NarrationPriority.Normal,
            NarrationChannel channel = NarrationChannel.Menu)
        {
            if (string.IsNullOrEmpty(text)) return;

            if (IsNativeRoute)
            {
                HandOverToNativeRoute(); // the reader may have started since the last Tick: never speak over self-voicing
                if (priority == NarrationPriority.Focus) return; // the screen reader already reads focused nodes
                Announce?.Invoke(text);
                return;
            }

            if (!CanSelfVoice(channel)) return;
            var entry = new Entry(text, priority, channel);

            if (priority == NarrationPriority.Focus)
            {
                _queue.RemoveAll(e => e.Priority == NarrationPriority.Focus); // stale focus speech is useless
                if (Backend.IsSpeaking && CurrentPriority == NarrationPriority.Focus)
                {
                    SpeakNow(entry, interrupt: true);
                    return;
                }
            }

            if (priority == NarrationPriority.Critical)
            {
                if (Backend.IsSpeaking && CurrentPriority != NarrationPriority.Critical)
                {
                    SpeakNow(entry, interrupt: true);
                    return;
                }
                _queue.Insert(CountLeadingCritical(), entry);
                return;
            }

            if (_queue.Count >= Math.Max(1, MaxQueue))
            {
                int drop = _queue.FindIndex(e => e.Priority != NarrationPriority.Critical);
                if (drop < 0) return;
                _queue.RemoveAt(drop);
            }
            _queue.Add(entry);
        }

        /// <summary>Call once per frame. Dispatches the next queued utterance when the backend is idle.</summary>
        public void Tick()
        {
            if (Backend == null) return;
            bool speaking = Backend.IsSpeaking;
            if (!speaking) _hasCurrent = false;

            if (IsNativeRoute)
            {
                HandOverToNativeRoute();
                return;
            }

            if (speaking) return;
            while (_queue.Count > 0)
            {
                Entry next = _queue[0];
                _queue.RemoveAt(0);
                if (!CanSelfVoice(next.Channel)) continue; // its setting was turned off, or the backend went away
                SpeakNow(next, interrupt: false);
                return;
            }
        }

        // The OS screen reader owns speech now. Policy "stop and transfer": any self-voiced utterance still playing is
        // stopped BEFORE anything is announced natively, so two voices never overlap. Then the still-required
        // entries are handed to the reader in order: the interrupted active utterance first (re-announced in full,
        // since the backend cannot report its position), then the queue (critical entries already lead it).
        // Menu and focus speech is dropped: the reader speaks the focused control itself.
        private void HandOverToNativeRoute()
        {
            bool transferCurrent = false;
            Entry current = _current;
            if (_hasCurrent)
            {
                _hasCurrent = false;
                if (Backend != null && Backend.IsSpeaking)
                {
                    Backend.Stop();
                    transferCurrent = StillRequired(current);
                }
            }
            if (!transferCurrent && _queue.Count == 0) return;
            var pending = _queue.ToArray();
            _queue.Clear();
            if (transferCurrent) Announce?.Invoke(current.Text);
            foreach (Entry e in pending)
            {
                if (StillRequired(e)) Announce?.Invoke(e.Text);
            }
        }

        // Critical information, and dialogue the player would otherwise miss, survives a route change.
        private static bool StillRequired(Entry e) =>
            e.Priority == NarrationPriority.Critical
            || (e.Channel == NarrationChannel.Dialogue && e.Priority != NarrationPriority.Focus);

        /// <summary>Stops and discards current and queued speech of one channel only.</summary>
        public void StopChannel(NarrationChannel channel)
        {
            _queue.RemoveAll(e => e.Channel == channel);
            if (_hasCurrent && _current.Channel == channel)
            {
                _hasCurrent = false;
                Backend?.Stop();
            }
        }

        /// <summary>Stops all self-voicing and empties the queue.</summary>
        public void Stop()
        {
            _queue.Clear();
            _hasCurrent = false;
            Backend?.Stop();
        }

        public void Dispose()
        {
            foreach (var s in _subscriptions) s.Dispose();
        }

        private int CountLeadingCritical()
        {
            int i = 0;
            while (i < _queue.Count && _queue[i].Priority == NarrationPriority.Critical) i++;
            return i;
        }

        private void SpeakNow(Entry entry, bool interrupt)
        {
            _current = entry;
            _hasCurrent = true;
            Backend.Speak(entry.Text, interrupt);
        }
    }
}
