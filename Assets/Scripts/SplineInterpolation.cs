using System.Collections.Generic;
using UnityEngine;

public class SplineInterpolation : MonoBehaviour
{
    [SerializeField] private List<Vector3> controlPoints; // ����_�̃��X�g
    [SerializeField] private int resolution = 10;        // �e�Z�O�����g�̕�����
    [SerializeField] private LineRenderer lineRenderer;  // �Ȑ��\���p

    private void Start()
    {
        List<Vector3> interpolatedPoints = GenerateCatmullRomSpline(controlPoints, resolution);

        // LineRenderer�ŕ\��
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
            Debug.LogWarning("����_��2�ȏ�K�v�ł�");
            return result;
        }

        // �e�Z�O�����g�ɂ��ĕ��
        for (int i = 0; i < points.Count - 1; i++)
        {
            // �O��̓_���擾�i���E�ł͓����_���J��Ԃ��j
            Vector3 p0 = i == 0 ? points[i] : points[i - 1];
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 p3 = i + 2 < points.Count ? points[i + 2] : points[i + 1];

            // �Z�O�����g���̕�ԓ_���v�Z
            for (int t = 0; t < resolution; t++)
            {
                float u = t / (float)resolution; // 0����1�͈̔�
                Vector3 point = CatmullRom(p0, p1, p2, p3, u);
                result.Add(point);
            }
        }

        // �Ō�̐���_��ǉ�
        result.Add(points[points.Count - 1]);

        return result;
    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        // Catmull-Rom�X�v���C���̌���
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
