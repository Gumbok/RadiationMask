using System;
using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    [Header("XP Settings")]
    [SerializeField] private AnimationCurve xpPerLevelCurve;

    [Header("Current XP Data")]
    public int currentXP = 0;
    public int xpToNextLevel = 0;
    public int playerLevel = 1;

    // Old event (kept for compatibility)
    public event Action<int, int> OnXPChanged;

    // New detailed event for UI animation
    public readonly struct XPChange
    {
        public readonly int OldLevel;
        public readonly int NewLevel;

        public readonly int OldXP;
        public readonly int NewXP;

        public readonly int OldNeeded;
        public readonly int NewNeeded;

        public readonly int DeltaXP;

        public XPChange(int oldLevel, int newLevel, int oldXP, int newXP, int oldNeeded, int newNeeded, int deltaXP)
        {
            OldLevel = oldLevel;
            NewLevel = newLevel;
            OldXP = oldXP;
            NewXP = newXP;
            OldNeeded = oldNeeded;
            NewNeeded = newNeeded;
            DeltaXP = deltaXP;
        }
    }

    public event Action<XPChange> OnXPChangedDetailed;

    private void Awake()
    {
        playerLevel = Mathf.Max(1, playerLevel);
        currentXP = Mathf.Max(0, currentXP);
        xpToNextLevel = GetXpToNextLevelForLevel(playerLevel);

        ResolveLevelUps();
    }

    public void AddXP(int amount)
    {
        if (amount <= 0) return;

        int oldLevel = playerLevel;
        int oldXP = currentXP;
        int oldNeeded = xpToNextLevel;

        currentXP += amount;

        ResolveLevelUps();

        var change = new XPChange(
            oldLevel, playerLevel,
            oldXP, currentXP,
            oldNeeded, xpToNextLevel,
            amount
        );

        // Fire AFTER state is consistent (important for UI)
        OnXPChangedDetailed?.Invoke(change);
        OnXPChanged?.Invoke(currentXP, xpToNextLevel);
    }

    [ContextMenu("Add Some XP")]
    public void AddSomeXp()
    {
        int rnd = UnityEngine.Random.Range(1, 30);
        AddXP(rnd);
    }

    private void ResolveLevelUps()
    {
        xpToNextLevel = Mathf.Max(1, xpToNextLevel);

        int safety = 0;
        while (currentXP >= xpToNextLevel && safety++ < 1000)
        {
            currentXP -= xpToNextLevel;
            playerLevel++;
            xpToNextLevel = GetXpToNextLevelForLevel(playerLevel);
        }

        currentXP = Mathf.Max(0, currentXP);
        playerLevel = Mathf.Max(1, playerLevel);
        xpToNextLevel = Mathf.Max(1, xpToNextLevel);
    }

    public int GetXpToNextLevelForLevel(int level)
    {
        level = Mathf.Max(1, level);

        if (xpPerLevelCurve == null)
            return 100;

        return Mathf.Max(1, Mathf.CeilToInt(xpPerLevelCurve.Evaluate(level)));
    }
}
