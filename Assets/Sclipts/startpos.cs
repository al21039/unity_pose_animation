using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class startpos : MonoBehaviour
{
    public string csvFilePath = "Assets/CSV/stretch.csv";
    private Dictionary<int, Vector3[]> landmarkData = new Dictionary<int, Vector3[]>();
    private int currentFrame = 0;
    private Animator animator;
    private int totalFlames = 0;
    [SerializeField] Vector3 forward;

    void Start()
    {
        Application.targetFrameRate = 30;
        animator = GetComponent<Animator>();
        LoadLandmarkData();
        
    }

    void Update()
    {

        if (landmarkData.ContainsKey(currentFrame))
        {
            Vector3[] landmarks = landmarkData[0];
            ApplyLandmarksToBones(landmarks);
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



}
