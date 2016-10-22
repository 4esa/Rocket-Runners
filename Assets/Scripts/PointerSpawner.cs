using UnityEngine;
using System.Collections;

public class PointerSpawner : MonoBehaviour
{
    public GameObject PointerObject;
    private GameObject _pointerInstance;
	// Use this for initialization
    void Start()
    {
        _pointerInstance = Instantiate(PointerObject);
        _pointerInstance.transform.parent = Camera.main.transform;
        _pointerInstance.transform.localPosition = new Vector3(Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0, 0)).x - Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x, gameObject.transform.position.y, _pointerInstance.transform.position.z);
    }

    void Update()
    {
        float distanceFromCamera = Mathf.Clamp( (gameObject.transform.position - Camera.main.transform.position).sqrMagnitude / 800f, 0.1f, 1);
        if (gameObject.transform.position.x < Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x)
        {
            Destroy(_pointerInstance);
        }
        else
        {
            _pointerInstance.transform.localScale = new Vector3(1 - distanceFromCamera, 1 - distanceFromCamera, 1 - distanceFromCamera);
        }
       
    }
}
