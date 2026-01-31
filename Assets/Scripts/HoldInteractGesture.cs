using UnityEngine;

namespace Game.FirstPerson
{
    /// <summary>
    /// For interactables only:
    /// - Tap: fires on press.
    /// - Hold: fires when holdSeconds reached, once per hold (waits for release).
    /// Resets immediately if the target key changes.
    /// </summary>
    public sealed class HoldInteractGesture
    {
        private Object _key;
        private bool _wasHeld;
        private float _timer;
        private bool _firedThisHold;

        public void Reset()
        {
            _key = null;
            _wasHeld = false;
            _timer = 0f;
            _firedThisHold = false;
        }

        public void Tick(
            Object key,
            bool isHold,
            float holdSeconds,
            bool heldNow,
            bool pressedThisFrame,
            float dt,
            out float progress01,
            out bool fired)
        {
            progress01 = 0f;
            fired = false;

            if (!ReferenceEquals(key, _key))
            {
                _key = key;
                _wasHeld = false;
                _timer = 0f;
                _firedThisHold = false;
            }

            if (!isHold)
            {
                // Tap fires on press (your requirement).
                fired = pressedThisFrame;
                progress01 = 0f;
                _wasHeld = heldNow;
                return;
            }

            // Hold interaction
            if (!heldNow)
            {
                _wasHeld = false;
                _timer = 0f;
                _firedThisHold = false;
                progress01 = 0f;
                return;
            }

            if (_firedThisHold)
            {
                // Wait for release before allowing another fire.
                progress01 = 1f;
                _wasHeld = true;
                return;
            }

            _timer += Mathf.Max(0f, dt);
            progress01 = Mathf.Clamp01(_timer / Mathf.Max(0.01f, holdSeconds));

            if (_timer >= holdSeconds)
            {
                fired = true;
                _firedThisHold = true;
                progress01 = 1f;
            }

            _wasHeld = true;
        }
    }
}
