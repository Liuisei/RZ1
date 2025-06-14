using System;
using UnityEngine;

public class EQS : MonoBehaviour
{
    [SerializeField] Transform _aiTransform;
    public float angle = 30f; // 合計の首振り角度（例：30 → -15〜+15）
    public float speed = 1f;  // 往復速度（回転の速さ）
    private float? _startAngle = null;
    public Action<string> OnTagHit; // tag名を渡すイベント
    public float distance = 10f;
    private Color rayColor = Color.green;

    private void Start()
    {
        if (_startAngle == null) _startAngle = transform.eulerAngles.y;
    }
    private void OnDrawGizmosSelected()
    {
        if (_aiTransform == null)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_aiTransform.position, 0.5f);
        RayCastVision();
    }
    private void Update()
    {
        RayCastVision();
        Swing();
    }

    private void RayCastVision()
    {
        if (Physics.Raycast(_aiTransform.position, transform.forward, out RaycastHit hit, distance))
        {
            string tag = hit.collider.tag;
            Debug.DrawLine(_aiTransform.position, hit.point, Color.red);
            Debug.Log($"Hit:{hit.collider.name} Tag:{hit.collider.tag}");
        }
        else
        {
            Debug.DrawRay(_aiTransform.position, transform.forward * distance, rayColor);
        }
    }
    private void Swing()
    {
        // 0～1～0を繰り返す → -0.5～+0.5 → -15～+15
        float offset = (Mathf.PingPong(Time.time * speed, 1f) - 0.5f) * angle;
        // 回転反映（Y軸）
        transform.rotation = Quaternion.Euler(0f, _startAngle.Value + offset, 0f);
    }
}
