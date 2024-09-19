using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trajectory_Button_Script : MonoBehaviour
{

    // Start is called before the first frame update

    public void ChangeToAnimationScene()
    {
        SceneManager.LoadScene("createAnimation");
    }
}
