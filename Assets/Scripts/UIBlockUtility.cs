using UnityEngine.EventSystems;

namespace Game.FirstPerson
{
    public static class UIBlockUtility
    {
        /// <summary>
        /// True when UI should block world interactions (pointer over UI or a selected UI element exists).
        /// </summary>
        public static bool IsUIBlockingWorld()
        {
            var es = EventSystem.current;
            if (es == null) return false;

            if (es.currentSelectedGameObject != null) return true;
            if (es.IsPointerOverGameObject()) return true; // mouse/touch pointer

            return false;
        }
    }
}
