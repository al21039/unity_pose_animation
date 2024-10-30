using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;

public class ImageSender : MonoBehaviour
{
    [SerializeField] private Texture2D _texture;
    [SerializeField] private LandmarkProcesser _processer;
    [SerializeField] private GameObject _spherePrefab;
    public float scale = 2.0f; // ���W�̃X�P�[�����O�t�@�N�^�[
    public Vector3 offset = new Vector3(0, 1, 0); // ���W�̃I�t�Z�b�g
    public Material lineMaterial; // ���C���̃}�e���A��

    private int[,] connections = new int[,]
    {
        {0, 1}, {1, 2}, {2, 3}, {3, 7}, {0, 4}, {4, 5}, {5, 6}, {6, 8},
        {9, 10}, {11, 12}, {11, 13}, {13, 15}, {15, 17}, {15, 19}, {15, 21},
        {17, 19}, {12, 14}, {14, 16}, {16, 18}, {16, 20}, {16, 22}, {18, 20},
        {11, 23}, {12, 24}, {23, 25}, {24, 26}, {25, 27}, {26, 28}, {27, 29},
        {28, 30}, {29, 31}, {30, 32}, {23, 24}, {27, 31}, {28, 32}
    };

    //Texture2D��PNG�`���ɕϊ�
    public void SendImageToPython()
    {
        Texture2D newTexture = new Texture2D(_texture.width, _texture.height, TextureFormat.RGBA32, false);
        newTexture.SetPixels(_texture.GetPixels());
        newTexture.Apply();

        byte[] imageData = newTexture.EncodeToPNG();
        StartCoroutine(SendImageData(imageData));
    }

    //Python�R�[�h��PNG�t�@�C����n���AMediaPipe�����Ă��炤
    private IEnumerator SendImageData(byte[] imageData)
    {
        UnityWebRequest request = new UnityWebRequest("http://localhost:5000/upload", "POST");
        request.uploadHandler = new UploadHandlerRaw(imageData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/octet-stream");

        //���N�G�X�g�̌��ʂ��Ԃ��Ă���܂ő҂�
        yield return request.SendWebRequest();

        //���N�G�X�g������Ɏ��s���ꂽ��
        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonData = request.downloadHandler.text;

            // JSON�f�[�^���t�@�C���ɕۑ�
            WriteJsonToFile(jsonData);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    //JSON�t�@�C����Assets/JSON�t�H���_�ɕۑ�
    private void WriteJsonToFile(string jsonData)
    {
        string jsonFileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".json";

        // �t�@�C���̕ۑ���p�X���w��
        string filePath = Path.Combine(Application.dataPath + "/JSON", jsonFileName);

        string directoryPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // JSON�f�[�^���t�@�C���ɏ�������
        File.WriteAllText(filePath, jsonData);
        Debug.Log("JSON data saved to: " + filePath);

        
        Vector3[] jsonLandmarks = _processer.GetLandmarksFromJson(filePath);

        for(int i = 0; i < jsonLandmarks.Length; i++)
        {
            Instantiate(_spherePrefab, jsonLandmarks[i], Quaternion.identity);
        }

        //AnimationSceneManager.GetInstance().SetNewKeyPoseFilePath(filePath);
    }

}