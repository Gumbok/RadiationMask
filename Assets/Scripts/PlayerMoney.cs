using System;
using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    public int money;
    
    public event Action OnMoneyChanged;

    private void Awake()
    {
        money = Math.Max(0, money);
    }

    public void AddMoney(int amount)
    {
        money += amount;
        OnMoneyChanged.Invoke();
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
