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

        Vector3 movement = new Vector3(horizontal, 0, vertical);
        transform.Translate(movement);

        // �J�����̉�]
        if (Input.GetMouseButton(1)) // �E�N���b�N�ŉ�]
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

            transform.Rotate(Vector3.up, mouseX);
            transform.Rotate(Vector3.left, mouseY);
        }
    }
}