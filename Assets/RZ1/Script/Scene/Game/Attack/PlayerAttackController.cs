using Unity.Netcode;
using UnityEngine;

public class PlayerAttackController : NetworkBehaviour
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    [SerializeField] private AttackBase _currentAttack;
    [SerializeField] private Transform _handTransform;
    [SerializeField] private NetworkObject _ownerNetworkObject;
    [SerializeField] private Animator _animator;
    private bool _hasAttackedThisPress = false;

    private void FixedUpdate()
    {
        if (!IsServer) return;

        if (NetworkInputHandler.TryGetInput(OwnerClientId, out var input))
        {
            if (input.IsButtonPressed(NetworkInputHandler.InputButton.Fire))
            {
                if (!_hasAttackedThisPress)
                {
                    _hasAttackedThisPress = true;

                    _currentAttack?.StartAttack();
                    _animator?.SetTrigger(Attack);
                }
            }
            else
            {
                _hasAttackedThisPress = false; // ボタンを離したらリセット
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
