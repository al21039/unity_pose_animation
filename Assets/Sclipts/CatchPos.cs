using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Animations.Rigging;
/*
public class CatchPos : MonoBehaviour
{
    public string csvFilePath = "Assets/CSV/stretch.csv";
    private Dictionary<int, Vector3[]> landmarkData = new Dictionary<int, Vector3[]>();
    private int currentFrame = 0;
    [SerializeField] GameObject humanoid;
    private Animator animator;
    private int totalFlames = 0;
    [SerializeField] Vector3 forward = new Vector3(0, 1, 0);
    public bool LHandButton;
    public bool RHandButton;
    public bool LFootButton;
    public bool RFootButton;
    Vector3[] LHandPos;
    Vector3[] RHandPos;
    Vector3[] LFootPos;
    Vector3[] RFootPos;
    
    bool disloop = true;
    Vector3[] landmarks;

    LineRenderer LHandlineRenderer;
    LineRenderer RHandlineRenderer;
    LineRenderer LFootlineRenderer;
    LineRenderer RFootlineRenderer;



    // Start is called before the first frame update
    void Start()
    {
        Vector3 movingPos;

        GameObject LHand = new GameObject();
        GameObject RHand = new GameObject();
        GameObject LFoot = new GameObject();
        GameObject RFoot = new GameObject();

        LHandlineRenderer = LHand.AddComponent<LineRenderer>();
        SettingLine(LHandlineRenderer);

        RHandlineRenderer = RHand.AddComponent<LineRenderer>();
        SettingLine(RHandlineRenderer);

        LFootlineRenderer = LFoot.AddComponent<LineRenderer>();
        SettingLine(LFootlineRenderer);

        RFootlineRenderer = RFoot.AddComponent<LineRenderer>();
        SettingLine(RFootlineRenderer);

        animator = humanoid.GetComponent<Animator>();
        LoadLandmarkData();

        LHandPos = new Vector3[totalFlames];
        RHandPos = new Vector3[totalFlames];
        LFootPos = new Vector3[totalFlames];
        RFootPos = new Vector3[totalFlames];
        

        for (int i = 0; i < totalFlames; i++)
        {
            if (landmarkData.ContainsKey(currentFrame))
            {
                Vector3[] landmarks = landmarkData[currentFrame];
                ApplyLandmarksToBones(landmarks);
                movingPos = new Vector3(0, 0, currentFrame * 0.15f);

                LHandPos[i] = animator.GetBoneTransform(HumanBodyBones.LeftHand).transform.position + movingPos;
                RHandPos[i] = animator.GetBoneTransform(HumanBodyBones.RightHand).transform.position + movingPos;
                LFootPos[i] = animator.GetBoneTransform(HumanBodyBones.LeftFoot).transform.position + movingPos;
                RFootPos[i] = animator.GetBoneTransform(HumanBodyBones.RightFoot).transform.position + movingPos;
            }
            currentFrame++;
        }

        GameObject StartPosition = Instantiate(humanoid, new Vector3(0, 0, 0), Quaternion.identity);
        landmarks = landmarkData[0];
        ApplyIdModel(landmarks, StartPosition);
        GameObject EndPosition = Instantiate(humanoid, new Vector3(0, 0, totalFlames * 0.15f), Quaternion.identity);
        landmarks = landmarkData[totalFlames - 1]; 
        GameObject pos1 = Instantiate(humanoid, new Vector3(0, 0, 27 * 0.15f), Quaternion.identity);
        landmarks = landmarkData[27];
        ApplyIdModel(landmarks, pos1);
        
        pos1 = Instantiate(humanoid, new Vector3(0, 0, 39 * 0.15f), Quaternion.identity);
        landmarks = landmarkData[39];
        ApplyIdModel(landmarks, pos1);
        pos1 = Instantiate(humanoid, new Vector3(0, 0, 47 * 0.15f), Quaternion.identity);
        landmarks = landmarkData[47];
        ApplyIdModel(landmarks, pos1);
        pos1 = Instantiate(humanoid, new Vector3(0, 0, 55 * 0.15f), Quaternion.identity);
        landmarks = landmarkData[55];
        ApplyIdModel(landmarks, pos1);
        pos1 = Instantiate(humanoid, new Vector3(0, 0, 60 * 0.15f), Quaternion.identity);
        landmarks = landmarkData[60];
        ApplyIdModel(landmarks, pos1);
        pos1 = Instantiate(humanoid, new Vector3(0, 0, 66 * 0.15f), Quaternion.identity);
        landmarks = landmarkData[66];
        ApplyIdModel(landmarks, pos1);
        pos1 = Instantiate(humanoid, new Vector3(0, 0, 77 * 0.15f), Quaternion.identity);
        landmarks = landmarkData[77];
        ApplyIdModel(landmarks, pos1);
        pos1 = Instantiate(humanoid, new Vector3(0, 0, 82 * 0.15f), Quaternion.identity);
        landmarks = landmarkData[82];
        ApplyIdModel(landmarks, pos1);
        



        LHandButton = true;
        RHandButton = false;
        LFootButton = false;
        RFootButton = false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
*/