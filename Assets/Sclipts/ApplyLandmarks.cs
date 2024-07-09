using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ApplyLandmarks : MonoBehaviour
{
    //public string csvFilePath = "Assets/CSV/stretch5.csv";
    private Dictionary<int, Vector3[]> landmarkData = new Dictionary<int, Vector3[]>();
    private int currentFrame = 0;
    private Animator animator;
    [SerializeField] 
    GameObject start;
    [SerializeField]
    GameObject end;

    bool check = true;

    void Start()
    {
        Application.targetFrameRate = 30;
        animator = GetComponent<Animator>();
        //LoadLandmarkData();
    }

    void Update()
    {
        /*
        if (landmarkData.ContainsKey(currentFrame))
        {
            Vector3[] landmarks = landmarkData[currentFrame];
            ApplyLandmarksToBones(landmarks);
        }

        currentFrame++;
        */

        if (check) { 
            Transform arm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            var diff = end.transform.position - start.transform.position;
            arm.rotation = Quaternion.LookRotation(diff);
            check = false;
        }
    }

    /*
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
                    landmarks[i] = new Vector3(x, y, z);
                }

                landmarkData[frame] = landmarks;
            }
        }
    }
    */

    void ApplyLandmarksToBones(Vector3[] landmarks)
    {
        if (landmarks.Length < 33) return;

        // Set rotations
        SetBoneRotation(HumanBodyBones.LeftUpperArm, landmarks[11], landmarks[13]);
        SetBoneRotation(HumanBodyBones.RightUpperArm, landmarks[12], landmarks[14]);
        SetBoneRotation(HumanBodyBones.LeftLowerArm, landmarks[13], landmarks[15]);
        SetBoneRotation(HumanBodyBones.RightLowerArm, landmarks[14], landmarks[16]);
        SetBoneRotation(HumanBodyBones.LeftHand, landmarks[15], landmarks[19]);
        SetBoneRotation(HumanBodyBones.LeftHand, landmarks[16], landmarks[20]);
        SetBoneRotation(HumanBodyBones.LeftUpperLeg, landmarks[23], landmarks[25]);
        SetBoneRotation(HumanBodyBones.RightUpperLeg, landmarks[24], landmarks[26]);
        SetBoneRotation(HumanBodyBones.LeftLowerLeg, landmarks[25], landmarks[27]);
        SetBoneRotation(HumanBodyBones.RightLowerLeg, landmarks[26], landmarks[28]);
        SetBoneRotation(HumanBodyBones.LeftFoot, landmarks[27], landmarks[29]);
        SetBoneRotation(HumanBodyBones.RightFoot, landmarks[28], landmarks[30]);


    }

    void SetBoneRotation(HumanBodyBones bone, Vector3 start, Vector3 end)
    {
        Transform boneTransform = animator.GetBoneTransform(bone);
        var diff = end - start;
        boneTransform.rotation = Quaternion.LookRotation(diff);
    }
}
