using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Radiation : MonoBehaviour
{
    public VisualEffect radiationEffect;
    public RadiationType radiationType;
    public float damageInterval = 1f;
    [SerializeField, Range(0, 100)] private int strength = 30;
    public int Strength
    {
        get => strength;
        set
        {
            strength = Mathf.Clamp(value, 0, 100);
            SyncRadiationType();
            SyncVfx();
        }
    }

    private PlayerHealth playerHealth;
    private IEnumerator coroutine;

    public enum RadiationType
    {
        Low,
        Medium,
        High
    }

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<PlayerHealth>(out playerHealth);
        if (playerHealth != null)
        {
            coroutine = DamageOverTime();
            StartCoroutine(coroutine);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerHealth != null)
        {
            StopCoroutine(coroutine);
            playerHealth = null;
        }
    }


    private void SyncVfx()
    {
        SyncRadiationType();
        if (radiationEffect != null)
        {
            radiationEffect.SetInt("Strength", strength);
        }
    }

    private void SyncRadiationType()
    {
        if (strength < 40)
        {
            radiationType = RadiationType.Low;
        }
        else if (strength < 80)
        {
            radiationType = RadiationType.Medium;
        }
        else
        {
            radiationType = RadiationType.High;
        }

    }

    private void OnEnable()
    {
        SyncRadiationType();
        SyncVfx();
    }

    private void OnValidate()
    {
        SyncVfx();
    }
    IEnumerator DamageOverTime()
    {
        while (true)
        {
            if (playerHealth != null)
            {

                DamageInfo damageInfo;

                switch (radiationType)
                {
                    case RadiationType.Low:
                        damageInfo = new DamageInfo().WithAmount(1);
                        break;
                    case RadiationType.Medium:
                        damageInfo = new DamageInfo().WithAmount(2);
                        break;
                    case RadiationType.High:
                        damageInfo = new DamageInfo().WithAmount(4);
                        break;
                    default:
                        damageInfo = new DamageInfo().WithAmount(0);
                        break;
                }

                playerHealth.TakeDamage(damageInfo);
            }
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
