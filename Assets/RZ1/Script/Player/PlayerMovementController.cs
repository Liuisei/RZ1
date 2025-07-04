using Unity.Netcode;
using UnityEngine;

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;

    private Rigidbody _rigidbody;
    private float _lastProcessedTime = -1f;

    private void Awake()
    {
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
    }

    private void Update() // Updateの代わりに
    {
        if (!IsServer) return; // IsHostからIsServerに変更

        if (NetworkInputHandler.TryGetInput(OwnerClientId, out var input))
        {
            Debug.Log(OwnerClientId + " : " + input);
            // 入力がある時のみ速度を更新
            if (input.Move.magnitude > 0.1f) // 入力の閾値
            {
                Vector3 move = new Vector3(input.Move.x, 0, input.Move.y) * _moveSpeed;
                // Vector3 velocity = new Vector3(move.x, _rigidbody.linearVelocity.y, move.z);
                //_rigidbody.AddForce(velocity);
                transform.position += move * 0.01f; // ここは実際の移動処理に置き換える必要があります
            }
            // 入力が0なら自然に減速（Rigidbodyのdragに任せる）
        }
    }
}