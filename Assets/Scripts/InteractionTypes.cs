using UnityEngine;

namespace Game.FirstPerson
{
    public readonly struct HitResult
    {
        public readonly bool HasHit;
        public readonly RaycastHit Hit;

        public readonly IInteractable Interactable;
        public readonly IHoverable Hoverable;
        public readonly IHighlightable Highlightable;

        /// <summary>Key object for hover/highlight transitions (prefer parent component).</summary>
        public readonly Object Key;

        public HitResult(
            bool hasHit,
            in RaycastHit hit,
            IInteractable interactable,
            IHoverable hoverable,
            IHighlightable highlightable,
            Object key)
        {
            HasHit = hasHit;
            Hit = hit;
            Interactable = interactable;
            Hoverable = hoverable;
            Highlightable = highlightable;
            Key = key;
        }

        public static HitResult None => default;
    }
}
