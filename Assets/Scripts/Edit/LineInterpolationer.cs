using System.Collections.Generic;
using UnityEngine;

public class LineInterpolation : MonoBehaviour
{
    private Dictionary<int, Vector3[]> _changedPos = new Dictionary<int, Vector3[]>();
    private List<int> _keyPoseList = new List<int>();
    private float _frameInterval = 0.30f;
    private List<float> _hipHeight = new List<float>();

    private void GetRequiredValue()
    {
        _changedPos = EditManager.GetInstance().ChangePos;
        _keyPoseList = EditManager.GetInstance().KeyPoseList;
        _hipHeight = EditManager.GetInstance().HipHeight;
    }

    public void InterpolationAllLine(bool isFirst)
    {
        GetRequiredValue();


        for (int i = 0; i < _keyPoseList.Count; i++)
        {
            if (i == 0)
            {
                for (int j = 0; j < 8; j++)
                {
                    SetFirstFrameLinePos(_changedPos[_keyPoseList[i]][j], j, isFirst, false);
                }
            }
            else if (i == _keyPoseList.Count - 1)
            {
                for (int j = 0; j < 8; j++)
                {
                    SetLastFrameLinePos(_changedPos[_keyPoseList[i]][j], j, isFirst, false);
                }
            }           
            else if(i == 1)
            {
                for (int j = 0; j < 8; j++)
                {
                    SetSecondLinePos(_changedPos[_keyPoseList[i]][j], j, isFirst, false);
                    SetOtherAfterLinePos(_changedPos[_keyPoseList[i]][j], j, _keyPoseList[i], isFirst, false);
                }
            } 
            else if(i == _keyPoseList.Count - 2)
            {
                for (int j = 0; j < 8; j++)
                {
                    SetOtherBeforeLinePos(_changedPos[_keyPoseList[i]][j], j, _keyPoseList[i], isFirst, false);
                    SetSecondToLastFrameLinePos(_changedPos[_keyPoseList[i]][j], j, isFirst, false);
                }
            }
            else
            {
                for (int j = 0; j < 8; j++)
                {
                    SetOtherAfterLinePos(_changedPos[_keyPoseList[i]][j], j, _keyPoseList[i], isFirst, false);
                    SetOtherBeforeLinePos(_changedPos[_keyPoseList[i]][j], j, _keyPoseList[i], isFirst, false);
                }
            }    
        }
    }

    public void InterpolationJson(int index)
    {
        GetRequiredValue();

        if (index == 0)
        {
            for (int j = 0; j < 10; j++)
            {
                SetFirstFrameLinePos(_changedPos[_keyPoseList[index]][j], j, false, false);
            }
        }
        else if (index == _keyPoseList.Count - 1)
        {
            for (int j = 0; j < 10; j++)
            {
                SetLastFrameLinePos(_changedPos[_keyPoseList[index]][j], j, false, false);
            }
        }
        else if (index == 1)
        {
            for (int j = 0; j < 10; j++)
            {
                SetSecondLinePos(_changedPos[_keyPoseList[index]][j], j, false, false);
                SetOtherAfterLinePos(_changedPos[_keyPoseList[index]][j], j, _keyPoseList[index], false, false);
            }
        }
        else if (index == _keyPoseList.Count - 2)
        {
            for (int j = 0; j < 10; j++)
            {
                SetOtherBeforeLinePos(_changedPos[_keyPoseList[index]][j], j, _keyPoseList[index], false, false);
                SetSecondToLastFrameLinePos(_changedPos[_keyPoseList[index]][j], j, false, false);
            }
        }
        else
        {
            for (int j = 0; j < 10; j++)
            {
                SetOtherAfterLinePos(_changedPos[_keyPoseList[index]][j], j, _keyPoseList[index], false, false);
                SetOtherBeforeLinePos(_changedPos[_keyPoseList[index]][j], j, _keyPoseList[index], false, false);
            }
        }

    }

