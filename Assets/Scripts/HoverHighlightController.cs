using UnityEngine;

namespace Game.FirstPerson
{
    public sealed class HoverHighlightController
    {
        private Object _currentKey;
        private IHoverable _currentHover;
        private IHighlightable _currentHighlight;

        public bool Apply(in HitResult hit, in Interactor interactor)
        {
            var key = hit.Key;

            if (ReferenceEquals(key, _currentKey))
                return false;

            // Exit old
            if (_currentHover != null)
                _currentHover.OnHoverExit(in interactor);

            if (_currentHighlight != null)
                _currentHighlight.SetHighlighted(false);

            _currentKey = key;
            _currentHover = hit.Hoverable;
            _currentHighlight = hit.Highlightable;

            // Enter new
            if (_currentHover != null)
                _currentHover.OnHoverEnter(in interactor);

            if (_currentHighlight != null)
                _currentHighlight.SetHighlighted(true);

            return true;
        }

        public void Clear(in Interactor interactor)
        {
            if (_currentHover != null)
                _currentHover.OnHoverExit(in interactor);

            if (_currentHighlight != null)
                _currentHighlight.SetHighlighted(false);

            _currentKey = null;
            _currentHover = null;
            _currentHighlight = null;
        }
    }
}
