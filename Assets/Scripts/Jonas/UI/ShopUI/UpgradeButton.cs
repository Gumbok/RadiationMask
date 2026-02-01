using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jonas.UI.ShopUI
{
    public class UpgradeButton : MonoBehaviour
    {
        private Upgrade _upgrade;
        private PlayerMoney _playerMoney;
        private UpgradeManager _upgradeManager;
        private Button _button;

        private Button Button
        {
            get
            {
                if (_button == null) _button = GetComponent<Button>();
                return _button;
            }
        }
        [SerializeField] private TMP_Text upgradeNameText;
        [SerializeField] private TMP_Text upgradeCostText;
        [SerializeField] private TMP_Text upgradeCurrentLevel;
        [SerializeField] private TMP_Text upgradeNextLevel;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public void Initialize(Upgrade upgrade, PlayerMoney playerMoney, UpgradeManager upgradeManager)
        {
            _playerMoney = playerMoney;
            _upgradeManager = upgradeManager;
            _upgrade = upgrade;
            playerMoney.OnMoneyChanged += UpdateButton;
            UpdateButton();
        }

        public void IncreaseUpdateLevel()
        {
            if (_upgrade.currentUpgradeLevel == _upgrade.maxUpgradeLevel) return;
            if (_playerMoney.money < (int)_upgrade.cost.Evaluate(_upgrade.currentUpgradeLevel+1)) return;

            _playerMoney.Purchase((int)_upgrade.cost.Evaluate(_upgrade.currentUpgradeLevel+1));
            _upgradeManager.upgrades.Find(x => x.name.Equals(_upgrade.name)).currentUpgradeLevel++;
            UpdateButton();
        }

        //Check if Upgrade can be done, show correct Cost and Level
        public void UpdateButton()
        {
            if (_upgrade.currentUpgradeLevel == _upgrade.maxUpgradeLevel) DisableButton();
            if (_playerMoney.money < (int)_upgrade.cost.Evaluate(_upgrade.currentUpgradeLevel+1)) DisableButton();
            
            upgradeNameText.text = _upgrade.name;
            upgradeCostText.text = ((int)_upgrade.cost.Evaluate(_upgrade.currentUpgradeLevel+1)).ToString();
            upgradeCurrentLevel.text = _upgrade.currentUpgradeLevel.ToString();
            int nextLevel = (_upgrade.currentUpgradeLevel != _upgrade.maxUpgradeLevel) ? _upgrade.currentUpgradeLevel + 1 : _upgrade.currentUpgradeLevel;
            upgradeNextLevel.text = nextLevel.ToString();
            
            EnableButton();
        }

        private void EnableButton()
        {
            Button.enabled = true;
            Button.interactable = true;
            Button.onClick.AddListener(IncreaseUpdateLevel);
        }
        
        private void DisableButton()
        {
            Button.interactable = false;
            Button.onClick.RemoveListener(IncreaseUpdateLevel);
        }
    }
}
