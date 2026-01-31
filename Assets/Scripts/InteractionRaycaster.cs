using System;
using UnityEngine;

namespace Game.FirstPerson
{
    [Serializable]
    public sealed class InteractionRaycaster
    {
        [Header("Cast")]
        [SerializeField] private float maxDistance = 2f;
        [SerializeField] private float sphereRadius = 0.12f;
        [SerializeField] private LayerMask castMask = ~0;
        [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;

        [Tooltip("If true, will skip colliders that have no interaction components and keep searching behind them.")]
        [SerializeField] private bool allowPassThroughNonInteractables = true;

        [Header("Non-Alloc Buffer")]
        [SerializeField, Min(1)] private int hitBufferSize = 16;

        private RaycastHit[] _hits;

        public float MaxDistance
        {
            get => maxDistance;
            set => maxDistance = Mathf.Max(0.1f, value);
        }

        public bool TryScan(Transform cameraTransform, Transform ignoreRoot, out HitResult result)
        {
            if (!cameraTransform)
            {
                result = HitResult.None;
                return false;
            }

            EnsureBuffer();

            var ray = new Ray(cameraTransform.position, cameraTransform.forward);
            int hitCount = Physics.SphereCastNonAlloc(
                ray,
                sphereRadius,
                _hits,
                maxDistance,
                castMask,
                triggerInteraction);

            if (hitCount <= 0)
            {
                result = HitResult.None;
                return false;
            }

            float bestDist = float.PositiveInfinity;
            int bestIndex = -1;

            IInteractable bestInteractable = null;
            IHoverable bestHover = null;
            IHighlightable bestHighlight = null;
            UnityEngine.Object bestKey = null;

            for (int i = 0; i < hitCount; i++)
            {
                var h = _hits[i];
                var col = h.collider;
                if (!col) continue;

                if (ignoreRoot && col.transform.IsChildOf(ignoreRoot))
                    continue;

                var interactable = col.GetComponentInParent<IInteractable>();
                var hover = col.GetComponentInParent<IHoverable>();
                var highlight = col.GetComponentInParent<IHighlightable>();

                bool valid = interactable != null || hover != null || highlight != null;

                if (!allowPassThroughNonInteractables)
                {
                    if (h.distance < bestDist)
                    {
                        bestDist = h.distance;
                        bestIndex = i;

                        bestInteractable = interactable;
                        bestHover = hover;
                        bestHighlight = highlight;

                        bestKey =
                            (interactable as Component)
                            ?? (hover as Component)
                            ?? (highlight as Component);
                    }

                    continue;
                }

                if (!valid)
                    continue;

                if (h.distance < bestDist)
                {
                    bestDist = h.distance;
                    bestIndex = i;

                    bestInteractable = interactable;
                    bestHover = hover;
                    bestHighlight = highlight;

                    bestKey =
                        (interactable as Component)
                        ?? (hover as Component)
                        ?? (highlight as Component);
                }
            }

            if (bestIndex < 0)
            {
                result = HitResult.None;
                return false;
            }

            if (!allowPassThroughNonInteractables &&
                bestInteractable == null && bestHover == null && bestHighlight == null)
            {
                result = HitResult.None;
                return false;
            }

            result = new HitResult(
                hasHit: true,
                hit: _hits[bestIndex],
                interactable: bestInteractable,
                hoverable: bestHover,
                highlightable: bestHighlight,
                key: bestKey);

            return true;
        }

        private void EnsureBuffer()
        {
            if (_hits != null && _hits.Length == hitBufferSize) return;
            _hits = new RaycastHit[Mathf.Max(1, hitBufferSize)];
        }
    }
}
