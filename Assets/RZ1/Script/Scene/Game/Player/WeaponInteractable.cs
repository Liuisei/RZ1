using UnityEngine;
using Unity.Netcode;

public class WeaponInteractable : InteractableBase
{
    [SerializeField] private string _weaponName = "DefaultWeapon";
    [SerializeField] private AttackBase _weaponPrefab; // 武器のプレハブ

    public override void Interact(ulong interactorClientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(interactorClientId, out var client))
        {
            var playerObject = client.PlayerObject; // = NetworkObject
            var playerGameObject = playerObject.gameObject;

            Debug.Log($"Player {interactorClientId} interacted with weapon!");

            // 例：PlayerWeaponController を取得して武器装備させる
            var weaponController = playerGameObject.GetComponent<PlayerAttackController>();
            if (weaponController != null)
            {
                weaponController.EquipWeapon(_weaponPrefab); // 引数のthisはこのインタラクト対象
            }
        }
        else
        {
            Debug.LogWarning($"Interactor with ClientId {interactorClientId} not found.");
        }
    }
}
