using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVReader : MonoBehaviour
{
    public string filePath="Assets/CSV/stretch"; // CSVファイルのパス
    private List<List<Vector3>> landmarks;

    void Start()
    {
        landmarks = new List<List<Vector3>>();
        ReadCSV();
    }

    void ReadCSV()
    {
        string[] lines = File.ReadAllLines(filePath);
        for (int i = 1; i < lines.Length; i++) // ヘッダーをスキップ
        {
            string[] values = lines[i].Split(',');
            List<Vector3> frameLandmarks = new List<Vector3>();
            for (int j = 1; j < values.Length; j += 3)
            {
                float x = float.Parse(values[j]);
                float y = float.Parse(values[j + 1]);
                float z = float.Parse(values[j + 2]);
                frameLandmarks.Add(new Vector3(x, y, z));
                //if(j == 1)
                //    Debug.Log($"{x},{y},{z}");
            }
            landmarks.Add(frameLandmarks);
        }
    }

    public List<Vector3> GetFrameLandmarks(int frame)
    {
        if (frame < 0 || frame >= landmarks.Count) return null;
        return landmarks[frame];
    }

    public int GetTotalFrames()
    {
        return landmarks.Count;
    }
}
