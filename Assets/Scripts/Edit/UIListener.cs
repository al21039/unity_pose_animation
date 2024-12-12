using UnityEngine;
using UnityEngine.UI;

public class UIListener : MonoBehaviour
{
    [SerializeField] private GameObject[] _UIs;
    [SerializeField] private ScrollViewButton _scrollViewButton;
    [SerializeField] private GameObject _addModelView;
    [SerializeField] private GameObject _heightClacButton;
    [SerializeField] private GameObject _backButton;

    public void OnClickedLeftHandSplineButton()
    {
        Spline.GetInstance().SwitchSplineDisplay(0);
    }

    public void OnClickedRightHandSplineButton()
    {
        Spline.GetInstance().SwitchSplineDisplay(1);
    }

    public void OnClickedLeftFootSplineButton()
    {
        Spline.GetInstance().SwitchSplineDisplay(2);
    }
    public void OnClickedRightFootSplineButton()
    {
        Spline.GetInstance().SwitchSplineDisplay(3);
    }

    public void OnClickedCreateAnimationButton()
    {
        ChangeUIDisplay(false);
        _backButton.SetActive(true);
        ChangeScrollDisplay(false);
        EditManager.GetInstance().DisplayNewAnimation();
    }

    public void OnClickedAddModelButton()
    {
        _UIs[4].SetActive(!_UIs[4].activeSelf);
        _UIs[5].SetActive(!_UIs[5].activeSelf);
        _UIs[6].SetActive(!_UIs[6].activeSelf);
        _UIs[8].SetActive(!_UIs[8].activeSelf);
        _UIs[9].SetActive(!_UIs[9].activeSelf);
        _addModelView.SetActive(!_addModelView.activeSelf);
        _scrollViewButton.DeleteAddFrameIndex();
    }

    public void ChangeUIDisplay(bool status)
    {
        for (int i = 0; i < _UIs.Length; i++)
        {
            _UIs[i].SetActive(status);
        }
    }

    public void ChangeScrollDisplay(bool status)
    {
        _addModelView.SetActive(status);
    }

    public void OnClickedIndirectButton()
    {
        _UIs[4].SetActive(!_UIs[4].activeSelf);
        _UIs[6].SetActive(!_UIs[6].activeSelf);
        _UIs[7].SetActive(!_UIs[7].activeSelf);
        _UIs[8].SetActive(!_UIs[8].activeSelf);
        _UIs[9].SetActive(!_UIs[9].activeSelf);
    }

    public void OnClickedHeightButton()
    {
        _UIs[4].SetActive(!_UIs[4].activeSelf);
        _UIs[5].SetActive(!_UIs[5].activeSelf);
        _UIs[7].SetActive(!_UIs[7].activeSelf);
        _UIs[8].SetActive(!_UIs[8].activeSelf);
        _UIs[9].SetActive(!_UIs[9].activeSelf);
        _heightClacButton.SetActive(!_heightClacButton.activeSelf);
    }

    public void OnClickedDeleteButton()
    {
        _UIs[4].SetActive(!_UIs[4].activeSelf);
        _UIs[5].SetActive(!_UIs[5].activeSelf);
        _UIs[6].SetActive(!_UIs[6].activeSelf);
        _UIs[7].SetActive(!_UIs[7].activeSelf);
        _UIs[9].SetActive(!_UIs[9].activeSelf);
    }
    public void OnClickedRotationButton()
    {
        _UIs[4].SetActive(!_UIs[4].activeSelf);
        _UIs[5].SetActive(!_UIs[5].activeSelf);
        _UIs[6].SetActive(!_UIs[6].activeSelf);
        _UIs[7].SetActive(!_UIs[7].activeSelf);
        _UIs[8].SetActive(!_UIs[8].activeSelf);
    }

    public void OnClickedBackButton()
    {
        ChangeUIDisplay(true);
        _backButton.SetActive(false);
    }
}
