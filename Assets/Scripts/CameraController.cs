using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f; // 移動速度
    public float lookSpeed = 2f;  // 視点の回転速度

    void Update()
    {
        // カメラの移動
        float horizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        Vector3 movement = new Vector3(horizontal, 0, vertical);
        transform.Translate(movement);

        // カメラの回転
        if (Input.GetMouseButton(1)) // 右クリックで回転
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

            transform.Rotate(Vector3.up, mouseX);
            transform.Rotate(Vector3.left, mouseY);
        }
    }
}