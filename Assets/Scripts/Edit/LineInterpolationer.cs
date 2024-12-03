using System.Collections.Generic;
using UnityEngine;

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

    public void InterpolationAllLine()
    {
        GetRequiredValue();


        for (int i = 0; i < _keyPoseList.Count; i++)
        {
            if (i == 0)
            {
                for (int j = 0; j < 4; j++)
                {
                    SetFirstFrameLinePos(_changedPos[_keyPoseList[i]][j] + new Vector3(0, 0, _keyPoseList[i] * _frameInterval), j, true);
                }
            }
            else if (i == _keyPoseList.Count - 1)
            {
                for (int j = 0; j < 4; j++)
                {
                    SetLastFrameLinePos(_changedPos[_keyPoseList[i]][j] + new Vector3(0, 0, _keyPoseList[i] * _frameInterval), j, true);
                }
            }           
            else if(i == 1)
            {
                for (int j = 0; j < 4; j++)
                {
                    SetSecondLinePos(_changedPos[_keyPoseList[i]][j] + new Vector3(0, 0, _keyPoseList[i] * _frameInterval), j, true);
                    SetOtherAfterLinePos(_changedPos[_keyPoseList[i]][j] + new Vector3(0, 0, _keyPoseList[i] * _frameInterval), j, _keyPoseList[i], true);
                }
            } 
            else if(i == _keyPoseList.Count - 2)
            {
                for (int j = 0; j < 4; j++)
                {
                    SetOtherBeforeLinePos(_changedPos[_keyPoseList[i]][j] + new Vector3(0, 0, _keyPoseList[i] * _frameInterval), j, _keyPoseList[i], true);
                    SetSecondToLastFrameLinePos(_changedPos[_keyPoseList[i]][j] + new Vector3(0, 0, _keyPoseList[i] * _frameInterval), j, true);
                }
            }
            else
            {
                for (int j = 0; j < 4; j++)
                {
                    SetOtherAfterLinePos(_changedPos[_keyPoseList[i]][j] + new Vector3(0, 0, _keyPoseList[i] * _frameInterval), j, _keyPoseList[i], true);
                    SetOtherBeforeLinePos(_changedPos[_keyPoseList[i]][j] + new Vector3(0, 0, _keyPoseList[i] * _frameInterval), j, _keyPoseList[i], true);
                }
            }    
        }
    }

    public void SetSplineAndJointPosition(int frame, Vector3 targetPosition, int positionID)
    {
        GetRequiredValue();

        if (_keyPoseList.Contains(frame))
        {
            //�ҏW�����L�[�t���[�����P�ԖڂȂ�
            if (frame == 0)
            {
                SetFirstFrameLinePos(targetPosition, positionID , false);
            }
            //�ҏW�����L�[�t���[�����Ō�Ȃ�
            else if (frame == _keyPoseList[_keyPoseList.Count - 1])
            {
                SetLastFrameLinePos(targetPosition, positionID, false);
            }
            //�ҏW�����L�[�t���[�����Q�ԖڂȂ�
            else if (frame == _keyPoseList[1])
            {
                SetSecondLinePos(targetPosition, positionID, false);
                SetOtherAfterLinePos(targetPosition, positionID, frame, false);
            }
            //�ҏW�����L�[�t���[�����Ōォ��Q�ԖڂȂ�
            else if (frame == _keyPoseList[_keyPoseList.Count - 2])
            {
                SetOtherBeforeLinePos(targetPosition, positionID, frame, false);
                SetSecondToLastFrameLinePos(targetPosition, positionID, false);
            }
            //�ҏW�����L�[�t���[������L�ȊO�Ȃ�
            else
            {
                SetOtherAfterLinePos(targetPosition, positionID, frame, false);
                SetOtherBeforeLinePos(targetPosition, positionID, frame, false);
            }
        }     
    }

    private void InitializeFirstFrameLinePos(Vector3 targetPosition, int positionID)
    {

    }


    //�ŏ��̃t���[���̕ҏW��
    public void SetFirstFrameLinePos(Vector3 targetPosition, int positionID, bool isFirst)
    {
        GetRequiredValue();

        int numberOfPoints = _keyPoseList[1];
        Vector3[] points = new Vector3[numberOfPoints];
        int afterKey = _keyPoseList[1];
        int twoAfterKey = _keyPoseList[2];
        points[0] = targetPosition;

        Vector3 p0 = targetPosition;
        Vector3 p1 = targetPosition;
        Vector3 p2 = _changedPos[afterKey][positionID] + new Vector3(0, 0, (afterKey) * _frameInterval);
        Vector3 p3 = _changedPos[twoAfterKey][positionID] + new Vector3(0, 0, (twoAfterKey) * _frameInterval);

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
                Spline.GetInstance().SetSpline(positionID, i, points[i]);
            }
        }

        if (!isFirst)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                _changedPos[i][positionID] = points[i] - new Vector3(0.0f, 0.0f, i * _frameInterval);
            }
            EditManager.GetInstance().ChangePos = _changedPos;
        }
    }

    //�Ō�̃t���[���̕ҏW��
    public void SetLastFrameLinePos(Vector3 targetPosition, int positionID, bool isFirst)
    {
        GetRequiredValue();

        int previousKey = _keyPoseList[_keyPoseList.Count - 2];
        int numberOfPoints = _keyPoseList[_keyPoseList.Count - 1] - previousKey;
        Vector3[] points = new Vector3[numberOfPoints];
        int twoPreviousKey = _keyPoseList[_keyPoseList.Count - 3];
        points[numberOfPoints - 1] = targetPosition;

        Vector3 p0 = _changedPos[twoPreviousKey][positionID] + new Vector3(0, 0, (twoPreviousKey) * _frameInterval);
        Vector3 p1 = _changedPos[previousKey][positionID] + new Vector3(0, 0, (previousKey) * _frameInterval);
        Vector3 p2 = targetPosition;
        Vector3 p3 = targetPosition;

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
                Spline.GetInstance().SetSpline(positionID, i + previousKey + 1, points[i]);
            }
        }

        if (!isFirst) {
            for (int i = 0; i < numberOfPoints; i++)
            {
                _changedPos[i + previousKey + 1][positionID] = points[i] - new Vector3(0.0f, 0.0f, (i + previousKey + 1) * _frameInterval);
            }
            EditManager.GetInstance().ChangePos = _changedPos;
        } 
    }

    public void SetSecondLinePos(Vector3 targetPosition, int positionID, bool isFirst)
    {
        GetRequiredValue();

        int previousKey = _keyPoseList[0];
        int currentKey = _keyPoseList[1];
        int afterKey = _keyPoseList[2];
        int numberOfPoints = currentKey - previousKey;
        Vector3[] beforePoints = new Vector3[numberOfPoints];
        beforePoints[numberOfPoints - 1] = targetPosition;

        Vector3 p0 = _changedPos[previousKey][positionID] + new Vector3(0, 0, (previousKey) * _frameInterval);
        Vector3 p1 = _changedPos[previousKey][positionID] + new Vector3(0, 0, (previousKey) * _frameInterval);
        Vector3 p2 = targetPosition;
        Vector3 p3 = _changedPos[afterKey][positionID] + new Vector3(0, 0, (afterKey) * _frameInterval);

        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            float t = (float)(i + 1) / (float)numberOfPoints;

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

        if (!isFirst)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                _changedPos[i + previousKey + 1][positionID] = beforePoints[i] - new Vector3(0.0f, 0.0f, (i + previousKey + 1) * _frameInterval);
            }
            EditManager.GetInstance().ChangePos = _changedPos;
        }
    }

    public void SetSecondToLastFrameLinePos(Vector3 targetPosition, int positionID, bool isFirst)
    {
        int currentKey = _keyPoseList[_keyPoseList.Count - 2];
        int afterKey = _keyPoseList[_keyPoseList.Count - 1];
        int previousKey = _keyPoseList[_keyPoseList.Count - 3];
        int numberOfPoints = currentKey - previousKey;
        Vector3[] beforePoints = new Vector3[numberOfPoints];
        numberOfPoints = afterKey - currentKey;
        Vector3[] afterPoints = new Vector3[numberOfPoints];
        afterPoints[0] = targetPosition;

        Vector3 p0 = _changedPos[previousKey][positionID] + new Vector3(0, 0, (previousKey) * _frameInterval);
        Vector3 p1 = targetPosition;
        Vector3 p2 = _changedPos[afterKey][positionID] + new Vector3(0, 0, (afterKey) * _frameInterval);
        Vector3 p3 = _changedPos[afterKey][positionID] + new Vector3(0, 0, (afterKey) * _frameInterval);

        for (int i = 1; i < numberOfPoints; i++)
        {
            float t = (float)i / (float)numberOfPoints;

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

        if (!isFirst)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                _changedPos[i + currentKey][positionID] = afterPoints[i] - new Vector3(0.0f, 0.0f, (i + currentKey) * _frameInterval);
            }
            EditManager.GetInstance().ChangePos = _changedPos;
        }
    }

    public void SetOtherBeforeLinePos(Vector3 targetPosition, int positionID, int frame, bool isFirst)
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

        Vector3 p0 = _changedPos[twoPreviousKey][positionID] + new Vector3(0, 0, (twoPreviousKey) * _frameInterval);
        Vector3 p1 = _changedPos[previousKey][positionID] + new Vector3(0, 0, (previousKey) * _frameInterval);
        Vector3 p2 = targetPosition;
        Vector3 p3 = _changedPos[afterKey][positionID] + new Vector3(0, 0, (afterKey) * _frameInterval);

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
                Spline.GetInstance().SetSpline(positionID, i + previousKey + 1, beforePoints[i]);
            }
        }

        if (!isFirst)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                _changedPos[i + previousKey + 1][positionID] = beforePoints[i] - new Vector3(0.0f, 0.0f, (i + previousKey + 1) * _frameInterval);
            }
            EditManager.GetInstance().ChangePos = _changedPos;
        }
    }

    public void SetOtherAfterLinePos(Vector3 targetPosition, int positionID, int frame, bool isFirst)
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

        Vector3 p0 = _changedPos[previousKey][positionID] + new Vector3(0, 0, (previousKey) * _frameInterval);
        Vector3 p1 = targetPosition;
        Vector3 p2 = _changedPos[afterKey][positionID] + new Vector3(0, 0, (afterKey) * _frameInterval);
        Vector3 p3 = _changedPos[twoAfterKey][positionID] + new Vector3(0, 0, (twoAfterKey) * _frameInterval);

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
                Spline.GetInstance().SetSpline(positionID, i + currentKey, afterPoints[i]);
            }
        }

        if (!isFirst)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                _changedPos[i + currentKey][positionID] = afterPoints[i] - new Vector3(0.0f, 0.0f, (i + currentKey) * _frameInterval);
            }
            EditManager.GetInstance().ChangePos = _changedPos;
        }
    }

    //�X�v���C���⊮�@�֐�
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
