using System.Collections.Generic;
using UnityEngine;

public class AudioService : MonoBehaviour
{
    public static AudioService Instance { get; private set; }

    [Header("Pools")]
    [Tooltip("Initial pooled AudioSource count.")]
    [Min(0)] public int initialAudioPool = 8;

    [Tooltip("Parent under which pooled instances will live (optional).")]
    public Transform poolParent;
     
    private readonly Queue<PooledAudioSource> _audioPool = new();
    private readonly HashSet<PooledAudioSource> _borrowedAudio = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple AudioService instances detected; destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (poolParent == null)
        {
            var go = new GameObject("Audio_Pool");
            go.transform.SetParent(transform, false);
            poolParent = go.transform;
        }

        for (int i = 0; i < initialAudioPool; i++)
            _audioPool.Enqueue(CreateAudioObject());
    }

    public void PlayOneShotAudio(in AudioRequest request)
    {
        PlayOneShotAudio(request, spatialBlend: 1f, minDistance: 1.5f, maxDistance: 45f);
    }

    public void PlayOneShotAudio(in AudioRequest request, float spatialBlend, float minDistance, float maxDistance)
    {
        if (request.Clip == null) return;

        var src = BorrowAudioInstance();
        src.Play(request.Clip, request.Position, Mathf.Clamp01(request.Volume), request.Pitch, spatialBlend, minDistance, maxDistance);
    }

    private PooledAudioSource CreateAudioObject()
    {
        var go = new GameObject("Audio");
        if (poolParent != null) go.transform.SetParent(poolParent, false);
        var pooled = go.AddComponent<PooledAudioSource>();
        pooled.Init(this);
        return pooled;
    }

    private PooledAudioSource BorrowAudioInstance()
    {
        var src = _audioPool.Count > 0 ? _audioPool.Dequeue() : CreateAudioObject();
        _borrowedAudio.Add(src);
        return src;
    }

    public void ReturnAudioInstance(PooledAudioSource src)
    {
        if (src == null) return;
        if (_borrowedAudio.Remove(src))
            _audioPool.Enqueue(src);
    }
}
public readonly struct AudioRequest
{
    public readonly AudioClip Clip;
    public readonly Vector3 Position;
    public readonly float Volume;
    public readonly float Pitch;

    public AudioRequest(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        Clip = clip;
        Position = position;
        Volume = volume;
        Pitch = pitch;
    }
}