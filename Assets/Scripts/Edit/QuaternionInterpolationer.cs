using System.Collections.Generic;
using UnityEngine;

public class QuaternionInterpolationer : MonoBehaviour
{
    private Dictionary<int, Quaternion[]> _QuaternionDic = new Dictionary<int, Quaternion[]>();
    private List<int> _keyPoseList = new List<int>();
    private List<Quaternion> quaternions = new List<Quaternion>();

    private void Prepare()
    {
        _QuaternionDic = EditManager.GetInstance().ChangeRot;
        _keyPoseList = EditManager.GetInstance().KeyPoseList;
        quaternions = EditManager.GetInstance().ModelEntireRot;
    }

    public void Interpolation(int listIndex)
    {
        _QuaternionDic = EditManager.GetInstance().ChangeRot;
        _keyPoseList = EditManager.GetInstance().KeyPoseList;

        if (listIndex >= 1 || listIndex <= _keyPoseList.Count - 2)
        {
            ChangeRotation(listIndex);
        }
    }

    public void EntireInterpolation(int listIndex)
    {
        if (listIndex >= 1 || listIndex <= _keyPoseList.Count - 2)
        {
            Prepare();

            int previousKey = _keyPoseList[listIndex - 1];
            int currentKey = _keyPoseList[listIndex];
            int afterKey = _keyPoseList[listIndex + 1];
            int numberOfPoints = currentKey - previousKey;
            Quaternion[] beforePoints = new Quaternion[numberOfPoints];

            Quaternion currentRotation = quaternions[currentKey];
            beforePoints[numberOfPoints - 1] = currentRotation;

            Quaternion p0 = quaternions[previousKey];
            Quaternion p1 = currentRotation;

            for (int j = 0; j < numberOfPoints - 1; j++)
            {
                float t = (float)(j + 1) / (float)numberOfPoints;

                var slerpRotation = Quaternion.Slerp(p0, p1, t);

                beforePoints[j] = slerpRotation;
            }

            for (int j = 0; j < numberOfPoints; j++)
            {
                quaternions[j + previousKey + 1] = beforePoints[j];
            }

            numberOfPoints = _keyPoseList[listIndex + 1] - currentKey;

            Quaternion[] afterPoints = new Quaternion[numberOfPoints];


            afterPoints[0] = currentRotation;

            p0 = currentRotation;
            p1 = quaternions[afterKey];

            for (int j = 1; j < numberOfPoints; j++)
            {
                float t = (float)(j + 1) / (float)numberOfPoints;

                var slerpRotation = Quaternion.Slerp(p0, p1, t);

                afterPoints[j] = slerpRotation;
            }

            for (int j = 0; j < numberOfPoints; j++)
            {
                quaternions[j + currentKey] = afterPoints[j];
            }

            EditManager.GetInstance().ModelEntireRot = quaternions;
        }
    }

    private void ChangeRotation(int listIndex)
    {

        if (listIndex == 1)
        {
            InterpolationSecond(listIndex);
            InterpolationAfter(listIndex);
        }
        else if ((listIndex == _keyPoseList.Count - 2))
        {
            InterpolationBefore(listIndex);
            InterpolationSecondToLast(listIndex);
        }
        else if (listIndex != 0 && listIndex !=  _keyPoseList.Count - 1)
        {
            InterpolationBefore(listIndex);
            InterpolationAfter(listIndex);
        }
    }

    private void InterpolationSecond(int listIndex)
    {
        Prepare();
        int previousKey = _keyPoseList[listIndex - 1];
        int currentKey = _keyPoseList[listIndex];
        int afterKey = _keyPoseList[listIndex + 1];
        int numberOfPoints = currentKey - previousKey;
        Quaternion[] beforePoints = new Quaternion[numberOfPoints];


        for (int i = 0; i < _QuaternionDic[_keyPoseList[listIndex]].Length - 1; i++)
        {
            Quaternion currentRotation = _QuaternionDic[currentKey][i];
            beforePoints[numberOfPoints - 1] = currentRotation;

            Quaternion p0 = _QuaternionDic[previousKey][i];
            Quaternion p1 = currentRotation;

            for (int j = 0; j < numberOfPoints - 1; j++)
            {
                float t = (float)(j + 1) / (float)numberOfPoints;

                var slerpRotation = Quaternion.Slerp(p0, p1, t);

                beforePoints[j] = slerpRotation;
            }

            for (int j = 0; j < numberOfPoints; j++)
            {
                _QuaternionDic[j + previousKey + 1][i] = beforePoints[j];
            }
        }
        EditManager.GetInstance().ChangeRot = _QuaternionDic;
    }

