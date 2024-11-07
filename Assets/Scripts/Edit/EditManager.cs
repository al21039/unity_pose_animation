using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditManager : MonoBehaviour
{
    [SerializeField] private Spline _spline;
    [SerializeField] private IndirectOperation indirectOperation;
    [SerializeField] private UIListener _uiListener;

    private Dictionary<int, Vector3[]> _changePos = new Dictionary<int, Vector3[]>();    //変更後の位置　　　　最初から曲線に変更
    private List<int> _keyPoseList = new List<int>();                                    //キーポーズのリスト　　　後からJSONファイルのやつ追加できるように

    public Dictionary<int, Vector3[]> ChangePos
    {
        get
        {
            return _changePos;
        }
        set
        {
            _changePos = value;
        }
    }

    private static EditManager instance;

    public static EditManager GetInstance() => instance;   //インスタンス返す

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }    //シングルトン処理

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
