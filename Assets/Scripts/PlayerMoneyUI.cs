using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class PlayerMoneyUI : MonoBehaviour
{
    private PlayerMoney _playerMoney;
    public TextMeshProUGUI moneyText;

    public void BindPlayer(PlayerMoney playerMoney)
    {
        _playerMoney = playerMoney;
        _playerMoney.OnMoneyChanged += OnMoneyChanged;  
        Refresh();
    }

    private void Awake()
    {     
        
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        if (_playerMoney)
            _playerMoney.OnMoneyChanged -= OnMoneyChanged;
    }

    private void OnMoneyChanged()
    {
        Refresh();
    }

    private void Refresh()
    {
        moneyText.SetText(_playerMoney.money.ToString());
    }
}
