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
        float upward = 0;

        // 上昇（スペースバー）と下降（Shiftキー）
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

        // カメラの回転
        if (Input.GetMouseButton(1)) // 右クリックで回転
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;

            // Y軸の回転のみ行う
            transform.Rotate(Vector3.up, mouseX);
        }
    }
}