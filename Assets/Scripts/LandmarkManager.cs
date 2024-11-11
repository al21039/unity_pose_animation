using System;
using System.Collections.Generic;
using UnityEngine;

public class LandmarkManager : MonoBehaviour
{
    [SerializeField] private GameObject _jsonModel;
    [SerializeField] private TextAsset[] _jsonFilePaths;

    private int _jsonCount = 0;

    private Dictionary<int, Vector3[]> _csvLandmarkPositions = new Dictionary<int, Vector3[]>();
    private Dictionary<int, Vector3[]> _jsonLandmarkPositions = new Dictionary<int, Vector3[]>();
    private List<int> _keyPoseList = new List<int>();
    private int _totalFrame = 0;

    private static LandmarkManager instance;

    private string fileName;

    private string _startDate;

    public Dictionary<int, Vector3[]> CSVLandmarkPositions
    {
        get
        {
            return _csvLandmarkPositions;
        }
        set
        {
            _csvLandmarkPositions = value;
            SetJsonLandmark();
        }
    }

    public Dictionary<int, Vector3[]> JSONLandmarkPositions
    {
        get
        {
            return _csvLandmarkPositions;
        }
    }

    public int TotalFrame
    {
        get 
        { 
            return _totalFrame;
        }
        set
        {
            _totalFrame = value;
        }
    }

    public string StartDate
    {
        get 
        {
            return _startDate;
        }
        set
        {
            _startDate = value;
        }
    }

    public int JsonCount
    {
        get { 
            return _jsonCount;
        }
        set { 
            
        }
    }

    public List<int> KeyPoseList
    {
        get
        {
            return _keyPoseList;
        }
        set
        {
            _keyPoseList = value;
        }
    }

    public string FileName
    {
        get
        {
            return fileName;
        }
        set
        {
            fileName = value;
        }
    }

    public void SetJsonLandmarkPosition(Vector3[] jsonLandmark)
    {
        _jsonLandmarkPositions.Add(_jsonCount, jsonLandmark);

        _jsonCount++;
        if(_jsonCount >= _jsonFilePaths.Length)
        {
            EditManager.GetInstance().PrepareEditing();
        }
        else
        {
            SetJsonLandmark();
        }
    }


    public static LandmarkManager GetInstance() => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        _startDate = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        Application.targetFrameRate = 30;
    }


    private void SetJsonLandmark()
    {
        GameObject imageModel = Instantiate(_jsonModel, Vector3.zero, Quaternion.identity);
        CreateFromJSON createFromJSON = imageModel.GetComponent<CreateFromJSON>();
        createFromJSON.SetJsonLandmark(_jsonFilePaths[_jsonCount]);
    }
}
