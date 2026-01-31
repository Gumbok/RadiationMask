using System;
using UnityEngine;

namespace Game.FirstPerson
{
    /// <summary>
    /// World interaction only. No equipment, no item-use, no wand.
    /// Tap fires on press. Hold uses IHoldInteractable.HoldSeconds.
    /// </summary>
    public sealed class FirstPersonInteractor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private Transform cameraTransform;

        [Header("Targeting")]
        [SerializeField] private InteractionRaycaster raycaster = new InteractionRaycaster();

        public event Action<string, float> OnPrompt;            // (text, progress01)
        public event Action<bool> OnCanInteractChanged;         // visible

        private readonly HoverHighlightController _hover = new HoverHighlightController();
        private readonly HoldInteractGesture _gesture = new HoldInteractGesture();

        private bool _visible;
        private string _lastPrompt = string.Empty;
        private float _lastProgress;

        private void Awake()
        {
            if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            if (!input || !cameraTransform)
                return;

            if (UIBlockUtility.IsUIBlockingWorld())
            {
                ClearAll();
                return;
            }

            var interactor = new Interactor(gameObject, transform, cameraTransform);

            HitResult hit = HitResult.None;
            raycaster.TryScan(cameraTransform, ignoreRoot: transform, out hit);

            // hover/highlight can still work even if object is not interactable
            _hover.Apply(hit, in interactor);

            var target = hit.Interactable;
            bool canInteract = target != null && target.CanInteract(in interactor);

            if (!canInteract)
            {
                _gesture.Reset();
                SetVisible(false);
                PushPrompt(string.Empty, 0f);
                return;
            }

            string prompt = target.GetPrompt(in interactor) ?? string.Empty;

            float holdSeconds = 0f;
            bool isHold = false;

            if (target is IHoldInteractable h)
            {
                holdSeconds = Mathf.Max(0.05f, h.HoldSeconds);
                isHold = holdSeconds > 0.05f;
            }

            _gesture.Tick(
                key: hit.Key ? hit.Key : (target as Component),
                isHold: isHold,
                holdSeconds: holdSeconds,
                heldNow: input.InteractHeld,
                pressedThisFrame: input.InteractPressedThisFrame,
                dt: Time.deltaTime,
                out float progress01,
                out bool fired);

            SetVisible(true);
            PushPrompt(prompt, isHold ? progress01 : 0f);

            if (fired)
                target.Interact(in interactor);
        }

        private void ClearAll()
        {
            var interactor = (!cameraTransform)
                ? new Interactor(gameObject, transform, null)
                : new Interactor(gameObject, transform, cameraTransform);

            _hover.Clear(in interactor);
            _gesture.Reset();
            SetVisible(false);
            PushPrompt(string.Empty, 0f);
        }

        private void SetVisible(bool visible)
        {
            if (_visible == visible) return;
            _visible = visible;
            OnCanInteractChanged?.Invoke(_visible);

            if (!_visible)
                PushPrompt(string.Empty, 0f);
        }

        private void PushPrompt(string prompt, float progress01)
        {
            prompt ??= string.Empty;

            if (ReferenceEquals(prompt, _lastPrompt) && Mathf.Abs(progress01 - _lastProgress) < 0.0005f)
                return;

            _lastPrompt = prompt;
            _lastProgress = progress01;
            OnPrompt?.Invoke(prompt, progress01);
        }
    }
}
