using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class Trajectory_Manager : MonoBehaviour
{
    [SerializeField] GameObject rig_model;

    GameObject[] keyPose_models = new GameObject[IK_target_pos.KeyPose_List.Count];
    private SetAnimationTransform SetAnimationTransform;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < IK_target_pos.KeyPose_List.Count; i++)
        {
            Debug.Log(i);
            for(int j = 0; j < 10; j++)
            {
                Vector3 aaa;
                aaa = IK_target_pos.modelPos[IK_target_pos.KeyPose_List[i]][j];
                Debug.Log(aaa);
            }
        }


        for(int i = 0; i < IK_target_pos.KeyPose_List.Count; i++)
        {
            keyPose_models[i] = Instantiate(rig_model, new Vector3(0, 0, (float)IK_target_pos.KeyPose_List[i] * 0.15f), Quaternion.identity);
            SetAnimationTransform = keyPose_models[i].GetComponent<SetAnimationTransform>();
            SetAnimationTransform.animation_frame = IK_target_pos.KeyPose_List[i];
            SetAnimationTransform.SetPartTransform();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setTransform(int frame) 
    {

    }
}
*/
