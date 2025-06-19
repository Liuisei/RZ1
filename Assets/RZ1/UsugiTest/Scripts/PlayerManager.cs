using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private bool isLocalPlayer = false;

    [SerializeField]
    private HealthSystem healthSystem;
    private List<IPlayerSystem> playerSystems = new List<IPlayerSystem>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isLocalPlayer = IsOwner;
        playerSystems = GetComponents<IPlayerSystem>().ToList();
        if (isLocalPlayer)
        {
            healthSystem.OnDeath += OnPlayerDeath;
            healthSystem.OnRevive += OnPlayerRevive;
        }
    }

    private void OnPlayerRevive()
    {
        foreach (var system in playerSystems)
        {
            system.OnPlayerRevive();
        }
    }

    private void OnPlayerDeath()
    {
        foreach (var system in playerSystems)
        {
            system.OnPlayerDeath();
        }
    }
}

public interface IPlayerSystem
{
    public void OnPlayerDeath();
    public void OnPlayerRevive();
}
