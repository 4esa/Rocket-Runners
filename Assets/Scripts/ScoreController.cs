using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class ScoreController : MonoBehaviour
{
    public int Score = 0;
    public Text ScoreText;
    public string PlayerName;
    public GameObject PointHover;

    void Start()
    {
        UpdateScore();
    }

	void UpdateScore ()
	{
	    try
	    {
	        ScoreText.text = PlayerName + ": " + Score;
	    }
	    catch (NullReferenceException)
	    {
	        Debug.Log("Score has not been set for player");
	    } 
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Score"))
        {
            Score++;
            PointHover.SetActive(true);
            UpdateScore();
            StartCoroutine(HidePointHover());
        }
	}

    IEnumerator HidePointHover()
    {
        yield return new WaitForSeconds(2.0f);
        PointHover.SetActive(false);
    }
}
