using Unity.Netcode;
using UnityEngine;

public class PlayerMover : NetworkBehaviour
{
    [SerializeField] float m_moveSpeed = 5f;
    [SerializeField] float m_jumpForce = 5f;
    private Rigidbody m_rigidBody;

    private Vector2 m_moveInput = Vector2.zero;
    [SerializeField] private float m_yawRotation = 0f;

    private float clientYawRotation = 0f;

    private bool isGrounded = true;

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
            bool jumpInput = Input.GetKey(KeyCode.Space);

            SetMoveInputServerRpc(inputX, inputY, clientYawRotation, jumpInput);
        }

        if (IsServer)
        {
            ServerUpdate();
        }
    }

    [ServerRpc]
    private void SetMoveInputServerRpc(float x, float y, float yaw, bool jump)
    {
        m_moveInput = new Vector2(x, y);
        m_yawRotation = yaw;

        if (jump && isGrounded)
        {
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.VelocityChange);
            isGrounded = false;
        }
    }

    private void ServerUpdate()
    {
        Quaternion rotation = Quaternion.Euler(0f, m_yawRotation, 0f);
        transform.rotation = rotation;

        Vector3 forward = rotation * Vector3.forward;
        Vector3 right = rotation * Vector3.right;

        Vector3 move = (right * m_moveInput.x + forward * m_moveInput.y).normalized * m_moveSpeed;
        Vector3 velocity = new Vector3(move.x, m_rigidBody.linearVelocity.y, move.z);
        m_rigidBody.linearVelocity = velocity;
    }

    public void SetYaw(float yaw)
    {
        clientYawRotation = yaw;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 簡易的に「何かに当たったら着地扱い」
        isGrounded = true;
    }
}
