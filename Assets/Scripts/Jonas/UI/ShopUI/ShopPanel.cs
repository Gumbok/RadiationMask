using Game.FirstPerson;
using UnityEngine;

namespace Jonas.UI.ShopUI
{
    public class ShopPanel : MonoBehaviour, IHoldInteractable
    {
        [Tooltip("Parent UI GameObject with the shop interface.")]
        public ShopUI shopUI;

        [SerializeField] private float interactionHoldTime = 0.25f;
    
        //can open Shop Panel
        public bool CanInteract(in Interactor interactor)
        {
            return !shopUI.gameObject.activeSelf;
        }

        public string GetPrompt(in Interactor interactor)
        {
            return "Open Shop";
        }

        public void Interact(in Interactor interactor)
        {
            shopUI.OpenShop();
        }

        public float InteractionHoldTimes => interactionHoldTime;
    }
}
