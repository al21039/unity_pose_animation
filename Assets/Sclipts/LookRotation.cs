using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookRotation : MonoBehaviour
{
    public GameObject cube1;
    public GameObject cube2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var aim = cube1.transform.position - cube2.transform.position;
        var look = Quaternion.LookRotation(aim, Vector3.up);
        cube1.transform.rotation = look;
    }
}
