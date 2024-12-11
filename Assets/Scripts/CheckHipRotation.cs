using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckHipRotation : MonoBehaviour
{
    public void SliderDemo(float value)
    {
        Vector3 rot = transform.rotation.eulerAngles;

        rot.y = -value;

        transform.rotation = Quaternion.Euler(rot);
    }


}
