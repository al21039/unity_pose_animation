using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateNewAnim : MonoBehaviour
{
    private Dictionary<int, Vector3[]> _modelPos;
    private int _totalFrame;
    private int _currentFrame = 0;
    private bool _isPlaying = false;

    private HumanPoseHandler _humanPoseHandler;
    private HumanPose _humanPose;
    private Animator _animator;

    private float fps = 30.0f; //メディアパイプのfps

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _humanPoseHandler = new HumanPoseHandler(_animator.avatar, _animator.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPlaying)
        {
            //CreateNewAnimation();
        }
    }

    public void SetStatus(Dictionary<int, Vector3[]> modelPos, int totalFrame)
    {
        _modelPos = modelPos;
        _totalFrame = totalFrame;
        _isPlaying = true;
    }

    public void SetNewAnimation(HumanPose framePose)
    {
        _animator = GetComponent<Animator>();
        Debug.Log(_animator);
        _humanPoseHandler = new HumanPoseHandler(_animator.avatar, _animator.transform);
        HumanPose humanPose = new HumanPose();
        humanPose = framePose;

        Debug.Log(framePose.muscles[32]);
        _humanPoseHandler.SetHumanPose(ref humanPose);
    }

    public void CreateNewAnimation(HumanPose[] keyFramePoses, List<int> keyFrameList)
    {
        float time = 0;

        AnimationClip animclip = new AnimationClip();
        AnimationCurve[] humanPoseMuscleCurves = new AnimationCurve[HumanTrait.MuscleCount]; //すべてのマッスル値の数　配列で確保
        AnimationCurve humanPosePositionCurve = new AnimationCurve();
        AnimationCurve humanPoseRotationCurve = new AnimationCurve();

        AnimationCurve rootTX = new AnimationCurve();
        AnimationCurve rootTY = new AnimationCurve();
        AnimationCurve rootTZ = new AnimationCurve();

        AnimationCurve rootQX = new AnimationCurve();
        AnimationCurve rootQY = new AnimationCurve();
        AnimationCurve rootQZ = new AnimationCurve();
        AnimationCurve rootQW = new AnimationCurve();

        for (int i = 0; i < humanPoseMuscleCurves.Length; i++)
        {
            humanPoseMuscleCurves[i] = new AnimationCurve(); //アニメーションカーブ初期化
        }

        for (int i = 0; i < keyFrameList.Count; i++)
        {
            HumanPose humanPose = keyFramePoses[i];
            time = keyFrameList[i] / fps;

            for (int j = 0; j < humanPose.muscles.Length; j++)
            {
                humanPoseMuscleCurves[j].AddKey(time, humanPose.muscles[j]);
            }

            rootQX.AddKey(time, humanPose.bodyRotation.x);
            rootQY.AddKey(time, humanPose.bodyRotation.y);
            rootQZ.AddKey(time, humanPose.bodyRotation.z);
            rootQW.AddKey(time, humanPose.bodyRotation.w);

            rootTX.AddKey(time, humanPose.bodyPosition.x);
            rootTY.AddKey(time, humanPose.bodyPosition.y);
            rootTZ.AddKey(time, humanPose.bodyPosition.z);
        }


        for (int i = 0; i < humanPoseMuscleCurves.Length; i++)
        {
            string muscleName = HumanTrait.MuscleName[i];
            animclip.SetCurve("", typeof(Animator), muscleName, humanPoseMuscleCurves[i]);
        }

        animclip.SetCurve("", typeof(Animator), "RootT.x", rootTX);
        animclip.SetCurve("", typeof(Animator), "RootT.y", rootTY);
        animclip.SetCurve("", typeof(Animator), "RootT.z", rootTZ);
        animclip.SetCurve("", typeof(Animator), "RootQ.x", rootQX);
        animclip.SetCurve("", typeof(Animator), "RootQ.y", rootQY);
        animclip.SetCurve("", typeof(Animator), "RootQ.z", rootQZ);
        animclip.SetCurve("", typeof(Animator), "RootQ.w", rootQW);

        //アニメーションを制作
        AssetDatabase.CreateAsset(animclip, AssetDatabase.GenerateUniqueAssetPath("Assets/kick.anim"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
