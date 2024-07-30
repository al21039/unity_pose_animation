using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ApplyIK : MonoBehaviour
{
    public string csvFilePath = "Assets/CSV/stretch.csv";
    private Dictionary<int, Vector3[]> landmarkData = new Dictionary<int, Vector3[]>();
    private int currentFrame = 0;
    private Animator animator;
    private int totalFlames = 0;
    [SerializeField] Vector3 forward = new Vector3(0, 1, 0);

    [SerializeField] GameObject Neck;
    [SerializeField] GameObject Hip;
    [SerializeField] GameObject RShoulder;
    [SerializeField] GameObject LShoulder;
    [SerializeField] GameObject LHip;
    [SerializeField] GameObject RHip;
    [SerializeField] GameObject RHand;
    [SerializeField] GameObject LHand;
    [SerializeField] GameObject LFoot;
    [SerializeField] GameObject RFoot;
    [SerializeField] GameObject RElbow;
    [SerializeField] GameObject LElbow;
    [SerializeField] GameObject LKnee;
    [SerializeField] GameObject RKnee;

    float model_HipToNeck_dis;
    float model_LShoulderToElbow_dis;
    float model_RShoulderToElbow_dis;
    float model_LElbowToHand_dis;
    float model_RElbowToHand_dis;
    float model_LHipToKnee_dis;
    float model_RHipToKnee_dis;
    float model_LKneeToFoot_dis;
    float model_RKneeToFoot_dis;


    float dis_corr = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 middleShoulder = (LShoulder.transform.position + RShoulder.transform.position) / 2;
        model_HipToNeck_dis = Vector3.Distance(middleShoulder, Hip.transform.position);
        model_LShoulderToElbow_dis = Vector3.Distance(LShoulder.transform.position, LElbow.transform.position);
        model_RShoulderToElbow_dis = Vector3.Distance(RShoulder.transform.position, RElbow.transform.position);
        model_LElbowToHand_dis = Vector3.Distance(LElbow.transform.position, LHand.transform.position);
        model_RElbowToHand_dis = Vector3.Distance(RElbow.transform.position, RHand.transform.position);
        model_LHipToKnee_dis = Vector3.Distance(LHip.transform.position, LKnee.transform.position);
        model_RHipToKnee_dis = Vector3.Distance(RHip.transform.position, RKnee.transform.position);
        model_LKneeToFoot_dis = Vector3.Distance(LKnee.transform.position, LFoot.transform.position);
        model_RKneeToFoot_dis = Vector3.Distance(RKnee.transform.position, RFoot.transform.position);


        Application.targetFrameRate = 30;
        LoadLandmarkData();
    }

    // Update is called once per frame
    void Update()
    {
        if(landmarkData.ContainsKey(currentFrame))
        {
            Vector3[] landmarks = landmarkData[currentFrame];
            ApplyLandmarksToIK(landmarks);
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
                    landmarks[i] = new Vector3(-x,  -y + 1, -z);
                }

                landmarkData[frame] = landmarks;
                totalFlames++;
            }
        }
    }

    void ApplyLandmarksToIK(Vector3[] landmarks)
    {
        Vector3 land_middleShoulder = (landmarks[11] + landmarks[12]) / 2;


        float land_HN_dis = Vector3.Distance(land_middleShoulder, new Vector3(0, 0, 0));

        dis_corr = model_HipToNeck_dis / land_HN_dis;

        Neck.transform.position = Hip.transform.position + (land_middleShoulder - new Vector3(0, 0, 0)) * dis_corr;

        float land_LSE_dis = Vector3.Distance(landmarks[11], landmarks[13]);
        

        dis_corr = model_LShoulderToElbow_dis / land_LSE_dis;

        LElbow.transform.position = LShoulder.transform.position + (landmarks[13] - landmarks[11]) * dis_corr;

        float land_RSE_dis = Vector3.Distance(landmarks[12], landmarks[14]);

        dis_corr = model_RShoulderToElbow_dis / land_RSE_dis;

        RElbow.transform.position = RShoulder.transform.position + (landmarks[14] - landmarks[12]) * dis_corr;

        float land_LEH_dis = Vector3.Distance(landmarks[13], landmarks[15]);

        dis_corr = model_LElbowToHand_dis / land_LEH_dis;

        LHand.transform.position = LElbow.transform.position + (landmarks[15] - landmarks[13]) * dis_corr;

        float land_REH_dis = Vector3.Distance(landmarks[14], landmarks[16]);

        dis_corr = model_RElbowToHand_dis / land_REH_dis;

        RHand.transform.position = RElbow.transform.position + (landmarks[16] - landmarks[14]) * dis_corr;

        float land_LHK_dis = Vector3.Distance(landmarks[23], landmarks[25]);

        dis_corr = model_LHipToKnee_dis / land_LHK_dis;

        LKnee.transform.position = LHip.transform.position + (landmarks[25] - landmarks[23]) * dis_corr;

        float land_RHK_dis = Vector3.Distance(landmarks[24], landmarks[26]);

        dis_corr = model_RHipToKnee_dis / land_LHK_dis;

        RKnee.transform.position = RHip.transform.position + (landmarks[26] - landmarks[24]) * dis_corr;

        float land_LKF_dis = Vector3.Distance(landmarks[25], landmarks[27]);

        dis_corr = model_LKneeToFoot_dis / land_LKF_dis;
        
        LFoot.transform.position = LKnee.transform.position + (landmarks[27] - landmarks[25]) * dis_corr;

        float land_RKF_dis = Vector3.Distance(landmarks[26], landmarks[28]);

        dis_corr = model_RKneeToFoot_dis / land_RKF_dis;

        RFoot.transform.position = RKnee.transform.position + (landmarks[28] - landmarks[26]) * dis_corr;


        
        
    }
}
