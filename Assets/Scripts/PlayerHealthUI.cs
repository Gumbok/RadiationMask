using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class PlayerHealthUI : MonoBehaviour
{
    private PlayerHealth _playerHealth;
    [SerializeField] private Image heartPrefab;

    [SerializeField] private Sprite full;
    [SerializeField] private Sprite threeQuarter;
    [SerializeField] private Sprite half;
    [SerializeField] private Sprite quarter;
    [SerializeField] private Sprite empty;

    private readonly List<Image> heartImages = new List<Image>();

    public void BindPlayer(PlayerHealth playerHealth)
    {
        _playerHealth = playerHealth;
        _playerHealth.OnHealthChanged += OnHealthChanged;  
        RebuildIfNeeded();
        Refresh();
    }

    private void Awake()
    {     
        if (!heartPrefab)
        {
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {


    }

    private void OnDisable()
    {
        if (_playerHealth)
            _playerHealth.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(HealthChangedInfo _)
    {
        RebuildIfNeeded();
        Refresh();
    }

    private void RebuildIfNeeded()
    {
        int target = Mathf.Max(0, _playerHealth.Hearts);

        while (heartImages.Count < target)
        {
            Image img = Instantiate(heartPrefab, transform);
            heartImages.Add(img);
        }

        while (heartImages.Count > target)
        {
            int last = heartImages.Count - 1;
            if (heartImages[last]) Destroy(heartImages[last].gameObject);
            heartImages.RemoveAt(last);
        }
    }

    private void Refresh()
    {
        int current = Mathf.Clamp(_playerHealth.CurrentFragments, 0, _playerHealth.MaxFragments);

        for (int i = 0; i < heartImages.Count; i++)
        {
            int heartStart = i * PlayerHealth.FragmentsPerHeart;
            int inHeart = Mathf.Clamp(current - heartStart, 0, PlayerHealth.FragmentsPerHeart);

            var img = heartImages[i];
            if (!img) continue;

            img.sprite = SpriteForFragments(inHeart);
        }
    }

    private Sprite SpriteForFragments(int fragmentsInHeart)
    {
        return fragmentsInHeart switch
        {
            4 => full,
            3 => threeQuarter,
            2 => half,
            1 => quarter,
            _ => empty,
        };
    }
}
