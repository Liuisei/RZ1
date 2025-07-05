using UnityEngine;
using Unity.Cinemachine;
using Unity.Netcode;

public class ThirdPersonCameraController : NetworkBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform _cameraTarget; // プレイヤーの肩などに置く
    [SerializeField] private float mouseSensitivity = 1.5f;
    [SerializeField] private float pitchClampMin = -40f;
    [SerializeField] private float pitchClampMax = 70f;

    private CinemachineCamera _virtualCamera;
    private CinemachineThirdPersonFollow _thirdPersonFollow;
    private Vector2 _lookInput;
    private float _currentPitch;
    private RZ1Input _inputActions;

    public override void OnNetworkSpawn()
    {
        _inputActions = new RZ1Input();
        _inputActions.Enable();
        _virtualCamera = FindAnyObjectByType<CinemachineCamera>();
        if (_virtualCamera)
        {
            _virtualCamera.Follow = _cameraTarget;
            _thirdPersonFollow = _virtualCamera.GetComponent<CinemachineThirdPersonFollow>();
        }

        if (!_cameraTarget)
        {
            Debug.LogError("Camera target (rotation pivot) is not assigned.");
        }
    }

    public override void OnNetworkDespawn()
    {
        _inputActions.Disable();
    }

    private void Update()
    {
        if (!IsOwner || !_thirdPersonFollow) return;
        _lookInput = _inputActions.Default.Look.ReadValue<Vector2>();
        ApplyCameraRotation();
    }

    private void ApplyCameraRotation()
    {
        if (!_cameraTarget) return;
        var look = _lookInput * mouseSensitivity;

        // 垂直回転（X軸）をピッチとして制限つきで回転
        _currentPitch = Mathf.Clamp(_currentPitch - look.y, pitchClampMin, pitchClampMax);

        // 回転の反映：X=ピッチ, Y=ヨー
        Quaternion targetRotation = Quaternion.Euler(_currentPitch, _cameraTarget.eulerAngles.y + look.x, 0f);
        _cameraTarget.rotation = targetRotation;
    }
}
