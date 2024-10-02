using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private GameObject _create_anim_button;

    [SerializeField] private GameObject _created_model;

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
            //移動した分を補間する処理 オブジェクトがあるなら
            if(_selectedObeject_name != null && _selected_frame != null)
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

    public void SetPosition(int frame, Vector3[] pos_list)
    {
        _keypose_model = Instantiate(humanoid_model, new Vector3(0, 0, frame * 0.3f), Quaternion.identity);
        _keypose_model.name = frame + "_frame_model";
        SetAnimationTransform setAnimationTransform = _keypose_model.GetComponent<SetAnimationTransform>();
        setAnimationTransform.SetPartTransform(frame, pos_list);
    }

    public void SetSpline()
    {
        _camera_obj.transform.position = new Vector3(5.0f, 2.0f, 20.0f);
        _camera_obj.transform.rotation = Quaternion.Euler(15.0f, -90.0f, 0.0f);
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

        for (int i = 0; i < KeyPose_List.Count; i++)
        {
            string tmp_name = KeyPose_List[i].ToString() + "_frame_model";
            GameObject destroy_obj = GameObject.Find(tmp_name);
            Destroy(destroy_obj);
        }
        GameObject created_model = Instantiate(_created_model, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        _set_new_position = created_model.GetComponent<SetNewPosition>();
        _set_new_position.SetStatus(modelPos, total_frame);

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
        string frame_obj = moved_frame + "_frame_model";
        Vector3 hit_target_pos = new Vector3(0.0f, 0.0f, 0.0f);

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
        Transform hit_target = null;
        switch(position_id)
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
        }

        if (hit_target != null)
        {
            hit_target_pos = hit_target.position;
        }
        else
        {
            Debug.Log("dont catch");
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

        float origin_effective = 0.5f;
        float bezier_effective = 0.90f;

        if(KeyPose_List.Contains(frame))
        {

            if(frame == 0) {
                number_of_points = KeyPose_List[1];
                Vector3[] points = new Vector3[number_of_points];
                points[0] = hit_target_pos;

                for(int i = 1; i < number_of_points; i++) 
                {
                    Vector3 tmpPos = Bezier(
                        points[0],
                        modelPos[i][position_id] + new Vector3(0.0f, 0.0f, i * 0.30f), 
                        modelPos[KeyPose_List[1]][position_id] + new Vector3(0.0f, 0.0f, KeyPose_List[1] * 0.30f),
                        bezier_effective);
                    
                    points[i] = Vector3.Lerp(
                        modelPos[i][position_id] + new Vector3(0.0f, 0.0f, i * 0.30f),
                        tmpPos,
                        origin_effective);
                }

                for (int i = 0; i < number_of_points; i++)
                {
                    line_renderer.SetPosition(i, points[i]);
                }

                for (int i = 0; i < number_of_points; i++)
                {
                    modelPos[i][position_id] = points[i] - new Vector3(0.0f, 0.0f, i * 0.30f);
                }

            }
            
            else if(frame == KeyPose_List[KeyPose_List.Count - 1])
            {
                int before_key = KeyPose_List[KeyPose_List.Count - 2];
                number_of_points = KeyPose_List[KeyPose_List.Count - 1] - before_key;
                Vector3[] points = new Vector3[number_of_points];
                points[number_of_points - 1] = hit_target_pos;             

                for (int i = 0; i < number_of_points - 1; i++)
                {
                    Vector3 tmpPos = Bezier(
                        modelPos[before_key][position_id] + new Vector3(0.0f, 0.0f, (before_key) * 0.30f), 
                        modelPos[i + before_key + 1][position_id] + new Vector3(0.0f, 0.0f, (i + before_key + 1) * 0.30f),
                        points[number_of_points - 1], 
                        bezier_effective);

                    points[i] = Vector3.Lerp(
                        modelPos[i + before_key + 1][position_id] + new Vector3(0.0f, 0.0f, (i + before_key + 1) * 0.30f), 
                        tmpPos, 
                        origin_effective);
                }

                for (int i = 0; i < number_of_points; i++)
                {
                    line_renderer.SetPosition(i + before_key + 1, points[i]);
                }

                for (int i = 0; i < number_of_points; i++)
                {
                    modelPos[i + before_key + 1][position_id] = points[i] - new Vector3(0.0f, 0.0f, (i + before_key + 1) * 0.30f);
                }
            }
            
            else
            {

                int list_index = KeyPose_List.IndexOf(frame);
                int before_key = KeyPose_List[list_index - 1];
                number_of_points = KeyPose_List[list_index] - before_key;
                Vector3[] before_points = new Vector3[number_of_points];
                before_points[number_of_points - 1] = hit_target_pos;
                

                for (int i = 0; i < number_of_points - 1; i++)
                {
                    Vector3 tmpPos = Bezier(
                        modelPos[before_key][position_id] + new Vector3(0.0f, 0.0f, (before_key + 1) * 0.30f),
                        modelPos[i + before_key + 1][position_id] + new Vector3(0.0f, 0.0f, (i + before_key + 1) * 0.30f),
                        before_points[number_of_points - 1], 
                        bezier_effective);

                    before_points[i] = Vector3.Lerp(
                        modelPos[i + before_key + 1][position_id] + new Vector3(0.0f, 0.0f, (i + before_key + 1) * 0.30f), 
                        tmpPos, 
                        origin_effective);
                }

                for (int i = 0; i < number_of_points; i++)
                {
                    line_renderer.SetPosition(i + before_key + 1, before_points[i]);
                }

                for (int i = 0; i < number_of_points; i++)
                {
                    modelPos[i + before_key + 1][position_id] = before_points[i] - new Vector3(0.0f, 0.0f, (i + before_key + 1) * 0.30f);
                }



                int current_key = KeyPose_List[list_index];
                number_of_points = KeyPose_List[list_index + 1] - current_key;
                Vector3[] after_points = new Vector3[number_of_points];
                after_points[0] = hit_target_pos;

                for (int i = 1; i < number_of_points; i++)
                {
                    Vector3 tmpPos = Bezier(
                        after_points[0],
                        modelPos[i + current_key][position_id] + new Vector3(0.0f, 0.0f, (i + current_key) * 0.30f),
                        modelPos[KeyPose_List[list_index + 1]][position_id] + new Vector3(0.0f, 0.0f, (KeyPose_List[list_index + 1]) * 0.30f),
                        bezier_effective);

                    after_points[i] = Vector3.Lerp(
                        modelPos[i + current_key][position_id] + new Vector3(0.0f, 0.0f, (i + current_key) * 0.30f), 
                        tmpPos, 
                        origin_effective);
                }

                for (int i = 0; i < number_of_points; i++)
                {
                    line_renderer.SetPosition(i + current_key, after_points[i]);
                }

                for (int i = 0; i < number_of_points; i++)
                {
                    modelPos[i + current_key][position_id] = after_points[i] - new Vector3(0.0f, 0.0f, (i + current_key) * 0.30f);
                }
            }
        }
        
    }

    private Vector3 Bezier(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        Vector3 p0 = Vector3.Lerp(start, control, t);
        Vector3 p1 = Vector3.Lerp(end, control, t);
        return Vector3.Lerp(p0, p1, t); 
    }
}
