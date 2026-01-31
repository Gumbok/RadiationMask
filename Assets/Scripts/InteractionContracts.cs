using UnityEngine;

namespace Game.FirstPerson
{
    public readonly struct Interactor
    {
        public readonly GameObject GameObject;
        public readonly Transform Transform;
        public readonly Transform CameraTransform;

        public Interactor(GameObject gameObject, Transform transform, Transform cameraTransform)
        {
            GameObject = gameObject;
            Transform = transform;
            CameraTransform = cameraTransform;
        }
    }

    public interface IInteractable
    {
        bool CanInteract(in Interactor interactor);
        string GetPrompt(in Interactor interactor);
        void Interact(in Interactor interactor);
    }

    /// <summary>Optional: per-object hold duration override.</summary>
    public interface IHoldInteractable : IInteractable
    {
        float HoldSeconds { get; }
    }

    /// <summary>Optional: hover callbacks (separate from highlight so you can play sounds, etc.).</summary>
    public interface IHoverable
    {
        void OnHoverEnter(in Interactor interactor);
        void OnHoverExit(in Interactor interactor);
    }

    /// <summary>
    /// Generic highlight hook. Implement this on whatever should toggle an outline/highlight.
    /// </summary>
    public interface IHighlightable
    {
        void SetHighlighted(bool highlighted);
    }
}
