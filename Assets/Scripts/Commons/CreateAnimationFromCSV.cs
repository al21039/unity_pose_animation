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
    private List<Quaternion> leftHand = new List<Quaternion>();
    private List<Quaternion> rightHand = new List<Quaternion>();
    private List<Quaternion> leftFoot = new List<Quaternion>();
    private List<Quaternion> rightFoot = new List<Quaternion>();
    private List<Vector3> leftHandPos = new List<Vector3>();
    private List<Vector3> rightHandPos = new List<Vector3>();
    private List<Vector3> leftFootPos = new List<Vector3>();
    private List<Vector3> rightFootPos = new List<Vector3>();

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

        AnimationCurve leftHandQX = new AnimationCurve();
        AnimationCurve leftHandQY = new AnimationCurve();
        AnimationCurve leftHandQZ = new AnimationCurve();
        AnimationCurve leftHandQW = new AnimationCurve();

        AnimationCurve rightHandQX = new AnimationCurve();
        AnimationCurve rightHandQY = new AnimationCurve();
        AnimationCurve rightHandQZ = new AnimationCurve();
        AnimationCurve rightHandQW = new AnimationCurve();

        AnimationCurve leftFootQX = new AnimationCurve();
        AnimationCurve leftFootQY = new AnimationCurve();
        AnimationCurve leftFootQZ = new AnimationCurve();
        AnimationCurve leftFootQW = new AnimationCurve();

        AnimationCurve rightFootQX = new AnimationCurve();
        AnimationCurve rightFootQY = new AnimationCurve();
        AnimationCurve rightFootQZ = new AnimationCurve();
        AnimationCurve rightFootQW = new AnimationCurve();

        AnimationCurve leftHandTX = new AnimationCurve();
        AnimationCurve leftHandTY = new AnimationCurve();
        AnimationCurve leftHandTZ = new AnimationCurve();

        AnimationCurve rightHandTX = new AnimationCurve();
        AnimationCurve rightHandTY = new AnimationCurve();
        AnimationCurve rightHandTZ = new AnimationCurve();

        AnimationCurve leftFootTX = new AnimationCurve();
        AnimationCurve leftFootTY = new AnimationCurve();
        AnimationCurve leftFootTZ = new AnimationCurve();

        AnimationCurve rightFootTX = new AnimationCurve();
        AnimationCurve rightFootTY = new AnimationCurve();
        AnimationCurve rightFootTZ = new AnimationCurve();


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

        count = 0;
        foreach (var lefthand in leftHand)
        {
            time = count / 30;

            leftHandQX.AddKey(time, lefthand.x);
            leftHandQY.AddKey(time, lefthand.y);
            leftHandQZ.AddKey(time, lefthand.z);
            leftHandQW.AddKey(time, lefthand.w);

            count++;
        }

        count = 0;
        foreach (var righthand in rightHand)
        {
            time = count / 30;

            rightHandQX.AddKey(time, righthand.x);
            rightHandQY.AddKey(time, righthand.y);
            rightHandQZ.AddKey(time, righthand.z);
            rightHandQW.AddKey(time, righthand.w);

            count++;
        }

        count = 0;
        foreach (var leftfoot in leftFoot)
        {
            time = count / 30;

            rightHandQX.AddKey(time, leftfoot.x);
            rightHandQY.AddKey(time, leftfoot.y);
            rightHandQZ.AddKey(time, leftfoot.z);
            rightHandQW.AddKey(time, leftfoot.w);

            count++;
        }

        count = 0;
        foreach (var rightfoot in rightFoot)
        {
            time = count / 30;

            rightHandQX.AddKey(time, rightfoot.x);
            rightHandQY.AddKey(time, rightfoot.y);
            rightHandQZ.AddKey(time, rightfoot.z);
            rightHandQW.AddKey(time, rightfoot.w);

            count++;
        }

        count = 0;
        foreach (var lefthand in leftHandPos)
        {
            time = count / 30;

            leftHandTX.AddKey(time, lefthand.x);
            leftHandTY.AddKey(time, lefthand.y);
            leftHandTZ.AddKey(time, lefthand.z);

            count++;
        }

        count = 0;
        foreach (var righthand in rightHandPos)
        {
            time = count / 30;

            leftHandTX.AddKey(time, righthand.x);
            leftHandTY.AddKey(time, righthand.y);
            leftHandTZ.AddKey(time, righthand.z);

            count++;
        }

        count = 0;
        foreach (var leftfoot in leftFootPos)
        {
            time = count / 30;

            leftHandTX.AddKey(time, leftfoot.x);
            leftHandTY.AddKey(time, leftfoot.y);
            leftHandTZ.AddKey(time, leftfoot.z);

            count++;
        }

        count = 0;
        foreach (var rightfoot in rightFootPos)
        {
            time = count / 30;

            leftHandTX.AddKey(time, rightfoot.x);
            leftHandTY.AddKey(time, rightfoot.y);
            leftHandTZ.AddKey(time, rightfoot.z);

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

        
        animclip.SetCurve("", typeof(Animator), "LeftHandQ.x", leftHandQX);
        animclip.SetCurve("", typeof(Animator), "LeftHandQ.y", leftHandQY);
        animclip.SetCurve("", typeof(Animator), "LeftHandQ.z", leftHandQZ);
        animclip.SetCurve("", typeof(Animator), "LeftHandQ.w", leftHandQW);

        animclip.SetCurve("", typeof(Animator), "RightHandQ.x", rightHandQX);
        animclip.SetCurve("", typeof(Animator), "RightHandQ.y", rightHandQY);
        animclip.SetCurve("", typeof(Animator), "RightHandQ.z", rightHandQZ);
        animclip.SetCurve("", typeof(Animator), "RightHandQ.w", rightHandQW);

        animclip.SetCurve("", typeof(Animator), "LeftFootQ.x", leftFootQX);
        animclip.SetCurve("", typeof(Animator), "LeftFootQ.y", leftFootQY);
        animclip.SetCurve("", typeof(Animator), "LeftFootQ.z", leftFootQZ);
        animclip.SetCurve("", typeof(Animator), "LeftFootQ.w", leftFootQW);

        animclip.SetCurve("", typeof(Animator), "RightFootQ.x", rightFootQX);
        animclip.SetCurve("", typeof(Animator), "RightFootQ.y", rightFootQY);
        animclip.SetCurve("", typeof(Animator), "RightFootQ.z", rightFootQZ);
        animclip.SetCurve("", typeof(Animator), "RightFootQ.w", rightFootQW);

        animclip.SetCurve("", typeof(Animator), "LeftHandT.x", leftHandTX);
        animclip.SetCurve("", typeof(Animator), "LeftHandT.y", leftHandTY);
        animclip.SetCurve("", typeof(Animator), "LeftHandT.z", leftHandTZ);

        animclip.SetCurve("", typeof(Animator), "RightHandT.x", rightHandTX);
        animclip.SetCurve("", typeof(Animator), "RightHandT.y", rightHandTY);
        animclip.SetCurve("", typeof(Animator), "RightHandT.z", rightHandTZ);

        animclip.SetCurve("", typeof(Animator), "LeftFootT.x", leftFootTX);
        animclip.SetCurve("", typeof(Animator), "LeftFootT.y", leftFootTY);
        animclip.SetCurve("", typeof(Animator), "LeftFootT.z", leftFootTZ);

        animclip.SetCurve("", typeof(Animator), "RightFootT.x", rightFootTX);
        animclip.SetCurve("", typeof(Animator), "RightFootT.y", rightFootTY);
        animclip.SetCurve("", typeof(Animator), "RightFootT.z", rightFootTZ);


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

                Quaternion leftHandRotation = new Quaternion(
                    float.Parse(values[102]),
                    float.Parse(values[103]),
                    float.Parse(values[104]),
                    float.Parse(values[105])
                );
                leftHand.Add(leftHandRotation);

                Quaternion rightHandRotation = new Quaternion(
                    float.Parse(values[106]),
                    float.Parse(values[107]),
                    float.Parse(values[108]),
                    float.Parse(values[109])
                );
                rightHand.Add(rightHandRotation);

                Quaternion leftFootRotation = new Quaternion(
                    float.Parse(values[110]),
                    float.Parse(values[111]),
                    float.Parse(values[112]),
                    float.Parse(values[113])
                );
                leftFoot.Add(leftFootRotation);

                Quaternion rightFootRotation = new Quaternion(
                    float.Parse(values[114]),
                    float.Parse(values[115]),
                    float.Parse(values[116]),
                    float.Parse(values[117])
                );
                rightFoot.Add(rightFootRotation);

                Vector3 leftHandPosition = new Vector3(
                    float.Parse(values[118]),    
                    float.Parse(values[119]),    
                    float.Parse(values[120])    
                );
                leftHandPos.Add(leftHandPosition);

                Vector3 rightHandPosition = new Vector3(
                    float.Parse(values[121]),
                    float.Parse(values[122]),
                    float.Parse(values[123])
                );
                rightHandPos.Add(rightHandPosition);

                Vector3 leftFootPosition = new Vector3(
                    float.Parse(values[124]),
                    float.Parse(values[125]),
                    float.Parse(values[126])
                );
                leftFootPos.Add(leftFootPosition);

                Vector3 rightFootPosition = new Vector3(
                    float.Parse(values[127]),
                    float.Parse(values[128]),
                    float.Parse(values[129])
                );
                rightFootPos.Add(rightFootPosition);

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
