using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class HeightInterpolationer : MonoBehaviour
{
    private List<float> _heightPosition = new List<float>();
    private List<int> _keyPoseList = new List<int>();

    public List<float> Interpolation(int frame, float heightPosition)
    {
        _keyPoseList = EditManager.GetInstance().KeyPoseList;

        if (_keyPoseList.Contains(frame))
        {
            //編集したキーフレームが１番目なら
            if (frame == 0)
            {
                ChangeFirstHeight(heightPosition);
            }
            //編集したキーフレームが最後なら
            else if (frame == _keyPoseList[_keyPoseList.Count - 1])
            {
                ChangeLastHeight(heightPosition);
            }
            //編集したキーフレームが２番目なら
            else if (frame == _keyPoseList[1])
            {
                ChangeSecondHeight(heightPosition, frame);
            }
            //編集したキーフレームが最後から２番目なら
            else if (frame == _keyPoseList[_keyPoseList.Count - 2])
            {
                ChangeLastSecondHeight(heightPosition, frame);
            }
            //編集したキーフレームが上記以外なら
            else
            {
                ChangeMiddleHeight(heightPosition, frame);
            }
        }
        return _heightPosition;
    }

    private void ChangeFirstHeight(float hipHeight)
    {
        _heightPosition = EditManager.GetInstance().HipHeight;
        int numberOfPoints = _keyPoseList[1];
        float[] points = new float[numberOfPoints];
        int afterKey = _keyPoseList[1];
        int twoAfterKey = _keyPoseList[2];
        points[0] = hipHeight;

        float p0 = hipHeight;
        float p1 = hipHeight;
        float p2 = _heightPosition[afterKey];
        float p3 = _heightPosition[twoAfterKey];

        for (int i = 1; i < numberOfPoints; i++)
        {
            float t = (float)i / (float)numberOfPoints;

            var splinedHeight = CatmullRom(p0, p1, p2, p3, t);

            points[i] = splinedHeight;
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _heightPosition[i] = points[i];
        }
        EditManager.GetInstance().HipHeight = _heightPosition;
    }

    private void ChangeLastHeight(float hipHeight)
    {
        _heightPosition = EditManager.GetInstance().HipHeight;
        int previousKey = _keyPoseList[_keyPoseList.Count - 2];
        int numberOfPoints = _keyPoseList[_keyPoseList.Count - 1] - previousKey;
        float[] points = new float[numberOfPoints];
        int twoPreviousKey = _keyPoseList[_keyPoseList.Count - 3];
        points[numberOfPoints - 1] = hipHeight;

        float p0 = _heightPosition[twoPreviousKey];
        float p1 = _heightPosition[previousKey];
        float p2 = hipHeight;
        float p3 = hipHeight;

        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            float t = (float)(i + 1) / (float)numberOfPoints;

            var splinedHeight = CatmullRom(p0, p1, p2, p3, t);

            points[i] = splinedHeight;
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _heightPosition[i + previousKey + 1] = points[i];
        }
        EditManager.GetInstance().HipHeight = _heightPosition;
    }

    private void ChangeSecondHeight(float hipHeight, int frame)
    {
        _heightPosition = EditManager.GetInstance().HipHeight;
        int previousKey = _keyPoseList[0];
        int currentKey = _keyPoseList[1];
        int afterKey = _keyPoseList[2];
        int numberOfPoints = currentKey - previousKey;
        float[] beforePoints = new float[numberOfPoints];
        beforePoints[numberOfPoints - 1] = hipHeight;

        float p0 = _heightPosition[previousKey];
        float p1 = _heightPosition[previousKey];
        float p2 = hipHeight;
        float p3 = _heightPosition[afterKey];
        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            float t = (float)(i + 1) / (float)numberOfPoints;

            var splinedHeight = CatmullRom(p0, p1, p2, p3, t);

            beforePoints[i] = splinedHeight;
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _heightPosition[i + previousKey + 1] = beforePoints[i];
        }

        int listIndex = _keyPoseList.IndexOf(frame);
        currentKey = _keyPoseList[listIndex];
        afterKey = _keyPoseList[listIndex + 1];
        previousKey = _keyPoseList[listIndex - 1];
        int twoAfterKey = _keyPoseList[listIndex + 2];
        numberOfPoints = _keyPoseList[listIndex + 1] - currentKey;
        float[] afterPoints = new float[numberOfPoints];
        afterPoints[0] = hipHeight;

        p0 = _heightPosition[previousKey];
        p1 = hipHeight;
        p2 = _heightPosition[afterKey];
        p3 = _heightPosition[twoAfterKey];

        for (int i = 1; i < numberOfPoints; i++)
        {
            float t = (float)i / (float)numberOfPoints;

            var splinedHeight = CatmullRom(p0, p1, p2, p3, t);

            afterPoints[i] = splinedHeight;
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _heightPosition[i + currentKey]= afterPoints[i];
        }

        EditManager.GetInstance().HipHeight = _heightPosition;
    }

    private void ChangeLastSecondHeight(float hipHeight, int frame)
    {
        _heightPosition = EditManager.GetInstance().HipHeight;
        int listIndex = _keyPoseList.IndexOf(frame);
        int currentKey = _keyPoseList[listIndex];
        int afterKey = _keyPoseList[listIndex + 1];
        int previousKey = _keyPoseList[listIndex - 1];
        int twoPreviousKey = _keyPoseList[listIndex - 2];
        int numberOfPoints = currentKey - previousKey;
        float[] beforePoints = new float[numberOfPoints];
        beforePoints[numberOfPoints - 1] = hipHeight;

        float p0 = _heightPosition[twoPreviousKey];
        float p1 = _heightPosition[previousKey];
        float p2 = hipHeight;
        float p3 = _heightPosition[afterKey];

        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            float t = (float)(i + 1) / (float)numberOfPoints;
            var splinedHeight = CatmullRom(p0, p1, p2, p3, t);
            beforePoints[i] = splinedHeight;
        }
        for (int i = 0; i < numberOfPoints; i++)
        {
            _heightPosition[i + previousKey + 1] = beforePoints[i];
        }


        currentKey = _keyPoseList[_keyPoseList.Count - 2];
        afterKey = _keyPoseList[_keyPoseList.Count - 1];
        previousKey = _keyPoseList[_keyPoseList.Count - 3];
        numberOfPoints = currentKey - previousKey;
        numberOfPoints = afterKey - currentKey;
        float[] afterPoints = new float[numberOfPoints];
        afterPoints[0] = hipHeight;

        p0 = _heightPosition[previousKey];
        p1 = hipHeight;
        p2 = _heightPosition[afterKey];
        p3 = _heightPosition[afterKey];

        for (int i = 1; i < numberOfPoints; i++)
        {
            float t = (float)i / (float)numberOfPoints;
            var splinedHeight = CatmullRom(p0, p1, p2, p3, t);
            afterPoints[i] = splinedHeight;
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _heightPosition[i + currentKey] = afterPoints[i];
        }

        EditManager.GetInstance().HipHeight = _heightPosition;
    }

    private void ChangeMiddleHeight(float hipHeight, int frame)
    {
        _heightPosition = EditManager.GetInstance().HipHeight;
        int listIndex = _keyPoseList.IndexOf(frame);
        int currentKey = _keyPoseList[listIndex];
        int afterKey = _keyPoseList[listIndex + 1];
        int previousKey = _keyPoseList[listIndex - 1];
        int twoPreviousKey = _keyPoseList[listIndex - 2];
        int numberOfPoints = currentKey - previousKey;
        float[] beforePoints = new float[numberOfPoints];
        beforePoints[numberOfPoints - 1] = hipHeight;

        float p0 = _heightPosition[twoPreviousKey];
        float p1 = _heightPosition[previousKey];
        float p2 = hipHeight;
        float p3 = _heightPosition[afterKey];

        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            float t = (float)(i + 1) / (float)numberOfPoints;
            var splinedHeight = CatmullRom(p0, p1, p2, p3, t);
            beforePoints[i] = splinedHeight;
        }
        for (int i = 0; i < numberOfPoints; i++)
        {
            _heightPosition[i + previousKey + 1] = beforePoints[i];
        }

        currentKey = _keyPoseList[listIndex];
        afterKey = _keyPoseList[listIndex + 1];
        previousKey = _keyPoseList[listIndex - 1];
        int twoAfterKey = _keyPoseList[listIndex + 2];
        numberOfPoints = _keyPoseList[listIndex + 1] - currentKey;
        float[] afterPoints = new float[numberOfPoints];
        afterPoints[0] = hipHeight;

        p0 = _heightPosition[previousKey];
        p1 = hipHeight;
        p2 = _heightPosition[afterKey];
        p3 = _heightPosition[twoAfterKey];

        for (int i = 1; i < numberOfPoints; i++)
        {
            float t = (float)i / (float)numberOfPoints;

            var splinedHeight = CatmullRom(p0, p1, p2, p3, t);

            afterPoints[i] = splinedHeight;
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _heightPosition[i + currentKey] = afterPoints[i];
        }

        EditManager.GetInstance().HipHeight = _heightPosition;

    }
    private float CatmullRom(float p0, float p1, float p2, float p3, float t)
    {
        // Catmull-Romスプラインの公式
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

}
