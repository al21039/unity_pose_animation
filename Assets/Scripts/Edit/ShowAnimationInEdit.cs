using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAnimationInEdit : MonoBehaviour
{
    [SerializeField] GameObject[] _modelPos = new GameObject[14];
    [SerializeField] GameObject[] _modelRot = new GameObject[7];


    private Dictionary<int, Vector3[]> _changePos = new Dictionary<int, Vector3[]>();    //変更後の位置　　　　最初から曲線に変更
    private Dictionary<int, Quaternion[]> _changeRot = new Dictionary<int, Quaternion[]>();
    private List<int> _keyPoseList = new List<int>();                                    //キーポーズのリスト　　　後からJSONファイルのやつ追加できるように
    private List<float> _hipHeight = new List<float>();
    private int _totalFrame = 0;
    private int _currentFrame = 0;

    private bool _isPrepared = false;

    private void FixedUpdate()
    {
        if (_isPrepared)
        {
            SetFramePose();
        }
    }

    public void Prepare()
    {
        _changePos = EditManager.GetInstance().ChangePos;
        _changeRot = EditManager.GetInstance().ChangeRot;   
        _keyPoseList = EditManager.GetInstance().KeyPoseList;
        _hipHeight = EditManager.GetInstance().HipHeight;
        _totalFrame = LandmarkManager.GetInstance().TotalFrame;

        _isPrepared = true;
    }

    private void SetFramePose()
    {
        transform.position = new Vector3(0, _hipHeight[_currentFrame] - 1, _currentFrame * 0.30f);

        for (int i = 0; i < _modelPos.Length; i++)
        {
            _modelPos[i].transform.position = _changePos[_currentFrame][i] + new Vector3(0, 0, _currentFrame * 0.30f);
        }

        for (int i = 0; i < _modelRot.Length; i++)
        {
            _modelRot[i].transform.rotation = _changeRot[_currentFrame][i];
        }

        _currentFrame++;
        if (_currentFrame > _totalFrame - 1)
        {
            Destroy(this.gameObject);
        }
    }




}
