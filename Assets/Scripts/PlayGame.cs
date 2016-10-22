using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayGame : MonoBehaviour
{

    // Use this for initialization

    public float CountdownTimeStart = 5.0f;
    private float _contdownTime;

    public AudioSource Source;

    private float _maxAudioVolume;

    public Text CountdownText;


    void Start()
    {
        CountdownText.text = "";
    }

    public void BeginCountdown()
    {
        StartCoroutine(RunCountdown());
    }

    IEnumerator RunCountdown()
    {
        _maxAudioVolume = Source.volume;
        _contdownTime = CountdownTimeStart;
        bool changeScene = false; //Used to allow a one frame delay
        while (true)
        {
            if (!changeScene)
            {
                if (_contdownTime > 0) //1
                {
                    Source.volume = Mathf.Lerp(0, _maxAudioVolume, _contdownTime / CountdownTimeStart);
                    CountdownText.text = _contdownTime.ToString("0.00");
                    _contdownTime -= Time.deltaTime;
                    yield return null;
                }
                else //2
                {
                    CountdownText.text = _contdownTime.ToString("0.00");
                    changeScene = true;
                    yield return null;
                }
            }
            else //3
            {
                CountdownText.text = "0.00";
                SceneManager.LoadScene("level");
                yield return null;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}

