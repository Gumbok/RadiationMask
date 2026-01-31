using UnityEngine;
using Game.Core;

namespace Game.GameFlow
{
    public sealed class PlayerDeathRelay : MonoBehaviour
    {
        public PlayerHealth playerHealth;


        private void OnEnable()
        {
            playerHealth.OnDied += HandlePlayerDied;
        }

        private void OnDisable()
        {
            playerHealth.OnDied -= HandlePlayerDied;
        }

        private void HandlePlayerDied(DamageInfo damageInfo)
        {
            //if (GameRoot.Instance && GameRoot.Instance.Flow)
            //    GameRoot.Instance.Flow.OnPlayerDied();
        }

        [ContextMenu("Debug Die")]
        public void Die()
        {

            //if (GameRoot.Instance && GameRoot.Instance.Flow)
            //    GameRoot.Instance.Flow.OnPlayerDied();
        }
    }
}
