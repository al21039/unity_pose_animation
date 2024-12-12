using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateAnimationFromCSV : MonoBehaviour
{
    [SerializeField] private TextAsset csvFile; // インスペクターで設定するCSVファイル
    [SerializeField] private Animator animator; // 対象のAnimator

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

        // HumanPoseHandlerを初期化
        poseHandler = new HumanPoseHandler(animator.avatar, animator.transform);

        // CSVファイルを読み取り
        List<HumanPose> humanPoses = ReadCSVToHumanPoses(csvFile.text);

        AnimationClip animclip = new AnimationClip();
        AnimationCurve[] humanPoseMuscleCurves = new AnimationCurve[HumanTrait.MuscleCount]; //すべてのマッスル値の数　配列で確保
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
            humanPoseMuscleCurves[i] = new AnimationCurve(); //アニメーションカーブ初期化
        }

        float count = 0;
        float time = count / 30;

        // デバッグ: 読み取ったポーズを確認
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
            if (i == 55) 
            {
                muscleName = "LeftHand.Thumb.1 Stretched";
            }
            else if (i == 57)
            {
                muscleName = "LeftHand.Thumb.2 Stretched";
            }
            else if (i == 58)
            {
                muscleName = "LeftHand.Thumb.3 Stretched";
            }
            else if (i == 59)
            {
                muscleName = "LeftHand.Index.1 Stretched";
            }
            else if (i == 61)
            {
                muscleName = "LeftHand.Index.2 Stretched";
            }
            else if (i == 62)
            {
                muscleName = "LeftHand.Index.3 Stretched";
            }
            else if (i == 63)
            {
                muscleName = "LeftHand.Middle.1 Stretched";
            }
            else if (i == 65)
            {
                muscleName = "LeftHand.Middle.2 Stretched";
            }
            else if (i == 66)
            {
                muscleName = "LeftHand.Middle.3 Stretched";
            }
            else if (i == 67)
            {
                muscleName = "LeftHand.Ring.1 Stretched";
            }
            else if (i == 69)
            {
                muscleName = "LeftHand.Ring.2 Stretched";
            }
            else if (i == 70)
            {
                muscleName = "LeftHand.Ring.3 Stretched";
            }
            else if (i == 71)
            {
                muscleName = "LeftHand.Little.1 Stretched";
            }
            else if (i == 73)
            {
                muscleName = "LeftHand.Little.2 Stretched";
            }
            else if (i == 74)
            {
                muscleName = "LeftHand.Little.3 Stretched";
            }
            else if (i == 75)
            {
                muscleName = "RightHand.Thumb.1 Stretched";
            }
            else if (i == 77)
            {
                muscleName = "RightHand.Thumb.2 Stretched";
            }
            else if (i == 78)
            {
                muscleName = "RightHand.Thumb.3 Stretched";
            }
            else if (i == 79)
            {
                muscleName = "RightHand.Index.1 Stretched";
            }
            else if (i == 81)
            {
                muscleName = "RightHand.Index.2 Stretched";
            }
            else if (i == 82)
            {
                muscleName = "RightHand.Index.3 Stretched";
            }
            else if (i == 83)
            {
                muscleName = "RightHand.Middle.1 Stretched";
            }
            else if (i == 85)
            {
                muscleName = "RightHand.Middle.2 Stretched";
            }
            else if (i == 86)
            {
                muscleName = "RightHand.Middle.3 Stretched";
            }
            else if (i == 87)
            {
                muscleName = "RightHand.Ring.1 Stretched";
            }
            else if (i == 89)
            {
                muscleName = "RightHand.Ring.2 Stretched";
            }
            else if (i == 90)
            {
                muscleName = "RightHand.Ring.3 Stretched";
            }
            else if (i == 91)
            {
                muscleName = "RightHand.Little.1 Stretched";
            }
            else if (i == 93)
            {
                muscleName = "RightHand.Little.2 Stretched";
            }
            else if (i == 94)
            {
                muscleName = "RightHand.Little.3 Stretched";
            }
            animclip.SetCurve("", typeof(Animator), muscleName, humanPoseMuscleCurves[i]);
        }

        animclip.SetCurve("", typeof(Animator), "RootT.x", rootTX);
        animclip.SetCurve("", typeof(Animator), "RootT.y", rootTY);
        animclip.SetCurve("", typeof(Animator), "RootT.z", rootTZ);
        animclip.SetCurve("", typeof(Animator), "RootQ.x", rootQX);
        animclip.SetCurve("", typeof(Animator), "RootQ.y", rootQY);
        animclip.SetCurve("", typeof(Animator), "RootQ.z", rootQZ);
        animclip.SetCurve("", typeof(Animator), "RootQ.w", rootQW);

        
        //アニメーションを制作
        AssetDatabase.CreateAsset(animclip, AssetDatabase.GenerateUniqueAssetPath("Assets/kick.anim"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        

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
