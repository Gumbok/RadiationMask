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

        private GameObject _player;
        private GameObject Player
        {
            get
            {
                if (!_player) _player = GameObject.Find("Player");
                return _player;
            }
        }
        
        public void OpenShop()
        {
            gameObject.SetActive(true);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            UpgradeManager.SelectFirstButton();
            Player.GetComponent<FirstPersonMovement>().enabled = false;
            Player.GetComponent<FirstPersonLook>().enabled = false;
        }

        public void CloseShop()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Player.GetComponent<FirstPersonMovement>().enabled = true;
            Player.GetComponent<FirstPersonLook>().enabled = true;
            gameObject.SetActive(false);
        }
    }
}
