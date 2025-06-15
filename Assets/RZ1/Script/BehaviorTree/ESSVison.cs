using System;
using UnityEngine;

public class ESSVison : MonoBehaviour, IBehaviorEnvironmentSearch
{
    [Header("首平行振り設定")]
    public float Hangle = 30f; // 合計の首振り角度（例：30 → -15〜+15）
    public float Hspeed = 1f;  // 往復速度（回転の速さ）
    [Header("首縦振り")]
    public float Vangle = 30f; // 合計の首振り角度（例：30 → -15〜+15）
    public float Vrenge = 5f;

    public Action<string> OnTagHit; // tag名を渡すイベント
    public float distance = 10f;
    private Color rayColor = Color.green;

    // このオブジェクトのローカル回転オフセット（首振り用）
    private Vector3 _swingOffset = Vector3.zero;

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        RayCastVision();
        CheckSwingRange();
    }

    private void CheckSwingRange()
    {
        // 現在のワールドポジション
        Vector3 origin = transform.position;

        // 親の回転を基準にした基本方向（首振りオフセットを除く）
        Vector3 baseForward = transform.parent != null ? transform.parent.forward : Vector3.forward;
        Vector3 baseRight = transform.parent != null ? transform.parent.right : Vector3.right;
        Vector3 baseUp = transform.parent != null ? transform.parent.up : Vector3.up;

        // 親の回転を基準とした角度計算
        float baseYAngle = transform.parent != null ? transform.parent.eulerAngles.y : 0f;
        float baseXAngle = transform.parent != null ? transform.parent.eulerAngles.x : 0f;

        // 水平方向の角度範囲（固定）
        float leftAngle = baseYAngle - Hangle / 2f;
        float rightAngle = baseYAngle + Hangle / 2f;

        // 垂直方向の角度範囲（固定）
        float upAngle = baseXAngle - Vangle / 2f;
        float downAngle = baseXAngle + Vangle / 2f;

        // 左右の上下範囲をチェックするための4つの方向ベクトル
        Vector3 leftUpDir = Quaternion.Euler(upAngle, leftAngle, 0f) * Vector3.forward;
        Vector3 leftDownDir = Quaternion.Euler(downAngle, leftAngle, 0f) * Vector3.forward;
        Vector3 rightUpDir = Quaternion.Euler(upAngle, rightAngle, 0f) * Vector3.forward;
        Vector3 rightDownDir = Quaternion.Euler(downAngle, rightAngle, 0f) * Vector3.forward;

        // 中央の上下
        Vector3 centerUpDir = Quaternion.Euler(upAngle, baseYAngle, 0f) * Vector3.forward;
        Vector3 centerDownDir = Quaternion.Euler(downAngle, baseYAngle, 0f) * Vector3.forward;

        // Gizmosで範囲線を表示
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origin, Quaternion.Euler(0f, leftAngle, 0f) * Vector3.forward * distance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(origin, Quaternion.Euler(0f, rightAngle, 0f) * Vector3.forward * distance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(origin, centerUpDir * distance);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(origin, centerDownDir * distance);

        Gizmos.color = Color.white;
        Gizmos.DrawRay(origin, leftUpDir * distance);
        Gizmos.DrawRay(origin, leftDownDir * distance);
        Gizmos.DrawRay(origin, rightUpDir * distance);
        Gizmos.DrawRay(origin, rightDownDir * distance);

        // 視界範囲の輪郭を線で繋ぐ
        Gizmos.color = Color.gray;
        Vector3 leftUpEnd = origin + leftUpDir * distance;
        Vector3 leftDownEnd = origin + leftDownDir * distance;
        Vector3 rightUpEnd = origin + rightUpDir * distance;
        Vector3 rightDownEnd = origin + rightDownDir * distance;

        Gizmos.DrawLine(leftUpEnd, rightUpEnd);
        Gizmos.DrawLine(leftDownEnd, rightDownEnd);
        Gizmos.DrawLine(leftUpEnd, leftDownEnd);
        Gizmos.DrawLine(rightUpEnd, rightDownEnd);
    }

#endif

    private void Update()
    {
        RayCastVision();
        LeftRightSwing();
    }

    private void RayCastVision()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, distance))
        {
            string tag = hit.collider.tag;
            Debug.DrawLine(transform.position, hit.point, Color.red);
            Debug.Log($"Hit:{hit.collider.name} Tag:{hit.collider.tag}");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * distance, rayColor);
        }
    }

    private void LeftRightSwing()
    {
        // 0～1～0を繰り返す → -0.5～+0.5 → -15～+15
        float horizontalOffset = (Mathf.PingPong(Time.time * Hspeed, 1f) - 0.5f) * Hangle;

        // 首振りオフセットを更新
        _swingOffset.y = horizontalOffset;
        _swingOffset.x = _currentVerticalOffset;

        // 親の現在の向き + 首振りオフセットを適用
        transform.localRotation = Quaternion.Euler(_swingOffset);

        if (CheckLeftRightSwingGoal(horizontalOffset)) UpDawnSwing();
    }

    // 追加する変数（クラスのフィールドとして追加）
    private float _lastOffset = 0f;
    private bool _hasReachedNegative = false;
    private bool _hasReachedPositive = false;

    /// <summary>
    /// 一往復したらゴール判定
    /// offsetが-から+に変化したらTrueを返す
    /// </summary>
    private bool CheckLeftRightSwingGoal(float horizontalOffset)
    {
        // 負の値に到達したかチェック
        if (horizontalOffset < 0 && _lastOffset >= 0)
        {
            _hasReachedNegative = true;
        }

        // 正の値に到達したかチェック
        if (horizontalOffset > 0 && _lastOffset <= 0)
        {
            _hasReachedPositive = true;
        }

        // 両方到達していたら1往復完了
        if (_hasReachedNegative && _hasReachedPositive)
        {
            // フラグリセット
            _hasReachedNegative = false;
            _hasReachedPositive = false;
            _lastOffset = horizontalOffset;

            return true; // ゴール到達
        }

        _lastOffset = horizontalOffset;
        return false; // まだゴール未到達
    }

    bool _upDawnSwingIsUp = true;
    private float _currentVerticalOffset = 0f; // 現在の縦方向オフセット

    /// <summary>
    /// CheckLeftRightSwingGoal でゴールに到達したら、変化する
    /// </summary>
    private void UpDawnSwing()
    {
        // 上限・下限をチェック
        float maxVerticalOffset = Vangle / 2f;
        float minVerticalOffset = -Vangle / 2f;

        if (_upDawnSwingIsUp)
        {
            _currentVerticalOffset += Vrenge;
            // 上限に達したら方向転換
            if (_currentVerticalOffset >= maxVerticalOffset)
            {
                _currentVerticalOffset = maxVerticalOffset;
                _upDawnSwingIsUp = false;
            }
        }
        else
        {
            _currentVerticalOffset -= Vrenge;
            // 下限に達したら方向転換
            if (_currentVerticalOffset <= minVerticalOffset)
            {
                _currentVerticalOffset = minVerticalOffset;
                _upDawnSwingIsUp = true;
            }
        }

        // 注意: UpDawnSwingは単独では回転を適用しない
        // LeftRightSwingで一緒に適用される
    }
}