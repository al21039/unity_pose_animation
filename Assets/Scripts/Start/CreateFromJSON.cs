using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CreateFromJSON : BaseCalculation
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

    [SerializeField] GameObject[] modelPartObject;
    [SerializeField] GameObject[] _modelRotation;

    private Vector3[] landmarksArray;
    private float[] _modelPartDistance;
    private float[] _mediapipePartDistance;
    private float[] _distanceDiff;
    private Vector3[] _mediaPipePositions;
    private Vector3[] _modelPartPosition;
    private Quaternion[] _modelPartRotation;


    public void SetJsonLandmark(TextAsset textFile)
    {
        landmarksArray = DetectionLandmarkPosition(textFile);

        if (landmarksArray.Length == 1)
        {
            Debug.LogError("正しく読み取れていません");
        }

        _mediaPipePositions = new Vector3[19] { 
            new Vector3(0, 1, 0),   //mediaPipeの腰の位置
            (landmarksArray[11] + landmarksArray[12] + landmarksArray[23] + landmarksArray[24]) / 4,   //体の中心
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
        _mediapipePartDistance = ReturnDistance(_mediaPipePositions);
        _distanceDiff = ReturnDiff(_modelPartDistance, _mediapipePartDistance);
        SetNewPosition();
    }

    private Vector3[] DetectionLandmarkPosition(TextAsset jsonData)
    {
        LandmarkData landmarkData = JsonUtility.FromJson<LandmarkData>(jsonData.text);

        Vector3[] landmarkArray = new Vector3[landmarkData.num_landmarks];

        for (int i = 0; i < landmarkData.num_landmarks; i++)
        {
            Landmark lm = landmarkData.landmarks[i];
            landmarkArray[i] = new Vector3(-lm.x, -lm.y + 1, -lm.z);
        }

        return landmarkArray;
    }

    private void SetNewPosition()
    {
        Vector3 middleHead = (landmarksArray[7] + landmarksArray[8]) / 2;
        Vector3 middleMouth = (landmarksArray[9] + landmarksArray[10]) / 2;
        Vector3 middleShoulder = (landmarksArray[11] + landmarksArray[12]) / 2;
        Vector3 middleThigh = (landmarksArray[23] + landmarksArray[24]) / 2;
        Vector3 middleLeftHand = (landmarksArray[17] + landmarksArray[19]) / 2;
        Vector3 middleRightHand = (landmarksArray[18] + landmarksArray[20]) / 2;

        Vector3 rawVerticalAxis = (middleShoulder - middleThigh).normalized;
        Vector3 horizontalAxis = (landmarksArray[24] - landmarksArray[23]).normalized;

        Vector3 verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;

        Vector3 hipForward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        Quaternion hip = Quaternion.LookRotation(hipForward, verticalAxis);
        _modelRotation[0].transform.rotation = hip;

        horizontalAxis = (landmarksArray[12] - landmarksArray[11]).normalized;
        verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;
        Vector3 shoulderForward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        Quaternion shoulder = Quaternion.LookRotation(shoulderForward, verticalAxis);
        _modelRotation[1].transform.rotation = shoulder;

        rawVerticalAxis = (middleHead - middleShoulder).normalized;
        horizontalAxis = (landmarksArray[8] - landmarksArray[7]).normalized;
        verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;
        Vector3 headForward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        Quaternion head = Quaternion.LookRotation(headForward, verticalAxis);

        _modelRotation[2].transform.rotation = head;


        rawVerticalAxis = (middleLeftHand - landmarksArray[15]).normalized;
        horizontalAxis = (landmarksArray[19] - landmarksArray[17]).normalized;
        verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;
        Vector3 leftHandForward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        Quaternion leftHand = Quaternion.LookRotation(leftHandForward, verticalAxis);
        var offsetRotation = Quaternion.FromToRotation(new Vector3(-1, 0, 0), Vector3.forward);

        _modelRotation[3].transform.rotation = leftHand * offsetRotation;

        rawVerticalAxis = (middleRightHand - landmarksArray[16]).normalized;
        horizontalAxis = (landmarksArray[18] - landmarksArray[20]).normalized;
        verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;
        Vector3 rightHandForward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        Quaternion rightHand = Quaternion.LookRotation(rightHandForward, verticalAxis);
        offsetRotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), Vector3.forward);

        _modelRotation[4].transform.rotation = rightHand * offsetRotation;

        Vector3 leftFootDirection = (landmarksArray[31] - landmarksArray[29]).normalized;
        Quaternion leftFootForward = Quaternion.LookRotation(leftFootDirection, Vector3.up);

        var targetRotation = Quaternion.FromToRotation(Vector3.up, leftHandForward);

        _modelRotation[5].transform.rotation = leftFootForward * targetRotation;


        Vector3 rightFootDirection = (landmarksArray[32] - landmarksArray[30]).normalized;
        Quaternion rightFootForward = Quaternion.LookRotation(rightFootDirection, Vector3.up);

        targetRotation = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
        _modelRotation[6].transform.rotation = rightFootForward * targetRotation;

        modelPartObject[1].transform.position = (_mediaPipePositions[1] - _mediaPipePositions[0]) * _distanceDiff[0] + modelPartObject[0].transform.position;
        modelPartObject[2].transform.position = (_mediaPipePositions[2] - _mediaPipePositions[1]) * _distanceDiff[1] + modelPartObject[1].transform.position;

        modelPartObject[4].transform.position = (_mediaPipePositions[4] - _mediaPipePositions[3]) * _distanceDiff[2] + modelPartObject[3].transform.position;
        modelPartObject[5].transform.position = (_mediaPipePositions[5] - _mediaPipePositions[4]) * _distanceDiff[3] + modelPartObject[4].transform.position;
        modelPartObject[6].transform.position = (_mediaPipePositions[6] - _mediaPipePositions[5]) * _distanceDiff[4] + modelPartObject[5].transform.position;     

        modelPartObject[8].transform.position = (_mediaPipePositions[8] - _mediaPipePositions[7]) * _distanceDiff[5] + modelPartObject[7].transform.position;
        modelPartObject[9].transform.position = (_mediaPipePositions[9] - _mediaPipePositions[8]) * _distanceDiff[6] + modelPartObject[8].transform.position;
        modelPartObject[10].transform.position = (_mediaPipePositions[10] - _mediaPipePositions[9]) * _distanceDiff[7] + modelPartObject[9].transform.position;

        modelPartObject[12].transform.position = (_mediaPipePositions[12] - _mediaPipePositions[11]) * _distanceDiff[8] + modelPartObject[11].transform.position;
        modelPartObject[13].transform.position = (_mediaPipePositions[13] - _mediaPipePositions[12]) * _distanceDiff[9] + modelPartObject[12].transform.position;
        modelPartObject[14].transform.position = (_mediaPipePositions[14] - _mediaPipePositions[13]) * _distanceDiff[10] + modelPartObject[13].transform.position;

        modelPartObject[16].transform.position = (_mediaPipePositions[16] - _mediaPipePositions[15]) * _distanceDiff[11] + modelPartObject[15].transform.position;
        modelPartObject[17].transform.position = (_mediaPipePositions[17] - _mediaPipePositions[16]) * _distanceDiff[12] + modelPartObject[16].transform.position;
        modelPartObject[18].transform.position = (_mediaPipePositions[18] - _mediaPipePositions[17]) * _distanceDiff[13] + modelPartObject[17].transform.position;


        _modelPartPosition = new Vector3[14] {
            modelPartObject[5].transform.position,
            modelPartObject[9].transform.position,
            modelPartObject[13].transform.position,
            modelPartObject[17].transform.position,
            modelPartObject[4].transform.position,
            modelPartObject[8].transform.position,
            modelPartObject[12].transform.position,
            modelPartObject[16].transform.position,
            modelPartObject[1].transform.position,
            modelPartObject[2].transform.position,
            modelPartObject[6].transform.position,
            modelPartObject[10].transform.position,
            modelPartObject[14].transform.position,
            modelPartObject[18].transform.position
        };

        _modelPartRotation = new Quaternion[7]
        {
            _modelRotation[0].transform.rotation,
            _modelRotation[1].transform.rotation,
            _modelRotation[2].transform.rotation,
            _modelRotation[3].transform.rotation,
            _modelRotation[4].transform.rotation,
            _modelRotation[5].transform.rotation,
            _modelRotation[6].transform.rotation
        };

        StartCoroutine(CaptureScreenshot());
        
    }

    private void CreatedAnimation()
    {
        LandmarkManager.GetInstance().SetJsonLandmarkPosition(_modelPartPosition, _modelPartRotation);
        Destroy(gameObject);
    }


    private IEnumerator CaptureScreenshot()
    {
        yield return new WaitForEndOfFrame();

        string startDate = LandmarkManager.GetInstance().StartDate;
        int jsonCount = LandmarkManager.GetInstance().JsonCount;

        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        string fileName = jsonCount + ".png";
        string folderPath = "JSON/ModelImages/";
        string fullFolderPath = Path.Combine(Application.dataPath, folderPath, startDate);
        if (!Directory.Exists(fullFolderPath))
        {
            Directory.CreateDirectory(fullFolderPath); // フォルダが存在しない場合は作成
        }

        string filePath = Path.Combine(fullFolderPath, fileName);

        // PNG形式で保存
        byte[] bytes = screenshot.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        LandmarkManager.GetInstance().FileName = folderPath + startDate;

        // テクスチャを破棄
        Destroy(screenshot);
        CreatedAnimation();
    }

}
