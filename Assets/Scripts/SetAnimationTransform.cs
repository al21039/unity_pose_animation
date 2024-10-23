using UnityEngine;

//キーフレーム毎にモデルの位置を定めそのフレームのアニメーションを行わせる
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


    private Animator _animator;
    private float _frameInterval = 0.30f;
    private HumanPoseHandler _humanPoseHandler;
    private HumanPose _humanPose;

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

        Left_hand.transform.position = positions[0] + new Vector3(0, 0, animation_frame * _frameInterval);
        Right_hand.transform.position = positions[1] + new Vector3(0, 0, animation_frame * _frameInterval);
        Left_ankle.transform.position = positions[2] + new Vector3(0, 0, animation_frame * _frameInterval);
        Right_ankle.transform.position = positions[3] + new Vector3(0, 0, animation_frame * _frameInterval);
        Left_elbow.transform.position = positions[4] + new Vector3(0, 0, animation_frame * _frameInterval);
        Right_elbow.transform.position = positions[5] + new Vector3(0, 0, animation_frame * _frameInterval);
        Left_knee.transform.position = positions[6] + new Vector3(0, 0, animation_frame * _frameInterval);
        Right_knee.transform.position = positions[7] + new Vector3(0, 0, animation_frame * _frameInterval);
        Body.transform.position = positions[8] + new Vector3(0, 0, animation_frame * _frameInterval);
        middleDot.transform.position = positions[9] + new Vector3(0, 0, animation_frame * _frameInterval);
    }

    public HumanPose GetKeyPoseMuscle()
    {
        _animator = GetComponent<Animator>();
        _humanPose = new HumanPose();
        _humanPoseHandler = new HumanPoseHandler(_animator.avatar, _animator.transform);
        _humanPoseHandler.GetHumanPose(ref _humanPose);
        return _humanPose;
    }
}

