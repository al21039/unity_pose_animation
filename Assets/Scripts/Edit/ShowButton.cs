using UnityEngine;

public class ShowButton : MonoBehaviour
{
    [SerializeField] GameObject _humanoid;

    public void SetHumanoid()
    {
        GameObject humanoid = Instantiate(_humanoid, Vector3.zero, Quaternion.identity);

        ShowAnimationInEdit showAnimationInEdit = humanoid.GetComponent<ShowAnimationInEdit>();
        showAnimationInEdit.Prepare();
    }

}
