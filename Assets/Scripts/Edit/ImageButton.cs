using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageButton : MonoBehaviour
{
    [SerializeField] private LineInterpolation _lineInterpolation;
    private int _addFrame = 100;


    public void OnClickedImageButton()
    {
        Debug.Log(this.gameObject.name);
        Vector3[] JsonLandmark = LandmarkManager.GetInstance().JSONLandmarkPositions[int.Parse(this.gameObject.name)];
        EditManager.GetInstance().SetPosition(_addFrame, JsonLandmark);
        List<int> keyPoseList = LandmarkManager.GetInstance().KeyPoseList;
        int index = keyPoseList.BinarySearch(_addFrame);

        keyPoseList.Insert(index, _addFrame);
        LandmarkManager.GetInstance().KeyPoseList = keyPoseList;
        for (int i = 0; i < 4; i++)
        {
            Spline.GetInstance().SetSpline(i, _addFrame, JsonLandmark[i]);
        }

        _lineInterpolation.InterpolationAllLine();
    }
}
