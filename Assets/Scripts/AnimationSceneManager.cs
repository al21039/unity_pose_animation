using System.Collections.Generic;
using UnityEngine;

public class AnimationSceneManager : MonoBehaviour
{
    [SerializeField] GameObject humanoid_model;

    //線
    [SerializeField] private GameObject _left_hand_spline;
    [SerializeField] private GameObject _right_hand_spline;
    [SerializeField] private GameObject _left_foot_spline;
    [SerializeField] private GameObject _right_foot_spline;

    [SerializeField] private GameObject _camera_obj;　//カメラ

    //編集時のボタン
    [SerializeField] private GameObject _left_hand_button;
    [SerializeField] private GameObject _right_hand_button;
    [SerializeField] private GameObject _left_foot_button;
    [SerializeField] private GameObject _right_foot_button;

    //編集を反映したアニメ動作チェック
    [SerializeField] private GameObject _create_anim_button;
    [SerializeField] private GameObject _indirectButton;
    [SerializeField] private GameObject _positionDropDown;

    //編集後に用いるモデルのプレハブ
    [SerializeField] private GameObject _created_model;


    [SerializeField] private GameObject _spherePrefab;
    [SerializeField] private ReplacePosition _replaceScript;
    [SerializeField] private SearchEndPoint _searchEndPointScript;

    private GameObject _keypose_model;
    private LineRenderer _left_hand_line_renderer;
    private LineRenderer _right_hand_line_renderer;
    private LineRenderer _left_foot_line_renderer;
    private LineRenderer _right_foot_line_renderer;

    private Camera _camera;
    private GameObject _selectedIKObject; //マウスで選択したIKのオブジェクト
    private GameObject _selectedKeyModel;
    private Transform _selectedTargetObject;
    private Transform _selectedTargetAnker;
    private Vector3 _IKTargetOffset;
    private Vector3 _IndirectSphereOffset;
    private bool _isSphereMoved = false; 
    private string _selectedIKObjectName;
    private string _selectedKeyModelName;
    private string _selected_frame;
    private SetNewPosition _set_new_position;
    private string _selecterPositonName; //ドロップダウンで選択した部位

    private Dictionary<int, Vector3[]> _modelPos = new Dictionary<int, Vector3[]>();
    private Dictionary<int, Vector3[]> _changedPos = new Dictionary<int, Vector3[]>();
    private List<int> _keyPoseList = new List<int>();
    private int _totalFrame;
    private GameObject indirectSphere;
    private int _selectPositionID = -1;

    private bool _touchIndirectSphere = false;
    private Vector3 _sphereDefaultPosition;

    public void SetSelectPositionID(int positionID)
    {
        _selectPositionID = positionID;
    }

    public void SetModelPos(Dictionary<int, Vector3[]> modelPos)
    {
        _modelPos = modelPos;
    }
    public void SetChangedPos(Dictionary<int, Vector3[]> changedPos)
    {
        _changedPos = changedPos;
    }
    public Dictionary<int, Vector3[]> GetModelPos() => _modelPos;

    public Dictionary<int, Vector3[]> GetChangedPos() => _changedPos;

    public void SetKeyPoseList(List<int> keyPoseList)
    {
        _keyPoseList = keyPoseList;
    }

    public List<int> GetKeyPoseList() => _keyPoseList;

    public void SetTotalFrame(int totalFrame)
    {
        _totalFrame = totalFrame;
    }
 
