using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyLandmarks : MonoBehaviour
{
    public CSVReader csvReader; // CSVReaderスクリプト
    public Animator animator; // 3DモデルのAnimator

    // HumanBodyBonesに対応するランドマークペア
    private Dictionary<HumanBodyBones, (int start, int end)> boneLandmarkPairs = new Dictionary<HumanBodyBones, (int start, int end)>()
    {
        {HumanBodyBones.Head, (0, 1) }, // 頭のボーンに対応するランドマークペア
        {HumanBodyBones.LeftUpperArm, (11, 13) },
        {HumanBodyBones.LeftLowerArm, (13, 15) },
        {HumanBodyBones.LeftHand,(15, 19)},
        // 必要に応じて他のペアを追加
    };

    private int currentFrame = 0;
    private List<Vector3> currentLandmarks;

    void Update()
    {
        currentLandmarks = csvReader.GetFrameLandmarks(currentFrame);
        if (currentLandmarks != null)
        {
            foreach (var pair in boneLandmarkPairs)
            {
                HumanBodyBones humanBone = pair.Key;
                (int start, int end) = pair.Value;

                // ベクトルを計算
                Vector3 startVec = currentLandmarks[start];
                Vector3 endVec = currentLandmarks[end];
                Vector3 direction = endVec - startVec;

                // ベクトル間の回転を計算
                Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, direction);

                // ボーンに回転を適用
                Transform boneTransform = animator.GetBoneTransform(humanBone);
                if (boneTransform != null)
                {
                    boneTransform.localRotation = rotation;
                }
                else
                {
                    Debug.LogError($"ボーン {humanBone} が見つかりませんでした");
                }
            }
            currentFrame++;
            if (currentFrame >= csvReader.GetTotalFrames()) currentFrame = 0; // ループ再生
        }
    }
}
