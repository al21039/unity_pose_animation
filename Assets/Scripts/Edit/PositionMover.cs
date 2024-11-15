using UnityEngine;
using UnityEngine.UI;

public class PositionMover : MonoBehaviour
{
    [SerializeField] private GameObject[] _indirectOptions;
    [SerializeField] private GameObject _spherePrefab;
    [SerializeField] private Dropdown _dropDown;
    [SerializeField] private Slider _slider;
    [SerializeField] private SearchEndPoint _searchEndPoint;
    [SerializeField] private LineInterpolation _lineInterpolation;

    private int _selectPositionID = -1;
    private bool _isDisplay = false;                              //球が表示されているかいないか
    private GameObject _indirectSphere;
    private Camera _mainCamera;
    private bool _isLineDisplay = false;

    private GameObject _selectedIKObject; //マウスで選択したIKのオブジェクト
    private GameObject _selectedKeyModel;
    private Transform _selectedTargetObject;
    private Transform _selectedTargetAnker;
    private Vector3 _IKTargetOffset;
    private Vector3 _IndirectSphereOffset;
    private bool _isSphereMoved = false;
    private string _selectedIKObjectName;
    private string _selectedKeyModelName;
    private string _selectedFrame;
    private CreateNewAnim _set_new_position;
    private string _selecterPositonName; //ドロップダウンで選択した部位

    private bool _touchIndirectSphere = false;
    private Vector3 _sphereDefaultPosition;
    private float _indeirectEffectiveGainValue = 0;

    public int SelectPositionID
    {
        get
        {
            return _selectPositionID;
        }
        set
        {
            _selectPositionID = value;
        }
    }

    public void DropdownValueChanged(Dropdown change)
    {
        int selectPosition = change.value - 1;
        SelectPositionID = selectPosition;
    }

    private void Start()
    {
        _dropDown.onValueChanged.AddListener(delegate { DropdownValueChanged(_dropDown); });
    }

