using Unity.Netcode;
using UnityEngine;

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Transform _cameraTransform; // シーン内のメインカメラをアタッチする
    [SerializeField] private Transform _playerTransform;

    private Rigidbody _rigidbody;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {
        if (!IsServer || _rigidbody == null) return;

        if (NetworkInputHandler.TryGetInput(OwnerClientId, out var input))
        {
            if (input.Move.magnitude > 0.1f)
            {
                // 入力をカメラの向きに変換
                Vector3 inputDir = new Vector3(input.Move.x, 0f, input.Move.y);

                // カメラの正面・右方向（地面に投影してY=0）
                Vector3 camForward = _cameraTransform.forward;
                Vector3 camRight = _cameraTransform.right;
                camForward.y = 0f;
                camRight.y = 0f;
                camForward.Normalize();
                camRight.Normalize();

                Vector3 moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;

                Vector3 velocity = moveDir * _moveSpeed;
                velocity.y = _rigidbody.linearVelocity.y; // Y速度は維持（ジャンプや落下など）

                _rigidbody.linearVelocity = velocity;

                // プレイヤーの向きを移動方向に合わせる
                if (moveDir != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                    _playerTransform.rotation = Quaternion.Slerp(_playerTransform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
                }
            }
        }
    }
}