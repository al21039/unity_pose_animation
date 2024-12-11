using UnityEngine;
using UnityEngine.UI;

public class HumanoidRotataionSetter : MonoBehaviour
{
    [SerializeField] private GameObject _slider;
    [SerializeField] private Slider _rotationSlider;

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
            Debug.Log(_rotationHumanoid);
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
        _rotationSlider.onValueChanged.AddListener(UpdateRotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateRotation(float value)
    {
        if (_animationTransform != null && frame != 0 && frame != EditManager.GetInstance().KeyPoseList.Count - 1)
        {
            _rotationHumanoid.transform.rotation = Quaternion.Euler(0, -value, 0);
            Vector3[] positionValue = _animationTransform.ReturnNewPositionValue();
            Quaternion[] rotationValue = _animationTransform.ReturnNewRotationValue();

            PositionMover.GetInstance().FixedCubeTransform();
            EditManager.GetInstance().ChangeToNewValue(frame, positionValue, rotationValue);
        }
    }

    public void OnClickedRotationButton()
    {
        _slider.SetActive(!_slider.activeSelf);
        _rotationSlider.value = 0;
    }
}
