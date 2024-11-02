using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IK_target_pos : BaseCalc
{
    public string csvFilePath = "Assets/CSV/stretch.csv";
    public bool roop = false;
    private Dictionary<int, Vector3[]> landmarkData = new Dictionary<int, Vector3[]>();
    public Dictionary<int, Vector3[]> modelPos = new Dictionary<int, Vector3[]>();
    [SerializeField] GameObject scene_manager_object;
    AnimationSceneManager AnimationSceneManager;
    
    public int currentFrame = 0;
    int totalFrames = 0;
    
    bool check = true;
    bool created_check = false;
    bool detection_check = false;
    bool isCreated = false;
    
    Vector3[] before_part_position = new Vector3[4];

    List<int> keyPose_candidate_frames = new List<int>();
    List<float> keyPose_candidate_frames_distance = new List<float>();
    public List<int> KeyPose_List = new List<int>();

    float[] part_distance = new float[4];
    public float threshold = 10.0f;
    [SerializeField] GameObject Hips;
    [SerializeField] GameObject Body;
    [SerializeField] GameObject middleDot;
    [SerializeField] GameObject _LeftToe;
    [SerializeField] GameObject _RightToe;
    [SerializeField] GameObject _LeftFinger;
    [SerializeField] GameObject _RightFinger;

    [SerializeField] GameObject[] _modelLimbObject = new GameObject[12];

    Vector3 mp_body_dot;

    float model_dis_body;
    float model_dis_shoulder;
    float _modelDisLToe;
    float _modelDisRToe;
    float _modelDisLFinger;
    float _modelDisRFinger;


    private float[] _modelLimbDistance = new float[8];
    private float[] _mediaPipeLimbDistance = new float[8];


    Vector3 mp_middleDot_shoulderpos = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        AnimationSceneManager = scene_manager_object.GetComponent<AnimationSceneManager>();

        //モデルの関節毎の距離計算　正規化で利用
        model_dis_body = Vector3.Distance(Hips.transform.position, Body.transform.position);　//腰から体の中心
        model_dis_shoulder = Vector3.Distance(Body.transform.position, middleDot.transform.position);　//体の中心から首

        _modelDisLToe = Vector3.Distance(_modelLimbObject[8].transform.position, _LeftToe.transform.position);
        _modelDisRToe = Vector3.Distance(_modelLimbObject[11].transform.position, _RightToe.transform.position);
        _modelDisLFinger = Vector3.Distance(_modelLimbObject[2].transform.position, _LeftFinger.transform.position);
        _modelDisRFinger = Vector3.Distance(_modelLimbObject[5].transform.position, _RightFinger.transform.position);

        _modelLimbDistance = ReturnLimbDistance(_modelLimbObject);
        
        Application.targetFrameRate = 30;
        LoadLandmarkData();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isCreated)
        {
            CreateAnimation();
        }
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
                totalFrames++;
            }
        }
    }

    void CreateAnimation()
    {
        //フレーム毎の各ランドマーク座標
        Vector3[] landmarks = landmarkData[currentFrame];

        mp_middleDot_shoulderpos = (landmarks[11] + landmarks[12]) / 2;
        mp_body_dot = (landmarks[11] + landmarks[12] + landmarks[23] + landmarks[24]) / 4;

        float mp_dis_body = Vector3.Distance(new Vector3(0, 1, 0), mp_body_dot);
        float mp_dis_shoulder = Vector3.Distance(mp_body_dot, mp_middleDot_shoulderpos);

        float mpDisLeftToe = Vector3.Distance(landmarks[29], landmarks[31]);
        float mpDisRightToe = Vector3.Distance(landmarks[30], landmarks[32]);
        float mpDisLeftFinger = Vector3.Distance(landmarks[15], landmarks[19]);
        float mpDisRightFinger = Vector3.Distance(landmarks[16], landmarks[20]);

        Vector3[] _mediaPipeLimbArray = new Vector3[12]
        {
            landmarks[11],
            landmarks[13],
            landmarks[15],
            landmarks[12],
            landmarks[14],
            landmarks[16],
            landmarks[23],
            landmarks[25],
            landmarks[29],
            landmarks[24],
            landmarks[26],
            landmarks[30]
        };

        //四肢の部位の距離を計算
        _mediaPipeLimbDistance = ReturnLimbDistance(_mediaPipeLimbArray);

        //モデルの隣接距離とランドマークの隣接距離で比率計算
        float dis_diff_body = model_dis_body / mp_dis_body;
        float dis_diff_shoulder = model_dis_shoulder / mp_dis_shoulder;

        float disDiffLeftToe = _modelDisLToe / mpDisLeftToe;
        float disDiffRightToe = _modelDisRToe / mpDisRightToe;
        float disDiffLeftFinger = _modelDisLFinger / mpDisLeftFinger;
        float disDiffRightFinger = _modelDisRFinger / mpDisRightFinger;

        //モデルの部位の距離と、mediaPipeの部位の距離で比率を計算
        float[] DistanceDiff = new float[8];
        for (int i = 0; i < DistanceDiff.Length; i++)
        {
            DistanceDiff[i] = _modelLimbDistance[i] / _mediaPipeLimbDistance[i];
        }

        Body.transform.position = (mp_body_dot - new Vector3(0, 1, 0)) * dis_diff_body + Hips.transform.position;

        middleDot.transform.position = (mp_middleDot_shoulderpos - mp_body_dot) * dis_diff_shoulder + Body.transform.position;

        _LeftToe.transform.position = (landmarks[31] - landmarks[29]) * disDiffLeftToe + _modelLimbObject[8].transform.position;
        _RightToe.transform.position = (landmarks[32] - landmarks[30]) * disDiffRightToe + _modelLimbObject[11].transform.position;
        _LeftFinger.transform.position = (landmarks[19] - landmarks[15]) * disDiffLeftFinger + _modelLimbObject[2].transform.position;
        _RightFinger.transform.position = (landmarks[20] - landmarks[16]) * disDiffRightFinger + _modelLimbObject[5].transform.position;

        _modelLimbObject[1].transform.position = (_mediaPipeLimbArray[1] - _mediaPipeLimbArray[0]) * DistanceDiff[0] + _modelLimbObject[0].transform.position;
        _modelLimbObject[2].transform.position = (_mediaPipeLimbArray[2] - _mediaPipeLimbArray[1]) * DistanceDiff[1] + _modelLimbObject[1].transform.position;
        _modelLimbObject[4].transform.position = (_mediaPipeLimbArray[4] - _mediaPipeLimbArray[3]) * DistanceDiff[2] + _modelLimbObject[3].transform.position;
        _modelLimbObject[5].transform.position = (_mediaPipeLimbArray[5] - _mediaPipeLimbArray[4]) * DistanceDiff[3] + _modelLimbObject[4].transform.position;
        _modelLimbObject[7].transform.position = (_mediaPipeLimbArray[7] - _mediaPipeLimbArray[6]) * DistanceDiff[4] + _modelLimbObject[6].transform.position;
        _modelLimbObject[8].transform.position = (_mediaPipeLimbArray[8] - _mediaPipeLimbArray[7]) * DistanceDiff[5] + _modelLimbObject[7].transform.position;
        _modelLimbObject[10].transform.position = (_mediaPipeLimbArray[10] - _mediaPipeLimbArray[9]) * DistanceDiff[6] + _modelLimbObject[9].transform.position;
        _modelLimbObject[11].transform.position = (_mediaPipeLimbArray[11] - _mediaPipeLimbArray[10]) * DistanceDiff[7] + _modelLimbObject[10].transform.position;

        

        Vector3[] part_position = new Vector3[]
        {
            _modelLimbObject[2].transform.position,
            _modelLimbObject[5].transform.position,
            _modelLimbObject[8].transform.position,
            _modelLimbObject[11].transform.position,
            _modelLimbObject[1].transform.position,
            _modelLimbObject[4].transform.position,
            _modelLimbObject[7].transform.position,
            _modelLimbObject[10].transform.position,
            Body.transform.position,
            middleDot.transform.position,
            _LeftFinger.transform.position,
            _RightFinger.transform.position,
            _LeftToe.transform.position,
            _RightToe.transform.position,
        };



        modelPos.Add(currentFrame, part_position);


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

        if(KeyPose_List.Count <= 3)
        {
            KeyPose_List.Clear();
            KeyPose_List.Add(0);

            for(int i = 1; i < totalFrames - 1; i++)
            {
                if(i % 10 == 0)
                {
                    KeyPose_List.Add(i);
                }
            }


        }

        nextPhase();
    }

    void nextPhase()
    {
        AnimationSceneManager.SetTotalKeyFrame(KeyPose_List.Count);
        for (int i = 0; i < KeyPose_List.Count; i++)
        {
            AnimationSceneManager.SetPosition(KeyPose_List[i], modelPos[KeyPose_List[i]]);
        }
        AnimationSceneManager.SetTotalFrame(totalFrames);
        AnimationSceneManager.SetKeyPoseList(KeyPose_List);
        AnimationSceneManager.SetModelPos(modelPos);
        AnimationSceneManager.SetChangedPos(modelPos);
        AnimationSceneManager.SetSpline();
        created_check = true;
        Destroy(this.gameObject);
    }
}
