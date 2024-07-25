using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AnimateButton : MonoBehaviour
{
    public void ChageToAnimateScene()
    {
        SceneManager.LoadScene("showAnimation");
    }

    public void ToTrajectory()
    {
        SceneManager.LoadScene("showTrajectoy");
    }
}
