using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetImagePosition : MonoBehaviour
{
    [SerializeField] private GameObject[] _Anchors = new GameObject[12];
    private float[] _modelDis = new float[8];
    private float[] _mediaPipeDis = new float[8];

    // Start is called before the first frame update
    void Start()
    {   

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalcModelDis()
    {
        _modelDis[0] = CalcModelPartDistance(_Anchors[0], _Anchors[1]);    //左肘
        _modelDis[1] = CalcModelPartDistance(_Anchors[1], _Anchors[2]);　　//左手
        _modelDis[2] = CalcModelPartDistance(_Anchors[3], _Anchors[4]);    //右ひじ
        _modelDis[3] = CalcModelPartDistance(_Anchors[4], _Anchors[5]);　　//右手
        _modelDis[4] = CalcModelPartDistance(_Anchors[6], _Anchors[7]);    //左ひざ
        _modelDis[5] = CalcModelPartDistance(_Anchors[7], _Anchors[8]);　　//左足
        _modelDis[6] = CalcModelPartDistance(_Anchors[9], _Anchors[10]);   //右ひざ
        _modelDis[7] = CalcModelPartDistance(_Anchors[10], _Anchors[11]);　//右足

        for (int i = 0; i < _Anchors.Length; i++)
        {
            Debug.Log(_Anchors[i].transform.position);
        }
    }

    public void SetImageModel(Vector3[] imagePosition)
    {
        _mediaPipeDis[0] = Vector3.Distance(imagePosition[11], imagePosition[13]);
        _mediaPipeDis[1] = Vector3.Distance(imagePosition[13], imagePosition[15]);
        _mediaPipeDis[2] = Vector3.Distance(imagePosition[12], imagePosition[14]);
        _mediaPipeDis[3] = Vector3.Distance(imagePosition[14], imagePosition[16]);
        _mediaPipeDis[4] = Vector3.Distance(imagePosition[23], imagePosition[25]);
        _mediaPipeDis[5] = Vector3.Distance(imagePosition[25], imagePosition[27]);
        _mediaPipeDis[6] = Vector3.Distance(imagePosition[24], imagePosition[26]);
        _mediaPipeDis[7] = Vector3.Distance(imagePosition[26], imagePosition[28]);

        float[] distanceDiff = new float[8];
        for(int i = 0; i < 8; i++)
        {
            distanceDiff[i] = _modelDis[i] / _mediaPipeDis[i];
            //Debug.Log(distanceDiff[i]);
        }


        _Anchors[1].transform.position = (imagePosition[13] - imagePosition[11]) * distanceDiff[0] + _Anchors[0].transform.position;
        Debug.Log(_Anchors[1].transform.position);
        _Anchors[2].transform.position = (imagePosition[15] - imagePosition[13]) * distanceDiff[1] + _Anchors[1].transform.position;
        Debug.Log(_Anchors[2].transform.position);
        _Anchors[4].transform.position = (imagePosition[14] - imagePosition[12]) * distanceDiff[2] + _Anchors[3].transform.position;
        Debug.Log(_Anchors[4].transform.position);
        _Anchors[5].transform.position = (imagePosition[16] - imagePosition[14]) * distanceDiff[3] + _Anchors[4].transform.position;
        Debug.Log(_Anchors[5].transform.position);
        _Anchors[7].transform.position = (imagePosition[25] - imagePosition[23]) * distanceDiff[4] + _Anchors[6].transform.position;
        Debug.Log(_Anchors[7].transform.position);
        _Anchors[8].transform.position = (imagePosition[29] - imagePosition[25]) * distanceDiff[5] + _Anchors[7].transform.position;
        Debug.Log(_Anchors[8].transform.position);
        _Anchors[10].transform.position = (imagePosition[26] - imagePosition[24]) * distanceDiff[6] + _Anchors[9].transform.position;
        Debug.Log(_Anchors[10].transform.position);
        _Anchors[11].transform.position = (imagePosition[30] - imagePosition[26]) * distanceDiff[7] + _Anchors[10].transform.position;
        Debug.Log(_Anchors[11].transform.position);
    }


    private float CalcModelPartDistance(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }
}
