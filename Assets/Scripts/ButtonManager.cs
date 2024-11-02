using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] GameObject _game_manager_script;
    [SerializeField] Dropdown dropdown;
    private AnimationSceneManager _scene_manager;
    private bool _isDisplay = false;

    // Start is called before the first frame update
    void Start()
    {
        _scene_manager = _game_manager_script.GetComponent<AnimationSceneManager>();
        dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdown); });

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

    public void OnClickedIndirectButton()
    {
        _isDisplay = !_isDisplay;
        if (_isDisplay)
        {
            _scene_manager.DisplayIndirectSphere();
        }
        else
        {
            _scene_manager.DestoryIndirectSphere();
        }
    }

    public void DropdownValueChanged(Dropdown change)
    {
        int selectPosition = change.value - 1;
        Debug.Log(selectPosition);
        _scene_manager.SetSelectPositionID(selectPosition);
    }
}
