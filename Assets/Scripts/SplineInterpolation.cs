using System.Collections.Generic;
using UnityEngine;

public class SplineInterpolation : MonoBehaviour
{
    [SerializeField] private List<Vector3> controlPoints; // 制御点のリスト
    [SerializeField] private int resolution = 10;        // 各セグメントの分割数
    [SerializeField] private LineRenderer lineRenderer;  // 曲線表示用

    private void Start()
    {
        List<Vector3> interpolatedPoints = GenerateCatmullRomSpline(controlPoints, resolution);

        // LineRendererで表示
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = interpolatedPoints.Count;
            lineRenderer.SetPositions(interpolatedPoints.ToArray());
        }
    }

    private List<Vector3> GenerateCatmullRomSpline(List<Vector3> points, int resolution)
    {
        List<Vector3> result = new List<Vector3>();

        if (points.Count < 2)
        {
            Debug.LogWarning("制御点が2つ以上必要です");
            return result;
        }

        // 各セグメントについて補間
        for (int i = 0; i < points.Count - 1; i++)
        {
            // 前後の点を取得（境界では同じ点を繰り返す）
            Vector3 p0 = i == 0 ? points[i] : points[i - 1];
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 p3 = i + 2 < points.Count ? points[i + 2] : points[i + 1];

            // セグメント内の補間点を計算
            for (int t = 0; t < resolution; t++)
            {
                float u = t / (float)resolution; // 0から1の範囲
                Vector3 point = CatmullRom(p0, p1, p2, p3, u);
                result.Add(point);
            }
        }

        // 最後の制御点を追加
        result.Add(points[points.Count - 1]);

        return result;
    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        // Catmull-Romスプラインの公式
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }
}
