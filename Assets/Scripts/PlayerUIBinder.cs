using UnityEngine;
using Game.Core;

using Game.FirstPerson; // adjust if needed

namespace Game.UI
{
    [DefaultExecutionOrder(1000)]
    public sealed class PlayerUIBinder : MonoBehaviour
    {
        [SerializeField] private PlayerFacade player;
        [Header("Assign or auto-find in children")]
        [SerializeField] private InteractionPromptUI interactionPromptUI;
        [SerializeField] private ExpUIController expUIController;
        [SerializeField] private PlayerHealthUI playerHealthUI;

        private void Awake()
        {
            if (!interactionPromptUI) interactionPromptUI = GetComponentInChildren<InteractionPromptUI>(true);
            if (!expUIController) expUIController = GetComponentInChildren<ExpUIController>(true);
            if (!playerHealthUI) playerHealthUI = GetComponentInChildren<PlayerHealthUI>(true);
        }

        private void Start()
        {
            TryBind(player);
        }

        private void TryBind(PlayerFacade player)
        {
            if (!player) return;


            if (interactionPromptUI)
               interactionPromptUI.BindPlayer(player.Interactor);

            if (expUIController)
                expUIController.BindPlayer(player.PlayerXP);

            if (playerHealthUI)
                playerHealthUI.BindPlayer(player.Health);

        }
    }
}
