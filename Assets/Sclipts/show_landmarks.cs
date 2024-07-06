using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class show_landmarks : MonoBehaviour
{
    public string csvFilePath = "Assets/CSV/stretch.csv"; // CSVファイルのパス
    public GameObject landmarkPrefab; // ランドマークを表示するためのプレハブ
    public float scale = 2.0f; // 座標のスケーリングファクター
    public Vector3 offset = new Vector3(0, 1, 0); // 座標のオフセット
    public Material lineMaterial; // ラインのマテリアル

    private List<List<Vector3>> framesLandmarks = new List<List<Vector3>>();
    private List<GameObject> landmarks = new List<GameObject>();
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();

    // BlazePoseの隣接するランドマークの接続関係
    private int[,] connections = new int[,]
    {
        {0, 1}, {1, 2}, {2, 3}, {3, 7}, {0, 4}, {4, 5}, {5, 6}, {6, 8},
        {9, 10}, {11, 12}, {11, 13}, {13, 15}, {15, 17}, {15, 19}, {15, 21},
        {17, 19}, {12, 14}, {14, 16}, {16, 18}, {16, 20}, {16, 22}, {18, 20},
        {11, 23}, {12, 24}, {23, 25}, {24, 26}, {25, 27}, {26, 28}, {27, 29},
        {28, 30}, {29, 31}, {30, 32}, {23, 24}, {27, 31}, {28, 32}
    };

    void Start()
    {
        LoadCSV();
        InitializeLandmarks();
        InitializeLineRenderers();
    }

    void LoadCSV()
    {
        using (var reader = new StreamReader(csvFilePath))
        {
            bool isHeader = true;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (isHeader)
                {
                    isHeader = false;
                    continue;
                }

                var values = line.Split(',');
                List<Vector3> frameLandmarks = new List<Vector3>();
                for (int i = 1; i < values.Length; i += 3)
                {
                    float x = float.Parse(values[i]) * scale;
                    float y = float.Parse(values[i + 1]) * scale;
                    float z = float.Parse(values[i + 2]) * scale;
                    frameLandmarks.Add(new Vector3(x, -y, z) + offset);
                }
                framesLandmarks.Add(frameLandmarks);
            }
        }
    }

    void InitializeLandmarks()
    {
        for (int i = 0; i < 33; i++)
        {
            GameObject landmark = Instantiate(landmarkPrefab, Vector3.zero, Quaternion.identity);
            landmarks.Add(landmark);
        }
    }

    void InitializeLineRenderers()
    {
        for (int i = 0; i < connections.GetLength(0); i++)
        {
            GameObject lineObject = new GameObject("Line" + i);
            lineObject.transform.parent = transform;
            LineRenderer lr = lineObject.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.positionCount = 2;
            lineRenderers.Add(lr);
        }
    }

    void Update()
    {
        int frameIndex = (int)(Time.time * 30) % framesLandmarks.Count; // 30FPSで再生する場合のフレームインデックス計算
        List<Vector3> currentFrameLandmarks = framesLandmarks[frameIndex];

        for (int i = 0; i < landmarks.Count; i++)
        {
            landmarks[i].transform.position = currentFrameLandmarks[i];
        }

        UpdateLineRenderers(currentFrameLandmarks);
    }

    void UpdateLineRenderers(List<Vector3> currentFrameLandmarks)
    {
        for (int i = 0; i < connections.GetLength(0); i++)
        {
            int startIdx = connections[i, 0];
            int endIdx = connections[i, 1];
            lineRenderers[i].SetPosition(0, currentFrameLandmarks[startIdx]);
            lineRenderers[i].SetPosition(1, currentFrameLandmarks[endIdx]);
        }
    }
}
