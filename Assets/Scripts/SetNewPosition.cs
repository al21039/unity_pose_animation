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
    [SerializeField] private GameObject _hipObject;
    [SerializeField] private GameObject[] _partObject;

    private Dictionary<int, Vector3[]> _changedPosition;
    private Dictionary<int, Quaternion[]> _chagedRotation;
    private List<Quaternion> _modelEntireRot;
    private List<HumanPose> _humanPoses = new List<HumanPose>();
    private List<Quaternion> _leftHandRot = new List<Quaternion>();
    private List<Quaternion> _rightHandRot = new List<Quaternion>();
    private List<Quaternion> _leftFootRot = new List<Quaternion>();
    private List<Quaternion> _rightFootRot = new List<Quaternion>();
    private List<Vector3> _leftHandPos = new List<Vector3>();
    private List<Vector3> _rightHandPos = new List<Vector3>();
    private List<Vector3> _leftFootPos = new List<Vector3>();
    private List<Vector3> _rightFootPos = new List<Vector3>();
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
        _modelEntireRot = EditManager.GetInstance().ModelEntireRot;
    }

    private void SetAnimation()
    {
        _humanPose = new HumanPose();

        transform.position = new Vector3(10.0f, _modelHeight[currentFrame] - 1.0f, 0.0f);
        transform.rotation = _modelEntireRot[currentFrame];

        for (int i = 0; i < modelPartObject.Length; i++)
        {
            modelPartObject[i].transform.position = _changedPosition[currentFrame][i] + new Vector3(10, 0, 0);
        }

        for (int i = 0; i < rotationObject.Length; i++)
        {
            rotationObject[i].transform.rotation = _chagedRotation[currentFrame][i];
        }


        _humanPoseHandler.GetHumanPose(ref _humanPose);
        _humanPose.bodyPosition = transform.localPosition + Vector3.up - new Vector3(10, 0, 0);
        _humanPose.bodyRotation = _hipObject.transform.rotation;
        _leftHandRot.Add(_partObject[0].transform.localRotation);
        _rightHandRot.Add(_partObject[1].transform.localRotation);
        _leftFootRot.Add(_partObject[2].transform.localRotation);
        _rightFootRot.Add(_partObject[3].transform.localRotation);
        _leftHandPos.Add(_partObject[0].transform.localPosition);
        _rightHandPos.Add(_partObject[1].transform.localPosition);
        _leftFootPos.Add(_partObject[2].transform.localPosition);
        _rightFootPos.Add(_partObject[3].transform.localPosition);
        _humanPoses.Add(_humanPose);
    }

    private void SaveHumanPose()
    {
        string timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
#if UNITY_EDITOR
        string folderPath = Path.Combine(Application.dataPath, "AnimCSV");
#else
        string folderPath = Path.Combine(Application.persistentDataPath, "AnimCSV");
#endif

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

                sb.Append(_leftHandRot[i].x + "," + _leftHandRot[i].y + "," + _leftHandRot[i].z + "," + _leftHandRot[i].w + ",");
                sb.Append(_rightHandRot[i].x + "," + _rightHandRot[i].y + "," + _rightHandRot[i].z + "," + _rightHandRot[i].w + ",");
                sb.Append(_leftFootRot[i].x + "," + _leftFootRot[i].y + "," + _leftFootRot[i].z + "," + _leftFootRot[i].w + ",");
                sb.Append(_rightFootRot[i].x + "," + _rightFootRot[i].y + "," + _rightFootRot[i].z + "," + _rightFootRot[i].w + ",");

                sb.Append(_leftHandPos[i].x + "," + _leftHandPos[i].y + "," + _leftHandPos[i].z + ",");
                sb.Append(_rightHandPos[i].x + "," + _rightHandPos[i].y + "," + _rightHandPos[i].z + ",");
                sb.Append(_leftFootPos[i].x + "," + _leftFootPos[i].y + "," + _leftFootPos[i].z + ",");
                sb.Append(_rightFootPos[i].x + "," + _rightFootPos[i].y + "," + _rightFootPos[i].z + ",");

                File.AppendAllText(filePath, sb.ToString() + Environment.NewLine);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to write to CSV: " + e.Message);
        }
    }
}
