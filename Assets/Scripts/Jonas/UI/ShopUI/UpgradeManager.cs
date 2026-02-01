using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jonas.UI.ShopUI
{
    //central control for upgrades
    public class UpgradeManager : MonoBehaviour
    {
        [Tooltip("Will set current upgrade levels to 0 if true. Set to false to have persistent levels when loading new levels.")]
        public bool resetUpgradeLevel = true;
        
        public List<Upgrade> upgrades = new ();
        public GameObject upgradeButtonTemplate;
        
        private PlayerMoney _playerMoney;
        private PlayerMoney PlayerMoney
        {
            get
            {
                if (_playerMoney == null) _playerMoney = GameObject.Find("Player").GetComponent<PlayerMoney>();
                
                return _playerMoney;
            }
        }
        private readonly Dictionary<Upgrade, Button> _buttonDict = new();
        

        private void Awake()
        {
            Transform buttonParent = upgradeButtonTemplate.transform.parent;
            foreach (Upgrade upgrade in upgrades)
            {
                if(resetUpgradeLevel) upgrade.SetCurrentUpgradeLevel(0); //TODO <- hardcoded for testing, will be problematic for loading new levels
                
                GameObject newUpgradeButton = Instantiate(upgradeButtonTemplate, buttonParent);
                newUpgradeButton.GetComponent<UpgradeButton>().Initialize(upgrade, PlayerMoney,this);
                _buttonDict.Add(upgrade, newUpgradeButton.GetComponent<Button>());
            }
            Destroy(upgradeButtonTemplate);
            
            gameObject.SetActive(false);
        }

        //Select FirstButton for Interaction
        public void SelectFirstButton()
        {
            if (_buttonDict.Count == 0) return;
            _buttonDict[upgrades[0]].Select();
        }
    }
}
