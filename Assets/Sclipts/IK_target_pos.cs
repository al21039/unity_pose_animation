using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class IK_target_pos : MonoBehaviour
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
    
    Vector3[] before_part_position = new Vector3[4];

    List<int> keyPose_candidate_frames = new List<int>();
    List<float> keyPose_candidate_frames_distance = new List<float>();
    public List<int> KeyPose_List = new List<int>();

    float[] part_distance = new float[4];
    public float threshold = 10.0f;
    [SerializeField] GameObject Hips;
    [SerializeField] GameObject Body;
    [SerializeField] GameObject Left_shoulder;
    [SerializeField] GameObject Left_elbow;
    [SerializeField] GameObject Left_hand;
    [SerializeField] GameObject Right_shoulder;
    [SerializeField] GameObject Right_elbow;
    [SerializeField] GameObject Right_hand;
    [SerializeField] GameObject Left_hip;
    [SerializeField] GameObject Left_knee;
    [SerializeField] GameObject Left_ankle;
    [SerializeField] GameObject Right_hip;
    [SerializeField] GameObject Right_knee;
    [SerializeField] GameObject Right_ankle;
    [SerializeField] GameObject middleDot;

    Vector3 mp_body_dot;

    float model_dis_body;
    float model_dis_shoulder;
    float model_dis_1;
    float model_dis_2;
    float model_dis_3;
    float model_dis_4;
    float model_dis_5;
    float model_dis_6;
    float model_dis_7;
    float model_dis_8;


    Vector3 mp_middleDot_shoulderpos = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        AnimationSceneManager = scene_manager_object.GetComponent<AnimationSceneManager>();

        //モデルの関節毎の距離計算　正規化で利用
        model_dis_body = Vector3.Distance(Hips.transform.position, Body.transform.position);　//腰から体の中心
        model_dis_shoulder = Vector3.Distance(Body.transform.position, middleDot.transform.position);　//体の中心から首
        
        model_dis_1 = Vector3.Distance(Left_shoulder.transform.position, Left_elbow.transform.position); //左肩から左肘
        model_dis_2 = Vector3.Distance(Left_elbow.transform.position, Left_hand.transform.position);　//左肘から左手
        model_dis_3 = Vector3.Distance(Right_shoulder.transform.position, Right_elbow.transform.position);　//右肩から右肘
        model_dis_4 = Vector3.Distance(Right_elbow.transform.position, Right_hand.transform.position); 　//右肘から右手
        
        model_dis_5 = Vector3.Distance(Left_hip.transform.position, Left_knee.transform.position);　//左腰から左膝
        model_dis_6 = Vector3.Distance(Left_knee.transform.position, Left_ankle.transform.position);　//左膝から左足首
        model_dis_7 = Vector3.Distance(Right_hip.transform.position, Right_knee.transform.position);　//右腰から右膝
        model_dis_8 = Vector3.Distance(Right_knee.transform.position, Right_ankle.transform.position);　//右膝から右足首
        
        Application.targetFrameRate = 30;
        LoadLandmarkData();
    }

    // Update is called once per frame
    void Update()
    {
        if(check)
        {
            CreateAnimation();
        }

        if(!check && !detection_check)
        {
            DetectionKeyPose();
        }

        if (detection_check && !created_check)
        {        
            for (int i = 0; i < KeyPose_List.Count; i++)
            {
                AnimationSceneManager.SetPosition(KeyPose_List[i], modelPos[KeyPose_List[i]]);
            }
            /*
            foreach (Vector3[] value in modelPos.Values)
            {
                foreach (Vector3 vec in value)
                {
                    Debug.Log(vec);
                }

            }
            */
            created_check = true;
            Destroy(this.gameObject);
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

        //ランドマーク隣接距離
        float mp_dis_1 = Vector3.Distance(landmarks[11], landmarks[13]);
        float mp_dis_2 = Vector3.Distance(landmarks[13], landmarks[15]);
        float mp_dis_3 = Vector3.Distance(landmarks[12], landmarks[14]);
        float mp_dis_4 = Vector3.Distance(landmarks[14], landmarks[16]);

        float mp_dis_5 = Vector3.Distance(landmarks[23], landmarks[25]);
        float mp_dis_6 = Vector3.Distance(landmarks[25], landmarks[29]);
        float mp_dis_7 = Vector3.Distance(landmarks[24], landmarks[26]);
        float mp_dis_8 = Vector3.Distance(landmarks[26], landmarks[30]);

        //モデルの隣接距離とランドマークの隣接距離で比率計算
        float dis_diff_body = model_dis_body / mp_dis_body;
        float dis_diff_shoulder = model_dis_shoulder / mp_dis_shoulder;

        float dis_diff_1 = model_dis_1 / mp_dis_1;
        float dis_diff_2 = model_dis_2 / mp_dis_2;
        float dis_diff_3 = model_dis_3 / mp_dis_3;
        float dis_diff_4 = model_dis_4 / mp_dis_4;

        float dis_diff_5 = model_dis_5 / mp_dis_5;
        float dis_diff_6 = model_dis_6 / mp_dis_6;
        float dis_diff_7 = model_dis_7 / mp_dis_7;
        float dis_diff_8 = model_dis_8 / mp_dis_8;


        Body.transform.position = (mp_body_dot - new Vector3(0, 1, 0)) * dis_diff_body + Hips.transform.position;

        middleDot.transform.position = (mp_middleDot_shoulderpos - mp_body_dot) * dis_diff_shoulder + Body.transform.position;


        Left_elbow.transform.position = (landmarks[13] - landmarks[11]) * dis_diff_1 + Left_shoulder.transform.position;
        Left_hand.transform.position = (landmarks[15] - landmarks[13]) * dis_diff_2 + Left_elbow.transform.position;
        Right_elbow.transform.position = (landmarks[14] - landmarks[12]) * dis_diff_3 + Right_shoulder.transform.position;
        Right_hand.transform.position = (landmarks[16] - landmarks[14]) * dis_diff_4 + Right_elbow.transform.position;

        Left_knee.transform.position = (landmarks[25] - landmarks[23]) * dis_diff_5 + Left_hip.transform.position;
        Left_ankle.transform.position = (landmarks[29] - landmarks[25]) * dis_diff_6 + Left_knee.transform.position;
        Right_knee.transform.position = (landmarks[26] - landmarks[24]) * dis_diff_7 + Right_hip.transform.position;
        Right_ankle.transform.position = (landmarks[30] - landmarks[26]) * dis_diff_8 + Right_knee.transform.position;

        Vector3[] part_position = new Vector3[]
        {
            Left_hand.transform.position,
            Right_hand.transform.position,
            Left_ankle.transform.position,
            Right_ankle.transform.position,
            Left_elbow.transform.position,
            Right_elbow.transform.position,
            Left_knee.transform.position,
            Right_knee.transform.position,
            Body.transform.position,
            middleDot.transform.position
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
            currentFrame = 0;
            check = false;
        }
    }


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
        detection_check = true;
    }
}
