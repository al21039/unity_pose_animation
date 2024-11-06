using System.Collections.Generic;
using UnityEngine;

public class LandmarkManager : MonoBehaviour
{
    [SerializeField] private GameObject _jsonModel;
    [SerializeField] private string[] _jsonFilePaths;

    private int _jsonCount = 0;

    private Dictionary<int, Vector3[]> _csvLandmarkPositions;
    private Dictionary<int, Vector3[]> _jsonLandmarkPositions;
    private int _totalFrame;

    private static LandmarkManager instance;

    public Dictionary<int, Vector3[]> CSVLandmarkPositions
    {
        get
        {
            return _csvLandmarkPositions;
        }
        set
        {
            _csvLandmarkPositions = value;
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

    public void IncrementJsonCount()
    {
        _jsonCount++;
        if(_jsonCount >_jsonFilePaths.Length)
        {
            ///
            //　　次の処理へ　　　　キーフレームを表示
            ///
        }
    }


    public static LandmarkManager GetInstance() => instance;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }   

    private void SetJsonLandmark()
    {
        for (int i = 0; i < _jsonFilePaths.Length; i++)
        {
            GameObject imageModel = Instantiate(_jsonModel, Vector3.zero, Quaternion.identity);
            
        }
    }

}
