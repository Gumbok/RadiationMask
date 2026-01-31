using UnityEngine;

namespace Game.FirstPerson
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class FirstPersonMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private Transform yawRoot;         // usually Player root (this.transform)
        [SerializeField] private Transform cameraRoot;      // Player/CameraRoot for eye height adjustment

        [Header("Movement Speeds (m/s)")]
        [SerializeField] private float walkSpeed = 4.5f;
        [SerializeField] private float sprintSpeed = 7.2f;
        [SerializeField] private float crouchSpeed = 2.6f;

        [SerializeField] private bool allowSprintWithAbility = false;

        [Header("Acceleration")]
        [SerializeField] private float speedChangeRate = 18f;

        [Header("Jump / Gravity")]
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -18f;

        [Header("Grounding")]
        [SerializeField] private LayerMask groundMask = ~0;
        [SerializeField] private float groundCheckOffset = 0.08f;
        [SerializeField] private float groundCheckRadiusScale = 0.95f;

        [Header("Crouch")]
        [SerializeField] private float standingHeight = 2.0f;
        [SerializeField] private float crouchHeight = 1.25f;
        [SerializeField] private float crouchTransitionSpeed = 12f;

        [SerializeField] private float standingEyeHeight = 1.70f;
        [SerializeField] private float crouchEyeHeight = 1.10f;

        [Header("Push Rigidbodies")]
        [SerializeField] private bool canPushRigidbodies = true;
        [SerializeField] private float pushForce = 2.5f;

        private CharacterController _controller;

        private float _currentSpeed;
        private float _verticalVelocity;
        private bool _isGrounded;
        private bool _isCrouched;

        private void Reset()
        {
            yawRoot = transform;
        }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (!yawRoot) yawRoot = transform;

            // Initialize to standing settings
            ApplyControllerHeightImmediate(standingHeight);
            ApplyEyeHeightImmediate(standingEyeHeight);
        }

        private void Update()
        {
            if (!input || !yawRoot) return;

            UpdateGrounded();
            UpdateCrouchState();
            UpdateMovement();
            UpdateGravityAndJump();
        }

        private void UpdateGrounded()
        {
            // Sphere check slightly below controller bottom
            float radius = _controller.radius * groundCheckRadiusScale;

            // CharacterController center is in local space; compute world-space bottom
            Vector3 centerWorld = transform.TransformPoint(_controller.center);
            Vector3 bottom = centerWorld + Vector3.down * (_controller.height * 0.5f - radius + groundCheckOffset);

            _isGrounded = Physics.CheckSphere(bottom, radius, groundMask, QueryTriggerInteraction.Ignore);

            if (_isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f; // keeps controller "stuck" to ground
        }

        private void UpdateCrouchState()
        {
            bool wantsCrouch = input.CrouchHeld;

            if (wantsCrouch)
            {
                _isCrouched = true;
            }
            else
            {
                // Only stand if there is headroom
                if (_isCrouched && CanStandUp())
                    _isCrouched = false;
            }

            float targetHeight = _isCrouched ? crouchHeight : standingHeight;
            float targetEye = _isCrouched ? crouchEyeHeight : standingEyeHeight;

            ApplyControllerHeightSmooth(targetHeight);
            ApplyEyeHeightSmooth(targetEye);
        }

        private bool CanStandUp()
        {
            // Check capsule for overlap at standing height using controller radius.
            float radius = _controller.radius;
            float targetHeight = standingHeight;

            Vector3 pos = transform.position;
            Vector3 bottom = pos + Vector3.up * radius;
            Vector3 top = pos + Vector3.up * (targetHeight - radius);

            // Ignore triggers to avoid UI volumes etc.
            return !Physics.CheckCapsule(bottom, top, radius, groundMask, QueryTriggerInteraction.Ignore);
        }

        private void UpdateMovement()
        {
            Vector2 move = input.Move;
            Vector3 moveDir = (yawRoot.right * move.x + yawRoot.forward * move.y);
            if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

            float targetSpeed = walkSpeed;

            bool hasMoveInput = move.sqrMagnitude > 0.0001f;
            if (!hasMoveInput) targetSpeed = 0f;

            if (_isCrouched)
                targetSpeed = Mathf.Min(targetSpeed, crouchSpeed);
            else if (input.SprintHeld && hasMoveInput)
            {
                if (!allowSprintWithAbility && (input.AttackHeld || input.InteractHeld))
                {
                    // No sprinting while using ability
                    targetSpeed = walkSpeed;
                }
                else
                {
                    targetSpeed = sprintSpeed;
                }
            }

            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, speedChangeRate * Time.deltaTime);

            Vector3 horizontalVelocity = moveDir * _currentSpeed;

            // Apply movement (horizontal + vertical)
            Vector3 velocity = horizontalVelocity + Vector3.up * _verticalVelocity;
            _controller.Move(velocity * Time.deltaTime);
        }

        private void UpdateGravityAndJump()
        {
            if (_isGrounded && input.JumpPressedThisFrame && !_isCrouched)
            {
                // v = sqrt(h * -2g)
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            _verticalVelocity += gravity * Time.deltaTime;
        }

        private void ApplyControllerHeightSmooth(float targetHeight)
        {
            float h = Mathf.Lerp(_controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
            _controller.height = h;

            // Keep feet planted by adjusting center
            Vector3 c = _controller.center;
            c.y = h * 0.5f;
            _controller.center = c;
        }

        private void ApplyControllerHeightImmediate(float targetHeight)
        {
            _controller.height = targetHeight;
            Vector3 c = _controller.center;
            c.y = targetHeight * 0.5f;
            _controller.center = c;
        }

        private void ApplyEyeHeightSmooth(float targetEye)
        {
            if (!cameraRoot) return;
            Vector3 lp = cameraRoot.localPosition;
            lp.y = Mathf.Lerp(lp.y, targetEye, crouchTransitionSpeed * Time.deltaTime);
            cameraRoot.localPosition = lp;
        }

        private void ApplyEyeHeightImmediate(float targetEye)
        {
            if (!cameraRoot) return;
            Vector3 lp = cameraRoot.localPosition;
            lp.y = targetEye;
            cameraRoot.localPosition = lp;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!canPushRigidbodies) return;

            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb == null || rb.isKinematic) return;

            // Don’t push things we’re standing on heavily
            if (hit.moveDirection.y < -0.3f) return;

            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
            rb.AddForce(pushDir * pushForce, ForceMode.Impulse);
        }
    }
}
