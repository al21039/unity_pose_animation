using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    [SerializeField] private Canvas _worldCanvas;
    [SerializeField] private GameObject _textPrefab;

    private Dictionary<int, Vector3[]> _changePos = new Dictionary<int, Vector3[]>();    //変更後の位置　　　　最初から曲線に変更
    private Dictionary<int, Quaternion[]> _changeRot = new Dictionary<int, Quaternion[]>();
    private List<int> _keyPoseList = new List<int>();                                    //キーポーズのリスト　　　後からJSONファイルのやつ追加できるように
    private List<float> _hipHeight = new List<float>();
    private List<Quaternion> _modelEntireRot = new List<Quaternion>();

    private List<GameObject> _keyPoseModel = new List<GameObject>();
    private List<GameObject> _keyCube = new List<GameObject>();
    private float _frameInterval = 0.30f;
    private int _totalFrames;
    private CreateNewAnim _createNewAnim;
    private string _startDate;
    private GameObject _createNewModel;

    private List<GameObject> _frameTextObj = new List<GameObject>();

    public List<Quaternion> ModelEntireRot
    {
        get
        {
            return _modelEntireRot;
        }
        set
        {
            _modelEntireRot = value;
        }
    }

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
        _lineInterpolation.InterpolationAllLine(false);
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
        Camera.main.transform.position = new Vector3(5.0f, 1.7f, 20.0f);
        Camera.main.transform.rotation = Quaternion.Euler(5.0f, -90.0f, 0.0f);
        _startDate = LandmarkManager.GetInstance().StartDate;
        ChangePos = LandmarkManager.GetInstance().CSVLandmarkPositions;
        ChangeRot = LandmarkManager.GetInstance().CSVLandmarkRotations;
        _keyPoseList = LandmarkManager.GetInstance().KeyPoseList;
        _totalFrames = LandmarkManager.GetInstance().TotalFrame;
        _hipHeight = LandmarkManager.GetInstance().HipHeight;
        
        for (int i = 0; i < _keyPoseList.Count; i++)
        {
            SetPosition(_keyPoseList[i], _changePos[_keyPoseList[i]], _changeRot[_keyPoseList[i]], _hipHeight[_keyPoseList[i]]);    //キーフレームのモデルを表示
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
        _scrollViewButton.InitializeValueInputField(_keyPoseList[1], _keyPoseList[_keyPoseList.Count - 2]);
        _lineInterpolation.InterpolationAllLine(false);
        _uiListener.ChangeUIDisplay(true);
        for (int i = 0; i < 4; i++)
        {
            Spline.GetInstance().SwitchSplineDisplay(i);
        }
    }

    public void SetPosition(int frame, Vector3[] posList, Quaternion[] posRot, float hipHeight)
    {
        GameObject frameText = Instantiate(_textPrefab, _worldCanvas.transform);
        frameText.transform.localPosition = new Vector3(frame * 6, 100, 0);
        TextMeshProUGUI textMeshPro = frameText.GetComponent<TextMeshProUGUI>();
        textMeshPro.text = frame.ToString();
        _frameTextObj.Add(frameText);

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
        Debug.Log("画像から追加");
        GameObject humanoid = Instantiate(_humanoidModel, new Vector3(0, 0, frame * _frameInterval), Quaternion.identity);
        GameObject cube = Instantiate(_cubePrefab, new Vector3(1.5f, 0.387f, frame * _frameInterval), Quaternion.identity);
        cube.transform.parent = humanoid.transform;
        humanoid.name = frame + "_frame_model";
        _keyPoseModel.Insert(listIndex, humanoid);

        _rotationInterpolationer.Interpolation(listIndex);

        SetAnimationTransform setAnimationTransform = humanoid.GetComponent<SetAnimationTransform>();
        setAnimationTransform.SetPartTransform(frame, posList, posRot, 1.0f);
    }

    //編集したアニメーションを表示
    public void DisplayNewAnimation()
    {
        HumanPose[] _keyPoseHumanPose = new HumanPose[_keyPoseList.Count];
        for (int i = 0; i < _keyPoseModel.Count; i++)
        {
            SetAnimationTransform setAnimationTransform = _keyPoseModel[i].GetComponent<SetAnimationTransform>();
            _keyPoseHumanPose[i] = setAnimationTransform.GetKeyPoseMuscle(_keyPoseList[i]);
        }

        foreach (var obj in _frameTextObj)
        {
            obj.SetActive(false);
        }

        Camera.main.transform.position = new Vector3(10.0f, 1.32f, 3.62f);
        Camera.main.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

        /*
        for (int i = 0; i < 4; i++)
        {
            Spline.GetInstance().SwitchSplineDisplay(i);
        }
        _positionMover.DeleteIndirectOption();

        for (int i = _keyPoseModel.Count - 1; i >= 0; i--)
        {
            _keyPoseModel[i].SetActive(false);
        }
        */

        _createNewModel = Instantiate(_createModel, new Vector3(10, 0, 0), Quaternion.identity);
    }

    public void ChangeToNewValue(int frame, Vector3[] positionArray, Quaternion[] rotationArray)
    {
        for (int i = 0; i < positionArray.Length; i++)
        {
            positionArray[i] -= new Vector3(0, 0, frame * _frameInterval);
        }

        _changePos[frame] = positionArray;
        _changeRot[frame] = rotationArray;

        int listIndex = _keyPoseList.IndexOf(frame);

        _lineInterpolation.InterpolationJson(listIndex);
        _rotationInterpolationer.Interpolation(listIndex);
    }

    public void ChangeToEntireRot(int frame, Quaternion entireRot)
    {
        _modelEntireRot[frame] = entireRot;

        int listIndex = _keyPoseList.IndexOf(frame);

        _rotationInterpolationer.EntireInterpolation(listIndex);

    }

    public void BackToEditInterface()
    {
        Camera.main.transform.position = new Vector3(5.0f, 1.7f, 20.0f);
        Camera.main.transform.rotation = Quaternion.Euler(5.0f, -90.0f, 0.0f);
        /*
        for (int i = 0; i < 4; i++)
        {
            Spline.GetInstance().SwitchSplineDisplay(i);
        }
        */

        foreach (var obj in _frameTextObj)
        {
            obj.SetActive(true);
        }

        /*
        for (int i = _keyPoseModel.Count - 1; i >= 0; i--)
        {
            _keyPoseModel[i].SetActive(true);
        }
        */

        if (_createNewModel != null)
        {
            Destroy(_createNewModel);
        }
    }
}
