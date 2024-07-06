using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyLandmarks : MonoBehaviour
{
    public CSVReader csvReader; // CSVReader�X�N���v�g
    public Animator animator; // 3D���f����Animator

    // HumanBodyBones�ɑΉ����郉���h�}�[�N�y�A
    private Dictionary<HumanBodyBones, (int start, int end)> boneLandmarkPairs = new Dictionary<HumanBodyBones, (int start, int end)>()
    {
        {HumanBodyBones.Head, (0, 1) }, // ���̃{�[���ɑΉ����郉���h�}�[�N�y�A
        {HumanBodyBones.LeftUpperArm, (11, 13) },
        {HumanBodyBones.LeftLowerArm, (13, 15) },
        {HumanBodyBones.LeftHand,(15, 19)},
        // �K�v�ɉ����đ��̃y�A��ǉ�
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

                // �x�N�g�����v�Z
                Vector3 startVec = currentLandmarks[start];
                Vector3 endVec = currentLandmarks[end];
                Vector3 direction = endVec - startVec;

                // �x�N�g���Ԃ̉�]���v�Z
                Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, direction);

                // �{�[���ɉ�]��K�p
                Transform boneTransform = animator.GetBoneTransform(humanBone);
                if (boneTransform != null)
                {
                    boneTransform.localRotation = rotation;
                }
                else
                {
                    Debug.LogError($"�{�[�� {humanBone} ��������܂���ł���");
                }
            }
            currentFrame++;
            if (currentFrame >= csvReader.GetTotalFrames()) currentFrame = 0; // ���[�v�Đ�
        }
    }
}
