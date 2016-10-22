using UnityEngine;

public class SpawnAtMouse : MonoBehaviour {

    public GameObject GObject;

    public Camera Cam;

    void Start()
    {
        if (Cam == null)
        {
            Cam = Camera.main;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateObject();
        }
    }

    public void CreateObject(){
        Vector3 rawPosition = Cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPosition = new Vector3(rawPosition.x, rawPosition.y, 0.0f);
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(GObject, targetPosition, spawnRotation);
    }
}
