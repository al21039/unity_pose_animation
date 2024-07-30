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
                movingPos = new Vector3(0, 0, currentFrame * 0.07f);

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
        GameObject EndPosition = Instantiate(humanoid, new Vector3(0, 0, totalFlames * 0.07f), Quaternion.identity);
        landmarks = landmarkData[totalFlames - 1];
        ApplyIdModel(landmarks, EndPosition);
        GameObject KeyFlamePos = Instantiate(humanoid, new Vector3(0, 0, 50 * 0.07f), Quaternion.identity);
        landmarks = landmarkData[50];
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

    void SettingLine(LineRenderer line)
    {
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.white;
        line.endColor = Color.white;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = 0;
        line.numCapVertices = 10;
        line.numCornerVertices = 10;
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
        //������������
        SetBoneRotation(HumanBodyBones.Head, (landmarks[11] + landmarks[12]) / 2, landmarks[0]); //��
        
        //��]�𐧌�����K�v����
        SetBoneRotation(HumanBodyBones.Spine, new Vector3(0, 0, 0), (landmarks[11] + landmarks[12]) / 2); //�Ғ�
        */

        //SetBoneRotation(HumanBodyBones.Hips, new Vector3(0, 0, 0), sholuderMiddle);


        SetBoneRotation(HumanBodyBones.LeftUpperArm, landmarks[11], landmarks[13]); //�� ��r
        SetBoneRotation(HumanBodyBones.RightUpperArm, landmarks[12], landmarks[14]); //�E ��r
        SetBoneRotation(HumanBodyBones.LeftLowerArm, landmarks[13], landmarks[15]); //�� ��̘r
        SetBoneRotation(HumanBodyBones.RightLowerArm, landmarks[14], landmarks[16]); //�E ��̘r
        SetBoneRotation(HumanBodyBones.LeftUpperLeg, landmarks[23], landmarks[25]); //�� ������
        SetBoneRotation(HumanBodyBones.RightUpperLeg, landmarks[24], landmarks[26]); //�E ������
        SetBoneRotation(HumanBodyBones.LeftLowerLeg, landmarks[25], landmarks[27]); //�� ��
        SetBoneRotation(HumanBodyBones.RightLowerLeg, landmarks[26], landmarks[28]); //�E ��
        SetBoneRotation(HumanBodyBones.LeftFoot, landmarks[27], landmarks[31]); //�� ����
        SetBoneRotation(HumanBodyBones.RightFoot, landmarks[28], landmarks[32]); //�E ����


        SetBoneRotation(HumanBodyBones.LeftShoulder, sholuderMiddle, landmarks[11]); //�� ��
        SetBoneRotation(HumanBodyBones.RightShoulder, sholuderMiddle, landmarks[12]); //�E ��


        //��ɂ��� HandTracking�𗘗p����K�v����
        /*
        SetBoneRotation(HumanBodyBones.LeftLittleProximal, landmarks[15], landmarks[17]); //�� �e�w
        SetBoneRotation(HumanBodyBones.LeftIndexProximal, landmarks[15], landmarks[19]); //�� �l�����w
        SetBoneRotation(HumanBodyBones.LeftThumbProximal, landmarks[15], landmarks[21]); //�� ���w
                                                                                             
        SetBoneRotation(HumanBodyBones.RightLittleProximal, landmarks[16], landmarks[18]); //�� �e�w
        SetBoneRotation(HumanBodyBones.RightIndexProximal, landmarks[16], landmarks[20]); //�� �l�����w
        SetBoneRotation(HumanBodyBones.RightThumbProximal, landmarks[16], landmarks[22]); //�� ���w
        */


    }

    //�e�{�[���ɉ�]��������
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
        //������������
        SetBoneRotation(HumanBodyBones.Head, (landmarks[11] + landmarks[12]) / 2, landmarks[0]); //��
        
        //��]�𐧌�����K�v����
        SetBoneRotation(HumanBodyBones.Spine, new Vector3(0, 0, 0), (landmarks[11] + landmarks[12]) / 2); //�Ғ�
        */

        //SetBoneRotation(HumanBodyBones.Hips, new Vector3(0, 0, 0), sholuderMiddle);


        SetRotationIdModel(HumanBodyBones.LeftUpperArm, landmarks[11], landmarks[13], humanoid); //�� ��r
        SetRotationIdModel(HumanBodyBones.RightUpperArm, landmarks[12], landmarks[14], humanoid); //�E ��r
        SetRotationIdModel(HumanBodyBones.LeftLowerArm, landmarks[13], landmarks[15], humanoid); //�� ��̘r
        SetRotationIdModel(HumanBodyBones.RightLowerArm, landmarks[14], landmarks[16], humanoid); //�E ��̘r
        SetRotationIdModel(HumanBodyBones.LeftUpperLeg, landmarks[23], landmarks[25], humanoid); //�� ������
        SetRotationIdModel(HumanBodyBones.RightUpperLeg, landmarks[24], landmarks[26], humanoid); //�E ������
        SetRotationIdModel(HumanBodyBones.LeftLowerLeg, landmarks[25], landmarks[27], humanoid); //�� ��
        SetRotationIdModel(HumanBodyBones.RightLowerLeg, landmarks[26], landmarks[28], humanoid); //�E ��
        SetRotationIdModel(HumanBodyBones.LeftFoot, landmarks[27], landmarks[31], humanoid); //�� ����
        SetRotationIdModel(HumanBodyBones.RightFoot, landmarks[28], landmarks[32], humanoid); //�E ����


        SetRotationIdModel(HumanBodyBones.LeftShoulder, sholuderMiddle, landmarks[11], humanoid); //�� ��
        SetRotationIdModel(HumanBodyBones.RightShoulder, sholuderMiddle, landmarks[12], humanoid); //�E ��


        //��ɂ��� HandTracking�𗘗p����K�v����
        /*
        SetBoneRotation(HumanBodyBones.LeftLittleProximal, landmarks[15], landmarks[17]); //�� �e�w
        SetBoneRotation(HumanBodyBones.LeftIndexProximal, landmarks[15], landmarks[19]); //�� �l�����w
        SetBoneRotation(HumanBodyBones.LeftThumbProximal, landmarks[15], landmarks[21]); //�� ���w
                                                                                             
        SetBoneRotation(HumanBodyBones.RightLittleProximal, landmarks[16], landmarks[18]); //�� �e�w
        SetBoneRotation(HumanBodyBones.RightIndexProximal, landmarks[16], landmarks[20]); //�� �l�����w
        SetBoneRotation(HumanBodyBones.RightThumbProximal, landmarks[16], landmarks[22]); //�� ���w
        */


    }

    //�e�{�[���ɉ�]��������
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