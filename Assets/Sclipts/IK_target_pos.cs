using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IK_target_pos : MonoBehaviour
{
    public string csvFilePath = "Assets/CSV/stretch.csv";
    private Dictionary<int, Vector3[]> landmarkData = new Dictionary<int, Vector3[]>();
    private int currentFrame = 0;
    private int totalFlames = 0;
    [SerializeField] Vector3 forward = new Vector3(0, 1, 0);
    [SerializeField] GameObject Hips;
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

    float model_dis_0 = 0;
    float model_dis_1 = 0;
    float model_dis_2 = 0;
    float model_dis_3 = 0;
    float model_dis_4 = 0;
    float model_dis_5 = 0;
    float model_dis_6 = 0;
    float model_dis_7 = 0;
    float model_dis_8 = 0;

    Vector3 mp_middleDot_shoulderpos = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        //ƒ‚ƒfƒ‹‚ÌŠÖß–ˆ‚Ì‹——£ŒvŽZ@³‹K‰»‚Å—˜—p
        model_dis_0 = Vector3.Distance(Hips.transform.position, middleDot.transform.position);
        
        model_dis_1 = Vector3.Distance(Left_shoulder.transform.position, Left_elbow.transform.position);
        model_dis_2 = Vector3.Distance(Left_elbow.transform.position, Left_hand.transform.position);
        model_dis_3 = Vector3.Distance(Right_shoulder.transform.position, Right_elbow.transform.position);
        model_dis_4 = Vector3.Distance(Right_elbow.transform.position, Right_hand.transform.position); 
        
        model_dis_5 = Vector3.Distance(Left_hip.transform.position, Left_knee.transform.position);
        model_dis_6 = Vector3.Distance(Left_knee.transform.position, Left_ankle.transform.position);
        model_dis_7 = Vector3.Distance(Right_hip.transform.position, Right_knee.transform.position);
        model_dis_8 = Vector3.Distance(Right_knee.transform.position, Right_ankle.transform.position);

        
        Application.targetFrameRate = 30;
        LoadLandmarkData();
    }

    // Update is called once per frame
    void Update()
    {
        if (landmarkData.ContainsKey(currentFrame))
        {
            Vector3[] landmarks = landmarkData[currentFrame];

            mp_middleDot_shoulderpos = (landmarks[11] + landmarks[12]) / 2;

            float mp_dis_0 = Vector3.Distance(new Vector3(0, 1, 0), mp_middleDot_shoulderpos);
            
            float mp_dis_1 = Vector3.Distance(landmarks[11], landmarks[13]);
            float mp_dis_2 = Vector3.Distance(landmarks[13], landmarks[15]);
            float mp_dis_3 = Vector3.Distance(landmarks[12], landmarks[14]);
            float mp_dis_4 = Vector3.Distance(landmarks[14], landmarks[16]);
            
            float mp_dis_5 = Vector3.Distance(landmarks[23], landmarks[25]);
            float mp_dis_6 = Vector3.Distance(landmarks[25], landmarks[29]);
            float mp_dis_7 = Vector3.Distance(landmarks[24], landmarks[26]);
            float mp_dis_8 = Vector3.Distance(landmarks[26], landmarks[30]);


            float dis_diff_0 = model_dis_0 / mp_dis_0;
            
            float dis_diff_1 = model_dis_1 / mp_dis_1;
            float dis_diff_2 = model_dis_2 / mp_dis_2;
            float dis_diff_3 = model_dis_3 / mp_dis_3;
            float dis_diff_4 = model_dis_4 / mp_dis_4;
            
            float dis_diff_5 = model_dis_5 / mp_dis_5;
            float dis_diff_6 = model_dis_6 / mp_dis_6;
            float dis_diff_7 = model_dis_7 / mp_dis_7;
            float dis_diff_8 = model_dis_8 / mp_dis_8;

            middleDot.transform.position = (mp_middleDot_shoulderpos - new Vector3(0, 1, 0)) * dis_diff_0 + Hips.transform.position;

            Left_elbow.transform.position = (landmarks[13] - landmarks[11]) * dis_diff_1 + Left_shoulder.transform.position;
            Left_hand.transform.position = (landmarks[15] - landmarks[13]) * dis_diff_2 + Left_elbow.transform.position;
            Right_elbow.transform.position = (landmarks[14] - landmarks[12]) * dis_diff_3 + Right_shoulder.transform.position;
            Right_hand.transform.position = (landmarks[16] - landmarks[14]) * dis_diff_4 + Right_elbow.transform.position;

            Left_knee.transform.position = (landmarks[25] - landmarks[23]) * dis_diff_5 + Left_hip.transform.position;
            Left_ankle.transform.position = (landmarks[29] - landmarks[25]) * dis_diff_6 + Left_knee.transform.position;
            Right_knee.transform.position = (landmarks[26] - landmarks[24]) * dis_diff_7 + Right_hip.transform.position;
            Right_ankle.transform.position = (landmarks[30] - landmarks[26]) * dis_diff_8 + Right_knee.transform.position;


        }


        currentFrame++;
        if (currentFrame >= totalFlames)
            currentFrame = 0;
    }

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
                totalFlames++;
            }
        }
    }
}
