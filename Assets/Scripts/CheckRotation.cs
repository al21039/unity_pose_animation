using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckRotation : MonoBehaviour
{
    [SerializeField] private GameObject[] modelObj = new GameObject[3];

    // Start is called before the first frame update
    void Start()
    {
        InitializePosition();
        SetRotation();
    }

    void InitializePosition()
    {
        foreach (var obj in modelObj)
        {
            obj.transform.position = new Vector3(obj.transform.position.x * -1.0f, obj.transform.position.y * -1.0f + 1.0f, obj.transform.position.z * -1.0f);
        }
    }

    void SetRotation()
    {
        Vector3 middleFinger = (modelObj[1].transform.position + modelObj[2].transform.position) / 2;

        Vector3 rawVerticalAxis = (middleFinger - modelObj[0].transform.position).normalized;
        Vector3 horizontalAxis = (modelObj[1].transform.position - modelObj[2].transform.position).normalized;

        Vector3 leftHandForward = Vector3.Cross(horizontalAxis, rawVerticalAxis).normalized;

        Debug.Log(rawVerticalAxis);
        Debug.Log(horizontalAxis);
        Debug.Log(leftHandForward);


        modelObj[0].transform.rotation = Quaternion.LookRotation(leftHandForward, rawVerticalAxis) * Quaternion.Euler(3.30f, 1.66f, 107.43f);

    }

}
