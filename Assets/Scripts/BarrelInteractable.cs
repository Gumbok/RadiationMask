using Game.FirstPerson;
using UnityEngine;

public class BarrelInteractable : MonoBehaviour, IInteractable, IHoldInteractable
{
    float IHoldInteractable.HoldSeconds => .3f;

    bool IInteractable.CanInteract(in Interactor interactor)
    {
       return true;
    }

    string IInteractable.GetPrompt(in Interactor interactor)
    {
        return "pick up barrel";
    }

    void IInteractable.Interact(in Interactor interactor)
    {
        interactor.GameObject.GetComponent<ItemCarrier>().PickUpItem(this.gameObject);
    }
}
