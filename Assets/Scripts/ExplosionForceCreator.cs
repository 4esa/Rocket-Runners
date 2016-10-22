using UnityEngine;
using System.Collections;

public class ExplosionForceCreator : MonoBehaviour {

    public float Duration;
    public float Force;
    private GameObject[] _affectedObjects;

	void Start () {
        _affectedObjects = GameObject.FindGameObjectsWithTag("Player");
        Destroy(gameObject, Duration);
	}
	
	void Update () {
	    {
            foreach (GameObject affectedObject in _affectedObjects)
            {
                Vector3 difference = gameObject.transform.position - affectedObject.transform.position;
	            float distMagnitude = difference.sqrMagnitude;
	            Debug.Log("Dist: " + distMagnitude);
	            float maxMagnitude = 50; //Might need to be adjusted
	            //float calculatedForce = Mathf.Lerp(0, Force, maxMagnitude - distMagnitude);
	            //affectedObject.GetComponent<Rigidbody2D>().AddForce(Mathf.Lerp(0, explosionForce, 1 - explosionDistance) * explosionDir, ForceMode2D.Force);
	        }
	    }
	}
}