    // Start is called before the first frame update
    void Start()
    {
        _left_hand_line_renderer = _left_hand_spline.GetComponent<LineRenderer>();
        _right_hand_line_renderer = _right_hand_spline.GetComponent<LineRenderer>();
        _left_foot_line_renderer = _left_foot_spline.GetComponent<LineRenderer>();
        _right_foot_line_renderer = _right_foot_spline.GetComponent<LineRenderer>();
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //オブジェクトをマウスで移動
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    //IKのオブジェクトを直接操作
                    if (hit.collider.CompareTag("IKObject"))
                    {
                        _selectedIKObject = hit.collider.gameObject;
                        _selectedIKObjectName = hit.collider.gameObject.name;
                        _selected_frame = hit.collider.gameObject.transform.root.gameObject.name.Replace("_frame_model", "");
                        _IKTargetOffset = _selectedIKObject.transform.position - GetMouseWorldPos(false);
                    }

                    else if(hit.collider.CompareTag("KeyModel"))
                    {
                        _selectedKeyModel = hit.collider.gameObject.transform.parent.gameObject; //元のモデルフレームを取得
                        Debug.Log(_selectedKeyModel);
                        _selectedKeyModelName = _selectedKeyModel.name; //フレームのモデルの名前
                    }

                    //フレームのモデルがある時かつ、ドロップダウンを選んでいるときに球を触ったら
                    else if(hit.collider.CompareTag("OperatingSphere"))
                    {
                        _touchIndirectSphere = true;　//球を触っている判定
                        Debug.Log("touch");
                        if (_selectedKeyModel != null && _selectPositionID != -1)
                        {
                            _sphereDefaultPosition = indirectSphere.transform.position;
                            _IndirectSphereOffset = indirectSphere.transform.position - GetMouseWorldPos(true);
                            _isSphereMoved = true;
                            _selectedTargetObject = _searchEndPointScript.ReturnEndPoint(_selectPositionID, _selectedKeyModel); //関節のアンカーを取得
                            _selectedTargetAnker = _searchEndPointScript.ReturnEndAnker(_selectPositionID, _selectedKeyModel); //関節のアンカーを取得
                            Debug.Log(_selectedTargetObject.gameObject.name);
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            //移動した分を補間する処理 オブジェクトがあるなら
            if (_selectedIKObjectName != null && _selected_frame != null)
            {
                ReplacePosition(_selectedIKObjectName, _selected_frame, _selectedIKObject);
                _selectedIKObject = null;
                _selectedIKObjectName = null;
                _selected_frame = null;
            }

            else if(_touchIndirectSphere)
            {
                if (_isSphereMoved)
                {
                    ReplacePosition(_selectedKeyModel, _selectPositionID, _selectedTargetObject);
                }
                _touchIndirectSphere = false;
                _isSphereMoved = false;
            }
            
        }

        if(_selectedIKObject != null)
        {
            _selectedIKObject.transform.position = GetMouseWorldPos(false) + _IKTargetOffset;
        }

        if(_isSphereMoved)
        {
            indirectSphere.transform.position = GetMouseWorldPos(true) + _IndirectSphereOffset; 
            _selectedTargetAnker.position += indirectSphere.transform.position - _sphereDefaultPosition;
            _sphereDefaultPosition = indirectSphere.transform.position;
        }
    }

    public void SetPosition(int frame, Vector3[] pos_list)
    {
        _keypose_model = Instantiate(humanoid_model, new Vector3(0, 0, frame * 0.3f), Quaternion.identity);
        _keypose_model.name = frame + "_frame_model";
        SetAnimationTransform setAnimationTransform = _keypose_model.GetComponent<SetAnimationTransform>();
        setAnimationTransform.SetPartTransform(frame, pos_list);
    }

    //線描画
    public void SetSpline()
    {
        _camera_obj.transform.position = new Vector3(5.0f, 2.0f, 20.0f);
        _camera_obj.transform.rotation = Quaternion.Euler(15.0f, -90.0f, 0.0f);
        _left_hand_spline.SetActive(true);
        _right_hand_spline.SetActive(true);
        _left_foot_spline.SetActive(true);
        _right_foot_spline.SetActive(true);

        _left_hand_line_renderer.positionCount = _totalFrame;
        _right_hand_line_renderer.positionCount = _totalFrame;
        _left_foot_line_renderer.positionCount = _totalFrame;
        _right_foot_line_renderer.positionCount = _totalFrame;

        for (int i = 0; i < _totalFrame; i++)
        {
            _left_hand_line_renderer.SetPosition(i, _modelPos[i][0] + new Vector3(0, 0, i * 0.3f));
            _right_hand_line_renderer.SetPosition(i, _modelPos[i][1] + new Vector3(0, 0, i * 0.3f));

            //Instantiate(_spherePrefab, _modelPos[i][1] + new Vector3(0, 0, i * 0.3f), Quaternion.identity);

            _left_foot_line_renderer.SetPosition(i, _modelPos[i][2] + new Vector3(0, 0, i * 0.3f));
            _right_foot_line_renderer.SetPosition(i, _modelPos[i][3] + new Vector3(0, 0, i * 0.3f));
        }
        _left_hand_button.SetActive(true);
        _right_hand_button.SetActive(true);
        _left_foot_button.SetActive(true);
        _right_foot_button.SetActive(true);
        _create_anim_button.SetActive(true);
        _indirectButton.SetActive(true);
    }

    public void DisplayLeftHandSpline()
    {
        _left_hand_spline.SetActive(!_left_hand_spline.activeSelf);
    }
    public void DisplayRightHandSpline()
    {
        _right_hand_spline.SetActive(!_right_hand_spline.activeSelf);
    }
    public void DisplayLeftFootSpline()
    {
        _left_foot_spline.SetActive(!_left_foot_spline.activeSelf);
    }
    public void DisplayRightFootSpline()
    {
        _right_foot_spline.SetActive(!_right_foot_spline.activeSelf);
    }
    public void DisplayIndirectSphere()
    {
        Vector3 spawnPosition = _camera.transform.position + _camera.transform.forward * 3.0f;
        if (indirectSphere == null)
        {
            indirectSphere = Instantiate(_spherePrefab, spawnPosition, Quaternion.identity);
        }
        _positionDropDown.SetActive(true);
    }
    public void DestoryIndirectSphere()
    {
        if (indirectSphere != null)
        {
            Destroy(indirectSphere);
        }
        _positionDropDown.SetActive(false);
    }
    public void DisplayNewAnimation()
    {
        _camera_obj.transform.position = new Vector3(0.0f, 1.32f, 3.62f);
        _camera_obj.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        _left_hand_spline.SetActive(false);
        _right_hand_spline.SetActive(false);
        _left_foot_spline.SetActive(false);
        _right_foot_spline.SetActive(false);

        _left_hand_button.SetActive(false);
        _right_hand_button.SetActive(false);
        _left_foot_button.SetActive(false);
        _right_foot_button.SetActive(false);
        _create_anim_button.SetActive(false);
        _indirectButton.SetActive(false);
        _positionDropDown.SetActive(false);
        if(indirectSphere != null)
        {
            Destroy(indirectSphere); 
        }

        for (int i = 0; i < _keyPoseList.Count; i++)
        {
            string tmp_name = _keyPoseList[i].ToString() + "_frame_model";
            GameObject destroy_obj = GameObject.Find(tmp_name);
            Destroy(destroy_obj);
        }
        GameObject created_model = Instantiate(_created_model, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        _set_new_position = created_model.GetComponent<SetNewPosition>();
        _set_new_position.SetStatus(_modelPos, _totalFrame);

    }

    //マウスの座標取得
    private Vector3 GetMouseWorldPos(bool sphereOperate)
    {
        Vector3 mousePoint = Input.mousePosition;
        if (sphereOperate)
        {
            mousePoint.z = _camera.WorldToScreenPoint(indirectSphere.transform.position).z;
        }
        else
        {
            mousePoint.z = _camera.WorldToScreenPoint(_selectedIKObject.transform.position).z;
        }
        return _camera.ScreenToWorldPoint(mousePoint);
    }

    //マウスで動かした後の修正 直接操作
    private void ReplacePosition(string moved_obj, string moved_frame, GameObject movedObject)
    {
        LineRenderer lineRenderer = null;
        int positionID = -1;
        string frame_obj = moved_frame + "_frame_model";
        Vector3 targetPosition = new Vector3(0.0f, 0.0f, 0.0f);

        if (moved_obj == "HandLTarget")
        {
            lineRenderer = _left_hand_line_renderer;
            positionID = 0;
        }
        else if (moved_obj == "HandRTarget")
        {
            lineRenderer = _right_hand_line_renderer;
            positionID = 1;
        }
        else if (moved_obj == "FootLTarget")
        {
            lineRenderer = _left_foot_line_renderer;
            positionID = 2;
        }
        else if (moved_obj == "FootRTarget")
        {
            lineRenderer = _right_foot_line_renderer;
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
        hit_target = _searchEndPointScript.ReturnEndPoint(positionID, frame_obj);

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
        SetSplineAndJointPosition(frame, targetPosition, positionID, lineRenderer);
    }

    //マウスで動かした後の修正 関節操作
    private void ReplacePosition(GameObject rootObject, int positionID, Transform moved_position)
    {
        LineRenderer lineRenderer = null;
        int frameNumber = int.Parse(rootObject.name.Replace("_frame_model", ""));
        Debug.Log(frameNumber);
        Vector3 targetPosition = new Vector3(0.0f, 0.0f, 0.0f);
        
        switch(positionID)
        {
            case 0:
                lineRenderer = _left_hand_line_renderer;
                break;

            case 1:
                lineRenderer = _right_hand_line_renderer;
                break;

            case 2:
                lineRenderer = _left_foot_line_renderer;
                break;

            case 3:
                lineRenderer= _right_foot_line_renderer;
                break;

            default :
                break;
        }

        Transform hitTarget = null;
        hitTarget = moved_position;

        if(hitTarget != null)
        {
            targetPosition = hitTarget.position;
        }
        else
        {
            Debug.Log("Dont Catch");
            return;
        }
        SetSplineAndJointPosition(frameNumber, targetPosition, positionID, lineRenderer);
    }

    private void SetSplineAndJointPosition(int frame, Vector3 targetPosition, int positionID, LineRenderer lineRenderer)
    {
        if (_keyPoseList.Contains(frame))
        {
            //編集したキーフレームが１番目なら
            if (frame == 0)
            {
                _replaceScript.SetFirstFrameLinePos(targetPosition, positionID, lineRenderer);
            }
            //編集したキーフレームが最後なら
            else if (frame == _keyPoseList[_keyPoseList.Count - 1])
            {
                _replaceScript.SetLastFrameLinePos(targetPosition, positionID, lineRenderer);
            }
            //編集したキーフレームが２番目なら
            else if (frame == _keyPoseList[1])
            {
                _replaceScript.SetSecondFrameLinePos(targetPosition, positionID, lineRenderer, frame);
            }
            //編集したキーフレームが最後から２番目なら
            else if (frame == _keyPoseList[_keyPoseList.Count - 2])
            {
                _replaceScript.SetSecondToLastFrameLinePos(targetPosition, positionID, lineRenderer, frame);
            }
            //編集したキーフレームが上記以外なら
            else
            {
                _replaceScript.SetOtherFramesLinePos(targetPosition, positionID, lineRenderer, frame);
            }
        }
    }
}
