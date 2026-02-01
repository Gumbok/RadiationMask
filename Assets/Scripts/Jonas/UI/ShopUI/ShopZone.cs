using UnityEngine;

namespace Jonas.UI.ShopUI
{
    public class ShopZone : MonoBehaviour
    {
        [Tooltip("Parent UI GameObject with the shop interface.")]
        private ShopUI _shopUI;
        private ShopUI ShopUI
        {
            get
            {
                if (!_shopUI) _shopUI = GameObject.Find("ShopUI").GetComponent<ShopUI>();
                return _shopUI;
            }
        }
        
        public void OnTriggerExit(Collider other)
        {
            if (!other.tag.Equals("Player")) return;
            
            ShopUI.CloseShop();
        }
    }
}
