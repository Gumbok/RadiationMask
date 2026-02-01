using Game.FirstPerson;
using Jonas.UI.ShopUI;
using UnityEngine;
using UnityEngine.UI;

public class RadiationMask : MonoBehaviour
{
    [SerializeField] private Upgrade damageReduction;
    [SerializeField] private Upgrade durability;
    
    public PlayerInputReader playerInputReader;
    public bool IsEquipped = false;

    [Tooltip("Durability per Upgrade")]
    [SerializeField] private int durabilityPerUpgrade = 25;
    [Tooltip("Minimal damage")]
    [SerializeField] private int minDamage = 0;

    public int baseDamageReduction = 1;
    public int baseMaxCharge = 100;

    public int MaxCharge => baseMaxCharge + durability.currentUpgradeLevel * durabilityPerUpgrade;
    public int currentCharge = 100;

    public Image radiationMaskImageFill;
    public Image radiationMaskImageToggle;

    private void Update()
    {
        if (playerInputReader != null && playerInputReader.MaskTogglePressedThisFrame)
        {
            ToggleEqipped();
        }


    }

    public void RefillMask()
    {
        currentCharge = MaxCharge;
        radiationMaskImageFill.fillAmount = 1f;
    }

    void ToggleEqipped()
    {
        IsEquipped = !IsEquipped;
        radiationMaskImageToggle.enabled = !IsEquipped;
    }

    public int AbsorbDamage(int damageAmount, Radiation.RadiationType radiationType)
    {
        if (currentCharge <= 0)
        {
            return damageAmount;
        }

        if(radiationType == Radiation.RadiationType.Low)
            currentCharge -= 1;
        else if(radiationType == Radiation.RadiationType.Medium)
            currentCharge -= 2;
        else if(radiationType == Radiation.RadiationType.High)
            currentCharge -= 3;

        //damage reduction upgrade
        damageAmount -= baseDamageReduction + damageReduction.currentUpgradeLevel;
        if (damageAmount <= minDamage) damageAmount = minDamage; //damage cap
        
        radiationMaskImageFill.fillAmount = Mathf.Clamp((float)currentCharge / (float)MaxCharge, 0f, 1f);
        
        return damageAmount; 
    }

}
