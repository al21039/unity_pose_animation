using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchButton : MonoBehaviour
{
    public CatchPos catchPos; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Push_LHand()
    {
        catchPos.LHandButton = !catchPos.LHandButton;
    }

    public void Push_RHand()
    {
        catchPos.RHandButton = !catchPos.RHandButton;
    }

    public void Push_LFoot()
    {
        catchPos.LFootButton = !catchPos.LFootButton;
    }

    public void Push_RFoot()
    {
        catchPos.RFootButton = !catchPos.RFootButton;
    }
}
