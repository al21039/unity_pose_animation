using UnityEngine;

public class CheckInverse : MonoBehaviour
{
    [SerializeField] GameObject leftFoot;
    // Start is called before the first frame update
    void Start()
    {
        Quaternion diff = Quaternion.Inverse(Quaternion.Euler(0, 90, 90)) * leftFoot.transform.rotation;
        Debug.Log(diff.eulerAngles);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
