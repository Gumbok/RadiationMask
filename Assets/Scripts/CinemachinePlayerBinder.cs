using Game.Core;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.PlayerCamera
{
    [DefaultExecutionOrder(1000)]
    public sealed class CinemachinePlayerBinder : MonoBehaviour
    {
        [SerializeField] private PlayerFacade player;
        [SerializeField] private CinemachineCamera cam;
        [SerializeField] private bool setFollow = true;
        [SerializeField] private bool setLookAt = true;

        private void Awake()
        {
            if (!cam) cam = GetComponent<CinemachineCamera>();
        }

        private void Start()
        {
            TryBind(player);
        }

        private void TryBind(PlayerFacade player)
        {
            if (!cam) return;
            if (!player) return;

            var target = player.CameraFollowTarget ? player.CameraFollowTarget : player.transform;

            if (setFollow) cam.Follow = target;
            if (setLookAt) cam.LookAt = target;
        }
    }
}
