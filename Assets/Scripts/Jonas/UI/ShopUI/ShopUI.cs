using UnityEngine;

namespace Jonas.UI.ShopUI
{
    public class ShopUI : MonoBehaviour
    {
        public void OpenShop()
        {
            gameObject.SetActive(true);
        }

        public void CloseShop()
        {
            gameObject.SetActive(false);
        }
    }
}
