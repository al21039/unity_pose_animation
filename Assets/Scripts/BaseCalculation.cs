using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCalculation : MonoBehaviour
{
    private float[] modelDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float[] ReturnDistance(GameObject[] modelObjects)
    {
        modelDistance = new float[14];
        modelDistance[0] = CalcDictance(modelObjects[0], modelObjects[1]);
        modelDistance[1] = CalcDictance(modelObjects[1], modelObjects[2]);
        modelDistance[2] = CalcDictance(modelObjects[3], modelObjects[4]);
        modelDistance[3] = CalcDictance(modelObjects[4], modelObjects[5]);
        modelDistance[4] = CalcDictance(modelObjects[5], modelObjects[6]);
        modelDistance[5] = CalcDictance(modelObjects[7], modelObjects[8]);
        modelDistance[6] = CalcDictance(modelObjects[8], modelObjects[9]);
        modelDistance[7] = CalcDictance(modelObjects[9], modelObjects[10]);
        modelDistance[8] = CalcDictance(modelObjects[11], modelObjects[12]);
        modelDistance[9] = CalcDictance(modelObjects[12], modelObjects[13]);
        modelDistance[10] = CalcDictance(modelObjects[13], modelObjects[14]);
        modelDistance[11] = CalcDictance(modelObjects[15], modelObjects[16]);
        modelDistance[12] = CalcDictance(modelObjects[16], modelObjects[17]);
        modelDistance[13] = CalcDictance(modelObjects[17], modelObjects[18]);

        return modelDistance;
    }

    public float[] ReturnDistance(Vector3[] mediaPipePosition)
    {
        modelDistance = new float[14];
        modelDistance[0] = Vector3.Distance(mediaPipePosition[0], mediaPipePosition[1]);
        modelDistance[1] = Vector3.Distance(mediaPipePosition[1], mediaPipePosition[2]);
        modelDistance[2] = Vector3.Distance(mediaPipePosition[3], mediaPipePosition[4]);
        modelDistance[3] = Vector3.Distance(mediaPipePosition[4], mediaPipePosition[5]);
        modelDistance[4] = Vector3.Distance(mediaPipePosition[5], mediaPipePosition[6]);
        modelDistance[5] = Vector3.Distance(mediaPipePosition[7], mediaPipePosition[8]);
        modelDistance[6] = Vector3.Distance(mediaPipePosition[8], mediaPipePosition[9]);
        modelDistance[7] = Vector3.Distance(mediaPipePosition[9], mediaPipePosition[10]);
        modelDistance[8] = Vector3.Distance(mediaPipePosition[11], mediaPipePosition[12]);
        modelDistance[9] = Vector3.Distance(mediaPipePosition[12], mediaPipePosition[13]);
        modelDistance[10] = Vector3.Distance(mediaPipePosition[13], mediaPipePosition[14]);
        modelDistance[11] = Vector3.Distance(mediaPipePosition[15], mediaPipePosition[16]);
        modelDistance[12] = Vector3.Distance(mediaPipePosition[16], mediaPipePosition[17]);
        modelDistance[13] = Vector3.Distance(mediaPipePosition[17], mediaPipePosition[18]);

        return modelDistance;
    }

    private float CalcDictance(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);   
    }

    public float[] ReturnDiff(float[] modelDis, float[] mediapipeDis)
    {
        float[] diff;

        if(modelDis.Length != mediapipeDis.Length)
        {
            return null;
        }

        diff = new float[modelDis.Length];
        for(int i = 0; i < modelDis.Length; i++)
        {
            diff[i] = modelDis[i] / mediapipeDis[i];
        }

        return diff;
    }
}