    public void SetSplineAndJointPosition(int frame, Vector3 targetPosition, int positionID)
    {
        GetRequiredValue();

        if (_keyPoseList.Contains(frame))
        {
            int index = _keyPoseList.IndexOf(frame);
            //編集したキーフレームが１番目なら
            if (frame == 0)
            {
                SetFirstFrameLinePos(targetPosition - new Vector3(0, _hipHeight[0] - 1.0f, 0), positionID , false, true);
            }
            //編集したキーフレームが最後なら
            else if (frame == _keyPoseList[_keyPoseList.Count - 1])
            {
                SetLastFrameLinePos(targetPosition - new Vector3(0, _hipHeight[_keyPoseList[_keyPoseList.Count - 1]] - 1.0f, _keyPoseList[_keyPoseList.Count - 1] * _frameInterval), positionID, false, true);
            }
            //編集したキーフレームが２番目なら
            else if (frame == _keyPoseList[1])
            {
                SetSecondLinePos(targetPosition - new Vector3(0, _hipHeight[_keyPoseList[1]] - 1.0f, _keyPoseList[1] * _frameInterval), positionID, false, true);
                SetOtherAfterLinePos(targetPosition - new Vector3(0, _hipHeight[_keyPoseList[1]] - 1.0f, _keyPoseList[1] * _frameInterval), positionID, frame, false, true);
            }
            //編集したキーフレームが最後から２番目なら
            else if (frame == _keyPoseList[_keyPoseList.Count - 2])
            {
                SetOtherBeforeLinePos(targetPosition - new Vector3(0, _hipHeight[_keyPoseList[_keyPoseList.Count - 2]] - 1.0f, _keyPoseList[_keyPoseList.Count - 2] * _frameInterval), positionID, frame, false, true);
                SetSecondToLastFrameLinePos(targetPosition - new Vector3(0, _hipHeight[_keyPoseList[_keyPoseList.Count - 2]] - 1.0f, _keyPoseList[_keyPoseList.Count - 2] * _frameInterval), positionID, false, true);
            }
            //編集したキーフレームが上記以外なら
            else
            {
                SetOtherAfterLinePos(targetPosition - new Vector3(0, _hipHeight[frame] - 1.0f, _keyPoseList[index] * _frameInterval), positionID, frame, false, true);
                SetOtherBeforeLinePos(targetPosition - new Vector3(0, _hipHeight[frame] - 1.0f, _keyPoseList[index] * _frameInterval), positionID, frame, false, true);
            }
        }     
    }

