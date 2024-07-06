using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RigControl : MonoBehaviour
{
    public CSVReader csvreader;
    public GameObject humanoid;
    public Vector3 bodyRotation = new Vector3(0, 0, 0);
    RigBone head;
    RigBone leftUpperArm;
    RigBone leftLowerArm;
    RigBone leftHand;
    RigBone rightUpperArm;  
    RigBone rightLowerArm;
    RigBone rightHand;
    RigBone spine;
    RigBone hips;
    RigBone leftUpperLeg;
    RigBone leftLowerLeg;
    RigBone leftFoot;
    RigBone rightUpperLeg;
    RigBone rightLowerLeg;
    RigBone rightFoot;
    void Start()
    {
        head = new RigBone(humanoid, HumanBodyBones.Head);
        leftUpperArm = new RigBone(humanoid, HumanBodyBones.LeftUpperArm);
        leftLowerArm = new RigBone(humanoid, HumanBodyBones.LeftLowerArm);
        leftHand = new RigBone(humanoid, HumanBodyBones.LeftHand);
        rightUpperArm = new RigBone(humanoid, HumanBodyBones.RightUpperArm);
        rightLowerArm = new RigBone(humanoid, HumanBodyBones.RightLowerArm);
        rightHand = new RigBone(humanoid, HumanBodyBones.RightHand);
        spine = new RigBone(humanoid, HumanBodyBones.Spine);
        hips = new RigBone(humanoid, HumanBodyBones.Hips);
        leftUpperLeg = new RigBone(humanoid, HumanBodyBones.LeftUpperLeg);
        leftLowerLeg = new RigBone(humanoid, HumanBodyBones.LeftLowerLeg);
        leftFoot = new RigBone(humanoid, HumanBodyBones.LeftFoot);
        rightUpperLeg = new RigBone(humanoid, HumanBodyBones.RightUpperLeg);
        rightLowerLeg = new RigBone(humanoid, HumanBodyBones.RightLowerLeg);
        rightFoot = new RigBone(humanoid, HumanBodyBones.RightFoot);

    }
    void Update()
    {
        double t = Math.Sin(Time.time * Math.PI); // [-1, 1]
        double s = (t + 1) / 2;                       // [0, 1]
        leftUpperArm.offset((float)(80 * t), 1, 0, 0);
        leftLowerArm.offset((float)(90 * s), 1, 0, 0);
        rightUpperArm.offset((float)(90 * t), 0, 0, 1);
        rightUpperLeg.offset((float)(-90 * s), 1, 0, 0);
        rightLowerLeg.offset((float)(90 * s), 1, 0, 0);
        humanoid.transform.rotation
          = Quaternion.AngleAxis(bodyRotation.z, new Vector3(0, 0, 1))
          * Quaternion.AngleAxis(bodyRotation.x, new Vector3(1, 0, 0))
          * Quaternion.AngleAxis(bodyRotation.y, new Vector3(0, 1, 0));
    }
}