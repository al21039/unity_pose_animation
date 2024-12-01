using UnityEngine;

//�L�[�t���[�����Ƀ��f���̈ʒu���߂��̃t���[���̃A�j���[�V�������s�킹��
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

    public GameObject[] IKObjectArray()
    {
        GameObject[] _IKModelPart = new GameObject[8]
        {
            _modelPart[0],
            _modelPart[1],
            _modelPart[2],
            _modelPart[3],
            _modelPart[4],
            _modelPart[5],
            _modelPart[6],
            _modelPart[7]
        };

        return _IKModelPart; 
    }

    public void SetPartTransform(int frame, Vector3[] pos_list, Quaternion[] posRot, float hieght)
    {
        Vector3[] positions = pos_list;
        animation_frame = frame;

        for (int i = 0; i < _modelPart.Length; i++)
        {
            _modelPart[i].transform.position = positions[i] + new Vector3(0, 0, animation_frame * _frameInterval);
        }

        for (int i = 0; i < _modelRotationPart.Length; i++)
        {
            _modelRotationPart[i].transform.rotation = posRot[i];
        }
    }

    //�e�L�[�t���[���̃��f����HumanPose��Ԃ�
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

