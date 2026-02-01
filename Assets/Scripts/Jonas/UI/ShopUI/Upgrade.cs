using UnityEngine;

namespace Jonas.UI.ShopUI
{
    [CreateAssetMenu(menuName = "RadiationMask/Upgrade")]
    public class Upgrade : ScriptableObject
    {
        public AnimationCurve cost;
        public int currentUpgradeLevel;
        public int maxUpgradeLevel;

        //needs some saving code, value is persistent between sessions
        public void SetCurrentUpgradeLevel(int level)
        {
            currentUpgradeLevel = level;
        }
    }
}
