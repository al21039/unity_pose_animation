using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCalc : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float[] ReturnLimbDistance(Vector3[] mediaPipeLandmarkPos)
    {
        float[] limbDistanceArray = new float[8];

        limbDistanceArray[0] = Vector3.Distance(mediaPipeLandmarkPos[0], mediaPipeLandmarkPos[1]);
        limbDistanceArray[1] = Vector3.Distance(mediaPipeLandmarkPos[1], mediaPipeLandmarkPos[2]);
        limbDistanceArray[2] = Vector3.Distance(mediaPipeLandmarkPos[3], mediaPipeLandmarkPos[4]);
        limbDistanceArray[3] = Vector3.Distance(mediaPipeLandmarkPos[4], mediaPipeLandmarkPos[5]);
        limbDistanceArray[4] = Vector3.Distance(mediaPipeLandmarkPos[6], mediaPipeLandmarkPos[7]);
        limbDistanceArray[5] = Vector3.Distance(mediaPipeLandmarkPos[7], mediaPipeLandmarkPos[8]);
        limbDistanceArray[6] = Vector3.Distance(mediaPipeLandmarkPos[9], mediaPipeLandmarkPos[10]);
        limbDistanceArray[7] = Vector3.Distance(mediaPipeLandmarkPos[10], mediaPipeLandmarkPos[11]);

        return limbDistanceArray;
    }

    public float[] ReturnLimbDistance(GameObject[] limbObject)
    {
        float[] limbDistanceArray = new float[8];

        limbDistanceArray[0] = CalcObjectDistance(limbObject[0], limbObject[1]);
        limbDistanceArray[1] = CalcObjectDistance(limbObject[1], limbObject[2]);
        limbDistanceArray[2] = CalcObjectDistance(limbObject[3], limbObject[4]);
        limbDistanceArray[3] = CalcObjectDistance(limbObject[4], limbObject[5]);
        limbDistanceArray[4] = CalcObjectDistance(limbObject[6], limbObject[7]);
        limbDistanceArray[5] = CalcObjectDistance(limbObject[7], limbObject[8]);
        limbDistanceArray[6] = CalcObjectDistance(limbObject[9], limbObject[10]);
        limbDistanceArray[7] = CalcObjectDistance(limbObject[10], limbObject[11]);

        return limbDistanceArray;
    }

    private float CalcObjectDistance(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

}
