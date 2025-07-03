using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        // サーバーのみ物理制御を行うため、他はKinematicにする
        if (!IsServer)
        {
            _rigidbody.isKinematic = true;
        }
    }

    private void Update()
    {
        if (!IsHost) return;

        // 入力取得（OwnerClientId = このプレイヤーのクライアント）
        if (NetworkInputHandler.TryGetInput(OwnerClientId, out var input))
        {
            Vector3 move = new Vector3(input.Move.x, 0, input.Move.y) * _moveSpeed;

            // 現在のY速度を保持して、横方向だけ移動
            Vector3 velocity = new Vector3(move.x, _rigidbody.linearVelocity.y, move.z);
            _rigidbody.linearVelocity = velocity;
            Debug.Log($"Player {OwnerClientId} moving with velocity: {velocity}");
        }
    }

    private void FixedUpdate()
    {

    }
}
