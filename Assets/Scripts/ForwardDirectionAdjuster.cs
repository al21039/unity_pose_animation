using UnityEngine;

public class ForwardDirectionAdjuster : MonoBehaviour
{
    void Update()
    {
        // オブジェクトの現在の正面方向（ワールド座標）
        Vector3 forwardDirection = transform.forward;

        // Y軸を中心に90度回転させた方向を計算
        Quaternion rotation = Quaternion.Euler(0, 90, 0);
        Vector3 adjustedDirection = rotation * forwardDirection;

        // 計算結果をデバッグログで表示（確認用）
        Debug.DrawRay(transform.position, forwardDirection, Color.red); // 元の方向（赤）
        Debug.DrawRay(transform.position, adjustedDirection, Color.green); // 調整後の方向（緑）
    }
}