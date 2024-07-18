using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrajectoryButton : MonoBehaviour
{
    public void ChangeToTrajectoryScene()
    {
        SceneManager.LoadScene("showTrajectroy");
    }
}
