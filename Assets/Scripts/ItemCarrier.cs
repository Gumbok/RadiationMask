using Game.FirstPerson;
using UnityEngine;

public class ItemCarrier : MonoBehaviour
{
    public PlayerInputReader playerInputReader;

    public Transform carryPivot;
    public bool isCarryingItem => carryPivot.childCount > 0;


    private void Update()
    {
        if (playerInputReader != null && playerInputReader.InventoryPressedThisFrame && isCarryingItem)
        {
            DropItem(carryPivot.GetChild(0).gameObject);
        }
    }

    public void PickUpItem(GameObject item)
    {
        item.TryGetComponent<Collider>(out Collider itemCollider);
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }
        item.TryGetComponent<Rigidbody>(out Rigidbody itemRigidbody);
        if (itemRigidbody != null)
        {
            itemRigidbody.isKinematic = true;
        }

        item.transform.SetParent(carryPivot);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    public void DropItem(GameObject item)
    {
        item.transform.SetParent(null);
        item.TryGetComponent<Collider>(out Collider itemCollider);
        if (itemCollider != null)
        {
            itemCollider.enabled = true;
        }
        item.TryGetComponent<Rigidbody>(out Rigidbody itemRigidbody);
        if (itemRigidbody != null)
        {
            itemRigidbody.isKinematic = false;
        }
    }
}
