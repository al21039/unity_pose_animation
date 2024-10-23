using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchEndPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform ReturnEndAnker(int positionID, GameObject rootObject)
    {
        Transform anker = null;
        switch (positionID)
        {
            case 0:
                anker = rootObject.transform.Find("Rig").Find("HandRig").Find("HandLTarget");
                break;
            
            case 1:
                anker = rootObject.transform.Find("Rig").Find("HandRig").Find("HandRTarget");
                break;

            case 2:
                anker = rootObject.transform.Find("Rig").Find("FootRig").Find("FootLTarget");
                break;

            case 3:
                anker = rootObject.transform.Find("Rig").Find("FootRig").Find("FootRTarget");
                break;

            case 4:
                anker = rootObject.transform.Find("Rig").Find("HandRig").Find("ElbowLTarget");
                break;

            case 5:
                anker = rootObject.transform.Find("Rig").Find("HandRig").Find("ElbowRTarget");
                break;

            case 6:
                anker = rootObject.transform.Find("Rig").Find("FootRig").Find("KneeLTarget");
                break;

            case 7:
                anker = rootObject.transform.Find("Rig").Find("FootRig").Find("KneeRTarget");
                break;

            default:
                break;
        }

        return anker;
        
    }

    //ä÷êﬂëÄçÏÇçsÇ¡ÇΩç€ÇÃä÷êﬂÇéÊìæ
    public Transform ReturnEndPoint(int positionID, GameObject rootObject)
    {
        Transform target = null;
        switch (positionID)
        {
            case 0:
                target = rootObject.transform.Find("Armature").Find("Hips").Find("Spine 1").Find("Spine 2").Find("Spine 3").Find("Left Shoulder").Find("Left Arm").Find("Left Forearm").Find("Left Hand");
                break;
            case 1:
                target = rootObject.transform.Find("Armature").Find("Hips").Find("Spine 1").Find("Spine 2").Find("Spine 3").Find("Right Shoulder").Find("Right Arm").Find("Right Forearm").Find("Right Hand");
                break;
            case 2:
                target = rootObject.transform.Find("Armature").Find("Hips").Find("Left Thigh").Find("Left Leg").Find("Left Foot");
                break;
            case 3:
                target = rootObject.transform.Find("Armature").Find("Hips").Find("Right Thigh").Find("Right Leg").Find("Right Foot");
                break;
            case 4:
                target = rootObject.transform.Find("Armature").Find("Hips").Find("Spine 1").Find("Spine 2").Find("Spine 3").Find("Left Shoulder").Find("Left Arm").Find("Left Forearm");
                break;
            case 5:
                target = rootObject.transform.Find("Armature").Find("Hips").Find("Spine 1").Find("Spine 2").Find("Spine 3").Find("Right Shoulder").Find("Right Arm").Find("Right Forearm");
                break;
            case 6:
                target = rootObject.transform.Find("Armature").Find("Hips").Find("Left Thigh").Find("Left Leg");
                break;
            case 7:
                target = rootObject.transform.Find("Armature").Find("Hips").Find("Right Thigh").Find("Right Leg");
                break;
        }
        return target;
    }

    //íºê⁄ëÄçÏÇçsÇ¡ÇΩç€ÇÃä÷êﬂÇéÊìæ
    public Transform ReturnEndPoint(int positionID, string frameObject)
    {
        Transform target = null;
        switch (positionID)
        {
            case 0:
                target = GameObject.Find(frameObject).transform.Find("Armature").Find("Hips").Find("Spine 1").Find("Spine 2").Find("Spine 3").Find("Left Shoulder").Find("Left Arm").Find("Left Forearm").Find("Left Hand");
                break;
            case 1:
                target = GameObject.Find(frameObject).transform.Find("Armature").Find("Hips").Find("Spine 1").Find("Spine 2").Find("Spine 3").Find("Right Shoulder").Find("Right Arm").Find("Right Forearm").Find("Right Hand");
                break;
            case 2:
                target = GameObject.Find(frameObject).transform.Find("Armature").Find("Hips").Find("Left Thigh").Find("Left Leg").Find("Left Foot");
                break;
            case 3:
                target = GameObject.Find(frameObject).transform.Find("Armature").Find("Hips").Find("Right Thigh").Find("Right Leg").Find("Right Foot");
                break;
            case 4:
                target = GameObject.Find(frameObject).transform.Find("Armature").Find("Hips").Find("Spine 1").Find("Spine 2").Find("Spine 3").Find("Left Shoulder").Find("Left Arm").Find("Left Forearm");
                break;
            case 5:
                target = GameObject.Find(frameObject).transform.Find("Armature").Find("Hips").Find("Spine 1").Find("Spine 2").Find("Spine 3").Find("Right Shoulder").Find("Right Arm").Find("Right Forearm");
                break;
            case 6:
                target = GameObject.Find(frameObject).transform.Find("Armature").Find("Hips").Find("Left Thigh").Find("Left Leg");
                break;
            case 7:
                target = GameObject.Find(frameObject).transform.Find("Armature").Find("Hips").Find("Right Thigh").Find("Right Leg");
                break;
        }
        return target;
    }
}
