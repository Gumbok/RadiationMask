using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.FirstPerson
{
    public sealed class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] private FirstPersonInteractor interactor;

        [Header("UI")]
        [SerializeField] private GameObject root;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private Image progressFill;

        public void BindPlayer(FirstPersonInteractor firstPersonInteractor)
        {
            interactor = firstPersonInteractor; 
            HandleVisible(false);

            if (interactor != null)
            {
                interactor.OnCanInteractChanged += HandleVisible;
                interactor.OnPrompt += HandlePrompt;
            }
        }

        private void OnDisable()
        {
            if (interactor != null)
            {
                interactor.OnCanInteractChanged -= HandleVisible;
                interactor.OnPrompt -= HandlePrompt;
            }
        }

        private void HandleVisible(bool visible)
        {
            if (root) root.SetActive(visible);
            if (!visible)
            {
                if (promptText) promptText.text = string.Empty;
                if (progressFill) progressFill.fillAmount = 0f;
            }
        }

        private void HandlePrompt(string text, float progress01)
        {
            if (promptText) promptText.text = text ?? string.Empty;
            if (progressFill) progressFill.fillAmount = progress01;
        }
    }
}
