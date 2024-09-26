using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSceneManager : MonoBehaviour
{
    [SerializeField] GameObject humanoid_model;

    [SerializeField] private GameObject _left_hand_spline;
    [SerializeField] private GameObject _right_hand_spline;
    [SerializeField] private GameObject _left_foot_spline;
    [SerializeField] private GameObject _right_foot_spline;
    [SerializeField] private GameObject _camera_obj;

    [SerializeField] private GameObject _left_hand_button;
    [SerializeField] private GameObject _right_hand_button;
    [SerializeField] private GameObject _left_foot_button;
    [SerializeField] private GameObject _right_foot_button;

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


    public Dictionary<int, Vector3[]> modelPos = new Dictionary<int, Vector3[]>();
    public List<int> KeyPose_List = new List<int>();
    public int total_frame;

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
                        _selected_frame = hit.collider.gameObject.transform.root.gameObject.name.Replace("_frame_model","");
                        _offset = _selectedObject.transform.position - GetMouseWorldPos();
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _selectedObject = null;
            //移動した分を補間する処理 オブジェクトがあるなら
            if(_selectedObeject_name != null && _selected_frame != null)
            {
                ReplacePosition(_selectedObeject_name, _selected_frame, _selectedObject.transform.position);
            }

            _selectedObeject_name = null;
            _selected_frame = null;
        }

        if (_selectedObject != null)
        {
            _selectedObject.transform.position = GetMouseWorldPos() + _offset;
        }
    }

    public void SetPosition(int frame, Vector3[] pos_list)
    {
        _keypose_model = Instantiate(humanoid_model, new Vector3(0, 0, frame * 0.3f), Quaternion.identity);
        _keypose_model.name = frame + "_frame_model";
        SetAnimationTransform setAnimationTransform = _keypose_model.GetComponent<SetAnimationTransform>();
        setAnimationTransform.SetPartTransform(frame, pos_list);
    }

    public void SetSpline()
    {
        _camera_obj.transform.position = new Vector3(4.0f, 3.0f, 13.0f);
        _camera_obj.transform.rotation = Quaternion.Euler(10.0f, -110.0f, -5.0f);
        _left_hand_spline.SetActive(true);
        _right_hand_spline.SetActive(true);
        _left_foot_spline.SetActive(true);
        _right_foot_spline.SetActive(true);

        _left_hand_line_renderer.positionCount = total_frame;
        _right_hand_line_renderer.positionCount = total_frame;
        _left_foot_line_renderer.positionCount = total_frame;
        _right_foot_line_renderer .positionCount = total_frame;

        for (int i = 0; i < total_frame; i++)
        {
            _left_hand_line_renderer.SetPosition(i, modelPos[i][0] + new Vector3(0, 0, i * 0.3f));
            _right_hand_line_renderer.SetPosition(i, modelPos[i][1] + new Vector3(0, 0, i * 0.3f));
            _left_foot_line_renderer.SetPosition(i, modelPos[i][2] + new Vector3(0, 0, i * 0.3f));
            _right_foot_line_renderer.SetPosition(i, modelPos[i][3] + new Vector3(0, 0, i * 0.3f));
        }
        _left_hand_button.SetActive(true);
        _right_hand_button.SetActive(true);
        _left_foot_button.SetActive(true);
        _right_foot_button.SetActive(true);
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
        LineRenderer line_renderer = null;
        int number_of_points = -1;
        int position_id = -1;

        if (moved_obj == "HandLTarget")
        {
            line_renderer = _left_hand_line_renderer;
            position_id = 0;
        } 
        else if(moved_obj == "HandRTarget")
        {
            line_renderer = _right_hand_line_renderer;
            position_id = 1;
        } 
        else if(moved_obj == "FootLTarget")
        {
            line_renderer = _left_foot_line_renderer;
            position_id = 2;
        } 
        else if(moved_obj == "FootRTarget")
        {
            line_renderer = _right_foot_line_renderer;
            position_id = 3;
        }
        else
        {
            Debug.Log("obj null");
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

        if(KeyPose_List.Contains(frame))
        {
            line_renderer.SetPosition(frame, moved_position);


            if(frame == 0) {
                number_of_points = KeyPose_List[1];
                Vector3[] points = new Vector3[number_of_points];
                points[0] = moved_position;

                for(int i = 1; i < number_of_points; i++) 
                {
                    points[i] = Vector3.Lerp(points[0], modelPos[i][position_id], (float)i / (number_of_points - 1));
                }

                for (int i = 0; i < number_of_points; i++)
                {
                    line_renderer.SetPosition(i, points[i]);
                }

            }
            else if(frame == KeyPose_List[KeyPose_List.Count - 1])
            {
                number_of_points = KeyPose_List[KeyPose_List.Count - 1] - KeyPose_List[KeyPose_List.Count - 2];
                Vector3[] points = new Vector3[number_of_points];
                points[number_of_points - 1] = moved_position;

                for (int i = 0; i < number_of_points - 1; i++)
                {
                    points[i] = Vector3.Lerp(points[number_of_points - 1], modelPos[i + KeyPose_List[KeyPose_List.Count - 2] + 1][position_id], (float)i / (number_of_points - 1));
                }

                for (int i = 0; i < number_of_points; i++)
                {
                    line_renderer.SetPosition(i + KeyPose_List[KeyPose_List.Count - 2] + 1, points[i]);
                }
            }

            else
            {
                int list_index = KeyPose_List.IndexOf(frame);
                number_of_points = KeyPose_List[list_index] - KeyPose_List[list_index - 1];
                Vector3[] before_points = new Vector3[number_of_points];
                before_points[number_of_points - 1] = moved_position;

                for (int i = 0; i < number_of_points - 1; i++)
                {
                    before_points[i] = Vector3.Lerp(before_points[number_of_points - 1], modelPos[KeyPose_List[list_index - 1] + 1][position_id], (float)i / (number_of_points - 1));
                }
            }
        }

    }
}