    private void InterpolationSecondToLast(int listIndex)
    {
        Prepare();
        int currentKey = _keyPoseList[listIndex];
        int afterKey = _keyPoseList[listIndex + 1];
        int numberOfPoints = _keyPoseList[listIndex + 1] - currentKey;

        Quaternion[] afterPoints = new Quaternion[numberOfPoints];

        for (int i = 0; i < _QuaternionDic[_keyPoseList[listIndex]].Length - 1; i++)
        {
            Quaternion currentRotation = _QuaternionDic[currentKey][i];
            afterPoints[0] = currentRotation;

            Quaternion p0 = currentRotation;
            Quaternion p1 = _QuaternionDic[afterKey][i];

            for (int j = 1; j < numberOfPoints; j++)
            {
                float t = (float)(j + 1) / (float)numberOfPoints;

                var slerpRotation = Quaternion.Slerp(p0, p1, t);

                afterPoints[j] = slerpRotation;
            }

            for (int j = 0; j < numberOfPoints; j++)
            {
                _QuaternionDic[j + currentKey][i] = afterPoints[j];
            }
        }
        EditManager.GetInstance().ChangeRot = _QuaternionDic;
    }

    private void InterpolationBefore(int listIndex)
    {
        Prepare();
        int previousKey = _keyPoseList[listIndex - 2];
        int currentKey = _keyPoseList[listIndex];
        int numberOfPoints = currentKey - previousKey;
        Quaternion[] beforePoints = new Quaternion[numberOfPoints];


        for (int i = 0; i < _QuaternionDic[_keyPoseList[listIndex]].Length - 1; i++)
        {
            Quaternion currentRotation = _QuaternionDic[currentKey][i];
            beforePoints[numberOfPoints - 1] = currentRotation;

            Quaternion p0 = _QuaternionDic[previousKey][i];
            Quaternion p1 = currentRotation;

            for (int j = 0; j < numberOfPoints - 1; j++)
            {
                float t = (float)(j + 1) / (float)numberOfPoints;

                var slerpRotation = Quaternion.Slerp(p0, p1, t);

                beforePoints[j] = slerpRotation;
            }

            for (int j = 0; j < numberOfPoints; j++)
            {
                _QuaternionDic[j + previousKey + 1][i] = beforePoints[j];
            }
            EditManager.GetInstance().ChangeRot = _QuaternionDic;
        }
    }

    private void InterpolationAfter(int listIndex)
    {
        Prepare();
        int currentKey = _keyPoseList[listIndex];
        int afterKey = _keyPoseList[listIndex + 2];
        int numberOfPoints = _keyPoseList[listIndex + 2] - currentKey;

        Quaternion[] afterPoints = new Quaternion[numberOfPoints];

        for (int i = 0; i < _QuaternionDic[_keyPoseList[listIndex]].Length - 1; i++)
        {
            Quaternion currentRotation = _QuaternionDic[currentKey][i];
            afterPoints[0] = currentRotation;

            Quaternion p0 = currentRotation;
            Quaternion p1 = _QuaternionDic[afterKey][i];

            for (int j = 1; j < numberOfPoints; j++)
            {
                float t = (float)(j + 1) / (float)numberOfPoints;

                var slerpRotation = Quaternion.Slerp(p0, p1, t);

                afterPoints[j] = slerpRotation;
            }

            for (int j = 0; j < numberOfPoints; j++)
            {
                _QuaternionDic[j + currentKey][i] = afterPoints[j];
            }
            EditManager.GetInstance().ChangeRot = _QuaternionDic;
        }
    }
}
