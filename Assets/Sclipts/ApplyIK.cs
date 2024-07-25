using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ApplyIK : MonoBehaviour
{
    public string csvFilePath = "Assets/CSV/stretch.csv";
    private Dictionary<int, Vector3[]> landmarkData = new Dictionary<int, Vector3[]>();
    private int currentFrame = 0;
    private Animator animator;
    private int totalFlames = 0;
    [SerializeField] Vector3 forward = new Vector3(0, 1, 0);
    [SerializeField] GameObject RHand;
    [SerializeField] GameObject LHand;
    [SerializeField] GameObject LFoot;
    [SerializeField] GameObject RFoot;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;
        LoadLandmarkData();
    }

    // Update is called once per frame
    void Update()
    {
        if(landmarkData.ContainsKey(currentFrame))
        {
            Vector3[] landmarks = landmarkData[currentFrame];
            ApplyLandmarksToIK(landmarks);
        }


        currentFrame++;
        if (currentFrame >= totalFlames)
            currentFrame = 0;
    }

    void LoadLandmarkData()
    {
        using (var reader = new StreamReader(csvFilePath))
        {
            bool isFirstLine = true;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue; // Skip header line
                }
                var values = line.Split(',');
                int frame = int.Parse(values[0]);
                Vector3[] landmarks = new Vector3[33];

                for (int i = 0; i < 33; i++)
                {
                    float x = float.Parse(values[1 + i * 3]);
                    float y = float.Parse(values[2 + i * 3]);
                    float z = float.Parse(values[3 + i * 3]);
                    landmarks[i] = new Vector3(-x, -y + 1, -z);
                }

                landmarkData[frame] = landmarks;
                totalFlames++;
            }
        }
    }

    void ApplyLandmarksToIK(Vector3[] landmarks)
    {
        
        LHand.transform.position = landmarks[15];
        RHand.transform.position = landmarks[16];
        LFoot.transform.position = landmarks[31];
        RFoot.transform.position = landmarks[32];
    }
}
