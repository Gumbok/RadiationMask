using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.FirstPerson
{
    public enum LookDeviceKind
    {
        MouseKeyboard = 0,
        Gamepad = 1
    }

    public sealed class PlayerInputReader : MonoBehaviour
    {
        [Header("Input Actions Asset")]
        [SerializeField] private InputActionAsset actions;
        [SerializeField] private bool instantiateActionsAsset = true;

        [Header("Action Paths (Map/Action)")]
        [SerializeField] private string movePath = "Player/Move";
        [SerializeField] private string lookPath = "Player/Look";
        [SerializeField] private string jumpPath = "Player/Jump";
        [SerializeField] private string sprintPath = "Player/Sprint";
        [SerializeField] private string crouchPath = "Player/Crouch";
        [SerializeField] private string maskTogglePath = "Player/MaskToggle";
        [SerializeField] private string interactPath = "Player/Interact";
        [SerializeField] private string attackPath = "Player/Attack";
        [SerializeField] private string inventoryPath = "Player/Inventory";

        [Header("Look Device Detection")]
        [SerializeField] private float lookDetectEpsilon = 0.0015f;

        private InputActionAsset _assetInstance;

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _jumpAction;
        private InputAction _sprintAction;
        private InputAction _crouchAction;
        private InputAction _maskToggleAction;
        private InputAction _interactAction;
        private InputAction _attackAction;
        private InputAction _inventoryAction;

        private Vector2 _move;
        private Vector2 _look;

        private LookDeviceKind _lastLookDevice = LookDeviceKind.MouseKeyboard;

        private bool _resolved;
        private bool _warnedMissing;

        public Vector2 Move => _move;
        public Vector2 Look => _look;

        public bool SprintHeld => _sprintAction != null && _sprintAction.IsPressed();
        public bool CrouchHeld => _crouchAction != null && _crouchAction.IsPressed();

        public bool InteractHeld => _interactAction != null && _interactAction.IsPressed();
        public bool AttackHeld => _attackAction != null && _attackAction.IsPressed();

        public bool InteractPressedThisFrame => _interactAction != null && _interactAction.WasPressedThisFrame();
        public bool AttackPressedThisFrame => _attackAction != null && _attackAction.WasPressedThisFrame();

        public bool MaskTogglePressedThisFrame => _maskToggleAction != null && _maskToggleAction.WasPressedThisFrame();
        public bool JumpPressedThisFrame => _jumpAction != null && _jumpAction.WasPressedThisFrame();
        public bool InventoryPressedThisFrame => _inventoryAction != null && _inventoryAction.WasPressedThisFrame();

        public LookDeviceKind LastLookDevice => _lastLookDevice;

        private void OnEnable()
        {
            ResolveActionsIfNeeded();
            EnableAction(_moveAction);
            EnableAction(_lookAction);
            EnableAction(_jumpAction);
            EnableAction(_sprintAction);
            EnableAction(_crouchAction);
            EnableAction(_maskToggleAction);
            EnableAction(_interactAction);
            EnableAction(_attackAction);
            EnableAction(_inventoryAction);
        }

        private void OnDisable()
        {
            DisableAction(_moveAction);
            DisableAction(_lookAction);
            DisableAction(_jumpAction);
            DisableAction(_sprintAction);
            DisableAction(_crouchAction);
            DisableAction(_maskToggleAction);
            DisableAction(_interactAction);
            DisableAction(_attackAction);
            DisableAction(_inventoryAction);
        }

        private void Update()
        {
            ResolveActionsIfNeeded();

            _move = ReadVector2(_moveAction);
            _look = ReadVector2(_lookAction);

            if (_look.sqrMagnitude >= lookDetectEpsilon * lookDetectEpsilon && _lookAction != null)
            {
                var device = _lookAction.activeControl?.device;
                if (device is Gamepad)
                    _lastLookDevice = LookDeviceKind.Gamepad;
                else if (device is Mouse)
                    _lastLookDevice = LookDeviceKind.MouseKeyboard;
            }
        }

        private void ResolveActionsIfNeeded()
        {
            if (_resolved) return;

            if (!actions)
            {
                if (!_warnedMissing)
                {
                    _warnedMissing = true;
                    Debug.LogWarning($"{nameof(PlayerInputReader)}: No InputActionAsset assigned on '{name}'. Input will be inactive.", this);
                }
                return;
            }

            if (!_assetInstance)
                _assetInstance = instantiateActionsAsset ? Instantiate(actions) : actions;

            _moveAction = FindActionSafe(_assetInstance, movePath);
            _lookAction = FindActionSafe(_assetInstance, lookPath);
            _jumpAction = FindActionSafe(_assetInstance, jumpPath);
            _sprintAction = FindActionSafe(_assetInstance, sprintPath);
            _crouchAction = FindActionSafe(_assetInstance, crouchPath);
            _maskToggleAction = FindActionSafe(_assetInstance, maskTogglePath);
            _interactAction = FindActionSafe(_assetInstance, interactPath);
            _attackAction = FindActionSafe(_assetInstance, attackPath);
            _inventoryAction = FindActionSafe(_assetInstance, inventoryPath);

            _resolved = true;
        }

        private static InputAction FindActionSafe(InputActionAsset asset, string path)
        {
            if (!asset || string.IsNullOrWhiteSpace(path)) return null;

            var action = asset.FindAction(path, throwIfNotFound: false);
            if (action != null) return action;

            int slash = path.LastIndexOf('/');
            if (slash >= 0 && slash + 1 < path.Length)
            {
                var nameOnly = path.Substring(slash + 1);
                action = asset.FindAction(nameOnly, throwIfNotFound: false);
            }

            return action;
        }

        private static Vector2 ReadVector2(InputAction action)
        {
            if (action == null) return Vector2.zero;
            return action.ReadValue<Vector2>();
        }

        private static void EnableAction(InputAction action)
        {
            if (action == null) return;
            if (!action.enabled) action.Enable();
        }

        private static void DisableAction(InputAction action)
        {
            if (action == null) return;
            if (action.enabled) action.Disable();
        }
    }
}
