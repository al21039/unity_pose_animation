using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    //�v���C���[��ϐ��Ɋi�[
    public GameObject Player;

    //��]������X�s�[�h

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        transform.RotateAround(Player.transform.position, new Vector3(0, 1, 0), 1.0f);
    }
}