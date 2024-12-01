using UnityEngine;

public class ClacheightButton : MonoBehaviour
{
    [SerializeField] private float addValue;

    public void AddHeight()
    {
        PositionMover.GetInstance().ChangeHeight(addValue);
    }
}
