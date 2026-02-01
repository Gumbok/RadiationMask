using System.Collections.Generic;
using UnityEngine;

public class VFXService : MonoBehaviour
{
    public static VFXService Instance { get; private set; }

    private readonly Dictionary<GameObject, Queue<PooledVfx>> _vfxPools = new();
    private readonly Dictionary<GameObject, HashSet<PooledVfx>> _borrowedVfx = new();

    Transform poolParent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple VFXService instances detected; destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (poolParent == null)
        {
            var go = new GameObject("VFX_Pool");
            go.transform.SetParent(transform, false);
            poolParent = go.transform;
        }

    }

    public void PlayOneShotVfx(in VfxRequest request)
    {
        if (request.Prefab == null) return;

        var inst = BorrowVfxInstance(request.Prefab);
        inst.Play(request.Position, request.Rotation, request.LifetimeOverride);
    }

    private PooledVfx CreateVfxObject(GameObject prefabKey)
    {
        var go = Instantiate(prefabKey, poolParent);
        go.name = $"FX_Vfx_{prefabKey.name}";
        var pooled = go.GetComponent<PooledVfx>();
        if (pooled == null) pooled = go.AddComponent<PooledVfx>();
        pooled.Init(this, prefabKey);
        return pooled;
    }

    private PooledVfx BorrowVfxInstance(GameObject prefabKey)
    {
        if (!_vfxPools.TryGetValue(prefabKey, out var pool))
        {
            pool = new Queue<PooledVfx>();
            _vfxPools[prefabKey] = pool;
            _borrowedVfx[prefabKey] = new HashSet<PooledVfx>();
        }

        var inst = pool.Count > 0 ? pool.Dequeue() : CreateVfxObject(prefabKey);
        _borrowedVfx[prefabKey].Add(inst);
        return inst;
    }

    public void ReturnVfxInstance(GameObject prefabKey, PooledVfx inst)
    {
        if (inst == null) return;
        if (_borrowedVfx.TryGetValue(prefabKey, out var set) && set.Remove(inst))
        {
            _vfxPools[prefabKey].Enqueue(inst);
        }
        else
        {
            Destroy(inst.gameObject);
        }
    }
}
public readonly struct VfxRequest
{
    public readonly GameObject Prefab;
    public readonly Vector3 Position;
    public readonly Quaternion Rotation;
    public readonly float LifetimeOverride;

    public VfxRequest(GameObject prefab, Vector3 position, Quaternion rotation, float lifetimeOverride = 0f)
    {
        Prefab = prefab;
        Position = position;
        Rotation = rotation;
        LifetimeOverride = lifetimeOverride;
    }
}