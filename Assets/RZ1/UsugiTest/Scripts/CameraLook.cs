using Unity.Netcode;
using UnityEngine;

public class CameraLook : NetworkBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerCameraTransform;

    private float xRotation = 0f;
    private float yawRotation = 0f;

    public PlayerMover playerMover;
    public Camera playerCamera { get; private set; }

    void Start()
    {
        playerCamera = playerCameraTransform.GetComponent<Camera>();
        // 自分のカメラだけ有効
        if (!IsOwner)
        {
            playerCamera.enabled = false;
            var audioListener = playerCameraTransform.GetComponent<AudioListener>();
            if (audioListener != null) audioListener.enabled = false;
            enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() // FixedUpdateからUpdateに変更
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity; // Time.deltaTimeを削除
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yawRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // yawRotationの値を-180〜180の範囲に正規化
        yawRotation = Mathf.Repeat(yawRotation + 180f, 360f) - 180f;

        playerCameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (playerMover != null)
        {
            playerMover.SetYaw(yawRotation);
        }
    }
}