using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateAnimationFromCSV : MonoBehaviour
{
    [SerializeField] private TextAsset csvFile; // �C���X�y�N�^�[�Őݒ肷��CSV�t�@�C��
    [SerializeField] private Animator animator; // �Ώۂ�Animator

    private HumanPoseHandler poseHandler;
    private HumanPose humanPose;

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned.");
            return;
        }

        if (csvFile == null)
        {
            Debug.LogError("CSV file is not assigned.");
            return;
        }

        // HumanPoseHandler��������
        poseHandler = new HumanPoseHandler(animator.avatar, animator.transform);

        // CSV�t�@�C����ǂݎ��
        List<HumanPose> humanPoses = ReadCSVToHumanPoses(csvFile.text);

        AnimationClip animclip = new AnimationClip();
        AnimationCurve[] humanPoseMuscleCurves = new AnimationCurve[HumanTrait.MuscleCount]; //���ׂẴ}�b�X���l�̐��@�z��Ŋm��
        AnimationCurve humanPosePositionCurve = new AnimationCurve();
        AnimationCurve humanPoseRotationCurve = new AnimationCurve();

        AnimationCurve rootTX = new AnimationCurve();
        AnimationCurve rootTY = new AnimationCurve();
        AnimationCurve rootTZ = new AnimationCurve();

        AnimationCurve rootQX = new AnimationCurve();
        AnimationCurve rootQY = new AnimationCurve();
        AnimationCurve rootQZ = new AnimationCurve();
        AnimationCurve rootQW = new AnimationCurve();

        for (int i = 0; i < humanPoseMuscleCurves.Length; i++)
        {
            humanPoseMuscleCurves[i] = new AnimationCurve(); //�A�j���[�V�����J�[�u������
        }

        float count = 0;
        float time = count / 30;

        // �f�o�b�O: �ǂݎ�����|�[�Y���m�F
        foreach (var pose in humanPoses)
        {
            time = count / 30;

            for (int j = 0; j < pose.muscles.Length; j++)
            {
                humanPoseMuscleCurves[j].AddKey(time, pose.muscles[j]);
            }

            rootQX.AddKey(time, pose.bodyRotation.x);
            rootQY.AddKey(time, pose.bodyRotation.y);
            rootQZ.AddKey(time, pose.bodyRotation.z);
            rootQW.AddKey(time, pose.bodyRotation.w);

            rootTX.AddKey(time, pose.bodyPosition.x);
            rootTY.AddKey(time, pose.bodyPosition.y);
            rootTZ.AddKey(time, pose.bodyPosition.z);

            count++;
        }

        for (int i = 0; i < humanPoseMuscleCurves.Length; i++)
        {
            string muscleName = HumanTrait.MuscleName[i];
            animclip.SetCurve("", typeof(Animator), muscleName, humanPoseMuscleCurves[i]);
        }

        animclip.SetCurve("", typeof(Animator), "RootT.x", rootTX);
        animclip.SetCurve("", typeof(Animator), "RootT.y", rootTY);
        animclip.SetCurve("", typeof(Animator), "RootT.z", rootTZ);
        animclip.SetCurve("", typeof(Animator), "RootQ.x", rootQX);
        animclip.SetCurve("", typeof(Animator), "RootQ.y", rootQY);
        animclip.SetCurve("", typeof(Animator), "RootQ.z", rootQZ);
        animclip.SetCurve("", typeof(Animator), "RootQ.w", rootQW);

        /*
        //�A�j���[�V�����𐧍�
        AssetDatabase.CreateAsset(animclip, AssetDatabase.GenerateUniqueAssetPath("Assets/kick.anim"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        */

        AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;

        overrideController["DefaultHumanoidAnimation"] = animclip;
        animator.Play("DefaultHumanoidAnimation");

        
        
        
        
    }

    List<HumanPose> ReadCSVToHumanPoses(string csvText)
    {
        var humanPoses = new List<HumanPose>();

        try
        {
            string[] lines = csvText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] values = line.Split(',');

                if (values.Length < 3 + 3 + HumanTrait.MuscleCount)
                {
                    Debug.LogWarning("Invalid CSV format.");
                    continue;
                }

                var pose = new HumanPose();

                pose.bodyPosition = new Vector3(
                    float.Parse(values[0]),
                    float.Parse(values[1]),
                    float.Parse(values[2])
                );

                Quaternion eulerRotation = new Quaternion(
                    float.Parse(values[3]),
                    float.Parse(values[4]),
                    float.Parse(values[5]),
                    float.Parse(values[6])
                );
                pose.bodyRotation = eulerRotation;

                pose.muscles = new float[HumanTrait.MuscleCount];
                for (int i = 0; i < HumanTrait.MuscleCount; i++)
                {
                    pose.muscles[i] = float.Parse(values[7 + i]);
                }

                humanPoses.Add(pose);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to parse CSV: " + e.Message);
        }

        return humanPoses;
    }
}
