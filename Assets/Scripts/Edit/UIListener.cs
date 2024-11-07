using UnityEngine;

public class UIListener : MonoBehaviour
{

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

    }

}
