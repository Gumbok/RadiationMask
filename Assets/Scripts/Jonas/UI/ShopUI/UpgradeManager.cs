using System.Collections.Generic;
using UnityEngine;

namespace Jonas.UI.ShopUI
{
    public class UpgradeManager : MonoBehaviour
    {
        public List<Upgrade> upgrades = new ();
        public GameObject upgradeButtonTemplate;
        public PlayerMoney playerMoney;

        private void Awake()
        {
            Transform buttonParent = upgradeButtonTemplate.transform.parent;
            foreach (Upgrade upgrade in upgrades)
            {
                GameObject newUpgradeButton = Instantiate(upgradeButtonTemplate, buttonParent);
                newUpgradeButton.GetComponent<UpgradeButton>().Initialize(upgrade, playerMoney,this);
            }
            Destroy(upgradeButtonTemplate);
        }
        
    }
}
