using Unity.Netcode;
using UnityEngine;

public class PlayerAttackController : NetworkBehaviour
{
    [SerializeField] private AttackBase _currentAttack;
    [SerializeField] private Transform _handTransform;

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
        if (_currentAttack != null)
        {
            Destroy(_currentAttack);
        }

        _currentAttack = newAttack;

        // SerializeField の値はコピーされないので明示的に設定する
        _currentAttack.transform.SetParent(_handTransform);
        _currentAttack.transform.localPosition = Vector3.zero;
    }
}
