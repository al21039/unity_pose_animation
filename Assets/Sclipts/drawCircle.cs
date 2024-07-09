using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawCircle : MonoBehaviour
{
    private int currentFrame = 0;
    public float radius = 1f;
    public int segments = 20;
    public float speed = 1.0f; // 回転速度
    private Animator animator;
    private Transform boneTransform;
    private float angle = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            boneTransform = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        }
        else
        {
            Debug.LogError("Animator component not found on this GameObject.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (boneTransform != null)
        {
            // 時間に基づいて角度を更新
            angle += speed * Time.deltaTime;

            // 新しい回転を計算
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            Debug.Log($"{x}, {y}, {boneTransform.localRotation.z}");

            // 手の回転を更新
            boneTransform.rotation = Quaternion.Euler(new Vector3(x, y, boneTransform.localRotation.z));
        }
    }
}