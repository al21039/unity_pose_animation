using UnityEngine;

public class ShowAnimation : MonoBehaviour
{
    [SerializeField] GameObject humanoid;
    private Animator animator;
    GameObject go;

    public bool show;
    public bool check;


    // Start is called before the first frame update
    void Start()
    {
        show = false;
        check = false;
        Application.targetFrameRate = 30;
        animator = humanoid.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (show)
        {
            go = Instantiate(humanoid, new Vector3(0, 0, 0), Quaternion.identity);
            show = false;
        }

        if (go)
        {
            if (check)
            {
                if (landmarkData.ContainsKey(currentFrame))
                {
                    Vector3[] landmarks = landmarkData[currentFrame];
                    ApplyLandmarksToBones(landmarks, go);
                    go.transform.Translate(0f, 0f, 0.07f);
                }

                currentFrame++;
                if (currentFrame >= totalFlames)
                {
                    check = false;
                    Destroy(go);
                }
            }
        }
        */

    }
}
