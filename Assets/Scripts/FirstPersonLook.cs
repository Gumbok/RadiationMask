using UnityEngine;

namespace Game.FirstPerson
{
    /// <summary>
    /// Script-driven look:
    /// - yaw: rotates the player root
    /// - pitch: rotates CameraRoot (child)
    /// Cinemachine should follow+match CameraRoot transform.
    /// </summary>
    public sealed class FirstPersonLook : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private Transform yawRoot;     // usually Player root (this.transform)
        [SerializeField] private Transform pitchRoot;   // usually Player/CameraRoot

        [Header("Pitch Clamp")]
        [SerializeField] private float minPitch = -85f;
        [SerializeField] private float maxPitch = 85f;

        [Header("Sensitivity")]
        [Tooltip("Degrees per pixel for mouse delta.")]
        [SerializeField] private float mouseSensitivity = 0.08f;

        [Tooltip("Degrees per second at full stick deflection.")]
        [SerializeField] private float gamepadSensitivity = 210f;

        [Header("Gamepad Tuning")]
        [SerializeField, Range(0f, 0.5f)] private float gamepadDeadzone = 0.12f;

        [Tooltip("0 = no acceleration. 1 = up to 2x at full deflection (with exponent=1).")]
        [SerializeField, Range(0f, 3f)] private float gamepadAcceleration = 0.9f;

        [SerializeField, Range(0.5f, 4f)] private float gamepadAccelerationExponent = 1.6f;

        [Header("Smoothing")]
        [SerializeField] private float mouseSmoothTime = 0.02f;
        [SerializeField] private float gamepadSmoothTime = 0.06f;

        private float _pitch;
        private Vector2 _smoothedDelta;
        private Vector2 _smoothVelocity;

        private bool _lookSuppressed;

        public Transform YawRoot => yawRoot;
        public Transform PitchRoot => pitchRoot;

        public float MinPitch => minPitch;
        public float MaxPitch => maxPitch;
        public float Pitch => _pitch;

        /// <summary>
        /// Adds an external look delta (same sign convention as input):
        /// - yawDeltaDeg: + = turn right
        /// - pitchDeltaDeg: + = look up (same as mouse/gamepad delta.y)
        /// </summary>
        public void AddExternalLookDelta(float yawDeltaDeg, float pitchDeltaDeg)
        {
            if (_lookSuppressed) return;
            if (!yawRoot || !pitchRoot) return;

            if (Mathf.Abs(yawDeltaDeg) > 0.0001f)
                yawRoot.Rotate(Vector3.up, yawDeltaDeg, Space.World);

            if (Mathf.Abs(pitchDeltaDeg) > 0.0001f)
            {
                _pitch -= pitchDeltaDeg; // IMPORTANT: matches Update() logic
                _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
                pitchRoot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            }
        }


        private void Reset()
        {
            yawRoot = transform;
        }

        private void Awake()
        {
            if (!yawRoot) yawRoot = transform;
            if (pitchRoot)
            {
                // Convert Unity's 0..360 euler to a signed pitch so we don't snap on start (e.g. -10° becomes 350°).
                _pitch = Mathf.DeltaAngle(0f, pitchRoot.localEulerAngles.x);
                _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
                pitchRoot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void SetLookSuppressed(bool suppressed)
        {
            _lookSuppressed = suppressed;
            _smoothedDelta = Vector2.zero;
            _smoothVelocity = Vector2.zero;
        }

        private void Update()
        {
            if (_lookSuppressed) return;
            if (!input || !yawRoot || !pitchRoot) return;

            Vector2 raw = input.Look;
            bool isGamepad = input.LastLookDevice == LookDeviceKind.Gamepad;

            Vector2 delta = isGamepad
                ? ComputeGamepadDelta(raw)
                : ComputeMouseDelta(raw);

            float smoothTime = isGamepad ? gamepadSmoothTime : mouseSmoothTime;
            _smoothedDelta = Vector2.SmoothDamp(_smoothedDelta, delta, ref _smoothVelocity, smoothTime);

            // Yaw: rotate player root around world up
            yawRoot.Rotate(Vector3.up, _smoothedDelta.x, Space.World);

            // Pitch: rotate camera root locally
            _pitch -= _smoothedDelta.y;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
            pitchRoot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private Vector2 ComputeMouseDelta(Vector2 mouseDeltaPixels)
        {
            // Mouse delta is already per-frame; scale in degrees/pixel.
            return mouseDeltaPixels * mouseSensitivity;
        }

        private Vector2 ComputeGamepadDelta(Vector2 stick)
        {
            // Deadzone
            float mag = stick.magnitude;
            if (mag < gamepadDeadzone) return Vector2.zero;

            Vector2 dir = stick / mag;
            float t = Mathf.InverseLerp(gamepadDeadzone, 1f, Mathf.Clamp01(mag));

            // Acceleration curve: higher response at higher deflection
            float accelMul = 1f + gamepadAcceleration * Mathf.Pow(t, gamepadAccelerationExponent);

            // Convert to degrees per frame
            float degPerFrame = gamepadSensitivity * accelMul * Time.deltaTime;
            return dir * (degPerFrame * t);
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
