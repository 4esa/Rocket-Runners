using UnityEngine;
using System.Collections;

public class PointController : MonoBehaviour
{
    public Camera Cam;
    public Canvas Canvas;
    public ParticleSystem Likes;

	void Start () {
        Likes.playOnAwake = false;
        Likes.Stop();
        Likes.loop = false;
	    if (Cam == null)
	    {
	        Cam = Camera.main;
	    }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        Likes.Play();
        GetComponent<AudioSource>().Play();
    }

    void Update()
    {
        if (gameObject.transform.position.x < Cam.ViewportToWorldPoint(new Vector3(-0.5f, 0, 0)).x)
        {
            Destroy(gameObject);
        }
    }
	
}
