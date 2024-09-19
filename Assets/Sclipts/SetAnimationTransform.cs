using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SetAnimationTransform : MonoBehaviour
{
    [SerializeField] GameObject Body;
    [SerializeField] GameObject middleDot;
    [SerializeField] GameObject Left_elbow;
    [SerializeField] GameObject Left_hand;
    [SerializeField] GameObject Right_elbow;
    [SerializeField] GameObject Right_hand;
    [SerializeField] GameObject Left_knee;
    [SerializeField] GameObject Left_ankle;
    [SerializeField] GameObject Right_knee;
    [SerializeField] GameObject Right_ankle;
    int animation_frame = 0;

    [SerializeField] GameObject manager_obj;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPartTransform(int frame)
    {
        animation_frame = frame;
        Vector3[] position = AnimationSceneManager.part_position[animation_frame];

        Left_hand.transform.position = position[0] + new Vector3(0, 0, animation_frame * 0.15f);
        Right_hand.transform.position = position[1] + new Vector3(0, 0, animation_frame * 0.15f);
        Left_ankle.transform.position = position[2] + new Vector3(0, 0, animation_frame * 0.15f);
        Right_ankle.transform.position = position[3] + new Vector3(0, 0, animation_frame * 0.15f);
        Left_elbow.transform.position = position[4] + new Vector3(0, 0, animation_frame * 0.15f);
        Right_elbow.transform.position = position[5] + new Vector3(0, 0, animation_frame * 0.15f);
        Left_knee.transform.position = position[6] + new Vector3(0, 0, animation_frame * 0.15f);
        Right_knee.transform.position = position[7] + new Vector3(0, 0, animation_frame * 0.15f);
        Body.transform.position = position[8] + new Vector3(0, 0, animation_frame * 0.15f);
        middleDot.transform.position = position[9] + new Vector3(0, 0, animation_frame * 0.15f);
    }
}

