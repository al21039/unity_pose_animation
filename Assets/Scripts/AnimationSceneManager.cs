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

    //編集後に用いるモデルのプレハブ
    [SerializeField] private GameObject _created_model;


    [SerializeField] private GameObject _spherePrefab;
    [SerializeField] private ReplacePosition _replaceScript;

    private GameObject _keypose_model;
    private LineRenderer _left_hand_line_renderer;
    private LineRenderer _right_hand_line_renderer;
    private LineRenderer _left_foot_line_renderer;
    private LineRenderer _right_foot_line_renderer;

    private Camera _camera;
    private GameObject _selectedObject;
    private Vector3 _offset;
    private string _selectedObeject_name;
    private string _selected_frame;
    private SetNewPosition _set_new_position;


    private Dictionary<int, Vector3[]> _modelPos = new Dictionary<int, Vector3[]>();
    private Dictionary<int, Vector3[]> _changedPos = new Dictionary<int, Vector3[]>();
    private List<int> _keyPoseList = new List<int>();
    private int _totalFrame;

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

    public 


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
                    if (hit.collider.CompareTag("Movable"))
                    {
                        _selectedObject = hit.collider.gameObject;
                        _selectedObeject_name = hit.collider.gameObject.name;
                        _selected_frame = hit.collider.gameObject.transform.root.gameObject.name.Replace("_frame_model", "");
                        _offset = _selectedObject.transform.position - GetMouseWorldPos();
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            //移動した分を補間する処理 オブジェクトがあるなら
            if (_selectedObeject_name != null && _selected_frame != null)
            {
                ReplacePosition(_selectedObeject_name, _selected_frame, _selectedObject.transform.position);
            }
            _selectedObject = null;
            _selectedObeject_name = null;
            _selected_frame = null;
        }

        if (_selectedObject != null)
        {
            _selectedObject.transform.position = GetMouseWorldPos() + _offset;
        }
    }

    //
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
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = _camera.WorldToScreenPoint(_selectedObject.transform.position).z;
        return _camera.ScreenToWorldPoint(mousePoint);
    }

    //マウスで動かした後の修正
    private void ReplacePosition(string moved_obj, string moved_frame, Vector3 moved_position)
    {
        LineRenderer lineRenderer = null;
        int numberOfPoints = -1;
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
        switch (positionID)
        {
            case 0:
                hit_target = GameObject.Find(frame_obj).transform.Find("Armature").Find("Hips").Find("Spine 1").Find("Spine 2").Find("Spine 3").Find("Left Shoulder").Find("Left Arm").Find("Left Forearm").Find("Left Hand");
                break;
            case 1:
                hit_target = GameObject.Find(frame_obj).transform.Find("Armature").Find("Hips").Find("Spine 1").Find("Spine 2").Find("Spine 3").Find("Right Shoulder").Find("Right Arm").Find("Right Forearm").Find("Right Hand");
                break;
            case 2:
                hit_target = GameObject.Find(frame_obj).transform.Find("Armature").Find("Hips").Find("Left Thigh").Find("Left Leg").Find("Left Foot");
                break;
            case 3:
                hit_target = GameObject.Find(frame_obj).transform.Find("Armature").Find("Hips").Find("Right Thigh").Find("Right Leg").Find("Right Foot");
                break;
            case 4:
                hit_target = GameObject.Find(frame_obj).transform.Find("Armature").Find("Hips").Find("Spine 1").Find("Spine 2").Find("Spine 3").Find("Left Shoulder").Find("Left Arm").Find("Left Forearm");
                break;
            case 5:
                hit_target = GameObject.Find(frame_obj).transform.Find("Armature").Find("Hips").Find("Spine 1").Find("Spine 2").Find("Spine 3").Find("Right Shoulder").Find("Right Arm").Find("Right Forearm");
                break;
            case 6:
                hit_target = GameObject.Find(frame_obj).transform.Find("Armature").Find("Hips").Find("Left Thigh").Find("Left Leg");
                break;
            case 7:
                hit_target = GameObject.Find(frame_obj).transform.Find("Armature").Find("Hips").Find("Right Thigh").Find("Right Leg");
                break;
        }

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

    Vector3 CatmullRomSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2.0f * p1) +
            (-p0 + p2) * t +
            (2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) * t2 +
            (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3
        );
    }
}
