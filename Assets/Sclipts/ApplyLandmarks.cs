using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyLandmarks : MonoBehaviour
{
    public CSVReader csvReader; // CSVReaderスクリプト
    public Transform[] bones; // 3Dモデルのボーン（必要なボーンのみ設定）

    private int currentFrame = 0;
    private List<Vector3> currentLandmarks;

    void Update()
    {
        currentLandmarks = csvReader.GetFrameLandmarks(currentFrame);
        if (currentLandmarks != null)
        {
            for (int i = 0; i < bones.Length; i++)
            {
                // 必要なランドマークインデックスを指定
                int landmarkIndex = GetCorrespondingLandmarkIndex(i);
                bones[i].position = currentLandmarks[landmarkIndex];
            }
            currentFrame++;
            if (currentFrame >= csvReader.GetTotalFrames()) currentFrame = 0; // ループ再生
        }
    }

    // 3Dモデルのボーンに対応するBlazePoseランドマークインデックスを返す
    int GetCorrespondingLandmarkIndex(int boneIndex)
    {
        int[] landmarkIndices = { 0, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 30, 32};
        return landmarkIndices[boneIndex];
    }
}
