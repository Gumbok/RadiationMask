using Game.FirstPerson;
using Jonas.UI.ShopUI;
using UnityEngine;

public class ShopPanel : MonoBehaviour, IHoldInteractable
{
    [Tooltip("Parent UI GameObject with the shop interface.")]
    public ShopUI shopUI;
    
    //can open Shop Panel
    public bool CanInteract(in Interactor interactor)
    {
        return !shopUI.gameObject.activeSelf;
    }

    public string GetPrompt(in Interactor interactor)
    {
        throw new System.NotImplementedException();
    }

    public void Interact(in Interactor interactor)
    {
        shopUI.OpenShop();
    }

    public float HoldSeconds { get; }
}
