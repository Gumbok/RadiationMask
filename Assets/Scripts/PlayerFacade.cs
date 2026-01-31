using UnityEngine;
using Game.FirstPerson;
//using Game.Inventory.Runtime;
//using Game.Inventory.World;
//using Game.Wand.Runtime;

namespace Game.Core
{
    public sealed class PlayerFacade : MonoBehaviour
    {
        [Header("Optional (auto-find if missing)")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private FirstPersonLook look;
        [SerializeField] private FirstPersonMovement movement;
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private PlayerHealth health;
        //[SerializeField] private PlayerWandCaster wandCaster;
        [SerializeField] private PlayerXP playerXP;
        [SerializeField] private FirstPersonInteractor interactor;

        [Header("Inventory")]
        //[SerializeField] private PlayerInventory playerInventory;
        //[SerializeField] private PlayerEquipment playerEquipment;
        //[SerializeField] private InventoryWorldDropper inventoryWorldDropper;

        [SerializeField] private Transform cameraFollowTarget;
        public Transform CameraFollowTarget => cameraFollowTarget;


        public CharacterController CharacterController => characterController;
        public FirstPersonLook Look => look;
        public FirstPersonMovement Movement => movement;
        public FirstPersonInteractor Interactor => interactor;
        public PlayerInputReader InputReader => inputReader;

        public PlayerHealth Health => health;

        //public PlayerWandCaster WandCaster => wandCaster;

        public PlayerXP PlayerXP => playerXP;

        //public PlayerInventory Inventory => playerInventory;
        //public PlayerEquipment Equipment => playerEquipment;
        //public InventoryWorldDropper InventoryWorldDropper => inventoryWorldDropper;

        private void Awake()
        {
            if (!characterController) characterController = GetComponentInChildren<CharacterController>();
            if (!look) look = GetComponentInChildren<FirstPersonLook>();
            if (!movement) movement = GetComponentInChildren<FirstPersonMovement>();
            if (!interactor) interactor = GetComponentInChildren<FirstPersonInteractor>();
            //if (!inputReader) inputReader = GetComponentInChildren<PlayerInputReader>();
            //if (!health) health = GetComponentInChildren<PlayerHealth>();
            //if (!wandCaster) wandCaster = GetComponentInChildren<PlayerWandCaster>();
            if (!playerXP) playerXP = GetComponentInChildren<PlayerXP>();

            //if (!playerInventory) playerInventory = GetComponentInChildren<PlayerInventory>();
            //if (!playerEquipment) playerEquipment = GetComponentInChildren<PlayerEquipment>();
            //if (!inventoryWorldDropper) inventoryWorldDropper = GetComponentInChildren<InventoryWorldDropper>();

            if (!cameraFollowTarget)
            {
                // Prefer an explicit child named "CameraRoot" or "CameraTarget"
                var t = transform.Find("CameraRoot") ?? transform.Find("CameraTarget");
                if (t) cameraFollowTarget = t;
                else
                {
                    var cam = GetComponentInChildren<UnityEngine.Camera>();
                    if (cam) cameraFollowTarget = cam.transform;
                    else cameraFollowTarget = transform;
                }
            }
        }

        public void TeleportTo(Transform target)
        {
            if (!target) return;

            var cc = characterController;
            if (cc) cc.enabled = false;

            transform.SetPositionAndRotation(target.position, target.rotation);

            if (cc) cc.enabled = true;
        }

        //public void WipeAllItems()
        //{
        //    // Inventory
        //    if (playerInventory && playerInventory.Model != null)
        //    {
        //        int n = playerInventory.Model.SlotCount;
        //        for (int i = 0; i < n; i++)
        //            playerInventory.Model.SetSlot(i, InventorySlotData.Empty);
        //    }

        //    // Equipment
        //    if (playerEquipment)
        //        playerEquipment.Unequip();
        //}
    }
}
