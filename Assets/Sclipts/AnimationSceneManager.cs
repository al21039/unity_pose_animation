using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSceneManager : MonoBehaviour
{
    [SerializeField] GameObject humanoid_model;

    GameObject keypose_model;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(int frame, Vector3[] pos_list)
    {
        for (int i = 0; i < pos_list.Length; i++)
        {
            //Debug.Log(pos_list[i]);
        }
        keypose_model = Instantiate(humanoid_model, new Vector3(0, 0, frame * 0.15f), Quaternion.identity);
        SetAnimationTransform setAnimationTransform = keypose_model.GetComponent<SetAnimationTransform>();
        setAnimationTransform.SetPartTransform(frame, pos_list);
    }
}
