using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpUIController : MonoBehaviour
{
    private PlayerXP _playerXP;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image xpBarImage;   // front (animated)
    [SerializeField] private Image xpBarImageB;  // back (instant target)

    [Header("Animation")]
    [SerializeField, Min(0.01f)] private float secondsPerFullBar = 2.0f;
    [SerializeField, Min(0.0f)] private float minDuration = 0.05f;
    [SerializeField, Min(0.0f)] private float maxDuration = 1.0f;
    [SerializeField, Min(0.0f)] private float levelUpPause = 0.35f;

    [Header("Text Formats")]
    [SerializeField] private string xpTextFormat = "{0}";
    [SerializeField] private string levelTextFormat = "Lv {0}";

    private Coroutine _routine;
    public void BindPlayer(PlayerXP playerXP)
    {
        _playerXP = playerXP;      
        _playerXP.OnXPChangedDetailed += OnXPChangedDetailed;
        SnapToPlayer();
        // This method can be expanded if player-specific data is needed in the future.
    }

    private void OnDisable()
    {
        if (_playerXP != null)
            _playerXP.OnXPChangedDetailed -= OnXPChangedDetailed;

        StopAnim();
    }

    private void StopAnim()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
    }

    private void SnapToPlayer()
    {
        int level = Mathf.Max(1, _playerXP.playerLevel);
        int needed = Mathf.Max(1, _playerXP.xpToNextLevel);
        int xp = Mathf.Clamp(_playerXP.currentXP, 0, needed);

        float fill = (needed > 0) ? Mathf.Clamp01((float)xp / needed) : 0f;

        SetFill(fill);
        SetXpText(xp);
        SetLevelText(level);
    }

    private void OnXPChangedDetailed(PlayerXP.XPChange change)
    {
        StopAnim();
        _routine = StartCoroutine(AnimateChange(change));
    }

    private IEnumerator AnimateChange(PlayerXP.XPChange change)
    {
        // We animate based on old->new state. Bar + xpText are driven from the SAME fill value each frame.
        int oldLevel = Mathf.Max(1, change.OldLevel);
        int newLevel = Mathf.Max(1, change.NewLevel);

        int levelsGained = Mathf.Max(0, newLevel - oldLevel);

        // Stage A: finish current bar to 100% if we leveled up, else just go to target remainder.
        if (levelsGained == 0)
        {
            int needed = Mathf.Max(1, change.NewNeeded);
            int fromXP = Mathf.Clamp(change.OldXP, 0, needed);
            int toXP = Mathf.Clamp(change.NewXP, 0, needed);

            SetLevelText(oldLevel);

            float fromFill = (float)fromXP / needed;
            float toFill = (float)toXP / needed;

            yield return AnimateFillSynced(oldLevel, needed, fromFill, toFill, toXP);

            SetFill(toFill);
            SetXpText(toXP);
            SetLevelText(newLevel);

            _routine = null;
            yield break;
        }

        // If we gained levels:
        // 1) Fill to 100% for old level (from oldXP -> oldNeeded)
        // 2) At EXACT moment bar hits 100%, level increments
        // 3) Reset bar/xp to 0, repeat for any full intermediate levels
        // 4) Finally fill remainder for the last level (0 -> newXP/newNeeded)

        int currentLevel = oldLevel;

        // Stage 1: old level remainder to full
        {
            int needed = Mathf.Max(1, change.OldNeeded);
            int fromXP = Mathf.Clamp(change.OldXP, 0, needed);
            int toXP = needed;

            SetLevelText(currentLevel);

            float fromFill = (float)fromXP / needed;
            float toFill = 1f;

            yield return AnimateFillSynced(currentLevel, needed, fromFill, toFill, toXP);

            // EXACT moment at 100%:
            SetFill(1f);
            SetXpText(needed);
            currentLevel++;
            SetLevelText(currentLevel);

            if (levelUpPause > 0f)
                yield return new WaitForSeconds(levelUpPause);

            SetFill(0f);
            SetXpText(0);
        }

        // Stage 2..N: full levels in between (0 -> 100%)
        // We need the threshold per level; PlayerXP provides GetXpToNextLevelForLevel(level).
        for (int i = 1; i < levelsGained; i++)
        {
            int needed = Mathf.Max(1, _playerXP.GetXpToNextLevelForLevel(currentLevel));
            SetLevelText(currentLevel);

            yield return AnimateFillSynced(currentLevel, needed, 0f, 1f, needed);

            // EXACT moment at 100%:
            SetFill(1f);
            SetXpText(needed);
            currentLevel++;
            SetLevelText(currentLevel);

            if (levelUpPause > 0f)
                yield return new WaitForSeconds(levelUpPause);

            SetFill(0f);
            SetXpText(0);
        }

        // Final stage: last level remainder (0 -> newXP/newNeeded)
        {
            int needed = Mathf.Max(1, change.NewNeeded);
            int toXP = Mathf.Clamp(change.NewXP, 0, needed);

            SetLevelText(newLevel);

            float toFill = (float)toXP / needed;
            yield return AnimateFillSynced(newLevel, needed, 0f, toFill, toXP);

            SetFill(toFill);
            SetXpText(toXP);
            SetLevelText(newLevel);
        }

        _routine = null;
    }

    private IEnumerator AnimateFillSynced(int level, int needed, float fromFill, float toFill, int finalXP)
    {
        fromFill = Mathf.Clamp01(fromFill);
        toFill = Mathf.Clamp01(toFill);

        // Back bar snaps to target; front bar animates.
        if (xpBarImageB) xpBarImageB.fillAmount = toFill;

        float delta = Mathf.Abs(toFill - fromFill);
        float duration = Mathf.Clamp(delta * secondsPerFullBar, minDuration, maxDuration);

        // Ensure initial sync
        SetLevelText(level);
        SetFill(fromFill);
        SetXpText(FillToXP(fromFill, needed, 0, needed));

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = (duration > 0f) ? Mathf.Clamp01(t / duration) : 1f;

            float fill = Mathf.Lerp(fromFill, toFill, a);
            SetFill(fill);

            // XP text is derived from current fill => perfectly synced
            int xp = FillToXP(fill, needed, 0, needed);
            SetXpText(xp);

            yield return null;
        }

        // Final snap for exactness
        SetFill(toFill);
        SetXpText(finalXP);
    }

    private static int FillToXP(float fill01, int needed, int minXP, int maxXP)
    {
        fill01 = Mathf.Clamp01(fill01);
        needed = Mathf.Max(1, needed);

        // Use floor for monotonic increase; clamp to provided bounds.
        int xp = Mathf.FloorToInt(fill01 * needed + 0.0001f);
        return Mathf.Clamp(xp, minXP, maxXP);
    }

    private void SetFill(float fill01)
    {
        if (xpBarImage) xpBarImage.fillAmount = Mathf.Clamp01(fill01);
        // xpBarImageB is set per-stage (target), not continuously
    }

    private void SetXpText(int xp)
    {
        if (!xpText) return;
        xpText.text = string.Format(xpTextFormat, xp);
    }

    private void SetLevelText(int level)
    {
        if (!levelText) return;
        levelText.text = string.Format(levelTextFormat, level);
    }
}
