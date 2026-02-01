using UnityEngine;

namespace Jonas.UI.ShopUI
{
    public class ShopZone : MonoBehaviour
    {
        [Tooltip("Parent UI GameObject with the shop interface.")]
        public ShopUI shopUI;

        public void OnTriggerEnter(Collider other)
        {
            if (!other.tag.Equals("Player")) return;
            
            shopUI.OpenShop();
        }
        
        public void OnTriggerExit(Collider other)
        {
            if (!other.tag.Equals("Player")) return;
            
            shopUI.CloseShop();
        }
    }
}
