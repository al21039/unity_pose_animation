using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageDownloader : MonoBehaviour
{
    [SerializeField] Image _image;
    public string imageUrl = "https://cdn-ak.f.st-hatena.com/images/fotolife/M/MouseComputer/20240731/20240731110005.jpg"; // 画像のURL

    public void Start()
    {
        StartCoroutine(DownloadImage(imageUrl));
    }

    private IEnumerator DownloadImage(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest(); // リクエストを送信して応答を待つ

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error downloading image: " + www.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www); // 画像データを取得
            _image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)); ;
        }
    }
}
