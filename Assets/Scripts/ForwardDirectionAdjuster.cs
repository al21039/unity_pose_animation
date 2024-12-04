using UnityEngine;

public class ForwardDirectionAdjuster : MonoBehaviour
{
    void Update()
    {
        // �I�u�W�F�N�g�̌��݂̐��ʕ����i���[���h���W�j
        Vector3 forwardDirection = transform.forward;

        // Y���𒆐S��90�x��]�������������v�Z
        Quaternion rotation = Quaternion.Euler(0, 90, 0);
        Vector3 adjustedDirection = rotation * forwardDirection;

        // �v�Z���ʂ��f�o�b�O���O�ŕ\���i�m�F�p�j
        Debug.DrawRay(transform.position, forwardDirection, Color.red); // ���̕����i�ԁj
        Debug.DrawRay(transform.position, adjustedDirection, Color.green); // ������̕����i�΁j
    }
}