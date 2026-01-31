using UnityEngine;

namespace Jonas.UI.ShopUI
{
    [CreateAssetMenu(menuName = "RadiationMask/Upgrade")]
    public class Upgrade : ScriptableObject
    {
        public AnimationCurve cost;
        public int currentUpgradeLevel;
        public int maxUpgradeLevel;
    }
}
