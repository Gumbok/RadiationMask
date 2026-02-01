using System;
using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    [SerializeField]private int _money;

    public int money
    {
        get => _money;
        set
        {
            _money = value;
            OnMoneyChanged?.Invoke();
        }
    }
    
    public event Action OnMoneyChanged;

    private void Awake()
    {
        _money = Math.Max(0, _money);
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }
    
    public Boolean HasEnoughMoney(int amount)
    {
        return money >= amount;
    }
    
    public void Purchase(int amount)
    {
        if (HasEnoughMoney(amount))
        {
            money -= amount;
        }
    }
}
