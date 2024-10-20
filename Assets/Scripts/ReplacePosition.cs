using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplacePosition : MonoBehaviour
{
    [SerializeField] AnimationSceneManager _sceneManager;

    private Dictionary<int, Vector3[]> _modelPos = new Dictionary<int, Vector3[]>();
    private Dictionary<int, Vector3[]> _changedPos = new Dictionary<int, Vector3[]>();
    private List<int> _keyPoseList = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetRequiredalue()
    {
        _modelPos = _sceneManager.GetModelPos();
        _changedPos = _sceneManager.GetChangedPos();
        _keyPoseList = _sceneManager.GetKeyPoseList();
    }

    //最初のフレームの編集時
    public void SetFirstFrameLinePos(Vector3 targetPosition, int positionID, LineRenderer lineRenderer)
    {
        GetRequiredalue();

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
            Vector3 p2 = _changedPos[afterKey][positionID] + new Vector3(0, 0, (afterKey) * 0.30f);
            Vector3 p3 = _changedPos[twoAfterKey][positionID] + new Vector3(0, 0, (twoAfterKey) * 0.30f);

            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);
            var savedPoint = _modelPos[i][positionID] + new Vector3(0.0f, 0.0f, (i) * 0.30f);

            points[i] = splinedPoint;
            points[i] = Vector3.Lerp(splinedPoint, savedPoint, 0.2f);     
        }
        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                lineRenderer.SetPosition(i, points[i]);
            }
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _changedPos[i][positionID] = points[i] - new Vector3(0.0f, 0.0f, i * 0.30f);
        }

        _sceneManager.SetChangedPos(_changedPos);
    }

    //最後のフレームの編集時
    public void SetLastFrameLinePos(Vector3 targetPosition, int positionID, LineRenderer lineRenderer)
    {
        GetRequiredalue();

        int previousKey = _keyPoseList[_keyPoseList.Count - 2];
        int numberOfPoints = _keyPoseList[_keyPoseList.Count - 1] - previousKey;
        Vector3[] points = new Vector3[numberOfPoints];
        int twoPreviousKey = _keyPoseList[_keyPoseList.Count - 3];
        points[numberOfPoints - 1] = targetPosition;

        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            float t = (float)i / (float)numberOfPoints;
            Vector3 p0 = _changedPos[twoPreviousKey][positionID] + new Vector3(0, 0, (twoPreviousKey) * 0.30f);
            Vector3 p1 = _changedPos[previousKey][positionID] + new Vector3(0, 0, (previousKey) * 0.30f);
            Vector3 p2 = targetPosition;
            Vector3 p3 = targetPosition;

            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);
            var savedPoint = _modelPos[i + previousKey + 1][positionID] + new Vector3(0.0f, 0.0f, (i + previousKey + 1) * 0.30f);

            points[i] = splinedPoint;
            points[i] = Vector3.Lerp(splinedPoint, savedPoint, 0.2f);
        }
        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                lineRenderer.SetPosition(i + previousKey + 1, points[i]);
            }
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _modelPos[i + previousKey + 1][positionID] = points[i] - new Vector3(0.0f, 0.0f, (i + previousKey + 1) * 0.30f);
        }

        _sceneManager.SetChangedPos(_changedPos);
    }

    //２番目のキーフレーム編集時
    public void SetSecondFrameLinePos(Vector3 targetPosition, int positionID, LineRenderer lineRenderer, int frame)
    {
        GetRequiredalue();

        int listIndex = _keyPoseList.IndexOf(frame);
        int currentKey = _keyPoseList[listIndex];
        int afterKey = _keyPoseList[listIndex + 1];
        int previousKey = _keyPoseList[listIndex - 1];
        int twoAfterKey = _keyPoseList[listIndex + 2];
        int numberOfPoints = currentKey - previousKey;
        Vector3[] beforePoints = new Vector3[numberOfPoints];
        beforePoints[numberOfPoints - 1] = targetPosition;


        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            float t = (float)(i + 1) / (float)numberOfPoints;
            Vector3 p0 = _changedPos[previousKey][positionID] + new Vector3(0, 0, (previousKey) * 0.30f);
            Vector3 p1 = _changedPos[previousKey][positionID] + new Vector3(0, 0, (previousKey) * 0.30f);
            Vector3 p2 = targetPosition;
            Vector3 p3 = _changedPos[afterKey][positionID] + new Vector3(0, 0, (afterKey) * 0.30f);

            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);
            var savedPoint = _modelPos[i + previousKey + 1][positionID] + new Vector3(0.0f, 0.0f, (i + previousKey + 1) * 0.30f);

            beforePoints[i] = splinedPoint;
            beforePoints[i] = Vector3.Lerp(splinedPoint, savedPoint, 0.2f);
        }

        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                lineRenderer.SetPosition(i + previousKey + 1, beforePoints[i]);
            }
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _changedPos[i + previousKey + 1][positionID] = beforePoints[i] - new Vector3(0.0f, 0.0f, (i + previousKey + 1) * 0.30f);
        }

        numberOfPoints = _keyPoseList[listIndex + 1] - currentKey;
        Vector3[] afterPoints = new Vector3[numberOfPoints];
        afterPoints[0] = targetPosition;

        for (int i = 1; i < numberOfPoints; i++)
        {
            float t = (float)i / (float)numberOfPoints;
            Vector3 p0 = _changedPos[previousKey][positionID] + new Vector3(0, 0, (currentKey - 1) * 0.30f);
            Vector3 p1 = targetPosition;
            Vector3 p2 = _changedPos[afterKey][positionID] + new Vector3(0, 0, (afterKey) * 0.30f);
            Vector3 p3 = _changedPos[twoAfterKey][positionID] + new Vector3(0, 0, (twoAfterKey) * 0.30f);

            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);
            var savedPoint = _modelPos[i + currentKey][positionID] + new Vector3(0.0f, 0.0f, (i + currentKey) * 0.30f);

            afterPoints[i] = splinedPoint;
            afterPoints[i] = Vector3.Lerp(splinedPoint, savedPoint, 0.2f);
        }
        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                lineRenderer.SetPosition(i + currentKey, afterPoints[i]);
            }
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _changedPos[i + currentKey][positionID] = afterPoints[i] - new Vector3(0.0f, 0.0f, (i + currentKey) * 0.30f);
        }

        _sceneManager.SetChangedPos(_changedPos);
    }

    //最後から２番目のキーフレーム編集時
    public void SetSecondToLastFrameLinePos(Vector3 targetPosition, int positionID, LineRenderer lineRenderer, int frame)
    {
        GetRequiredalue();

        int listIndex = _keyPoseList.IndexOf(frame);
        int currentKey = _keyPoseList[listIndex];
        int afterKey = _keyPoseList[listIndex + 1];
        int previousKey = _keyPoseList[listIndex - 1];
        int twoPreviousKey = _keyPoseList[listIndex - 2];
        int numberOfPoints = currentKey - previousKey;
        Vector3[] before_points = new Vector3[numberOfPoints];
        before_points[numberOfPoints - 1] = targetPosition;


        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            float t = (float)(i + 1) / (float)numberOfPoints;
            Vector3 p0 = _changedPos[twoPreviousKey][positionID] + new Vector3(0, 0, (twoPreviousKey) * 0.30f);
            Vector3 p1 = _changedPos[previousKey][positionID] + new Vector3(0, 0, (previousKey) * 0.30f);
            Vector3 p2 = targetPosition;
            Vector3 p3 = _changedPos[afterKey][positionID] + new Vector3(0, 0, (afterKey) * 0.30f);

            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);
            var savedPoint = _modelPos[i + previousKey + 1][positionID] + new Vector3(0.0f, 0.0f, (i + previousKey + 1) * 0.30f);

            before_points[i] = splinedPoint;
            before_points[i] = Vector3.Lerp(splinedPoint, savedPoint, 0.2f);
        }

        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                lineRenderer.SetPosition(i + previousKey + 1, before_points[i]);
            }
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _changedPos[i + previousKey + 1][positionID] = before_points[i] - new Vector3(0.0f, 0.0f, (i + previousKey + 1) * 0.30f);
        }

        numberOfPoints = _keyPoseList[listIndex + 1] - currentKey;
        Vector3[] afterPoints = new Vector3[numberOfPoints];
        afterPoints[0] = targetPosition;

        for (int i = 1; i < numberOfPoints; i++)
        {
            float t = (float)i / (float)numberOfPoints;
            Vector3 p0 = _changedPos[previousKey][positionID] + new Vector3(0, 0, (currentKey - 1) * 0.30f);
            Vector3 p1 = targetPosition;
            Vector3 p2 = _changedPos[afterKey][positionID] + new Vector3(0, 0, (afterKey) * 0.30f);
            Vector3 p3 = _changedPos[afterKey][positionID] + new Vector3(0, 0, (afterKey) * 0.30f);

            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);
            var savedPoint = _modelPos[i + currentKey][positionID] + new Vector3(0.0f, 0.0f, (i + currentKey) * 0.30f);

            afterPoints[i] = splinedPoint;
            afterPoints[i] = Vector3.Lerp(splinedPoint, savedPoint, 0.2f);
        }
        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                lineRenderer.SetPosition(i + currentKey, afterPoints[i]);
            }
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _changedPos[i + currentKey][positionID] = afterPoints[i] - new Vector3(0.0f, 0.0f, (i + currentKey) * 0.30f);
        }

        _sceneManager.SetChangedPos(_changedPos);
    }

    //それ以外のフレーム編集時
    public void SetOtherFramesLinePos(Vector3 targetPosition, int positionID, LineRenderer lineRenderer, int frame)
    {
        GetRequiredalue();

        int listIndex = _keyPoseList.IndexOf(frame);
        int currentKey = _keyPoseList[listIndex];
        int afterKey = _keyPoseList[listIndex + 1];
        int previousKey = _keyPoseList[listIndex - 1];
        int twoPreviousKey = _keyPoseList[listIndex - 2];
        int twoAfterKey = _keyPoseList[listIndex + 2];
        int numberOfPoints = currentKey - previousKey;
        Vector3[] before_points = new Vector3[numberOfPoints];
        before_points[numberOfPoints - 1] = targetPosition;


        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            float t = (float)(i + 1) / (float)numberOfPoints;
            Vector3 p0 = _changedPos[twoPreviousKey][positionID] + new Vector3(0, 0, (twoPreviousKey) * 0.30f);
            Vector3 p1 = _changedPos[previousKey][positionID] + new Vector3(0, 0, (previousKey) * 0.30f);
            Vector3 p2 = targetPosition;
            Vector3 p3 = _changedPos[afterKey][positionID] + new Vector3(0, 0, (afterKey) * 0.30f);

            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);
            var savedPoint = _modelPos[i + previousKey + 1][positionID] + new Vector3(0.0f, 0.0f, (i + previousKey + 1) * 0.30f);

            before_points[i] = splinedPoint;
            before_points[i] = Vector3.Lerp(splinedPoint, savedPoint, 0.2f);

        }
        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                lineRenderer.SetPosition(i + previousKey + 1, before_points[i]);
            }
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _changedPos[i + previousKey + 1][positionID] = before_points[i] - new Vector3(0.0f, 0.0f, (i + previousKey + 1) * 0.30f);
        }




        numberOfPoints = _keyPoseList[listIndex + 1] - currentKey;
        Vector3[] afterPoints = new Vector3[numberOfPoints];
        afterPoints[0] = targetPosition;

        for (int i = 1; i < numberOfPoints; i++)
        {
            float t = (float)i / (float)numberOfPoints;
            Vector3 p0 = _changedPos[previousKey][positionID] + new Vector3(0, 0, (currentKey - 1) * 0.30f);
            Vector3 p1 = targetPosition;
            Vector3 p2 = _changedPos[afterKey][positionID] + new Vector3(0, 0, (afterKey) * 0.30f);
            Vector3 p3 = _changedPos[twoAfterKey][positionID] + new Vector3(0, 0, (twoAfterKey) * 0.30f);

            var splinedPoint = CatmullRomSpline(p0, p1, p2, p3, t);
            var savedPoint = _modelPos[i + currentKey][positionID] + new Vector3(0.0f, 0.0f, (i + currentKey) * 0.30f);

            afterPoints[i] = splinedPoint;
            afterPoints[i] = Vector3.Lerp(splinedPoint, savedPoint, 0.2f);
        }

        if (positionID < 4)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                lineRenderer.SetPosition(i + currentKey, afterPoints[i]);
            }
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            _changedPos[i + currentKey][positionID] = afterPoints[i] - new Vector3(0.0f, 0.0f, (i + currentKey) * 0.30f);
        }
        
        _sceneManager.SetChangedPos(_changedPos);
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