    private void Update()
    {
        //オブジェクトをマウスで移動
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    //IKのオブジェクトを直接操作
                    if (hit.collider.CompareTag("IKObject"))
                    {
                        _selectedIKObject = hit.collider.gameObject;
                        _selectedIKObjectName = hit.collider.gameObject.name;
                        _selectedFrame = hit.collider.gameObject.transform.root.gameObject.name.Replace("_frame_model", "");
                        _IKTargetOffset = _selectedIKObject.transform.position - GetMouseWorldPos(false);
                    }

                    else if (hit.collider.CompareTag("KeyModel"))
                    {
                        if (_selectedKeyModel != null)
                        {
                            ReplaceSpherePosition();
                        }
                        _selectedKeyModel = hit.collider.gameObject.transform.parent.gameObject; //元のモデルフレームを取得
                        _selectedKeyModelName = _selectedKeyModel.name; //フレームのモデルの名前
                    }

                    //フレームのモデルがある時かつ、ドロップダウンを選んでいるときに球を触ったら
                    else if (hit.collider.CompareTag("OperatingSphere"))
                    {
                        _indeirectEffectiveGainValue = _slider.value;
                        _touchIndirectSphere = true;　//球を触っている判定
                        if (_selectedKeyModel != null && _selectPositionID != -1)
                        {
                            _sphereDefaultPosition = _indirectSphere.transform.position;
                            _IndirectSphereOffset = _indirectSphere.transform.position - GetMouseWorldPos(true);
                            _isSphereMoved = true;
                            _selectedTargetObject = _searchEndPoint.ReturnEndPoint(_selectPositionID, _selectedKeyModel); //関節のアンカーを取得
                            _selectedTargetAnker = _searchEndPoint.ReturnEndAnker(_selectPositionID, _selectedKeyModel); //関節のアンカーを取得
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            //移動した分を補間する処理 オブジェクトがあるなら
            if (_selectedIKObjectName != null && _selectedFrame != null)
            {
                ReplacePosition(_selectedIKObjectName, _selectedFrame, _selectedIKObject);
                _selectedIKObject = null;
                _selectedIKObjectName = null;
                _selectedFrame = null;
            }

            else if (_touchIndirectSphere)
            {
                if (_isSphereMoved)
                {
                    ReplacePosition(_selectedKeyModel, _selectPositionID, _selectedTargetObject);
                }
                _touchIndirectSphere = false;
                _isSphereMoved = false;
            }

        }

        if (_selectedIKObject != null)
        {
            _selectedIKObject.transform.position = GetMouseWorldPos(false) + _IKTargetOffset;
        }

        if (_isSphereMoved)
        {
            _indirectSphere.transform.position = GetMouseWorldPos(true) + _IndirectSphereOffset;
            _selectedTargetAnker.position += (_indirectSphere.transform.position - _sphereDefaultPosition) * _indeirectEffectiveGainValue;
            _sphereDefaultPosition = _indirectSphere.transform.position;
        }

    }


    private void ReplaceSpherePosition()
    {
        if (_indirectSphere != null) 
        {
            Destroy(_indirectSphere);
        }
        
        Vector3 _sphereSpawnPosition = Camera.main.transform.position + _mainCamera.transform.forward * 3.0f;       //カメラより少し前の位置
        _indirectSphere = Instantiate(_spherePrefab, _sphereSpawnPosition, Quaternion.identity);
    }

    public void OnClickedIndirectButton()
    {
        _isDisplay = !_isDisplay;

        _indirectOptions[0].SetActive(_isDisplay);
        _indirectOptions[1].SetActive(_isDisplay);

        if(_isDisplay)
        {
            _mainCamera = Camera.main;
            Vector3 _sphereSpawnPosition = _mainCamera.transform.position + _mainCamera.transform.forward * 3.0f;       //カメラより少し前の位置

            _indirectSphere = Instantiate(_spherePrefab, _sphereSpawnPosition, Quaternion.identity);
        }
        else
        {
            Destroy(_indirectSphere);
        }
    }

    public void DeleteIndirectOption()
    {
        _indirectOptions[0].SetActive(false);
        _indirectOptions[1].SetActive(false);
        if (_indirectSphere != null)
        {
            Destroy(_indirectSphere);
        }
    }

    //マウスの座標取得
    private Vector3 GetMouseWorldPos(bool sphereOperate)
    {
        Vector3 mousePoint = Input.mousePosition;
        if (sphereOperate)
        {
            mousePoint.z = Camera.main.WorldToScreenPoint(_indirectSphere.transform.position).z;
        }
        else
        {
            mousePoint.z = Camera.main.WorldToScreenPoint(_selectedIKObject.transform.position).z;
        }
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    //マウスで動かした後の修正 直接操作
    private void ReplacePosition(string moved_obj, string moved_frame, GameObject movedObject)
    {
        int positionID = -1;
        string frame_obj = moved_frame + "_frame_model";
        Vector3 targetPosition = new Vector3(0.0f, 0.0f, 0.0f);

        if (moved_obj == "HandLTarget")
        {
            positionID = 0;
        }
        else if (moved_obj == "HandRTarget")
        {
            positionID = 1;
        }
        else if (moved_obj == "FootLTarget")
        {
            positionID = 2;
        }
        else if (moved_obj == "FootRTarget")
        {
            positionID = 3;
        }
        else if (moved_obj == "ElbowLTarget")
        {
            positionID = 4;
        }
        else if (moved_obj == "ElbowRTarget")
        {
            positionID = 5;
        }
        else if (moved_obj == "KneeLTarget")
        {
            positionID = 6;
        }
        else if (moved_obj == "KneeRTarget")
        {
            positionID = 7;
        }
        else
        {
            Debug.Log("obj null");
            return;
        }
        Transform hit_target = null;
        hit_target = _searchEndPoint.ReturnEndPoint(positionID, frame_obj);

        if (hit_target != null)
        {
            targetPosition = hit_target.position;
        }
        else
        {
            Debug.Log("dont catch");
            return;
        }
        int frame;
        try
        {
            frame = int.Parse(moved_frame);
        }
        catch
        {
            return;
        }
        _lineInterpolation.SetSplineAndJointPosition(frame, targetPosition, positionID);
    }

    //マウスで動かした後の修正 関節操作
    private void ReplacePosition(GameObject rootObject, int positionID, Transform moved_position)
    {
        int frameNumber = int.Parse(rootObject.name.Replace("_frame_model", ""));
        Vector3 targetPosition = new Vector3(0.0f, 0.0f, 0.0f);

        Transform hitTarget = null;
        hitTarget = moved_position;

        if (hitTarget != null)
        {
            targetPosition = hitTarget.position;
        }
        else
        {
            Debug.Log("Dont Catch");
            return;
        }
        _lineInterpolation.SetSplineAndJointPosition(frameNumber, targetPosition, positionID);
    }
}
