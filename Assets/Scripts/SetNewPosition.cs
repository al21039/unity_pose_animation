using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SetNewPosition : MonoBehaviour
{
    [SerializeField] private GameObject[] modelPartObject;
    [SerializeField] private GameObject[] rotationObject;
    [SerializeField] private Animator _thisAnimator;

    private Dictionary<int, Vector3[]> _changedPosition;
    private Dictionary<int, Quaternion[]> _chagedRotation;
    private List<HumanPose> _humanPoses = new List<HumanPose>();
    private List<float> _modelHeight;
    private int _totalFrame;
    private int _currentFrame = 0;
    private bool _firstLoop = true;

    private HumanPoseHandler _humanPoseHandler;
    private HumanPose _humanPose;
    private int currentFrame = 0;

    // Start is called before the first frame update
    void Start()
    {
        SetStatus();
        _humanPoseHandler = new HumanPoseHandler(_thisAnimator.avatar, _thisAnimator.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentFrame < _totalFrame)
        {
            SetAnimation();


            currentFrame++;
        }
        else
        {
            if (_firstLoop) 
            {
                SaveHumanPose();
                _firstLoop = false;
            }
            currentFrame = 0;

        }
    }

    public void SetStatus()
    {
        _changedPosition = EditManager.GetInstance().ChangePos;
        _chagedRotation = EditManager.GetInstance().ChangeRot;
        _totalFrame = LandmarkManager.GetInstance().TotalFrame;
        _modelHeight = EditManager.GetInstance().HipHeight;
    }

    private void SetAnimation()
    {
        _humanPose = new HumanPose();

        transform.position = new Vector3(0.0f, _modelHeight[currentFrame] - 1.0f, 0.0f);

        for (int i = 0; i < modelPartObject.Length; i++)
        {
            modelPartObject[i].transform.position = _changedPosition[currentFrame][i];
        }

        for (int i = 0; i < rotationObject.Length; i++)
        {
            rotationObject[i].transform.rotation = _chagedRotation[currentFrame][i];
        }


        _humanPoseHandler.GetHumanPose(ref _humanPose);
        _humanPose.bodyPosition = transform.position + Vector3.up;
        _humanPose.bodyRotation = _chagedRotation[currentFrame][0];
        
        _humanPoses.Add(_humanPose);
    }

    private void SaveHumanPose()
    {
        string timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        string folderPath = Path.Combine(Application.dataPath, "AnimCSV");
        Directory.CreateDirectory(folderPath);
        string filePath = Path.Combine(folderPath, $"{timestamp}.csv");

        try
        {
            for (int i = 0;i < _totalFrame; i++)
            {
                var sb = new StringBuilder();
                sb.Append(_humanPoses[i].bodyPosition.x + "," + _humanPoses[i].bodyPosition.y + "," + _humanPoses[i].bodyPosition.z + ",");
                sb.Append(_humanPoses[i].bodyRotation.x + "," + _humanPoses[i].bodyRotation.y + "," + _humanPoses[i].bodyRotation.z + "," + _humanPoses[i].bodyRotation.w + ",");
                foreach (var muscle in _humanPoses[i].muscles)
                {
                    sb.Append(muscle + ",");
                }

                File.AppendAllText(filePath, sb.ToString() + Environment.NewLine);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to write to CSV: " + e.Message);
        }
    }
}
