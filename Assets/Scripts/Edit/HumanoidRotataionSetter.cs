using UnityEngine;
using UnityEngine.UI;

public class HumanoidRotataionSetter : MonoBehaviour
{
    [SerializeField] private GameObject _sliderX;
    [SerializeField] private GameObject _sliderY;
    [SerializeField] private GameObject _sliderZ;
    [SerializeField] private Slider _rotationSliderX;
    [SerializeField] private Slider _rotationSliderY;
    [SerializeField] private Slider _rotationSliderZ;

    private GameObject _rotationHumanoid;
    private int frame;
    SetAnimationTransform _animationTransform;

    public GameObject RotationHumanoid
    {
        get
        {
            return _rotationHumanoid;
        }
        set
        {
            _rotationHumanoid = value;
            if (RotationHumanoid != null)
            {
                _animationTransform = _rotationHumanoid.GetComponent<SetAnimationTransform>();
            }
            else
            {
                _animationTransform = null;
            }
        }
    }

    public int Frame
    {
        get
        {
            return frame;
        }
        set
        {
            frame = value;
            Debug.Log(frame);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _rotationSliderX.onValueChanged.AddListener(UpdateRotationX);
        _rotationSliderY.onValueChanged.AddListener(UpdateRotationY);
        _rotationSliderZ.onValueChanged.AddListener(UpdateRotationZ);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateRotationX(float value)
    {
        if (_animationTransform != null && frame != 0 && frame != EditManager.GetInstance().KeyPoseList.Count - 1)
        {
           Vector3 originalRot = _rotationHumanoid.transform.rotation.eulerAngles;

            _rotationHumanoid.transform.rotation = Quaternion.Euler(value, originalRot.y, originalRot.z);
            Vector3[] positionValue = _animationTransform.ReturnNewPositionValue();
            Quaternion[] rotationValue = _animationTransform.ReturnNewRotationValue();

            PositionMover.GetInstance().FixedCubeTransform();
            EditManager.GetInstance().ChangeToNewValue(frame, positionValue, rotationValue);
            EditManager.GetInstance().ChangeToEntireRot(frame, _rotationHumanoid.transform.rotation);

        }
    }

    private void UpdateRotationY(float value)
    {
        if (_animationTransform != null && frame != 0 && frame != EditManager.GetInstance().KeyPoseList.Count - 1)
        {
            Vector3 originalRot = _rotationHumanoid.transform.rotation.eulerAngles;

            _rotationHumanoid.transform.rotation = Quaternion.Euler(originalRot.x, -value, originalRot.z);
            Vector3[] positionValue = _animationTransform.ReturnNewPositionValue();
            Quaternion[] rotationValue = _animationTransform.ReturnNewRotationValue();

            PositionMover.GetInstance().FixedCubeTransform();
            EditManager.GetInstance().ChangeToNewValue(frame, positionValue, rotationValue);
            EditManager.GetInstance().ChangeToEntireRot(frame, _rotationHumanoid.transform.rotation);
        }
    }
    private void UpdateRotationZ(float value)
    {
        if (_animationTransform != null && frame != 0 && frame != EditManager.GetInstance().KeyPoseList.Count - 1)
        {
            Vector3 originalRot = _rotationHumanoid.transform.rotation.eulerAngles;

            _rotationHumanoid.transform.rotation = Quaternion.Euler(originalRot.x, originalRot.y, value);
            Vector3[] positionValue = _animationTransform.ReturnNewPositionValue();
            Quaternion[] rotationValue = _animationTransform.ReturnNewRotationValue();

            PositionMover.GetInstance().FixedCubeTransform();
            EditManager.GetInstance().ChangeToNewValue(frame, positionValue, rotationValue);
            EditManager.GetInstance().ChangeToEntireRot(frame, _rotationHumanoid.transform.rotation);
        }
    }

    public void OnClickedRotationButton()
    {
        _sliderX.SetActive(!_sliderX.activeSelf);
        _sliderY.SetActive(!_sliderY.activeSelf);
        _sliderZ.SetActive(!_sliderZ.activeSelf);
        _rotationSliderX.value = 0;
        _rotationSliderY.value = 0;
        _rotationSliderZ.value = 0;
    }
}
