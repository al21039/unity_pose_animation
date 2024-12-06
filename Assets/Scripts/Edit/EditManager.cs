using System.Collections.Generic;
using UnityEngine;

public class EditManager : MonoBehaviour
{
    [SerializeField] private GameObject _humanoidModel;

    [SerializeField] private PositionMover _positionMover;
    [SerializeField] private UIListener _uiListener;
    [SerializeField] private LineInterpolation _lineInterpolation;
    [SerializeField] private GameObject _createModel;
    [SerializeField] private ScrollViewButton _scrollViewButton;
    [SerializeField] private GameObject _cubePrefab;
    [SerializeField] private GameObject _celllingPrefab;
    [SerializeField] private HeightInterpolationer _heightInterpolationer;
    [SerializeField] private QuaternionInterpolationer _rotationInterpolationer;

    private Dictionary<int, Vector3[]> _changePos = new Dictionary<int, Vector3[]>();    //変更後の位置　　　　最初から曲線に変更
    private Dictionary<int, Quaternion[]> _changeRot = new Dictionary<int, Quaternion[]>();
    private List<int> _keyPoseList = new List<int>();                                    //キーポーズのリスト　　　後からJSONファイルのやつ追加できるように
    private List<float> _hipHeight = new List<float>();

    private List<GameObject> _keyPoseModel = new List<GameObject>();
    private List<GameObject> _keyCube = new List<GameObject>();
    private float _frameInterval = 0.30f;
    private int _totalFrames;
    private CreateNewAnim _createNewAnim;
    private string _startDate;

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

    public Dictionary<int, Quaternion[]> ChangeRot
    { 
        get
        {
            return _changeRot;
        }
        set
        {
            _changeRot = value;
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

    public int GetLastKeyPoseFrame()
    {
        return _keyPoseList[_keyPoseList.Count - 1];
    }

    public List<float> HipHeight
    {
        get
        {
            return _hipHeight;
        }
        set
        {
            _hipHeight = value;
        }
    }

    public void DeleteKeyPose(int index)
    {
        int listNo = _keyPoseList.IndexOf(index);
        _keyPoseList.RemoveAt(listNo);
        _keyPoseModel.RemoveAt(listNo);
        _lineInterpolation.InterpolationAllLine();
    }

    public void SetFrameHipHeight(int frame, float hipHeight)
    {
        _heightInterpolationer.Interpolation(frame, hipHeight);
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
        Camera.main.transform.position = new Vector3(5.0f, 2.0f, 20.0f);
        Camera.main.transform.rotation = Quaternion.Euler(15.0f, -90.0f, 0.0f);
        _startDate = LandmarkManager.GetInstance().StartDate;
        ChangePos = LandmarkManager.GetInstance().CSVLandmarkPositions;
        ChangeRot = LandmarkManager.GetInstance().CSVLandmarkRotations;
        _keyPoseList = LandmarkManager.GetInstance().KeyPoseList;
        _totalFrames = LandmarkManager.GetInstance().TotalFrame;
        _hipHeight = LandmarkManager.GetInstance().HipHeight;
        
        for (int i = 0; i < _keyPoseList.Count; i++)
        {
            SetPosition(_keyPoseList[i], _changePos[_keyPoseList[i]], _changeRot[_keyPoseList[i]], _hipHeight[  _keyPoseList[i]]);    //キーフレームのモデルを表示
        }

        Spline.GetInstance().SerializeSpline(_totalFrames);

        for (int i = 0; i < _totalFrames; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Spline.GetInstance().SetSpline(j, i, _changePos[i][j] + new Vector3(0, 0, i * _frameInterval));
            }
        }

        _scrollViewButton.LoadImagesFromFolder();
        _lineInterpolation.InterpolationAllLine();
        _uiListener.ChangeUIDisplay(true);
        for (int i = 0; i < 4; i++)
        {
            Spline.GetInstance().SwitchSplineDisplay(i);
        }
    }

    public void SetPosition(int frame, Vector3[] posList, Quaternion[] posRot, float hipHeight)
    {
        GameObject humanoid = Instantiate(_humanoidModel, new Vector3(0.0f, hipHeight - 1.0f, frame * _frameInterval), Quaternion.identity);
        GameObject cube = Instantiate(_cubePrefab, new Vector3(1.5f, 0.387f, frame * _frameInterval), Quaternion.identity);
        cube.transform.parent = humanoid.transform;
        humanoid.name = frame + "_frame_model";
        _keyPoseModel.Add(humanoid);
        SetAnimationTransform setAnimationTransform = humanoid.GetComponent<SetAnimationTransform>();
        setAnimationTransform.SetPartTransform(frame, posList, posRot, hipHeight);
    }

    public void SetJsonPosition(int frame, Vector3[] posList, int listIndex, Quaternion[] posRot)
    {
        GameObject humanoid = Instantiate(_humanoidModel, new Vector3(0, 0, frame * _frameInterval), Quaternion.identity);
        GameObject cube = Instantiate(_cubePrefab, new Vector3(1.5f, 0.387f, frame * _frameInterval), Quaternion.identity);
        cube.transform.parent = humanoid.transform;
        humanoid.name = frame + "_frame_model";
        _keyPoseModel.Insert(listIndex, humanoid);

        _rotationInterpolationer.Interpolation(listIndex);

        SetAnimationTransform setAnimationTransform = humanoid.GetComponent<SetAnimationTransform>();
        setAnimationTransform.SetPartTransform(frame, posList, posRot, 1.0f);
    }

    public void DisplayNewAnimation()
    {
        HumanPose[] _keyPoseHumanPose = new HumanPose[_keyPoseList.Count];
        for (int i = 0; i < _keyPoseModel.Count; i++)
        {
            SetAnimationTransform setAnimationTransform = _keyPoseModel[i].GetComponent<SetAnimationTransform>();
            _keyPoseHumanPose[i] = setAnimationTransform.GetKeyPoseMuscle(_keyPoseList[i]);
        }

        Camera.main.transform.position = new Vector3(0.0f, 1.32f, 3.62f);
        Camera.main.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

        for (int i = 0; i < 4; i++)
        {
            Spline.GetInstance().SwitchSplineDisplay(i);
        }
        _uiListener.ChangeUIDisplay(false);
        _uiListener.ChangeScrollDisplay(false);
        _positionMover.DeleteIndirectOption();

        for (int i = _keyPoseModel.Count - 1; i >= 0; i--)
        {
            Destroy(_keyPoseModel[i]);
        }

        GameObject createAnimationModel = Instantiate(_createModel, Vector3.zero, Quaternion.identity);

        
        /*
        GameObject created_model = Instantiate(_createModel, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        _createNewAnim = created_model.GetComponent<CreateNewAnim>();

        _createNewAnim.CreateNewAnimation(_keyPoseHumanPose, _keyPoseList);
        */
    }
}
