using UnityEngine;

public class CheckDirection : MonoBehaviour
{
    [SerializeField] private GameObject _rightShoulder;
    [SerializeField] private GameObject _leftShoulder;
    [SerializeField] private GameObject _rightThigh;
    [SerializeField] private GameObject _leftThigh;

    [SerializeField] private GameObject _spherePrefab;
    [SerializeField] private Transform _ModelTransform;

    Vector3 _middleShoulder;
    Vector3 _middleThigh;

    // Start is called before the first frame update
    void Start()
    {
        _middleShoulder = (_rightShoulder.transform.position + _leftShoulder.transform.position) / 2;
        _middleThigh = (_rightThigh.transform.position + _leftThigh.transform.position) / 2;

        GetHumanoidForward();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetHumanoidForward()
    {
        Vector3 haimen = (_middleShoulder - _middleThigh).normalized;
        Debug.Log("çòÇ©ÇÁå®" + haimen);
        
        Vector3 migi = (_rightThigh.transform.position - _leftThigh.transform.position).normalized;
        Debug.Log("ç∂çòÇ©ÇÁâEçò" + migi);

        Vector3 syoumen = Vector3.Cross(migi, haimen).normalized;
        Debug.Log("ê≥ñ ï˚å¸" + syoumen);


        _ModelTransform.rotation = Quaternion.LookRotation(syoumen, haimen);

        Instantiate(_spherePrefab, syoumen, Quaternion.identity);
    }
}
