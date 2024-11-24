using UnityEngine;

//キーフレーム毎にモデルの位置を定めそのフレームのアニメーションを行わせる
public class SetAnimationTransform : MonoBehaviour
{
    [SerializeField] private GameObject[] _modelPart;
    [SerializeField] private GameObject[] _modelRotationPart;
    int animation_frame = 0;

    private Animator _animator;
    private float _frameInterval = 0.30f;
    private HumanPoseHandler _humanPoseHandler;
    private HumanPose _humanPose;

    GameObject manager_obj;
    void Start()
    {
        manager_obj = GameObject.Find("AnimationSceneManager");
    }

    public void SetPartTransform(int frame, Vector3[] pos_list, Quaternion[] posRot)
    {
        Vector3[] positions = pos_list;
        animation_frame = frame;

        for (int i = 0; i < _modelPart.Length; i++)
        {
            _modelPart[i].transform.position = positions[i] + new Vector3(0, 0, animation_frame * _frameInterval);
        }

        _modelRotationPart[0].transform.rotation = posRot[0];
        _modelRotationPart[1].transform.rotation = posRot[1];

    }

    //各キーフレームのモデルのHumanPoseを返す
    public HumanPose GetKeyPoseMuscle(int frame)
    {
        _animator = GetComponent<Animator>();
        _humanPose = new HumanPose();
        _humanPoseHandler = new HumanPoseHandler(_animator.avatar, _animator.transform);
        _humanPoseHandler.GetHumanPose(ref _humanPose);

        _humanPose.bodyPosition -= new Vector3(0, 0, frame * _frameInterval);

        return _humanPose;
    }
}

