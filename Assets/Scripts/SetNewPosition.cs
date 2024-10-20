using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNewPosition : MonoBehaviour
{
    [SerializeField] GameObject Hips;
    [SerializeField] GameObject Body;
    [SerializeField] GameObject Left_shoulder;
    [SerializeField] GameObject Left_elbow;
    [SerializeField] GameObject Left_hand;
    [SerializeField] GameObject Right_shoulder;
    [SerializeField] GameObject Right_elbow;
    [SerializeField] GameObject Right_hand;
    [SerializeField] GameObject Left_hip;
    [SerializeField] GameObject Left_knee;
    [SerializeField] GameObject Left_ankle;
    [SerializeField] GameObject Right_hip;
    [SerializeField] GameObject Right_knee;
    [SerializeField] GameObject Right_ankle;
    [SerializeField] GameObject middleDot;

    private Dictionary<int, Vector3[]> _modelPos;
    private int _totalFrame;
    private int _currentFrame = 0;
    private bool _isPlaying = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPlaying)
        {
            CreateNewAnimation();
        }
    }

    public void SetStatus(Dictionary<int, Vector3[]> modelPos, int totalFrame)
    {
        _modelPos = modelPos;
        _totalFrame = totalFrame;
        _isPlaying = true;
    }

    private void CreateNewAnimation()
    {
        Left_hand.transform.position = _modelPos[_currentFrame][0];
        Right_hand.transform.position = _modelPos[_currentFrame][1];
        Left_ankle.transform.position = _modelPos[_currentFrame][2];
        Right_ankle.transform.position = _modelPos[_currentFrame][3];
        Left_elbow.transform.position = _modelPos[_currentFrame][4];
        Right_elbow.transform.position = _modelPos[_currentFrame][5];
        Left_knee.transform.position = _modelPos[_currentFrame][6];
        Right_knee.transform.position= _modelPos[_currentFrame][7];
        Body.transform.position = _modelPos[_currentFrame][8];
        middleDot.transform.position = _modelPos[_currentFrame][9];

        _currentFrame++;
        if (_currentFrame >= _totalFrame)
        {
            _currentFrame = 0;
        }
        
    }
}
