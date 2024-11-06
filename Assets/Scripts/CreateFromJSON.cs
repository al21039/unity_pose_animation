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
    public List<Landmark> landmarksArray;
}

public class CreateFromJSON : BaseCalculation
{
    [SerializeField] GameObject[] modelPartObject;

    private Vector3[] landmarksArray;
    private float[] _modelPartDistance;
    private float[] _mediapipePartDistance;
    private float[] _distanceDiff;
    private Vector3[] mediaPipePositions;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetJsonLandmark(string fileName)
    {
        DetectionLandmarkPosition(fileName);

        mediaPipePositions = new Vector3[19] { 
            new Vector3(0, 1, 0),   //mediaPipeÇÃçòÇÃà íu
            (landmarksArray[11] + landmarksArray[12] + landmarksArray[23] + landmarksArray[24]) / 4,   //ëÃÇÃíÜêS
            (landmarksArray[11] + landmarksArray[12]) / 2,
            landmarksArray[11],
            landmarksArray[13],
            landmarksArray[15],
            landmarksArray[19],
            landmarksArray[12],
            landmarksArray[14],
            landmarksArray[16],
            landmarksArray[20],
            landmarksArray[23],
            landmarksArray[25],
            landmarksArray[29],
            landmarksArray[31],
            landmarksArray[24],
            landmarksArray[26],
            landmarksArray[30],
            landmarksArray[32]
    };


        _modelPartDistance = ReturnDistance(modelPartObject);
        _mediapipePartDistance = ReturnDistance(mediaPipePositions);
        _distanceDiff = ReturnDiff(_modelPartDistance, _mediapipePartDistance);
        SetNewPosition();
    }

    private void DetectionLandmarkPosition(string fileName)
    {
        string filePath = Path.Combine(Application.dataPath, fileName);
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            LandmarkData landmarkData = JsonUtility.FromJson<LandmarkData>(jsonData);
            landmarksArray = new Vector3[landmarkData.num_landmarks];

            for (int i = 0; i < landmarkData.landmarksArray.Count; i++)
            {
                landmarksArray[i] = new Vector3(-landmarkData.landmarksArray[i].x, -landmarkData.landmarksArray[i].y + 1, -landmarkData.landmarksArray[i].z);
            }
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }

    private void SetNewPosition()
    {
        modelPartObject[1].transform.position = (mediaPipePositions[1] - mediaPipePositions[0]) * _distanceDiff[0] + mediaPipePositions[0];
        modelPartObject[2].transform.position = (mediaPipePositions[2] - mediaPipePositions[1]) * _distanceDiff[1] + mediaPipePositions[1];

        modelPartObject[4].transform.position = (mediaPipePositions[4] - mediaPipePositions[3]) * _distanceDiff[2] + mediaPipePositions[3];
        modelPartObject[5].transform.position = (mediaPipePositions[5] - mediaPipePositions[4]) * _distanceDiff[3] + mediaPipePositions[4];
        modelPartObject[6].transform.position = (mediaPipePositions[6] - mediaPipePositions[5]) * _distanceDiff[4] + mediaPipePositions[5];

        modelPartObject[8].transform.position = (mediaPipePositions[8] - mediaPipePositions[7]) * _distanceDiff[5] + mediaPipePositions[7];
        modelPartObject[9].transform.position = (mediaPipePositions[9] - mediaPipePositions[8]) * _distanceDiff[6] + mediaPipePositions[8];
        modelPartObject[10].transform.position = (mediaPipePositions[10] - mediaPipePositions[9]) * _distanceDiff[7] + mediaPipePositions[9];

        modelPartObject[12].transform.position = (mediaPipePositions[12] - mediaPipePositions[11]) * _distanceDiff[8] + mediaPipePositions[11];
        modelPartObject[13].transform.position = (mediaPipePositions[13] - mediaPipePositions[12]) * _distanceDiff[9] + mediaPipePositions[12];
        modelPartObject[14].transform.position = (mediaPipePositions[14] - mediaPipePositions[13]) * _distanceDiff[10] + mediaPipePositions[13];

        modelPartObject[16].transform.position = (mediaPipePositions[16] - mediaPipePositions[17]) * _distanceDiff[11] + mediaPipePositions[15];
        modelPartObject[17].transform.position = (mediaPipePositions[17] - mediaPipePositions[16]) * _distanceDiff[12] + mediaPipePositions[16];
        modelPartObject[18].transform.position = (mediaPipePositions[18] - mediaPipePositions[17]) * _distanceDiff[13] + mediaPipePositions[17];

    }

}
