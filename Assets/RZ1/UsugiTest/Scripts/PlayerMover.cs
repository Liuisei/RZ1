using Unity.Netcode;
using UnityEngine;

public class PlayerMover : NetworkBehaviour
{
    [SerializeField] float m_moveSpeed = 5f;
    private Rigidbody m_rigidBody;

    private Vector2 m_moveInput = Vector2.zero;
    [SerializeField] private float m_yawRotation = 0f;

    // クライアント内で毎フレーム更新される
    private float clientYawRotation = 0f;

    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();

        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void FixedUpdate()
    {
        if (IsOwner)
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputY = Input.GetAxisRaw("Vertical");

            SetMoveInputServerRpc(inputX, inputY, clientYawRotation);
        }

        if (IsServer)
        {
            ServerUpdate();
        }
    }

    [ServerRpc]
    private void SetMoveInputServerRpc(float x, float y, float yaw)
    {
        m_moveInput = new Vector2(x, y);
        m_yawRotation = yaw;
    }

    private void ServerUpdate()
    {
        // サーバー側は送られてきたYawを基準に移動方向を決定
        Quaternion rotation = Quaternion.Euler(0f, m_yawRotation, 0f);
        transform.rotation = rotation;

        Vector3 forward = rotation * Vector3.forward;
        Vector3 right = rotation * Vector3.right;

        Vector3 move = (right * m_moveInput.x + forward * m_moveInput.y).normalized * m_moveSpeed;
        Vector3 velocity = new Vector3(move.x, m_rigidBody.linearVelocity.y, move.z);
        m_rigidBody.linearVelocity = velocity;
    }

    // クライアントからカメラLook経由で受け取る
    public void SetYaw(float yaw)
    {
        clientYawRotation = yaw;
    }
}