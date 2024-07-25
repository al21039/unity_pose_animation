using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimButton : MonoBehaviour
{
    public ShowAnimation showAnimation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void push()
    {
        if(!showAnimation.show && !showAnimation.check)
        {
            showAnimation.currentFrame = 0;
            showAnimation.show = true;
            showAnimation.check = true;
        }
    }
}
