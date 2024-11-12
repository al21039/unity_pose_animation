using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ScrollViewButton : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;         // ボタンのプレハブ
    [SerializeField] private Transform contentParent;         // Scroll ViewのContent（ボタンを配置する親）
    [SerializeField] private LineInterpolation _lineInterpolation;
    [SerializeField] private int _addFrame = 100;
    private Texture2D[] loadedImages;
    private float _frameInterval = 0.30f;

    public void LoadImagesFromFolder()
    {
        
        string absoluteFolderPath = Path.Combine(Application.dataPath, LandmarkManager.GetInstance().FileName);

        if (!Directory.Exists(absoluteFolderPath))
        {
            Debug.LogError("指定されたフォルダが存在しません: " + absoluteFolderPath);
            return;
        }

        string[] imagePaths = Directory.GetFiles(absoluteFolderPath, "*.png"); // 画像ファイルの拡張子がpngの場合
        loadedImages = new Texture2D[imagePaths.Length];

        for (int i = 0; i < imagePaths.Length; i++)
        {
            string assetPath = imagePaths[i].Replace("\\", "/").Replace(Application.dataPath, "Assets");
            byte[] fileData = File.ReadAllBytes(assetPath);
            Texture2D texture = new Texture2D(2, 2);  // 一時的なサイズを指定
            texture.LoadImage(fileData);
            loadedImages[i] = texture;

            if (loadedImages[i] == null)
            {
                Debug.LogError("画像の読み込みに失敗しました: " + assetPath);
            }
        }
        InstantiateButtons();
    }

    void InstantiateButtons()
    {
        int count = 0;
        foreach (Texture2D image in loadedImages)
        {
            // ボタンプレハブをインスタンス化
            GameObject buttonInstance = Instantiate(buttonPrefab, contentParent);

            // Image コンポーネントを取得し、null チェックを追加
            Image buttonImage = buttonInstance.GetComponent<Image>();
            if (buttonImage == null)
            {
                Debug.LogError("ボタンプレハブに Image コンポーネントがありません。");
                Destroy(buttonInstance);
                continue;
            }

            // Image コンポーネントに Texture2D から Sprite を設定
            buttonImage.sprite = Sprite.Create(
                image,
                new Rect(0, 0, image.width, image.height),
                new Vector2(0.5f, 0.5f)
            );

            // 必要であればボタンに他の設定（例えば名前やイベントリスナー）も追加
            buttonInstance.name = count.ToString();
            buttonInstance.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(count));
            count++;
        }
    }

    void OnButtonClick(int buttonNo)
    {
        Vector3[] JsonLandmark = LandmarkManager.GetInstance().JSONLandmarkPositions(buttonNo);
        Dictionary<int, Vector3[]> changedPos = EditManager.GetInstance().ChangePos;   
        List<int> keyPoseList = LandmarkManager.GetInstance().KeyPoseList;

        if(!keyPoseList.Contains(_addFrame))
        {
            int index = keyPoseList.BinarySearch(_addFrame);
            if (index < 0)
            {
                index = ~index;
            }

            keyPoseList.Insert(index, _addFrame);
            changedPos[_addFrame] = JsonLandmark;
            EditManager.GetInstance().SetJsonPosition(_addFrame, JsonLandmark, index);
            LandmarkManager.GetInstance().KeyPoseList = keyPoseList;
            EditManager.GetInstance().ChangePos = changedPos;


            for (int i = 0; i < 4; i++)
            {
                Spline.GetInstance().SetSpline(i, _addFrame, JsonLandmark[i] + new Vector3(0, 0, _addFrame * _frameInterval));
            }

            _lineInterpolation.InterpolationAllLine();
        } 
    }
}
