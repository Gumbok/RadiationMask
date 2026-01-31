using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PooledAudioSource : MonoBehaviour
{
    private AudioService _owner;
    private AudioSource _source;
    private Coroutine _returnCoro;

    public void Init(AudioService owner)
    {
        _owner = owner;
        _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.spatialBlend = 1f;
        _source.rolloffMode = AudioRolloffMode.Linear;
        gameObject.SetActive(false);
    }

    public void Play(AudioClip clip, Vector3 position, float volume, float pitch, float spatialBlend, float minDistance, float maxDistance)
    {
        if (clip == null) return;

        transform.position = position;
        gameObject.SetActive(true);

        _source.clip = clip;
        _source.volume = volume;
        _source.pitch = pitch;
        _source.spatialBlend = spatialBlend;
        _source.minDistance = minDistance;
        _source.maxDistance = Mathf.Max(minDistance + 0.01f, maxDistance);

        _source.Play();

        if (_returnCoro != null) StopCoroutine(_returnCoro);
        float duration = clip.length / Mathf.Max(0.01f, Mathf.Abs(pitch));
        _returnCoro = StartCoroutine(ReturnAfter(duration + 0.05f));
    }

    private IEnumerator ReturnAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        if (_returnCoro != null)
        {
            StopCoroutine(_returnCoro);
            _returnCoro = null;
        }
        _source.Stop();
        _source.clip = null;
        gameObject.SetActive(false);
        _owner.ReturnAudioInstance(this);
    }
}

