using UnityEngine;
using System.Collections;

public class ThrustSFXController : MonoBehaviour {

    private AudioSource _audio;

    private bool _keyDown = false;

    private float _fadeOutTime = 0.25f;

    private bool _fadingOut = false;

    private float _currentFadeOutTime = 0.0f;

    public float MaxVolume;

    void Start()
    {
    	_audio = GetComponent<AudioSource>();
    }

    public void Thrust(float thrustSpeed)
    {
        SetVolume(thrustSpeed);
        if (!_keyDown)
        {
            _fadingOut = false;
            _keyDown = true;
            _audio.volume = MaxVolume;
            _audio.Play();
        }
        
    }

    private void SetVolume(float v)
    {
        if (!_fadingOut)
        {
            _audio.volume = v*MaxVolume;
        }
    }

    public void ReleaseThrust()
    {
        if (_keyDown)
        {
            _keyDown = false;
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        if (!_fadingOut) { //Ignore if already fading out
            _fadingOut = true;
            _currentFadeOutTime = 0;
            float startVolume = _audio.volume;
            float startTime = Time.time;

            while (_audio.volume > 0 && _fadingOut)
            {
                _currentFadeOutTime = Time.time - startTime;
                _currentFadeOutTime /= _fadeOutTime;
                _audio.volume = Mathf.Lerp(startVolume, 0, _currentFadeOutTime);
                yield return 0; //returning 0 will make it wait 1 frame
            }
            if (_audio.volume < 0.01 ) //meaning it completed successfully without being cancelled
            {
                _audio.Pause();                
            }
            _fadingOut = false;
        }
    }
}
