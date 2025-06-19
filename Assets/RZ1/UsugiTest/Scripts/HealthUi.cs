using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HealthUi : MonoBehaviour
{
    [SerializeField] private Text healthText;
    private HealthSystem healthSystem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var playerManager = FindObjectsByType<PlayerManager>(sortMode: FindObjectsSortMode.None);
        healthSystem = playerManager.First(x => x.IsOwner)
            .GetComponentInChildren<HealthSystem>();
    }

    private void Update()
    {
        if (healthSystem != null)
        {
            // 体力の値を取得
            int currentHealth = healthSystem.GetCurrentHealth();

            // UIに表示
            healthText.text = $"HP: {currentHealth}";
        }
        else
        {
            Debug.LogWarning("HealthSystem is not found.");
        }
    }
}
