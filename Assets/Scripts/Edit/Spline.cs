using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spline : MonoBehaviour
{
    [SerializeField] private GameObject[] _splines;

    private static Spline instance;

    private LineRenderer[] _splineRenderer;

    public static Spline GetInstance() => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

        void Start()
    {
        _splineRenderer = new LineRenderer[_splines.Length];
        for (int i = 0; i < _splineRenderer.Length; i++)
        {
            _splineRenderer[i] = _splines[i].GetComponent<LineRenderer>();       //SplineRenderer�̎擾
        }

    }


    //�X�v���C���̕\���𔽓]
    public void SwitchSplineDisplay(int splineNo)
    {
        _splines[splineNo].SetActive(!_splines[splineNo].activeSelf);
    }


    //�X�v���C���̓_�̐ݒ�
    public void SetSpline(int splineNo, int index, Vector3 point)
    {
        _splineRenderer[splineNo].SetPosition(index, point);
    }

    public void SerializeSpline(int totaFrame)
    {
        for (int i = 0; i < _splines.Length; i++)
        {
            _splineRenderer[i].positionCount = totaFrame;
        }
    }
}
