using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class ScrollViewButton : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;         // �{�^���̃v���n�u
    [SerializeField] private Transform contentParent;         // Scroll View��Content�i�{�^����z�u����e�j
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
        _text.text = min + "�`" + max;
        _inputField.onEndEdit.AddListener(ValidateInput);
    }

    public void DeleteAddFrameIndex()
    {
        _addFrame = -1;
        _inputField.text = "";
    }

    private void ValidateInput(string input)
    {
        // ���͂����l���ǂ������m�F
        if (int.TryParse(input, out int value))
        {
            Debug.Log(value);
            _inputField.text = "";
            // �͈͓��̒l�ɐ���
            value = Mathf.Clamp(value, _minValue, _maxValue);

            // 5�̔{���ɒ���
            int closestMultipleOfFive = Mathf.RoundToInt(value / 5.0f) * 5;

            // ������̒l��ݒ�
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
            // ���l�łȂ��ꍇ�͒l���N���A
            _inputField.text = "";
        }
    }

    public void LoadImagesFromFolder()
    {

        string absoluteFolderPath = Path.Combine(Application.dataPath, LandmarkManager.GetInstance().FileName);

        if (!Directory.Exists(absoluteFolderPath))
        {
            Debug.LogError("�w�肳�ꂽ�t�H���_�����݂��܂���: " + absoluteFolderPath);
            return;
        }

        string[] imagePaths = Directory.GetFiles(absoluteFolderPath, "*.png"); // �摜�t�@�C���̊g���q��png�̏ꍇ
        loadedImages = new Texture2D[imagePaths.Length];

        for (int i = 0; i < imagePaths.Length; i++)
        {
            string assetPath = imagePaths[i].Replace("\\", "/").Replace(Application.dataPath, "Assets");
            byte[] fileData = File.ReadAllBytes(assetPath);
            Texture2D texture = new Texture2D(2, 2);  // �ꎞ�I�ȃT�C�Y���w��
            texture.LoadImage(fileData);
            loadedImages[i] = texture;

            if (loadedImages[i] == null)
            {
                Debug.LogError("�摜�̓ǂݍ��݂Ɏ��s���܂���: " + assetPath);
            }
        }
        InstantiateButtons();
    }

    void InstantiateButtons()
    {
        int count = 0;
        foreach (Texture2D image in loadedImages)
        {
            // �{�^���v���n�u���C���X�^���X��
            GameObject buttonInstance = Instantiate(buttonPrefab, contentParent);

            // �{�^���R���|�[�l���g�̎擾
            Button button = buttonInstance.GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError("�{�^���v���n�u�� Button �R���|�[�l���g������܂���B");
                Destroy(buttonInstance);
                continue;
            }

            // Image �R���|�[�l���g�̎擾�i�{�^�������̎q�I�u�W�F�N�g�ɃA�^�b�`����Ă���ꍇ�j
            Image buttonImage = buttonInstance.GetComponentInChildren<Image>();
            if (buttonImage == null)
            {
                Debug.LogError("�{�^���v���n�u�� Image �R���|�[�l���g��������܂���B");
                Destroy(buttonInstance);
                continue;
            }

            // Image �R���|�[�l���g�� Texture2D ���� Sprite ��ݒ�
            buttonImage.sprite = Sprite.Create(
                image,
                new Rect(0, 0, image.width, image.height),
                new Vector2(0.5f, 0.5f)
            );

            // ���O��ݒ�
            buttonInstance.name = "Button_" + count;

            // ���[�J���ϐ��ŃL���v�`�������
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
