using System.Collections.Generic;
using UnityEngine;

public class SetNewPosition : MonoBehaviour
{
    [SerializeField] private GameObject[] modelPartObject;
    [SerializeField] private GameObject[] rotationObject;
    [SerializeField] private Animator _thisAnimator;

    private Dictionary<int, Vector3[]> _changedPosition;
    private Dictionary<int, Quaternion[]> _chagedRotation;
    private List<float> _modelHeight;
    private int _totalFrame;
    private int _currentFrame = 0;

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
        transform.position = new Vector3(0.0f, _modelHeight[currentFrame] - 1.0f, 0.0f);

        for (int i = 0; i < modelPartObject.Length; i++)
        {
            modelPartObject[i].transform.position = _changedPosition[currentFrame][i];
        }

        for (int i = 0; i < rotationObject.Length; i++)
        {
            rotationObject[i].transform.rotation = _chagedRotation[currentFrame][i];
        }
    }
}
