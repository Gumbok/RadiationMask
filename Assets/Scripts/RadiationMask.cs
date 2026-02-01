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

    public int maxCharge = 100;

    public int MaxCharge => maxCharge + durability.currentUpgradeLevel * durabilityPerUpgrade;
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
        if (currentCharge <= 0)
        {
            return damageAmount;
        }

        currentCharge -= 1;
        
        //damage reduction upgrade
        damageAmount -= damageReduction.currentUpgradeLevel;
        if (damageAmount <= minDamage) damageAmount = minDamage; //damage cap
        
        radiationMaskImageFill.fillAmount = Mathf.Clamp(currentCharge / MaxCharge, 0f, 1f);
        
        return damageAmount; 
    }

}
