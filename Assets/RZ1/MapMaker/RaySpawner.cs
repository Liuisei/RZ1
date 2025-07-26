using UnityEngine;

public class RaySpawner : MonoBehaviour
{
    [SerializeField] private Camera cam;              // 使用するカメラ
    [SerializeField] private GameObject prefab;       // 生成するプレハブ
    [SerializeField] private float rayDistance = 5f;  // 初期Ray距離
    [SerializeField] private float scrollSpeed = 2f;  // ホイール感度
    [SerializeField] private LayerMask hitLayers;     // 当たりたいレイヤー

    void Update()
    {
        // ホイールで距離調整
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        rayDistance += scroll * scrollSpeed;
        rayDistance = Mathf.Clamp(rayDistance, 1f, 100f);

        // Rayの可視化
        Vector3 origin = cam.transform.position;
        Vector3 direction = cam.transform.forward;
        Debug.DrawRay(origin, direction * rayDistance, Color.red);

        // 左クリックで生成
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(origin, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, hitLayers))
            {
                Instantiate(prefab, hit.point, Quaternion.identity);
            }
            else
            {
                // ヒットしなかったらRayの先に生成
                Vector3 spawnPoint = origin + direction * rayDistance;
                Instantiate(prefab, spawnPoint, Quaternion.identity);
            }
        }
    }
}
