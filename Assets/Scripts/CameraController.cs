using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f; // �ړ����x
    public float lookSpeed = 2f;  // ���_�̉�]���x

    void Update()
    {
        // �J�����̈ړ�
        float horizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float upward = 0;

        // �㏸�i�X�y�[�X�o�[�j�Ɖ��~�iShift�L�[�j
        if (Input.GetKey(KeyCode.Space))
        {
            upward = moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            upward = -moveSpeed * Time.deltaTime;
        }

        Vector3 movement = new Vector3(horizontal, upward, vertical);
        transform.Translate(movement);

        // �J�����̉�]
        if (Input.GetMouseButton(1)) // �E�N���b�N�ŉ�]
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;

            // Y���̉�]�̂ݍs��
            transform.Rotate(Vector3.up, mouseX);
        }
    }
}