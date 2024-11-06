using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPositionFromJson : MonoBehaviour
{
    [SerializeField] private Animator _avatarAnimator;

    private Transform modelPosition;

    private Dictionary<int, HumanPose> JsonPositions = new Dictionary<int, HumanPose>();

    public HumanPose GetJsonPosition(int index)
    {
        return JsonPositions[index];
    } 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetBoneTransform()
    {
        
    }

}
