using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyLandmarks : MonoBehaviour
{
    public CSVReader csvReader; // CSVReader�X�N���v�g
    public Transform[] bones; // 3D���f���̃{�[���i�K�v�ȃ{�[���̂ݐݒ�j

    private int currentFrame = 0;
    private List<Vector3> currentLandmarks;

    void Update()
    {
        currentLandmarks = csvReader.GetFrameLandmarks(currentFrame);
        if (currentLandmarks != null)
        {
            for (int i = 0; i < bones.Length; i++)
            {
                // �K�v�ȃ����h�}�[�N�C���f�b�N�X���w��
                int landmarkIndex = GetCorrespondingLandmarkIndex(i);
                bones[i].position = currentLandmarks[landmarkIndex];
            }
            currentFrame++;
            if (currentFrame >= csvReader.GetTotalFrames()) currentFrame = 0; // ���[�v�Đ�
        }
    }

    // 3D���f���̃{�[���ɑΉ�����BlazePose�����h�}�[�N�C���f�b�N�X��Ԃ�
    int GetCorrespondingLandmarkIndex(int boneIndex)
    {
        int[] landmarkIndices = { 0, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 30, 32};
        return landmarkIndices[boneIndex];
    }
}
