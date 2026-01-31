using System;
using UnityEngine;
using UnityEngine.Serialization;

public enum HealthChangeReason
{
    Damage = 0,
    Heal = 1,
    Regeneration = 2,
    Set = 3,
}

public readonly struct HealthChangedInfo
{
    public readonly int Previous;
    public readonly int Current;
    public readonly int Max;
    public readonly HealthChangeReason Reason;
    public readonly DamageInfo DamageInfo;

    public int Delta => Current - Previous;

    public HealthChangedInfo(int previous, int current, int max, HealthChangeReason reason, DamageInfo damageInfo = default)
    {
        Previous = previous;
        Current = current;
        Max = max;
        Reason = reason;
        DamageInfo = damageInfo;
    }
}

public sealed class PlayerHealth : MonoBehaviour, IDamageable
{
    public RadiationMask radiationMask;

    public const int FragmentsPerHeart = 4;

    [Header("Hearts")]
    [SerializeField, Min(0)] private int hearts = 3;

    [Header("Start State")]
    [Tooltip("If enabled, current health starts at max (hearts * 4).")]
    [SerializeField] private bool startAtFullHealth = true;

    [FormerlySerializedAs("currentHp")]
    [SerializeField] private int currentFragments = 0;

    public int Hearts => hearts;
    public int MaxFragments => Mathf.Max(0, hearts) * FragmentsPerHeart;
    public int CurrentFragments => currentFragments;

    public bool IsDead => currentFragments <= 0;
    public float CurrentHearts => currentFragments / (float)FragmentsPerHeart;

    public event Action<DamageInfo> OnDamaged;
    public event Action<DamageInfo> OnDied;

    public event Action<HealthChangedInfo> OnHealthChanged;

    private void Awake()
    {
        hearts = Mathf.Max(0, hearts);

        if (startAtFullHealth)
            currentFragments = MaxFragments;
        else
            currentFragments = Mathf.Clamp(currentFragments, 0, MaxFragments);

        OnHealthChanged?.Invoke(new HealthChangedInfo(currentFragments, currentFragments, MaxFragments, HealthChangeReason.Set));
    }

    private void OnValidate()
    {
        hearts = Mathf.Max(0, hearts);
        currentFragments = Mathf.Clamp(currentFragments, 0, MaxFragments);
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (IsDead) return;

        int damageAmount = damageInfo.Amount;

        if (damageAmount <= 0) return;

        if(radiationMask != null && radiationMask.IsEquipped)
        {
           damageAmount= radiationMask.AbsorbDamage(damageAmount);
        }

        int prev = currentFragments;
        int next = Mathf.Clamp(prev - damageAmount, 0, MaxFragments);
        if (next == prev) return;

        currentFragments = next;

        OnDamaged?.Invoke(damageInfo);
        OnHealthChanged?.Invoke(new HealthChangedInfo(prev, next, MaxFragments, HealthChangeReason.Damage, damageInfo));

        if (prev > 0 && next <= 0)
            OnDied?.Invoke(damageInfo);
    }

    public int Heal(int amount)
    {
        if (amount <= 0) return 0;

        int prev = currentFragments;
        int next = Mathf.Clamp(prev + amount, 0, MaxFragments);
        if (next == prev) return 0;

        currentFragments = next;
        OnHealthChanged?.Invoke(new HealthChangedInfo(prev, next, MaxFragments, HealthChangeReason.Heal));
        return next - prev;
    }

    public int Regenerate(int amount)
    {
        if (amount <= 0) return 0;
        if (IsDead) return 0;

        int prev = currentFragments;

        int remainder = prev % FragmentsPerHeart;
        if (remainder == 0) return 0;

        int boundary = prev + (FragmentsPerHeart - remainder);
        boundary = Mathf.Min(boundary, MaxFragments);

        int next = Mathf.Min(prev + amount, boundary);
        if (next == prev) return 0;

        currentFragments = next;
        OnHealthChanged?.Invoke(new HealthChangedInfo(prev, next, MaxFragments, HealthChangeReason.Regeneration));
        return next - prev;
    }

    public void SetCurrentFragments(int fragments)
    {
        int prev = currentFragments;
        int next = Mathf.Clamp(fragments, 0, MaxFragments);
        if (next == prev) return;

        currentFragments = next;
        OnHealthChanged?.Invoke(new HealthChangedInfo(prev, next, MaxFragments, HealthChangeReason.Set));
    }

    [ContextMenu("KillSelf")]
    private void KillSelf()
    {
        if (currentFragments <= 0) return;
        TakeDamage(new DamageInfo(currentFragments, gameObject));
    }
}
