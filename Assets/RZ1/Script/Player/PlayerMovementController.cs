using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _runSpeed = 6f;

    [SerializeField] private Transform _cameraTransform; // シーン内のメインカメラをアタッチする
    [SerializeField] private Transform _playerTransform;

    private Rigidbody _rigidbody;
    private CharacterAnimationController _characterAnimationController;
    private MovementAnimationTrack _movementTrack;

    public override void OnNetworkSpawn()
    {
        _characterAnimationController = GetComponent<CharacterAnimationController>();
        _movementTrack = _characterAnimationController.GetTrack<MovementAnimationTrack>();
        if (IsServer)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        var speed = _rigidbody.linearVelocity.magnitude;
        _movementTrack?.UpdateMoveSpeed(speed);
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
        }
    }
}