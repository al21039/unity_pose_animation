using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LandmarkProcesser : MonoBehaviour
{
    [System.Serializable]
    public class Landmark
    {
        public float x;
        public float y;
        public float z;
    }

    [System.Serializable]
    public class LandmarkData
    {
        public int num_landmarks;
        public List<Landmark> landmarks;
    }

    public Vector3[] GetLandmarksFromJson(string jsonFilePath)
    {
        if (File.Exists(jsonFilePath))
        {
            //ファイルパスからJSONの内容を取得
            string jsonText = File.ReadAllText(jsonFilePath);


            LandmarkData data = JsonUtility.FromJson<LandmarkData>(jsonText);



            Vector3[] ImageLandmarks = new Vector3[data.num_landmarks];

            for (int i = 0; i < data.landmarks.Count; i++)
            {
                ImageLandmarks[i] = new Vector3(
                    -data.landmarks[i].x,
                    -data.landmarks[i].y + 1,
                    -data.landmarks[i].z
                );
            }
            return ImageLandmarks;
        }

        else
        {
            Debug.LogError("File Not Found");
        }

        return null;
    }
}
