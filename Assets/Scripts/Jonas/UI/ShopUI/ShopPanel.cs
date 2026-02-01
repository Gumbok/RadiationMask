using Game.FirstPerson;
using UnityEngine;

namespace Jonas.UI.ShopUI
{
    public class ShopPanel : MonoBehaviour, IHoldInteractable
    {
        private ShopUI _shopUI;
        private ShopUI ShopUI
        {
            get
            {
                if (!_shopUI) _shopUI = GameObject.Find("ShopUI").GetComponent<ShopUI>();
                return _shopUI;
            }
        }

        [SerializeField] private float interactionHoldTime = 0.25f;
    
        //can open Shop Panel
        public bool CanInteract(in Interactor interactor)
        {
            return !ShopUI.IsOpened;
        }

        public string GetPrompt(in Interactor interactor)
        {
            return "Open Shop";
        }

        public void Interact(in Interactor interactor)
        {
            Debug.Log("1");
            ShopUI.OpenShop();
        }

        public float InteractionHoldTimes => interactionHoldTime;
    }
}
