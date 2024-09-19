using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSceneManager : MonoBehaviour
{
    [SerializeField] GameObject humanoid_model;

    public static Dictionary<int, Vector3[]> part_position = new Dictionary<int, Vector3[]>();
    public static List<int> KeyPoses = new List<int>();
    public bool makedAnimation = false;
    bool putOn = false;
    

    GameObject[] keypose_models;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(makedAnimation && putOn)
        {
            IK_target_pos iK_Target_Pos = humanoid_model.GetComponent<IK_target_pos>();
            iK_Target_Pos.enabled = false;
            SetAnimationTransform setAnimationTransform = humanoid_model.GetComponent<SetAnimationTransform>();
            setAnimationTransform.enabled = true;

            keypose_models = new GameObject[KeyPoses.Count];
            for(int i = 0; i < KeyPoses.Count; i++)
            {
                keypose_models[i] = Instantiate(humanoid_model, new Vector3(0, 0, KeyPoses[i] * 0.15f), Quaternion.identity);
                setAnimationTransform.SetPartTransform(KeyPoses[i]);
            }

            putOn = true;
        }
    }
}
