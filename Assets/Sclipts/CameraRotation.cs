using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    //プレイヤーを変数に格納
    public GameObject Player;

    //回転させるスピード

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