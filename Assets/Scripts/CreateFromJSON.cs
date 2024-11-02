using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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


public class CreateFromJSON : BaseCalc
{
    [SerializeField] GameObject _humanoidPrefab;
    [SerializeField] string fileName;

    // Start is called before the first frame update
    void Start()
    {
        string filePath = Path.Combine(Application.dataPath, fileName);
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            LandmarkData landmarkData = JsonUtility.FromJson<LandmarkData>(jsonData);
            Vector3[] landmarksArray = new Vector3[landmarkData.num_landmarks];

            for (int i = 0; i < landmarkData.landmarks.Count; i++)
            {
                landmarksArray[i] = new Vector3(-landmarkData.landmarks[i].x, -landmarkData.landmarks[i].y + 1, -landmarkData.landmarks[i].z);
            }
      

        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetJsonLandmark()
    {
        
    }


}
