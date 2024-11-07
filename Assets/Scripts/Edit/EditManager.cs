using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditManager : MonoBehaviour
{
    [SerializeField] private Spline _spline;
    [SerializeField] private IndirectOperation indirectOperation;
    [SerializeField] private UIListener _uiListener;

    private Dictionary<int, Vector3[]> _changePos = new Dictionary<int, Vector3[]>();    //�ύX��̈ʒu�@�@�@�@�ŏ�����Ȑ��ɕύX
    private List<int> _keyPoseList = new List<int>();                                    //�L�[�|�[�Y�̃��X�g�@�@�@�ォ��JSON�t�@�C���̂�ǉ��ł���悤��

    public Dictionary<int, Vector3[]> ChangePos
    {
        get
        {
            return _changePos;
        }
        set
        {
            _changePos = value;
        }
    }

    private static EditManager instance;

    public static EditManager GetInstance() => instance;   //�C���X�^���X�Ԃ�

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }    //�V���O���g������

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
