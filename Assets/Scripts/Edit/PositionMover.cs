using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PositionMover : MonoBehaviour
{
    [SerializeField] private GameObject[] _indirectOptions;
    [SerializeField] private GameObject _spherePrefab;
    [SerializeField] private Dropdown _dropDown;


    private bool _isDisplay = false;                              //球が表示されているかいないか
    private GameObject _indirectSphere;
    private Camera _mainCamera;
    private int _positionID = -1;

    public void DropdownValueChanged(Dropdown change)
    {
        int selectPosition = change.value - 1;
        Debug.Log(selectPosition);
    }

    private void Start()
    {
        _dropDown.onValueChanged.AddListener(delegate { DropdownValueChanged(_dropDown); });
    }

    public void OnClickedIndirectButton()
    {
        _isDisplay = !_isDisplay;

        _indirectOptions[0].SetActive(_isDisplay);
        _indirectOptions[1].SetActive(_isDisplay);

        if(_isDisplay)
        {
            _mainCamera = Camera.main;
            Vector3 _sphereSpawnPosition = _mainCamera.transform.position + _mainCamera.transform.forward * 3.0f;       //カメラより少し前の位置

            _indirectSphere = Instantiate(_spherePrefab, _sphereSpawnPosition, Quaternion.identity);

        }
        else
        {
            Destroy(_indirectSphere);

        }

    }




}
