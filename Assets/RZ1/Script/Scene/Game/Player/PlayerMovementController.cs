using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovementController : NetworkBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _runSpeed = 6f;
    [SerializeField] private float _jumpHeight = 3f;

    [SerializeField] private Transform _cameraTransform; // シーン内のメインカメラをアタッチする
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Animator _animator;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _groundDistance = 0.2f;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private Vector3 _groundCheckOffset = Vector3.up * 1;

    private Rigidbody _rigidbody;
    private bool _isJumpButtonPressed;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        //水平方向のRigidBody速度を計算
        if (!_rigidbody) return;
        if (_animator)
        {
            float horizontalSpeed = Mathf.Abs(_rigidbody.linearVelocity.x) + Mathf.Abs(_rigidbody.linearVelocity.z);
            _animator.SetFloat(Speed, horizontalSpeed, 0.1f, Time.deltaTime);
        }

        //地面に接地しているかどうかレイを飛ばして確認
        _isGrounded = Physics.Raycast(_playerTransform.position + _groundCheckOffset, Vector3.down, _groundDistance, _groundMask);
    }

    private void FixedUpdate()
    {
        if (!IsServer || !_rigidbody) return;

        if (NetworkInputHandler.TryGetInput(OwnerClientId, out var input))
        {
            if (input.Move.magnitude > 0.1f)
            {
                Vector3 inputDir = new Vector3(input.Move.x, 0f, input.Move.y);

                Vector3 camForward = _cameraTransform.forward;
                Vector3 camRight = _cameraTransform.right;
                camForward.y = 0f;
                camRight.y = 0f;
                camForward.Normalize();
                camRight.Normalize();

                Vector3 moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;

                float targetSpeed = input.IsButtonPressed(NetworkInputHandler.InputButton.Dash) ? _runSpeed : _walkSpeed;

                Vector3 velocity = moveDir * targetSpeed;
                velocity.y = _rigidbody.linearVelocity.y;

                _rigidbody.linearVelocity = velocity;

                if (moveDir != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                    _playerTransform.rotation = Quaternion.Slerp(_playerTransform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
                }
            }

            // ジャンプ処理
            if (input.IsButtonPressed(NetworkInputHandler.InputButton.Jump))
            {
                Debug.Log("Jump button pressed");
            }
            if (input.IsButtonPressed(NetworkInputHandler.InputButton.Jump) && _isGrounded)
            {
                // ジャンプ処理
                _rigidbody.AddForce(Vector3.up * _jumpHeight, ForceMode.Impulse);
            }
        }
    }
}