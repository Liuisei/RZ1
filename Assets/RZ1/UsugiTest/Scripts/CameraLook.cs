using Unity.Netcode;
using UnityEngine;

public class CameraLook : NetworkBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerCameraTransform;

    private float xRotation = 0f;
    private float yawRotation = 0f;

    public PlayerMover playerMover;

    void Start()
    {
        // 自分のカメラだけ有効
        if (!IsOwner)
        {
            playerCameraTransform.GetComponent<Camera>().enabled = false;
            var audioListener = playerCameraTransform.GetComponent<AudioListener>();
            if (audioListener != null) audioListener.enabled = false;
            enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yawRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // カメラ上下回転のみローカルで処理
        playerCameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 毎フレームの回転角をPlayerMoverに伝える
        if (playerMover != null)
        {
            playerMover.SetYaw(yawRotation);
        }
    }
}