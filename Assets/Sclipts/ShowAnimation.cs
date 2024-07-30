using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShowAnimation : MonoBehaviour
{
    public string csvFilePath = "Assets/CSV/stretch.csv";
    private Dictionary<int, Vector3[]> landmarkData = new Dictionary<int, Vector3[]>();
    public int currentFrame = 0;
    [SerializeField] GameObject humanoid;
    private Animator animator;
    private int totalFlames = 0;
    [SerializeField] Vector3 forward = new Vector3(0, 1, 0);
    GameObject go;

    public bool show;
    public bool check;


    // Start is called before the first frame update
    void Start()
    {
        show = false;
        check = false;
        Application.targetFrameRate = 30;
        animator = humanoid.GetComponent<Animator>();
        LoadLandmarkData();
    }

    // Update is called once per frame
    void Update()
    {
        if(show)
        {
            go = Instantiate(humanoid, new Vector3(0, 0, 0), Quaternion.identity);
            show = false;
        }

        if(go)
        {
            if (check)
            {
                if (landmarkData.ContainsKey(currentFrame))
                {
                    Vector3[] landmarks = landmarkData[currentFrame];
                    ApplyLandmarksToBones(landmarks, go);
                    go.transform.Translate(0f, 0f, 0.07f);
                }

                currentFrame++;
                if (currentFrame >= totalFlames)
                {
                    check = false;
                    Destroy(go);
                }
            }
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



    void ApplyLandmarksToBones(Vector3[] landmarks, GameObject humanoid)
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


        SetBoneRotation(HumanBodyBones.LeftUpperArm, landmarks[11], landmarks[13], humanoid); //�� ��r
        SetBoneRotation(HumanBodyBones.RightUpperArm, landmarks[12], landmarks[14], humanoid); //�E ��r
        SetBoneRotation(HumanBodyBones.LeftLowerArm, landmarks[13], landmarks[15], humanoid); //�� ��̘r
        SetBoneRotation(HumanBodyBones.RightLowerArm, landmarks[14], landmarks[16], humanoid); //�E ��̘r
        SetBoneRotation(HumanBodyBones.LeftUpperLeg, landmarks[23], landmarks[25], humanoid); //�� ������
        SetBoneRotation(HumanBodyBones.RightUpperLeg, landmarks[24], landmarks[26], humanoid); //�E ������
        SetBoneRotation(HumanBodyBones.LeftLowerLeg, landmarks[25], landmarks[27], humanoid); //�� ��
        SetBoneRotation(HumanBodyBones.RightLowerLeg, landmarks[26], landmarks[28], humanoid); //�E ��
        SetBoneRotation(HumanBodyBones.LeftFoot, landmarks[27], landmarks[31], humanoid); //�� ����
        SetBoneRotation(HumanBodyBones.RightFoot, landmarks[28], landmarks[32], humanoid); //�E ����


        SetBoneRotation(HumanBodyBones.LeftShoulder, sholuderMiddle, landmarks[11], humanoid); //�� ��
        SetBoneRotation(HumanBodyBones.RightShoulder, sholuderMiddle, landmarks[12], humanoid); //�E ��


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
    void SetBoneRotation(HumanBodyBones bone, Vector3 start, Vector3 end, GameObject humanoid)
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
