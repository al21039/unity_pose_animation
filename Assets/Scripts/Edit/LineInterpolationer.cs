using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using UnityEngine.UIElements;
public class LineInterpolation : MonoBehaviour
{
    private Dictionary<int, Vector3[]> _changedPos = new Dictionary<int, Vector3[]>();
    private List<int> _keyPoseList = new List<int>();
    private float _frameInterval = 0.30f;

    private void GetRequiredValue()
    {
        _changedPos = EditManager.GetInstance().ChangePos;
        _keyPoseList = EditManager.GetInstance().KeyPoseList;
    }

    private void InterpolationAllLine()
    {
        for (int i = 0; i < _keyPoseList.Count; i++)
        {
            if (i == 0)
            {

            }
            else if (i == _keyPoseList.Count - 1)
            {

            }
            else if(i == 1)
            {

            }
            else if(i == _keyPoseList.Count - 2)
            {

            }
            else
            {

            }
            
        }
    }

    //最初のフレームの編集時
    public void SetFirstFrameLinePos(Vector3 targetPosition, int positionID, LineRenderer lineRenderer)
    {
        GetRequiredValue();

        int numberOfPoints = _keyPoseList[1];
        Vector3[] points = new Vector3[numberOfPoints];
        int afterKey = _keyPoseList[1];
        int twoAfterKey = _keyPoseList[2];
        points[0] = targetPosition;

        for (int i = 1; i < numberOfPoints; i++)
        {
            float t = (float)i / (float)numberOfPoints;
            Vector3 p0 = targetPosition;
            Vector3 p1 = targetPosition;
            Vector3 p2 = _changedPos[afterKey][positionID] + new Vector3(0, 0, (afterKey) * _frameInterval);
            Vector3 p3 = _changedPos[twoAfterKey][positionID] + new Vector3(0, 0, (twoAfterKey) * _frameInterval);

            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);
            var savedPoint = _changedPos[i][positionID] + new Vector3(0.0f, 0.0f, (i) * _frameInterval);
        }
        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                Spline.GetInstance().SetSpline(positionID, i, points[i]);
            }
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _changedPos[i][positionID] = points[i] - new Vector3(0.0f, 0.0f, i * _frameInterval);
        }
    }

    //最後のフレームの編集時
    public void SetLastFrameLinePos(Vector3 targetPosition, int positionID)
    {
        GetRequiredValue();

        int previousKey = _keyPoseList[_keyPoseList.Count - 2];
        int numberOfPoints = _keyPoseList[_keyPoseList.Count - 1] - previousKey;
        Vector3[] points = new Vector3[numberOfPoints];
        int twoPreviousKey = _keyPoseList[_keyPoseList.Count - 3];
        points[numberOfPoints - 1] = targetPosition;

        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            float t = (float)i / (float)numberOfPoints;
            Vector3 p0 = _changedPos[twoPreviousKey][positionID] + new Vector3(0, 0, (twoPreviousKey) * _frameInterval);
            Vector3 p1 = _changedPos[previousKey][positionID] + new Vector3(0, 0, (previousKey) * _frameInterval);
            Vector3 p2 = targetPosition;
            Vector3 p3 = targetPosition;

            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);
            var savedPoint = _changedPos[i + previousKey + 1][positionID] + new Vector3(0.0f, 0.0f, (i + previousKey + 1) * _frameInterval);

            points[i] = splinedPoint;
        }
        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                Spline.GetInstance().SetSpline(positionID, i + previousKey + 1, points[i]);
            }
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _changedPos[i + previousKey + 1][positionID] = points[i] - new Vector3(0.0f, 0.0f, (i + previousKey + 1) * _frameInterval);
        }
    }

    public void SetOtherBeforeLinePos(Vector3 targetPosition, int positionID, int frame)
    {
        GetRequiredValue();

        int listIndex = _keyPoseList.IndexOf(frame);
        int currentKey = _keyPoseList[listIndex];
        int afterKey = _keyPoseList[listIndex + 1];
        int previousKey = _keyPoseList[listIndex - 1];
        int twoPreviousKey = _keyPoseList[listIndex - 2];
        int twoAfterKey = _keyPoseList[listIndex + 2];
        int numberOfPoints = currentKey - previousKey;
        Vector3[] beforePoints = new Vector3[numberOfPoints];
        beforePoints[numberOfPoints - 1] = targetPosition;


        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            float t = (float)(i + 1) / (float)numberOfPoints;
            Vector3 p0 = _changedPos[twoPreviousKey][positionID] + new Vector3(0, 0, (twoPreviousKey) * _frameInterval);
            Vector3 p1 = _changedPos[previousKey][positionID] + new Vector3(0, 0, (previousKey) * _frameInterval);
            Vector3 p2 = targetPosition;
            Vector3 p3 = _changedPos[afterKey][positionID] + new Vector3(0, 0, (afterKey) * _frameInterval);

            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);
            var savedPoint = _changedPos[i + previousKey + 1][positionID] + new Vector3(0.0f, 0.0f, (i + previousKey + 1) * _frameInterval);

            beforePoints[i] = splinedPoint;
        }
        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                Spline.GetInstance().SetSpline(positionID, i + previousKey + 1, beforePoints[i]);
            }
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _changedPos[i + previousKey + 1][positionID] = beforePoints[i] - new Vector3(0.0f, 0.0f, (i + previousKey + 1) * _frameInterval);
        }
    }

    public void SetOtherAfterLinePos(Vector3 targetPosition, int positionID, int frame)
    {
        GetRequiredValue();

        int listIndex = _keyPoseList.IndexOf(frame);
        int currentKey = _keyPoseList[listIndex];
        int afterKey = _keyPoseList[listIndex + 1];
        int previousKey = _keyPoseList[listIndex - 1];
        int twoAfterKey = _keyPoseList[listIndex + 2];
        int numberOfPoints = _keyPoseList[listIndex + 1] - currentKey;
        Vector3[] afterPoints = new Vector3[numberOfPoints];
        afterPoints[0] = targetPosition;

        for (int i = 1; i < numberOfPoints; i++)
        {
            float t = (float)i / (float)numberOfPoints;
            Vector3 p0 = _changedPos[previousKey][positionID] + new Vector3(0, 0, (currentKey - 1) * _frameInterval);
            Vector3 p1 = targetPosition;
            Vector3 p2 = _changedPos[afterKey][positionID] + new Vector3(0, 0, (afterKey) * _frameInterval);
            Vector3 p3 = _changedPos[twoAfterKey][positionID] + new Vector3(0, 0, (twoAfterKey) * _frameInterval);

            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);
            var savedPoint = _changedPos[i + currentKey][positionID] + new Vector3(0.0f, 0.0f, (i + currentKey) * _frameInterval);

            afterPoints[i] = splinedPoint;
        }

        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                Spline.GetInstance().SetSpline(positionID, i + currentKey, afterPoints[i]);
            }
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _changedPos[i + currentKey][positionID] = afterPoints[i] - new Vector3(0.0f, 0.0f, (i + currentKey) * _frameInterval);
        }
    }

    //スプライン補完　関数
    private Vector3 CatmullRomSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2.0f * p1) +
            (-p0 + p2) * t +
            (2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) * t2 +
            (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3
        );
    }
}
