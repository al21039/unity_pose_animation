using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.IO;
using System;

public class ImageSender : MonoBehaviour
{
    [SerializeField] private Texture2D _texture;
    [SerializeField] private LandmarkProcesser _processer;

    //Texture2DをPNG形式に変換
    public void SendImageToPython()
    {
        Texture2D newTexture = new Texture2D(_texture.width, _texture.height, TextureFormat.RGBA32, false);
        newTexture.SetPixels(_texture.GetPixels());
        newTexture.Apply();

        byte[] imageData = newTexture.EncodeToPNG();
        StartCoroutine(SendImageData(imageData));
    }

    //PythonコードにPNGファイルを渡し、MediaPipeをしてもらう
    private IEnumerator SendImageData(byte[] imageData)
    {
        UnityWebRequest request = new UnityWebRequest("http://localhost:5000/upload", "POST");
        request.uploadHandler = new UploadHandlerRaw(imageData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/octet-stream");

        //リクエストの結果が返ってくるまで待つ
        yield return request.SendWebRequest();

        //リクエストが正常に実行されたら
        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonData = request.downloadHandler.text;

            // JSONデータをファイルに保存
            WriteJsonToFile(jsonData);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    //JSONファイルをAssets/JSONフォルダに保存
    private void WriteJsonToFile(string jsonData)
    {
        string jsonFileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".json";

        // ファイルの保存先パスを指定
        string filePath = Path.Combine(Application.dataPath + "/JSON", jsonFileName);

        string directoryPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // JSONデータをファイルに書き込み
        File.WriteAllText(filePath, jsonData);
        Debug.Log("JSON data saved to: " + filePath);

        
        Vector3[] jsonLandmarks = _processer.GetLandmarksFromJson(jsonData);


        //AnimationSceneManager.GetInstance().SetNewKeyPoseFilePath(filePath);
    }
}