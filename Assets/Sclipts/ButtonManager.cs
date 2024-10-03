using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] GameObject _game_manager_script;
    private AnimationSceneManager _scene_manager;
    // Start is called before the first frame update
    void Start()
    {
        _scene_manager = _game_manager_script.GetComponent<AnimationSceneManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickedLeftHandButton()
    {
        _scene_manager.DisplayLeftHandSpline();
    }

    public void OnClickedRightHandButton()
    {
        _scene_manager.DisplayRightHandSpline();
    }

    public void OnClickedLeftFootButton()
    {
        _scene_manager.DisplayLeftFootSpline();

    }

    public void OnClickedRightFootButton()
    {
        _scene_manager.DisplayRightFootSpline();
    }

    public void OnClickedCreateAnimationButton()
    {
        _scene_manager.DisplayNewAnimation();
    }
}
