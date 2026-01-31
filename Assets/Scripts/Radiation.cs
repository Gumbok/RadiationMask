using System.Collections;
using UnityEngine;

public class Radiation : MonoBehaviour
{
    PlayerHealth playerHealth;
    IEnumerator coroutine;

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

    IEnumerator DamageOverTime()
    {
        while (true)
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(new DamageInfo(1));
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
