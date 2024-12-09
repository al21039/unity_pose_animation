using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckLegRotation : MonoBehaviour
{
    [SerializeField] private GameObject[] parts;
    [SerializeField] private GameObject rotatino;

    private void Start()
    {
        foreach (var obj in parts)
        {
            obj.transform.position = new Vector3(obj.transform.position.x * -1, obj.transform.position.y * -1 + 1, obj.transform.position.z * -1);
        }
        Calc();
    }

    private void Calc()
    {
        Vector3 direction = parts[3].transform.position - parts[2].transform.position;
        Vector3 up = parts[4].transform.position - parts[2].transform.position;

        Quaternion settingRotation = Quaternion.LookRotation(direction, up);

        parts[2].transform.rotation = settingRotation * Quaternion.Euler(0,90,90);
    }
}
