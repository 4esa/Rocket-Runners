using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ExitOnButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("ExitButton"))
        {//When a key is pressed down it see if it was the escape key if it was it will execute the code
            SceneManager.LoadScene("menu");
        }
	}
}
