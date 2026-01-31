using Game.FirstPerson;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RadiationMask : MonoBehaviour
{
    public PlayerInputReader playerInputReader;
    public bool IsEquipped = false;

    public int maxCharge = 100;
    public int currentCharge = 100;

    public Image radiationMaskImageFill;

    private void Update()
    {
        if (playerInputReader != null && playerInputReader.MaskTogglePressedThisFrame)
        {
            ToggleEqipped();
        }
    }

    void ToggleEqipped()
    {
        IsEquipped = !IsEquipped;
    }

    public int AbsorbDamage(int damageAmount)
    {
        if(currentCharge <= 0)
        {
            return damageAmount;
        }

        if(damageAmount <= currentCharge)
        {
            currentCharge -= damageAmount;
            radiationMaskImageFill.fillAmount = (float)currentCharge / maxCharge;
            return 0;
        }
        else
        {
            int remainingDamage = damageAmount - currentCharge;
            currentCharge = 0;
            radiationMaskImageFill.fillAmount = 0f;
            return remainingDamage;
        }
    }

}