    //最初のフレームの編集時
    public void SetFirstFrameLinePos(Vector3 targetPosition, int positionID, bool isFirst, bool heightCheck)
    {
        GetRequiredValue();

        int numberOfPoints = _keyPoseList[1];
        Vector3[] points = new Vector3[numberOfPoints];
        int afterKey = _keyPoseList[1];
        int twoAfterKey = _keyPoseList[2];
        points[0] = targetPosition;

        Vector3 p0;
        Vector3 p1;
        Vector3 p2;
        Vector3 p3;

        if (!heightCheck)
        {
            p0 = targetPosition;
            p1 = targetPosition;
            p2 = _changedPos[afterKey][positionID];
            p3 = _changedPos[twoAfterKey][positionID];
        }
        else
        {
            p0 = targetPosition;
            p1 = targetPosition;
            p2 = _changedPos[afterKey][positionID] - new Vector3(0, _hipHeight[afterKey] - 1.0f, 0);
            p3 = _changedPos[twoAfterKey][positionID] - new Vector3(0, _hipHeight[twoAfterKey] - 1.0f, 0);
        }

        for (int i = 1; i < numberOfPoints; i++)
        {
            float t = (float)i / (float)numberOfPoints;
            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);
            points[i] = splinedPoint;
        }
        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                if (!heightCheck)
                {
                    Spline.GetInstance().SetSpline(positionID, i, points[i] + new Vector3(0, 0, (i) * _frameInterval));
                }
                else
                {
                    Spline.GetInstance().SetSpline(positionID, i, points[i] + new Vector3(0, _hipHeight[i] - 1, (i) * _frameInterval));
                }
            }
        }

        if (!isFirst)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                if (!heightCheck)
                {
                    _changedPos[i][positionID] = points[i];
                }
                else
                {
                    _changedPos[i][positionID] = points[i] + new Vector3(0, _hipHeight[i] - 1, 0);
                }

            }
            EditManager.GetInstance().ChangePos = _changedPos;

        }
    }

    //最後のフレームの編集時
    public void SetLastFrameLinePos(Vector3 targetPosition, int positionID, bool isFirst, bool heightCheck)
    {
        GetRequiredValue();

        int previousKey = _keyPoseList[_keyPoseList.Count - 2];
        int numberOfPoints = _keyPoseList[_keyPoseList.Count - 1] - previousKey;
        Vector3[] points = new Vector3[numberOfPoints];
        int twoPreviousKey = _keyPoseList[_keyPoseList.Count - 3];
        points[numberOfPoints - 1] = targetPosition;

        Vector3 p0;
        Vector3 p1;
        Vector3 p2;
        Vector3 p3;
        
        if (!heightCheck)
        {
            p0 = _changedPos[twoPreviousKey][positionID];
            p1 = _changedPos[previousKey][positionID];
            p2 = targetPosition;
            p3 = targetPosition;
        }
        else
        {
            p0 = _changedPos[twoPreviousKey][positionID] - new Vector3(0, _hipHeight[twoPreviousKey] - 1.0f, 0);
            p1 = _changedPos[previousKey][positionID] - new Vector3(0, _hipHeight[previousKey] - 1.0f, 0);
            p2 = targetPosition;
            p3 = targetPosition;
        }


        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            float t = (float)(i + 1) / (float)numberOfPoints;
            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);
            points[i] = splinedPoint;
        }

        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                if (!heightCheck)
                {
                    Spline.GetInstance().SetSpline(positionID, i + previousKey + 1, points[i] + new Vector3(0, 0, (i + previousKey + 1) * _frameInterval));
                }
                else
                {
                    Spline.GetInstance().SetSpline(positionID, i + previousKey + 1, points[i] + new Vector3(0, _hipHeight[i + previousKey + 1] - 1, (i + previousKey + 1) * _frameInterval));
                }
            }
        }

        if (!isFirst) {
            for (int i = 0; i < numberOfPoints; i++)
            {
                if (!heightCheck)
                {
                    _changedPos[i + previousKey + 1][positionID] = points[i];
                }
                else
                {
                    _changedPos[i + previousKey + 1][positionID] = points[i] + new Vector3(0, _hipHeight[i + previousKey + 1] - 1, 0);
                }
            }
            EditManager.GetInstance().ChangePos = _changedPos;
        } 
    }

    public void SetSecondLinePos(Vector3 targetPosition, int positionID, bool isFirst, bool heightCheck)
    {
        GetRequiredValue();

        int previousKey = _keyPoseList[0];
        int currentKey = _keyPoseList[1];
        int afterKey = _keyPoseList[2];
        int numberOfPoints = currentKey - previousKey;
        Vector3[] beforePoints = new Vector3[numberOfPoints];
        beforePoints[numberOfPoints - 1] = targetPosition;

        Vector3 p0;
        Vector3 p1;
        Vector3 p2;
        Vector3 p3;

        if (!heightCheck)
        {
            p0 = _changedPos[previousKey][positionID];
            p1 = _changedPos[previousKey][positionID];
            p2 = targetPosition;
            p3 = _changedPos[afterKey][positionID];
        }
        else
        {
            p0 = _changedPos[previousKey][positionID] - new Vector3(0, _hipHeight[previousKey] - 1.0f, 0);
            p1 = _changedPos[previousKey][positionID] - new Vector3(0, _hipHeight[previousKey] - 1.0f, 0);
            p2 = targetPosition;
            p3 = _changedPos[afterKey][positionID] - new Vector3(0, _hipHeight[afterKey] - 1.0f, 0);
        }

        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            float t = (float)(i + 1) / (float)numberOfPoints;

            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);
            beforePoints[i] = splinedPoint;
        }

        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                if (!heightCheck)
                {
                    Spline.GetInstance().SetSpline(positionID, i + previousKey + 1, beforePoints[i] + new Vector3(0, 0, (i + previousKey + 1) * _frameInterval));
                }
                else
                {
                    Spline.GetInstance().SetSpline(positionID, i + previousKey + 1, beforePoints[i] + new Vector3(0, _hipHeight[i + previousKey + 1] - 1, (i + previousKey + 1) * _frameInterval));
                }
            }
        }

        if (!isFirst)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                if (!heightCheck)
                {
                    _changedPos[i + previousKey + 1][positionID] = beforePoints[i];
                }
                else
                {
                    _changedPos[i + previousKey + 1][positionID] = beforePoints[i] + new Vector3(0, _hipHeight[i + previousKey + 1] - 1, 0);
                }
            }
            EditManager.GetInstance().ChangePos = _changedPos;
        }
    }

    public void SetSecondToLastFrameLinePos(Vector3 targetPosition, int positionID, bool isFirst, bool heightCheck)
    {
        int currentKey = _keyPoseList[_keyPoseList.Count - 2];
        int afterKey = _keyPoseList[_keyPoseList.Count - 1];
        int previousKey = _keyPoseList[_keyPoseList.Count - 3];
        int numberOfPoints = currentKey - previousKey;
        Vector3[] beforePoints = new Vector3[numberOfPoints];
        numberOfPoints = afterKey - currentKey;
        Vector3[] afterPoints = new Vector3[numberOfPoints];
        afterPoints[0] = targetPosition;

        Vector3 p0;
        Vector3 p1;
        Vector3 p2;
        Vector3 p3;

        if (!heightCheck)
        {
            p0 = _changedPos[previousKey][positionID];
            p1 = targetPosition;
            p2 = _changedPos[afterKey][positionID];
            p3 = _changedPos[afterKey][positionID];
        }
        else
        {
            p0 = _changedPos[previousKey][positionID] - new Vector3(0, _hipHeight[previousKey] - 1.0f, 0);
            p1 = targetPosition;
            p2 = _changedPos[afterKey][positionID] - new Vector3(0, _hipHeight[afterKey] - 1.0f, 0);
            p3 = _changedPos[afterKey][positionID] - new Vector3(0, _hipHeight[afterKey] - 1.0f, 0);
        }

        for (int i = 1; i < numberOfPoints; i++)
        {
            float t = (float)i / (float)numberOfPoints;

            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);
            afterPoints[i] = splinedPoint;
        }
        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                if (!heightCheck)
                {
                    Spline.GetInstance().SetSpline(positionID, i + currentKey, afterPoints[i] + new Vector3(0, 0, (i + currentKey) * _frameInterval));
                }
                else
                {
                    Spline.GetInstance().SetSpline(positionID, i + currentKey, afterPoints[i] + new Vector3(0, _hipHeight[i + currentKey] - 1, (i + currentKey) * _frameInterval));
                }
            }
        }

        if (!isFirst)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                if (!heightCheck)
                {
                    _changedPos[i + currentKey][positionID] = afterPoints[i];
                }
                else
                {
                    _changedPos[i + currentKey][positionID] = afterPoints[i] + new Vector3(0, _hipHeight[i + currentKey] - 1, 0);
                }
            }
            EditManager.GetInstance().ChangePos = _changedPos;
        }
    }

    public void SetOtherBeforeLinePos(Vector3 targetPosition, int positionID, int frame, bool isFirst, bool heightCheck)
    {
        GetRequiredValue();

        int listIndex = _keyPoseList.IndexOf(frame);
        int currentKey = _keyPoseList[listIndex];
        int afterKey = _keyPoseList[listIndex + 1];
        int previousKey = _keyPoseList[listIndex - 1];
        int twoPreviousKey = _keyPoseList[listIndex - 2];
        int numberOfPoints = currentKey - previousKey;
        Vector3[] beforePoints = new Vector3[numberOfPoints];
        beforePoints[numberOfPoints - 1] = targetPosition;

        Vector3 p0;
        Vector3 p1;
        Vector3 p2;
        Vector3 p3;

        if (!heightCheck)
        {
            p0 = _changedPos[twoPreviousKey][positionID];
            p1 = _changedPos[previousKey][positionID];
            p2 = targetPosition;
            p3 = _changedPos[afterKey][positionID];
        }
        else
        {
            p0 = _changedPos[twoPreviousKey][positionID] - new Vector3(0, _hipHeight[twoPreviousKey] - 1.0f, 0);
            p1 = _changedPos[previousKey][positionID] - new Vector3(0, _hipHeight[previousKey] - 1.0f, 0);
            p2 = targetPosition;
            p3 = _changedPos[afterKey][positionID] - new Vector3(0, _hipHeight[afterKey] - 1.0f, 0);
        }

        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            float t = (float)(i + 1) / (float)numberOfPoints;

            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);

            beforePoints[i] = splinedPoint;
        }
        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                if (!heightCheck)
                {
                    Spline.GetInstance().SetSpline(positionID, i + previousKey + 1, beforePoints[i] + new Vector3(0, 0, (i + previousKey + 1) * _frameInterval));
                }
                else
                {
                    Spline.GetInstance().SetSpline(positionID, i + previousKey + 1, beforePoints[i] + new Vector3(0, _hipHeight[i + previousKey + 1] - 1, (i + previousKey + 1) * _frameInterval));
                }
            }
        }

        if (!isFirst)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                if (!heightCheck)
                {
                    _changedPos[i + previousKey + 1][positionID] = beforePoints[i];
                }
                else
                {
                    _changedPos[i + previousKey + 1][positionID] = beforePoints[i] + new Vector3(0, _hipHeight[i + previousKey + 1] - 1, 0);
                }
            }
            EditManager.GetInstance().ChangePos = _changedPos;
        }
    }

    public void SetOtherAfterLinePos(Vector3 targetPosition, int positionID, int frame, bool isFirst, bool heightCheck)
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

        Vector3 p0;
        Vector3 p1;
        Vector3 p2;
        Vector3 p3;

        if (!heightCheck)
        {
            p0 = _changedPos[previousKey][positionID];
            p1 = targetPosition;
            p2 = _changedPos[afterKey][positionID];
            p3 = _changedPos[twoAfterKey][positionID];
        }
        else
        {
            p0 = _changedPos[previousKey][positionID] - new Vector3(0, _hipHeight[previousKey] - 1.0f, 0);
            p1 = targetPosition;
            p2 = _changedPos[afterKey][positionID] - new Vector3(0, _hipHeight[afterKey] - 1.0f, 0);
            p3 = _changedPos[twoAfterKey][positionID] - new Vector3(0, _hipHeight[twoAfterKey] - 1.0f, 0);
        }

        for (int i = 1; i < numberOfPoints; i++)
        {
            float t = (float)i / (float)numberOfPoints;

            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);

            afterPoints[i] = splinedPoint;
        }

        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                if (!heightCheck)
                {
                    Spline.GetInstance().SetSpline(positionID, i + currentKey, afterPoints[i] + new Vector3(0, 0, (i + currentKey) * _frameInterval));
                }
                else
                {
                    Spline.GetInstance().SetSpline(positionID, i + currentKey, afterPoints[i] + new Vector3(0, _hipHeight[i + currentKey] - 1, (i + currentKey) * _frameInterval));
                }

            }
        }

        if (!isFirst)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                if (!heightCheck)
                {
                    _changedPos[i + currentKey][positionID] = afterPoints[i];
                }
                else
                {
                    _changedPos[i + currentKey][positionID] = afterPoints[i] + new Vector3(0, _hipHeight[i + currentKey] - 1, 0);
                }
            }
            EditManager.GetInstance().ChangePos = _changedPos;
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
