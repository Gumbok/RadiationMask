using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Radiation : MonoBehaviour
{
    public VisualEffect radiationEffect;
    public AudioSource radiationSound;
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
        if (other.tag != "Player") return;
        other.TryGetComponent<PlayerHealth>(out playerHealth);
        if (playerHealth != null)
        {     
            if (radiationSound != null && !radiationSound.isPlaying)
            radiationSound.Play();
            coroutine = DamageOverTime();
            StartCoroutine(coroutine);
        }



    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") return;
        if (playerHealth != null)
        {      
            if (radiationSound != null && radiationSound.isPlaying)
            radiationSound.Stop();
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
                        damageInfo = new DamageInfo(1, this.gameObject);
                        break;
                    case RadiationType.Medium:
                        damageInfo = new DamageInfo(2, this.gameObject);
                        break;
                    case RadiationType.High:
                        damageInfo = new DamageInfo(4, this.gameObject);
                        break;
                    default:
                        damageInfo = new DamageInfo(0, this.gameObject);
                        break;
                }

                playerHealth.TakeDamage(damageInfo);
            }
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
