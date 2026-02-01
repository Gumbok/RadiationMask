using System;
using Game.FirstPerson;
using UnityEngine;

namespace Jonas.UI.ShopUI
{
    public class ShopUI : MonoBehaviour
    {
        private UpgradeManager _upgradeManager;
        private UpgradeManager UpgradeManager
        {
            get
            {
                if (!_upgradeManager) _upgradeManager = GetComponentInChildren<UpgradeManager>();
                return _upgradeManager;
            }
        }

        [SerializeField] private GameObject shopUpgradeMenu;

        private GameObject _player;
        private GameObject Player
        {
            get
            {
                if (!_player) _player = GameObject.Find("Player");
                return _player;
            }
        }

        [NonSerialized] public bool IsOpened = false;

        private void Awake()
        {
            shopUpgradeMenu.SetActive(false);
        }

        public void OpenShop()
        {
            IsOpened = true;
            shopUpgradeMenu.SetActive(true);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            UpgradeManager.SelectFirstButton();
            Player.GetComponent<FirstPersonMovement>().enabled = false;
            Player.GetComponent<FirstPersonLook>().enabled = false;
        }

        public void CloseShop()
        {
            IsOpened = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Player.GetComponent<FirstPersonMovement>().enabled = true;
            Player.GetComponent<FirstPersonLook>().enabled = true;
            shopUpgradeMenu.SetActive(false);
        }
    }
}
