using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookRotation : MonoBehaviour
{
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var aim = this.transform.position - target.transform.position;
        var look = Quaternion.LookRotation(aim);
        this.transform.rotation = look;
    }
}
