using System;
using UnityEngine;

public class StoreDropOff : MonoBehaviour
{
    
    public PlayerMoney _playerMoney;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Barrel") && other.TryGetComponent(out Barrel barrel) &&
            !barrel.triggered)
        {
            barrel.triggered = true;
            _playerMoney.AddMoney(barrel.value);
            Destroy(other.gameObject);
        }
    }
}
