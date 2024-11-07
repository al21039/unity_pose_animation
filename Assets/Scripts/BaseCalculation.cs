using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCalculation : MonoBehaviour
{

    public float[] ReturnDistance(GameObject[] modelObjects)
    {
        float[] modelDistance = new float[19];

        if (modelObjects.Length != 19)
        {
            Debug.LogError("óvëfêîÇ™ë´ÇËÇ‹ÇπÇÒ");
        }
        else
        {
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

        }
        return modelDistance;
    }

    public float[] ReturnDistance(Vector3[] mediaPipePosition)
    {
        float[] mediaPipeDistance = new float[19];

        if (mediaPipePosition.Length != 19)
        {
            Debug.LogError("óvëfêîÇ™ë´ÇËÇ‹ÇπÇÒ");
        }
        else
        {
            mediaPipeDistance[0] = Vector3.Distance(mediaPipePosition[0], mediaPipePosition[1]);
            mediaPipeDistance[1] = Vector3.Distance(mediaPipePosition[1], mediaPipePosition[2]);
            mediaPipeDistance[2] = Vector3.Distance(mediaPipePosition[3], mediaPipePosition[4]);
            mediaPipeDistance[3] = Vector3.Distance(mediaPipePosition[4], mediaPipePosition[5]);
            mediaPipeDistance[4] = Vector3.Distance(mediaPipePosition[5], mediaPipePosition[6]);
            mediaPipeDistance[5] = Vector3.Distance(mediaPipePosition[7], mediaPipePosition[8]);
            mediaPipeDistance[6] = Vector3.Distance(mediaPipePosition[8], mediaPipePosition[9]);
            mediaPipeDistance[7] = Vector3.Distance(mediaPipePosition[9], mediaPipePosition[10]);
            mediaPipeDistance[8] = Vector3.Distance(mediaPipePosition[11], mediaPipePosition[12]);
            mediaPipeDistance[9] = Vector3.Distance(mediaPipePosition[12], mediaPipePosition[13]);
            mediaPipeDistance[10] = Vector3.Distance(mediaPipePosition[13], mediaPipePosition[14]);
            mediaPipeDistance[11] = Vector3.Distance(mediaPipePosition[15], mediaPipePosition[16]);
            mediaPipeDistance[12] = Vector3.Distance(mediaPipePosition[16], mediaPipePosition[17]);
            mediaPipeDistance[13] = Vector3.Distance(mediaPipePosition[17], mediaPipePosition[18]);
        }
        return mediaPipeDistance;
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
