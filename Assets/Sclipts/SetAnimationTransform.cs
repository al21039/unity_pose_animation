using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//�L�[�t���[�����Ƀ��f���̈ʒu���߂��̃t���[���̃A�j���[�V�������s�킹��
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

    GameObject manager_obj;
    private AnimationSceneManager manager_script;


    // Start is called before the first frame update
    void Start()
    {
        manager_obj = GameObject.Find("AnimationSceneManager");
        manager_script = manager_obj.GetComponent<AnimationSceneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPartTransform(int frame, Vector3[] pos_list)
    {
        Vector3[] positions = pos_list;
        animation_frame = frame;

        Left_hand.transform.position = positions[0] + new Vector3(0, 0, animation_frame * 0.3f);
        Right_hand.transform.position = positions[1] + new Vector3(0, 0, animation_frame * 0.3f);
        Left_ankle.transform.position = positions[2] + new Vector3(0, 0, animation_frame * 0.3f);
        Right_ankle.transform.position = positions[3] + new Vector3(0, 0, animation_frame * 0.3f);
        Left_elbow.transform.position = positions[4] + new Vector3(0, 0, animation_frame * 0.3f);
        Right_elbow.transform.position = positions[5] + new Vector3(0, 0, animation_frame * 0.3f);
        Left_knee.transform.position = positions[6] + new Vector3(0, 0, animation_frame * 0.3f);
        Right_knee.transform.position = positions[7] + new Vector3(0, 0, animation_frame * 0.3f);
        Body.transform.position = positions[8] + new Vector3(0, 0, animation_frame * 0.3f);
        middleDot.transform.position = positions[9] + new Vector3(0, 0, animation_frame * 0.3f);
    }
}
