using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditManager : MonoBehaviour
{
    [SerializeField] private GameObject _humanoidModel;

    [SerializeField] private Spline _spline;
    [SerializeField] private PositionMover _positionMover;
    [SerializeField] private UIListener _uiListener;

    private Dictionary<int, Vector3[]> _changePos = new Dictionary<int, Vector3[]>();    //変更後の位置　　　　最初から曲線に変更
    private List<int> _keyPoseList = new List<int>();                                    //キーポーズのリスト　　　後からJSONファイルのやつ追加できるように

    private List<GameObject> _keyPoseModel = new List<GameObject>();
    private int _keyPoseModelCount = 0;
    private float _frameInterval = 0.30f;

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

    public List<int> KeyPoseList
    {
        get
        {
            return _keyPoseList;
        }
        set
        {
            _keyPoseList = value;
        }
    }

    private static EditManager instance;

    public static EditManager GetInstance() => instance;   //インスタンス返す

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
    }    //シングルトン処理

    public void PrepareEditing()
    {
        ChangePos = LandmarkManager.GetInstance().CSVLandmarkPositions;
        _keyPoseList = LandmarkManager.GetInstance().KeyPoseList;
        for (int i = 0; i < _keyPoseList.Count; i++)
        {
            Debug.Log("一つ目");
            SetPosition(_keyPoseList[i], _changePos[_keyPoseList[i]]);    //キーフレームのモデルを表示
        }
    }

    public void SetPosition(int frame, Vector3[] pos_list)
    {
        GameObject humanoid = Instantiate(_humanoidModel, new Vector3(0, 0, frame * _frameInterval), Quaternion.identity);
        humanoid.name = frame + "_frame_model";
        _keyPoseModel.Add(humanoid);
        SetAnimationTransform setAnimationTransform = humanoid.GetComponent<SetAnimationTransform>();
        setAnimationTransform.SetPartTransform(frame, pos_list);
        _keyPoseModelCount++;
    }

}
