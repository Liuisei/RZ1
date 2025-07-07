using Unity.Netcode;
using UnityEngine;

public class PlayerAttackController : NetworkBehaviour
{
    [SerializeField] private AttackBase _currentAttack;
    [SerializeField] private Transform _handTransform;
    [SerializeField] private NetworkObject _ownerNetworkObject;

    private void FixedUpdate()
    {
        if (!IsServer) return;
        if (NetworkInputHandler.TryGetInput(OwnerClientId, out var input))
        {
            if (input.IsButtonPressed(NetworkInputHandler.InputButton.Fire))
            {
                _currentAttack?.StartAttack();
            }
        }
    }

    /// <summary>
    /// 武器の切り替えを行う
    /// </summary>
    public void EquipWeapon(AttackBase newAttack)
    {
        _currentAttack = newAttack;

        // SerializeField の値はコピーされないので明示的に設定する
        if (_currentAttack.GetComponent<NetworkObject>().TrySetParent(_ownerNetworkObject))
        {
            transform.SetParent(_handTransform, false);
            Debug.Log("Successfully set parent for the attack object.");
            // ローカル位置・回転を合わせる（武器ごとに補正してもOK）
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.Log("Failed to set parent for the attack object.");
        }
    }
}
