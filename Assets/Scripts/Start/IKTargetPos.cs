using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IKTargetPos : BaseCalculation
{
    [SerializeField] private string csvFilePath = "Assets/CSV/kick.csv";
    [SerializeField] GameObject[] _modelLimbObject = new GameObject[19];
    [SerializeField] GameObject[] _modelTransform = new GameObject[3];
    [SerializeField] private List<float> _hipHeightRatio = new List<float>();
    [SerializeField] private Animator _modelAnimator;
    [SerializeField] private Transform[] _modelPartTransform = new Transform[2];
    
    private Dictionary<int, Vector3[]> landmarkData = new Dictionary<int, Vector3[]>();
    private Dictionary<int, Vector3[]> modelPos = new Dictionary<int, Vector3[]>();
    private Dictionary<int, Quaternion[]> _modelQuaternion = new Dictionary<int, Quaternion[]>();

    
    public int currentFrame = 0;
    int totalFrames = 0;
    
    bool isCreated = false;
    bool isLoaded = false;
    private Quaternion _leftFootRotationOffset;
    private Quaternion _rightFootRotationOffset;
    private Quaternion _leftHandRotationOffset;
    private Quaternion _rightHandRotationOffset;
    
    Vector3[] before_part_position = new Vector3[4];

    List<int> keyPose_candidate_frames = new List<int>();
    List<float> keyPose_candidate_frames_distance = new List<float>();
    private List<int> KeyPose_List = new List<int>();

    float[] part_distance = new float[4];
    public float threshold = 10.0f;

    private float[] _modelLimbDistance;
    private float[] _mediaPipeLimbDistance;

    void Start()
    {
        _leftHandRotationOffset = _modelPartTransform[0].rotation;
        _rightHandRotationOffset = _modelPartTransform[1].rotation;
        _leftFootRotationOffset = _modelTransform[5].transform.rotation;
        _rightFootRotationOffset = _modelTransform[6].transform.rotation;
        Debug.Log(_leftHandRotationOffset.eulerAngles);
        CalcModelDistance();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLoaded && !isCreated)
        {
            if (currentFrame == 0) { 
                StartCoroutine(CreateFramePose());
            }
            else
            {
                StartCoroutine(CreateFramePose());
                //CreateAnimation();
            }
        }
    }
    private void CalcModelDistance()
    {
        _modelLimbDistance = ReturnDistance(_modelLimbObject);

        LoadLandmarkData();
    }

    //CSVファイルからランドマークデータ読み出し
    void LoadLandmarkData()
    {
        using (var reader = new StreamReader(csvFilePath))
        {
            bool isFirstLine = true;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue; // Skip header line
                }
                var values = line.Split(',');
                int frame = int.Parse(values[0]);
                Vector3[] landmarks = new Vector3[33];

                for (int i = 0; i < 33; i++)
                {
                    float x = float.Parse(values[1 + i * 3]);
                    float y = float.Parse(values[2 + i * 3]);
                    float z = float.Parse(values[3 + i * 3]);
                    landmarks[i] = new Vector3(-x, -y + 1, -z);
                }
                landmarkData[frame] = landmarks;
                _hipHeightRatio.Add(float.Parse(values[100]));
                totalFrames++;
            }
        }
        isLoaded = true;
    }

    IEnumerator CreateFramePose()
    {
        //フレーム毎の各ランドマーク座標
        Vector3[] landmarks = landmarkData[currentFrame];
        float hipHight = _hipHeightRatio[currentFrame];

        transform.position = new Vector3(0.0f, hipHight - 1.0f, 0.0f);

        Vector3 middleHead = (landmarks[7] + landmarks[8]) / 2;
        Vector3 middleMouth = (landmarks[9] + landmarks[10]) / 2;
        Vector3 middleShoulder = (landmarks[11] + landmarks[12]) / 2;
        Vector3 middleThigh = (landmarks[23] + landmarks[24]) / 2;
        Vector3 middleLeftHand = (landmarks[17] + landmarks[19]) / 2;
        Vector3 middleRightHand = (landmarks[18] + landmarks[20]) / 2;

        Vector3 rawVerticalAxis = (middleShoulder - middleThigh).normalized;
        Vector3 horizontalAxis = (landmarks[24] - landmarks[23]).normalized;

        Vector3 verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;

        Vector3 hipForward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        Quaternion hip = Quaternion.LookRotation(hipForward, verticalAxis);
        _modelTransform[0].transform.rotation = hip;

        horizontalAxis = (landmarks[12] - landmarks[11]).normalized;
        verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;
        Vector3 shoulderForward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        Quaternion shoulder = Quaternion.LookRotation(shoulderForward, verticalAxis);
        _modelTransform[1].transform.localRotation = shoulder;

        rawVerticalAxis = (middleHead - middleShoulder).normalized;
        horizontalAxis = (landmarks[8] - landmarks[7]).normalized;
        verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;
        Vector3 headForward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        Quaternion head = Quaternion.LookRotation(headForward, verticalAxis);

        _modelTransform[2].transform.rotation = head;


        rawVerticalAxis = (middleLeftHand - landmarks[15]).normalized;
        horizontalAxis = (landmarks[19] - landmarks[17]).normalized;
        verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;
        Vector3 leftHandForward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        Quaternion leftHand = Quaternion.LookRotation(leftHandForward, rawVerticalAxis);
        var rot = Quaternion.AngleAxis(90, Vector3.up);


        _modelTransform[3].transform.rotation = leftHand * rot;

        rawVerticalAxis = (middleRightHand - landmarks[16]).normalized;
        horizontalAxis = (landmarks[18] - landmarks[20]).normalized;
        verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;
        Vector3 rightHandForward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        Quaternion rightHand = Quaternion.LookRotation(rightHandForward, verticalAxis);
        rot = Quaternion.AngleAxis(-90, Vector3.up);

        _modelTransform[4].transform.rotation = rightHand * rot;

        Vector3 leftFootDirection = (landmarks[31] - landmarks[29]).normalized;
        Quaternion leftFootForward = Quaternion.LookRotation(leftFootDirection, Vector3.up);

        var targetRotation = Quaternion.FromToRotation(_modelPartTransform[0].forward, leftHandForward);

        _modelTransform[5].transform.rotation = Quaternion.Inverse(_modelPartTransform[0].parent.rotation) * targetRotation;


        Vector3 rightFootDirection = (landmarks[32] - landmarks[30]).normalized;
        Quaternion rightFootForward = Quaternion.LookRotation(rightFootDirection, Vector3.up);

        targetRotation = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
        _modelTransform[6].transform.rotation = rightFootForward * targetRotation;


        Vector3[] _mediaPipeLimbArray = new Vector3[19]
        {
            new Vector3(0, 1, 0),
            (landmarks[11] + landmarks[12] + landmarks[23] + landmarks[24]) / 4,
            (landmarks[11] + landmarks[12]) / 2,
            landmarks[11],
            landmarks[13],
            landmarks[15],
            landmarks[19],
            landmarks[12],
            landmarks[14],
            landmarks[16],
            landmarks[20],
            landmarks[23],
            landmarks[25],
            landmarks[29],
            landmarks[31],
            landmarks[24],
            landmarks[26],
            landmarks[30],
            landmarks[32]
        };

        //四肢の部位の距離を計算  (MediaPipe)
        _mediaPipeLimbDistance = ReturnDistance(_mediaPipeLimbArray);

        //モデルの部位の距離と、mediaPipeの部位の距離で比率を計算
        float[] DistanceDiff;
        DistanceDiff = ReturnDiff(_modelLimbDistance, _mediaPipeLimbDistance);

        yield return null;
        _modelLimbObject[1].transform.position = (_mediaPipeLimbArray[1] - _mediaPipeLimbArray[0]) * DistanceDiff[0] + _modelLimbObject[0].transform.position;
        _modelLimbObject[2].transform.position = (_mediaPipeLimbArray[2] - _mediaPipeLimbArray[1]) * DistanceDiff[1] + _modelLimbObject[1].transform.position;

        yield return null;

        _modelLimbObject[4].transform.position = (_mediaPipeLimbArray[4] - _mediaPipeLimbArray[3]) * DistanceDiff[2] + _modelLimbObject[3].transform.position;
        _modelLimbObject[8].transform.position = (_mediaPipeLimbArray[8] - _mediaPipeLimbArray[7]) * DistanceDiff[5] + _modelLimbObject[7].transform.position;
        _modelLimbObject[12].transform.position = (_mediaPipeLimbArray[12] - _mediaPipeLimbArray[11]) * DistanceDiff[8] + _modelLimbObject[11].transform.position;
        _modelLimbObject[16].transform.position = (_mediaPipeLimbArray[16] - _mediaPipeLimbArray[15]) * DistanceDiff[11] + _modelLimbObject[15].transform.position;

        yield return null;

        _modelLimbObject[5].transform.position = (_mediaPipeLimbArray[5] - _mediaPipeLimbArray[4]) * DistanceDiff[3] + _modelLimbObject[4].transform.position;
        _modelLimbObject[6].transform.position = (_mediaPipeLimbArray[6] - _mediaPipeLimbArray[5]) * DistanceDiff[4] + _modelLimbObject[5].transform.position;

        _modelLimbObject[9].transform.position = (_mediaPipeLimbArray[9] - _mediaPipeLimbArray[8]) * DistanceDiff[6] + _modelLimbObject[8].transform.position;
        _modelLimbObject[10].transform.position = (_mediaPipeLimbArray[10] - _mediaPipeLimbArray[9]) * DistanceDiff[7] + _modelLimbObject[9].transform.position;

        _modelLimbObject[13].transform.position = (_mediaPipeLimbArray[13] - _mediaPipeLimbArray[12]) * DistanceDiff[9] + _modelLimbObject[12].transform.position;
        _modelLimbObject[14].transform.position = (_mediaPipeLimbArray[14] - _mediaPipeLimbArray[13]) * DistanceDiff[10] + _modelLimbObject[13].transform.position;

        _modelLimbObject[17].transform.position = (_mediaPipeLimbArray[17] - _mediaPipeLimbArray[16]) * DistanceDiff[12] + _modelLimbObject[16].transform.position;
        _modelLimbObject[18].transform.position = (_mediaPipeLimbArray[18] - _mediaPipeLimbArray[17]) * DistanceDiff[13] + _modelLimbObject[17].transform.position;


        Vector3[] part_position = new Vector3[]
        {
            _modelLimbObject[5].transform.position,
            _modelLimbObject[9].transform.position,
            _modelLimbObject[13].transform.position,
            _modelLimbObject[17].transform.position,
            _modelLimbObject[4].transform.position,
            _modelLimbObject[8].transform.position,
            _modelLimbObject[12].transform.position,
            _modelLimbObject[16].transform.position,
            _modelLimbObject[1].transform.position,
            _modelLimbObject[2].transform.position,
            _modelLimbObject[6].transform.position,
            _modelLimbObject[10].transform.position,
            _modelLimbObject[14].transform.position,
            _modelLimbObject[18].transform.position
        };

        Quaternion[] _modelRotation = new Quaternion[]
        {
            _modelTransform[0].transform.rotation,
            _modelTransform[1].transform.rotation,
            _modelTransform[2].transform.rotation,
            _modelTransform[3].transform.rotation,
            _modelTransform[4].transform.rotation,
            _modelTransform[5].transform.rotation,
            _modelTransform[6].transform.rotation,
        };

        modelPos.Add(currentFrame, part_position);
        _modelQuaternion.Add(currentFrame, _modelRotation);

        currentFrame++;

        if (currentFrame > totalFrames - 1)
        {
            DetectionKeyPose();
            isCreated = true;
        }
    }


    void CreateAnimation()
    {
        //フレーム毎の各ランドマーク座標
        Vector3[] landmarks = landmarkData[currentFrame];
        float hipHight = _hipHeightRatio[currentFrame];

        transform.position = new Vector3(0.0f, hipHight - 1.0f, 0.0f);

        Vector3 middleHead = (landmarks[7] + landmarks[8]) / 2;
        Vector3 middleMouth = (landmarks[9] + landmarks[10]) / 2;
        Vector3 middleShoulder = (landmarks[11] + landmarks[12]) / 2;
        Vector3 middleThigh = (landmarks[23] + landmarks[24]) / 2;
        Vector3 middleLeftHand = (landmarks[17] + landmarks[19]) / 2;
        Vector3 middleRightHand = (landmarks[18] + landmarks[20]) / 2;
        
        Vector3 rawVerticalAxis = (middleShoulder - middleThigh).normalized;
        Vector3 horizontalAxis = (landmarks[24] -  landmarks[23]).normalized;

        Vector3 verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;

        Vector3 hipForward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        Quaternion hip = Quaternion.LookRotation(hipForward, verticalAxis);
        _modelTransform[0].transform.rotation = hip;

        horizontalAxis = (landmarks[12] - landmarks[11]).normalized;
        verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;
        Vector3 shoulderForward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        Quaternion shoulder = Quaternion.LookRotation(shoulderForward, verticalAxis);
        _modelTransform[1].transform.localRotation = shoulder;

        rawVerticalAxis = (middleHead - middleShoulder).normalized;
        horizontalAxis = (landmarks[8] - landmarks[7]).normalized;
        verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;
        Vector3 headForward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        Quaternion head = Quaternion.LookRotation(headForward, verticalAxis);

        _modelTransform[2].transform.rotation = head;


        rawVerticalAxis = (middleLeftHand - landmarks[15]).normalized;
        horizontalAxis = (landmarks[19] - landmarks[17]).normalized;
        verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;
        Vector3 leftHandForward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        Quaternion leftHand = Quaternion.LookRotation(leftHandForward, verticalAxis);
        var offsetRotation = Quaternion.FromToRotation(new Vector3(-1, 0, 0), Vector3.forward);

        _modelTransform[3].transform.rotation = leftHand * Quaternion.Euler(0, 90, 0);

        rawVerticalAxis = (middleRightHand - landmarks[16]).normalized;
        horizontalAxis = (landmarks[18] - landmarks[20]).normalized;
        verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;
        Vector3 rightHandForward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        Quaternion rightHand = Quaternion.LookRotation(rightHandForward, verticalAxis);
        offsetRotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), Vector3.forward);

        _modelTransform[4].transform.rotation = rightHand * offsetRotation;

        Vector3 leftFootDirection = (landmarks[31] - landmarks[29]).normalized;
        Quaternion leftFootForward = Quaternion.LookRotation(leftFootDirection, Vector3.up);

        var targetRotation = Quaternion.FromToRotation(_modelPartTransform[0].forward, leftHandForward);

        _modelTransform[5].transform.rotation = Quaternion.Inverse(_modelPartTransform[0].parent.rotation) * targetRotation;


        Vector3 rightFootDirection = (landmarks[32] - landmarks[30]).normalized;
        Quaternion rightFootForward = Quaternion.LookRotation(rightFootDirection, Vector3.up);

        targetRotation = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
        _modelTransform[6].transform.rotation = rightFootForward * targetRotation;


        Vector3[] _mediaPipeLimbArray = new Vector3[19]
        {
            new Vector3(0, 1, 0),
            (landmarks[11] + landmarks[12] + landmarks[23] + landmarks[24]) / 4,
            (landmarks[11] + landmarks[12]) / 2,
            landmarks[11],
            landmarks[13],
            landmarks[15],
            landmarks[19],
            landmarks[12],
            landmarks[14],
            landmarks[16],
            landmarks[20],
            landmarks[23],
            landmarks[25],
            landmarks[29],
            landmarks[31],
            landmarks[24],
            landmarks[26],
            landmarks[30],
            landmarks[32]
        };

        //四肢の部位の距離を計算  (MediaPipe)
        _mediaPipeLimbDistance = ReturnDistance(_mediaPipeLimbArray);

        //モデルの部位の距離と、mediaPipeの部位の距離で比率を計算
        float[] DistanceDiff;
        DistanceDiff = ReturnDiff(_modelLimbDistance, _mediaPipeLimbDistance);

        _modelLimbObject[1].transform.position = (_mediaPipeLimbArray[1] - _mediaPipeLimbArray[0]) * DistanceDiff[0] + _modelLimbObject[0].transform.position;
        _modelLimbObject[2].transform.position = (_mediaPipeLimbArray[2] - _mediaPipeLimbArray[1]) * DistanceDiff[1] + _modelLimbObject[1].transform.position;

        _modelLimbObject[4].transform.position = (_mediaPipeLimbArray[4] - _mediaPipeLimbArray[3]) * DistanceDiff[2] + _modelLimbObject[3].transform.position;
        _modelLimbObject[8].transform.position = (_mediaPipeLimbArray[8] - _mediaPipeLimbArray[7]) * DistanceDiff[5] + _modelLimbObject[7].transform.position;
        _modelLimbObject[12].transform.position = (_mediaPipeLimbArray[12] - _mediaPipeLimbArray[11]) * DistanceDiff[8] + _modelLimbObject[11].transform.position;
        _modelLimbObject[16].transform.position = (_mediaPipeLimbArray[16] - _mediaPipeLimbArray[15]) * DistanceDiff[11] + _modelLimbObject[15].transform.position;


        _modelLimbObject[5].transform.position = (_mediaPipeLimbArray[5] - _mediaPipeLimbArray[4]) * DistanceDiff[3] + _modelLimbObject[4].transform.position;
        _modelLimbObject[6].transform.position = (_mediaPipeLimbArray[6] - _mediaPipeLimbArray[5]) * DistanceDiff[4] + _modelLimbObject[5].transform.position;

        _modelLimbObject[9].transform.position = (_mediaPipeLimbArray[9] - _mediaPipeLimbArray[8]) * DistanceDiff[6] + _modelLimbObject[8].transform.position;
        _modelLimbObject[10].transform.position = (_mediaPipeLimbArray[10] - _mediaPipeLimbArray[9]) * DistanceDiff[7] + _modelLimbObject[9].transform.position;

        _modelLimbObject[13].transform.position = (_mediaPipeLimbArray[13] - _mediaPipeLimbArray[12]) * DistanceDiff[9] + _modelLimbObject[12].transform.position;
        _modelLimbObject[14].transform.position = (_mediaPipeLimbArray[14] - _mediaPipeLimbArray[13]) * DistanceDiff[10] + _modelLimbObject[13].transform.position;

        _modelLimbObject[17].transform.position = (_mediaPipeLimbArray[17] - _mediaPipeLimbArray[16]) * DistanceDiff[12] + _modelLimbObject[16].transform.position;
        _modelLimbObject[18].transform.position = (_mediaPipeLimbArray[18] - _mediaPipeLimbArray[17]) * DistanceDiff[13] + _modelLimbObject[17].transform.position;
        

        Vector3[] part_position = new Vector3[]
        {
            _modelLimbObject[5].transform.position,
            _modelLimbObject[9].transform.position,
            _modelLimbObject[13].transform.position,
            _modelLimbObject[17].transform.position,
            _modelLimbObject[4].transform.position,
            _modelLimbObject[8].transform.position,
            _modelLimbObject[12].transform.position,
            _modelLimbObject[16].transform.position,
            _modelLimbObject[1].transform.position,
            _modelLimbObject[2].transform.position,
            _modelLimbObject[6].transform.position,
            _modelLimbObject[10].transform.position,
            _modelLimbObject[14].transform.position,
            _modelLimbObject[18].transform.position
        };

        Quaternion[] _modelRotation = new Quaternion[]
        {
            _modelTransform[0].transform.rotation,
            _modelTransform[1].transform.rotation,
            _modelTransform[2].transform.rotation,
            _modelTransform[3].transform.rotation,
            _modelTransform[4].transform.rotation,
            _modelTransform[5].transform.rotation,
            _modelTransform[6].transform.rotation,
        };

        modelPos.Add(currentFrame, part_position);
        _modelQuaternion.Add(currentFrame, _modelRotation);

        /*
        //関節の前フレームとの距離検出
        if (currentFrame == 0 || currentFrame == totalFrames - 1)
        {
            keyPose_candidate_frames.Add(currentFrame);
            keyPose_candidate_frames_distance.Add(0.0f);
        }
        else
        {
            bool key_check = false;
            float max = 0;
            int over_threhold = 0;
            int tmp = 0;

            for (int i = 0; i < 4; i++)
            {
                part_distance[i] = Vector3.Distance(part_position[i] * 100, before_part_position[i] * 100);
                if (part_distance[i] > threshold)
                {
                    over_threhold++;
                    if (!key_check)
                    {
                        keyPose_candidate_frames.Add(currentFrame);
                    }
                    key_check = true;
                    if (max < part_distance[i])
                    {
                        max = part_distance[i];
                        tmp = i;
                    }
                }
            }
            if (key_check)
            {
                keyPose_candidate_frames_distance.Add(max);
            }
        }
        for (int i = 0; i < 4; i++)
        {
            before_part_position[i] = part_position[i];
        }
        */

        currentFrame++;

        if (currentFrame > totalFrames - 1)
        {
            DetectionKeyPose();
            isCreated = true;
        }
    }

    //キーポーズの摘出
    void DetectionKeyPose()
    {
        /*
        bool serial = false;
        bool first = true;
        int num = 0;
        int first_address = 0;
        int special_flg = 0;
        float max = 0;
        int max_number = 0;

        for(int i = 0; i < keyPose_candidate_frames.Count; i++)
        {
            if(i == 0)
            {
                special_flg = 1;
            }

            if (i == keyPose_candidate_frames.Count - 1)
            {
                special_flg = 2;
            }

            if (special_flg != 2)
            {
                if (keyPose_candidate_frames[i + 1] - keyPose_candidate_frames[i] < 6)
                {
                    num++;
                    if (first)
                    {
                        serial = true;
                        first_address = keyPose_candidate_frames[i];
                        first = false;
                    }

                    if (max < keyPose_candidate_frames_distance[i] && special_flg == 0)
                    {
                        max = keyPose_candidate_frames_distance[i];
                        max_number = keyPose_candidate_frames[i];
                    }
                }
                else if (serial)
                {
                    max = 0;
                    serial = false;
                }
            }
            else
            {
                max = 0;
                serial = false;
            }

            if (!serial)
            {
                if (!first)
                {
                    if (num > 9)
                    {
                        KeyPose_List.Add(first_address);
                        KeyPose_List.Add(keyPose_candidate_frames[i]);
                    }
                    else
                    {
                        if (special_flg == 1)
                        {
                            KeyPose_List.Add(0);
                            special_flg = 0;
                        }
                        else if (special_flg == 2)
                        {
                            KeyPose_List.Add(i + 1);
                            special_flg = 0;
                        }
                        else
                        {
                            KeyPose_List.Add(max_number);
                        }
                    }
                    first = true;
                    first_address = 0;
                    max_number = 0;
                    num = 0;
                }
                else
                {
                    if (special_flg == 1)
                    {
                        KeyPose_List.Add(0);
                        special_flg = 0;
                    }
                    else if (special_flg == 2)
                    {
                        KeyPose_List.Add(keyPose_candidate_frames[i]);
                        special_flg = 0;
                    }
                    else
                    {
                        KeyPose_List.Add(keyPose_candidate_frames[i]);
                    }
                }
            }
        }
        */

        if(KeyPose_List.Count <= 3)
        {
            KeyPose_List.Clear();
            KeyPose_List.Add(0);

            for(int i = 1; i < totalFrames - 5; i++)
            {
                if(i % 5 == 0 && (totalFrames -1) - i >= 5)
                {
                    KeyPose_List.Add(i);
                }
            }

            KeyPose_List.Add(totalFrames - 1);
        }
        
        LandmarkManager.GetInstance().TotalFrame = totalFrames;
        LandmarkManager.GetInstance().CSVLandmarkPositions = modelPos;
        LandmarkManager.GetInstance().CSVLandmarkRotations = _modelQuaternion;
        LandmarkManager.GetInstance().KeyPoseList = KeyPose_List;
        LandmarkManager.GetInstance().HipHeight = _hipHeightRatio;
        Destroy(this.gameObject);
    }

}
