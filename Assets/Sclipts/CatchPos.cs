using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CatchPos : MonoBehaviour
{
    public string csvFilePath = "Assets/CSV/stretch.csv";
    private Dictionary<int, Vector3[]> landmarkData = new Dictionary<int, Vector3[]>();
    private int currentFrame = 0;
    [SerializeField] GameObject humanoid;
    public Material material;
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
        LHandlineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        LHandlineRenderer.startColor = Color.white;
        LHandlineRenderer.endColor = Color.white;
        LHandlineRenderer.startWidth = 0.1f;
        LHandlineRenderer.endWidth = 0.1f;
        LHandlineRenderer.positionCount = 0;
        LHandlineRenderer.numCapVertices = 10;
        LHandlineRenderer.numCornerVertices = 10;

        RHandlineRenderer = RHand.AddComponent<LineRenderer>();
        RHandlineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        RHandlineRenderer.startColor = Color.white;
        RHandlineRenderer.endColor = Color.white;
        RHandlineRenderer.startWidth = 0.1f;
        RHandlineRenderer.endWidth = 0.1f;
        RHandlineRenderer.positionCount = 0;
        RHandlineRenderer.numCapVertices = 10;
        RHandlineRenderer.numCornerVertices = 10;

        LFootlineRenderer = LFoot.AddComponent<LineRenderer>();
        LFootlineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        LFootlineRenderer.startColor = Color.white;
        LFootlineRenderer.endColor = Color.white;
        LFootlineRenderer.startWidth = 0.1f;
        LFootlineRenderer.endWidth = 0.1f;
        LFootlineRenderer.positionCount = 0;
        LFootlineRenderer.numCapVertices = 10;
        LFootlineRenderer.numCornerVertices = 10;

        RFootlineRenderer = RFoot.AddComponent<LineRenderer>();
        RFootlineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        RFootlineRenderer.startColor = Color.white;
        RFootlineRenderer.endColor = Color.white;
        RFootlineRenderer.startWidth = 0.1f;
        RFootlineRenderer.endWidth = 0.1f;
        RFootlineRenderer.positionCount = 0;
        RFootlineRenderer.numCapVertices = 10;
        RFootlineRenderer.numCornerVertices = 10;

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
                movingPos = new Vector3(0, 0, currentFrame * 0.05f);

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
        GameObject EndPosition = Instantiate(humanoid, new Vector3(0, 0, totalFlames * 0.05f), Quaternion.identity);
        landmarks = landmarkData[totalFlames - 1];
        ApplyIdModel(landmarks, EndPosition);
        GameObject KeyFlamePos = Instantiate(humanoid, new Vector3(0, 0, 60 * 0.05f), Quaternion.identity);
        landmarks = landmarkData[60];
        ApplyIdModel(landmarks, KeyFlamePos);

        LHandButton = true;
        RHandButton = false;
        LFootButton = false;
        RFootButton = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (LHandButton)
        {
            LHandlineRenderer.positionCount = LHandPos.Length;
            LHandlineRenderer.SetPositions(LHandPos);
        }
        else
        {
            LHandlineRenderer.positionCount = 0;
        }

        if (RHandButton)
        {
            RHandlineRenderer.positionCount = RHandPos.Length;
            RHandlineRenderer.SetPositions(RHandPos);
        }
        else
        {
            RHandlineRenderer.positionCount = 0;
        }

        if(LFootButton)
        {
            LFootlineRenderer.positionCount = LFootPos.Length;
            LFootlineRenderer.SetPositions(LFootPos);
        }
        else
        {
            LFootlineRenderer.positionCount = 0;
        }

        if(RFootButton)
        {
            RFootlineRenderer.positionCount = RFootPos.Length;
            RFootlineRenderer.SetPositions(RFootPos);
        } 
        else
        {
            RFootlineRenderer.positionCount = 0;
        }
        
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
                    landmarks[i] = new Vector3(-x, -y, -z);
                }

                landmarkData[frame] = landmarks;
                totalFlames++;
            }
        }
    }



    void ApplyLandmarksToBones(Vector3[] landmarks)
    {
        if (landmarks.Length < 33) return;


        Vector3 sholuderMiddle = (landmarks[11] + landmarks[12]) / 2;

        /*
        //挙動おかしい
        SetBoneRotation(HumanBodyBones.Head, (landmarks[11] + landmarks[12]) / 2, landmarks[0]); //頭
        
        //回転を制限する必要あり
        SetBoneRotation(HumanBodyBones.Spine, new Vector3(0, 0, 0), (landmarks[11] + landmarks[12]) / 2); //脊椎
        */

        //SetBoneRotation(HumanBodyBones.Hips, new Vector3(0, 0, 0), sholuderMiddle);


        SetBoneRotation(HumanBodyBones.LeftUpperArm, landmarks[11], landmarks[13]); //左 上腕
        SetBoneRotation(HumanBodyBones.RightUpperArm, landmarks[12], landmarks[14]); //右 上腕
        SetBoneRotation(HumanBodyBones.LeftLowerArm, landmarks[13], landmarks[15]); //左 二の腕
        SetBoneRotation(HumanBodyBones.RightLowerArm, landmarks[14], landmarks[16]); //右 二の腕
        SetBoneRotation(HumanBodyBones.LeftUpperLeg, landmarks[23], landmarks[25]); //左 太もも
        SetBoneRotation(HumanBodyBones.RightUpperLeg, landmarks[24], landmarks[26]); //右 太もも
        SetBoneRotation(HumanBodyBones.LeftLowerLeg, landmarks[25], landmarks[27]); //左 脛
        SetBoneRotation(HumanBodyBones.RightLowerLeg, landmarks[26], landmarks[28]); //右 脛
        SetBoneRotation(HumanBodyBones.LeftFoot, landmarks[27], landmarks[31]); //左 足首
        SetBoneRotation(HumanBodyBones.RightFoot, landmarks[28], landmarks[32]); //右 足首


        SetBoneRotation(HumanBodyBones.LeftShoulder, sholuderMiddle, landmarks[11]); //左 肩
        SetBoneRotation(HumanBodyBones.RightShoulder, sholuderMiddle, landmarks[12]); //右 肩


        //手について HandTrackingを利用する必要あり
        /*
        SetBoneRotation(HumanBodyBones.LeftLittleProximal, landmarks[15], landmarks[17]); //左 親指
        SetBoneRotation(HumanBodyBones.LeftIndexProximal, landmarks[15], landmarks[19]); //左 人差し指
        SetBoneRotation(HumanBodyBones.LeftThumbProximal, landmarks[15], landmarks[21]); //左 小指
                                                                                             
        SetBoneRotation(HumanBodyBones.RightLittleProximal, landmarks[16], landmarks[18]); //左 親指
        SetBoneRotation(HumanBodyBones.RightIndexProximal, landmarks[16], landmarks[20]); //左 人差し指
        SetBoneRotation(HumanBodyBones.RightThumbProximal, landmarks[16], landmarks[22]); //左 小指
        */


    }

    //各ボーンに回転を加える
    void SetBoneRotation(HumanBodyBones bone, Vector3 start, Vector3 end)
    {
        if (animator.GetBoneTransform(bone) != null)
        {
            Transform boneTransform = animator.GetBoneTransform(bone);
            var dir = end - start;
            var lookAtRotation = Quaternion.LookRotation(dir, Vector3.up);
            var offsetRotation = Quaternion.FromToRotation(forward, Vector3.forward);
            boneTransform.rotation = lookAtRotation * offsetRotation;
        }
    }

    void ApplyIdModel(Vector3[] landmarks, GameObject humanoid)
    {
        if (landmarks.Length < 33) return;


        Vector3 sholuderMiddle = (landmarks[11] + landmarks[12]) / 2;

        /*
        //挙動おかしい
        SetBoneRotation(HumanBodyBones.Head, (landmarks[11] + landmarks[12]) / 2, landmarks[0]); //頭
        
        //回転を制限する必要あり
        SetBoneRotation(HumanBodyBones.Spine, new Vector3(0, 0, 0), (landmarks[11] + landmarks[12]) / 2); //脊椎
        */

        //SetBoneRotation(HumanBodyBones.Hips, new Vector3(0, 0, 0), sholuderMiddle);


        SetRotationIdModel(HumanBodyBones.LeftUpperArm, landmarks[11], landmarks[13], humanoid); //左 上腕
        SetRotationIdModel(HumanBodyBones.RightUpperArm, landmarks[12], landmarks[14], humanoid); //右 上腕
        SetRotationIdModel(HumanBodyBones.LeftLowerArm, landmarks[13], landmarks[15], humanoid); //左 二の腕
        SetRotationIdModel(HumanBodyBones.RightLowerArm, landmarks[14], landmarks[16], humanoid); //右 二の腕
        SetRotationIdModel(HumanBodyBones.LeftUpperLeg, landmarks[23], landmarks[25], humanoid); //左 太もも
        SetRotationIdModel(HumanBodyBones.RightUpperLeg, landmarks[24], landmarks[26], humanoid); //右 太もも
        SetRotationIdModel(HumanBodyBones.LeftLowerLeg, landmarks[25], landmarks[27], humanoid); //左 脛
        SetRotationIdModel(HumanBodyBones.RightLowerLeg, landmarks[26], landmarks[28], humanoid); //右 脛
        SetRotationIdModel(HumanBodyBones.LeftFoot, landmarks[27], landmarks[31], humanoid); //左 足首
        SetRotationIdModel(HumanBodyBones.RightFoot, landmarks[28], landmarks[32], humanoid); //右 足首


        SetRotationIdModel(HumanBodyBones.LeftShoulder, sholuderMiddle, landmarks[11], humanoid); //左 肩
        SetRotationIdModel(HumanBodyBones.RightShoulder, sholuderMiddle, landmarks[12], humanoid); //右 肩


        //手について HandTrackingを利用する必要あり
        /*
        SetBoneRotation(HumanBodyBones.LeftLittleProximal, landmarks[15], landmarks[17]); //左 親指
        SetBoneRotation(HumanBodyBones.LeftIndexProximal, landmarks[15], landmarks[19]); //左 人差し指
        SetBoneRotation(HumanBodyBones.LeftThumbProximal, landmarks[15], landmarks[21]); //左 小指
                                                                                             
        SetBoneRotation(HumanBodyBones.RightLittleProximal, landmarks[16], landmarks[18]); //左 親指
        SetBoneRotation(HumanBodyBones.RightIndexProximal, landmarks[16], landmarks[20]); //左 人差し指
        SetBoneRotation(HumanBodyBones.RightThumbProximal, landmarks[16], landmarks[22]); //左 小指
        */


    }

    //各ボーンに回転を加える
    void SetRotationIdModel(HumanBodyBones bone, Vector3 start, Vector3 end, GameObject humanoid)
    {
        if (humanoid.GetComponent<Animator>().GetBoneTransform(bone) != null)
        {
            Transform boneTransform = humanoid.GetComponent<Animator>().GetBoneTransform(bone);
            var dir = end - start;
            var lookAtRotation = Quaternion.LookRotation(dir, Vector3.up);
            var offsetRotation = Quaternion.FromToRotation(forward, Vector3.forward);
            boneTransform.rotation = lookAtRotation * offsetRotation;

            //LHandPos.Add
        }
    }
}
