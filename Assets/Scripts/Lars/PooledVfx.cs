using System.Collections;
using UnityEngine;
using UnityEngine.VFX;


public class PooledVfx : MonoBehaviour
{
    private VFXService _owner;
    private GameObject _prefabKey;
    private Coroutine _lifetimeCoro;


    private VisualEffect[] _vfx;


    public void Init(VFXService owner, GameObject prefabKey)
    {
        _owner = owner;
        _prefabKey = prefabKey;
        _vfx = GetComponentsInChildren<VisualEffect>(true);
        gameObject.SetActive(false);
    }

    public void Play(Vector3 position, Quaternion rotation, float lifetimeOverrideSeconds, Transform parent = null)
    {
        transform.SetPositionAndRotation(position, rotation);
        gameObject.SetActive(true);
        if (parent != null)
            transform.SetParent(parent, worldPositionStays: true);

        float maxLifetime = 0f;


        if (_vfx != null && _vfx.Length > 0)
        {
            foreach (var v in _vfx)
            {
                v.Reinit();
                v.Play();
                maxLifetime = Mathf.Max(maxLifetime, 4f);
            }
        }


        if (lifetimeOverrideSeconds > 0f)
            maxLifetime = lifetimeOverrideSeconds;
        if (maxLifetime <= 0f)
            maxLifetime = 5f;

        if (_lifetimeCoro != null) StopCoroutine(_lifetimeCoro);
        _lifetimeCoro = StartCoroutine(ReturnAfter(maxLifetime));
    }

    private IEnumerator ReturnAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        if (_lifetimeCoro != null)
        {
            StopCoroutine(_lifetimeCoro);
            _lifetimeCoro = null;
        }

        if (_vfx != null)
        {
            foreach (var v in _vfx) v.Stop();
        }

        gameObject.SetActive(false);
        _owner.ReturnVfxInstance(_prefabKey, this);
    }
}

