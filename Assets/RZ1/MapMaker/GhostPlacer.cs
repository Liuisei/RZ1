using UnityEngine;

public class GhostPlacer : MonoBehaviour
{
    [Header("基本設定")]
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Material ghostMaterial;
    [SerializeField] private LayerMask hitLayers = Physics.DefaultRaycastLayers;

    [Header("距離＆スナップ")]
    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private float scrollSpeed = 2f;
    [SerializeField] private float snapSize = 1f;

    private GameObject ghostObject;

    void Start()
    {
        // 仮オブジェクト生成
        ghostObject = Instantiate(prefab);
        ApplyGhostMaterial(ghostObject);
    }

    void Update()
    {
        HandleScroll();
        Vector3 targetPos = GetRaycastPoint();
        targetPos = SnapToGrid(targetPos);
        ghostObject.transform.position = targetPos;

        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(prefab, targetPos, Quaternion.identity);
        }

        Debug.DrawRay(cam.transform.position, cam.transform.forward * rayDistance, Color.red);
    }

    void HandleScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        rayDistance += scroll * scrollSpeed;
        rayDistance = Mathf.Clamp(rayDistance, 1f, 100f);
    }

    Vector3 GetRaycastPoint()
    {
        Vector3 origin = cam.transform.position;
        Vector3 dir = cam.transform.forward;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, rayDistance, hitLayers))
        {
            return hit.point;
        }
        else
        {
            return origin + dir * rayDistance;
        }
    }

    Vector3 SnapToGrid(Vector3 pos)
    {
        float x = Mathf.Round(pos.x / snapSize) * snapSize;
        float y = Mathf.Round(pos.y / snapSize) * snapSize;
        float z = Mathf.Round(pos.z / snapSize) * snapSize;
        return new Vector3(x, y, z);
    }

    void ApplyGhostMaterial(GameObject obj)
    {
        foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            var mats = renderer.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = ghostMaterial;
            }
            renderer.materials = mats;
        }
    }
}
