using Game.FirstPerson;
using UnityEngine;

public class RefillStation : MonoBehaviour, IHoldInteractable, IInteractable
{
    public float InteractionHoldTimes => 5f;

    public bool CanInteract(in Interactor interactor)
    {
       return true;
    }

    public string GetPrompt(in Interactor interactor)
    {
        return "refill mask";
    }

    public void Interact(in Interactor interactor)
    {
        interactor.GameObject.GetComponent<RadiationMask>().RefillMask();
    }
}
