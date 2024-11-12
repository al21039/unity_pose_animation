using UnityEngine;

public class UIListener : MonoBehaviour
{
    [SerializeField] private GameObject[] _UIs;
    [SerializeField] private GameObject _scrollView;

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
        EditManager.GetInstance().DisplayNewAnimation();
    }

    public void OnClickedAddModelButton()
    {
        _scrollView.SetActive(!_scrollView.activeSelf);
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
        _scrollView.SetActive(status);
    }

}
