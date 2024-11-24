using UnityEngine;

public class CheckDirection : MonoBehaviour
{
    [SerializeField] private GameObject _rightShoulder;
    [SerializeField] private GameObject _leftShoulder;
    [SerializeField] private GameObject _rightThigh;
    [SerializeField] private GameObject _leftThigh;
    [SerializeField] private GameObject _neck;

    [SerializeField] private Transform[] _ModelTransform;

    [SerializeField] private Vector3 _landmarkRightShoulder;
    [SerializeField] private Vector3 _landmarkLeftShoulder;
    [SerializeField] private Vector3 _landmarkRightThigh;
    [SerializeField] private Vector3 _landmarkLeftThigh;

    Vector3 _middleShoulder;
    Vector3 _middleThigh;

    // Start is called before the first frame update
    void Start()
    {
        _middleShoulder = (_rightShoulder.transform.position + _leftShoulder.transform.position) / 2;
        _middleThigh = (_rightThigh.transform.position + _leftThigh.transform.position) / 2;

        LandmarkHipRotation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LandmarkHipRotation()
    {
        _landmarkLeftThigh += new Vector3(0, 1, 0);
        _landmarkRightThigh += new Vector3(0, 1, 0);
        _landmarkLeftShoulder += new Vector3(0, 1, 0);
        _landmarkLeftShoulder += new Vector3(0, 1, 0);

        Vector3 middleShoulder = (_landmarkLeftShoulder + _landmarkRightShoulder) / 2;
        Vector3 middleThigh = (_landmarkLeftThigh + _landmarkRightThigh) / 2;

        Vector3 horizontalAxis = (_landmarkRightThigh - _landmarkLeftThigh).normalized;
        Vector3 rawVerticalAxis = (middleShoulder - middleThigh).normalized;
        Vector3 verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;

        Vector3 forward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        _ModelTransform[0].rotation = Quaternion.LookRotation(forward, verticalAxis);
    }


    private void HipRotation()
    {
        Vector3 horizontalAxis = (_rightThigh.transform.position - _leftThigh.transform.position).normalized;
        Vector3 rawVerticalAxis = (_middleShoulder - _middleThigh).normalized;
        Vector3 verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;

        Vector3 forward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        _ModelTransform[0].rotation = Quaternion.LookRotation(forward, verticalAxis);
    }

    private void ChestRotation()
    {
        Vector3 horizontalAxis = (_rightThigh.transform.position - _leftThigh.transform.position).normalized;
        Vector3 rawVerticalAxis = (_middleShoulder - _middleThigh).normalized;
        Vector3 verticalAxis = Orthogonalize(horizontalAxis, rawVerticalAxis).normalized;

        Vector3 forward = Vector3.Cross(horizontalAxis, verticalAxis).normalized;

        _ModelTransform[0].rotation = Quaternion.LookRotation(forward, verticalAxis);
    }
    Vector3 Orthogonalize(Vector3 baseVector, Vector3 toOrthogonalize)
    {
        return toOrthogonalize - Vector3.Project(toOrthogonalize, baseVector);
    }
}
