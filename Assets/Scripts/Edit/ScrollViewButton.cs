using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class ScrollViewButton : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;         // ボタンのプレハブ
    [SerializeField] private Transform contentParent;         // Scroll ViewのContent（ボタンを配置する親）
    [SerializeField] private LineInterpolation _lineInterpolation;
    [SerializeField] private InputField _inputField;
    [SerializeField] private Text _text;
    private int _addFrame = -1;
    private Texture2D[] loadedImages;
    private float _frameInterval = 0.30f;
    private int _maxValue;
    private int _minValue;

    public void InitializeValueInputField(int min, int max)
    {
        _maxValue = max;
        _minValue = min;
        Debug.Log(max + " " + min);
        _text.text = min + "～" + max;
        _inputField.onEndEdit.AddListener(ValidateInput);
    }

    public void DeleteAddFrameIndex()
    {
        _addFrame = -1;
        _inputField.text = "";
    }

    private void ValidateInput(string input)
    {
        // 入力が数値かどうかを確認
        if (int.TryParse(input, out int value))
        {
            Debug.Log(value);
            _inputField.text = "";
            // 範囲内の値に制限
            value = Mathf.Clamp(value, _minValue, _maxValue);

            // 5の倍数に調整
            int closestMultipleOfFive = Mathf.RoundToInt(value / 5.0f) * 5;

            // 調整後の値を設定
            if (value >= _minValue && value <= _maxValue)
            {
                _inputField.text = closestMultipleOfFive.ToString();
                _addFrame = closestMultipleOfFive;
            }
            else
            {
                _inputField.text = "";
                _addFrame = -1;
            }
        }
        else if (!string.IsNullOrEmpty(input))
        {
            // 数値でない場合は値をクリア
            _inputField.text = "";
        }
    }

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

            // ボタンコンポーネントの取得
            Button button = buttonInstance.GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError("ボタンプレハブに Button コンポーネントがありません。");
                Destroy(buttonInstance);
                continue;
            }

            // Image コンポーネントの取得（ボタン内部の子オブジェクトにアタッチされている場合）
            Image buttonImage = buttonInstance.GetComponentInChildren<Image>();
            if (buttonImage == null)
            {
                Debug.LogError("ボタンプレハブに Image コンポーネントが見つかりません。");
                Destroy(buttonInstance);
                continue;
            }

            // Image コンポーネントに Texture2D から Sprite を設定
            buttonImage.sprite = Sprite.Create(
                image,
                new Rect(0, 0, image.width, image.height),
                new Vector2(0.5f, 0.5f)
            );

            // 名前を設定
            buttonInstance.name = "Button_" + count;

            // ローカル変数でキャプチャを回避
            int buttonIndex = count;
            button.onClick.AddListener(() => OnButtonClick(buttonIndex));

            count++;
        }
    }

    void OnButtonClick(int buttonNo)
    {
        if (_addFrame == -1)
        {
            return;
        }

        Vector3[] JsonLandmark = LandmarkManager.GetInstance().JSONLandmarkPositions(buttonNo);
        Quaternion[] JsonRotation = LandmarkManager.GetInstance().JSONLandmarkRotations(buttonNo);
        Dictionary<int, Vector3[]> changedPos = EditManager.GetInstance().ChangePos;
        Dictionary<int, Quaternion[]> changedRot = EditManager.GetInstance().ChangeRot;
        List<int> keyPoseList = LandmarkManager.GetInstance().KeyPoseList;

        if (!keyPoseList.Contains(_addFrame))
        {
            int index = keyPoseList.BinarySearch(_addFrame);
            if (index < 0)
            {
                index = ~index;
            }

            keyPoseList.Insert(index, _addFrame);
            changedPos[_addFrame] = JsonLandmark;
            changedRot[_addFrame] = JsonRotation;
            LandmarkManager.GetInstance().KeyPoseList = keyPoseList;
            EditManager.GetInstance().ChangePos = changedPos;
            EditManager.GetInstance().ChangeRot = changedRot;

            EditManager.GetInstance().SetJsonPosition(_addFrame, JsonLandmark, index, JsonRotation);

            for (int i = 0; i < 4; i++)
            {
                Spline.GetInstance().SetSpline(i, _addFrame, JsonLandmark[i] + new Vector3(0, 0, _addFrame * _frameInterval));
            }

            _lineInterpolation.InterpolationJson(index);
        }

        _addFrame = -1;
        _inputField.text = "";
    }
}
