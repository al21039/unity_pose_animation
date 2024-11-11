using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class aaa : MonoBehaviour
{
    [SerializeField] private Image _button;
    // Start is called before the first frame update
    void Start()
    {
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/0-1_2-3.png");
        _button.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
