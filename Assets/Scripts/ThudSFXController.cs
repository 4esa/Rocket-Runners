using UnityEngine;
using System.Collections;

public class ThudSFXController : MonoBehaviour
{
    private float _deltaV;
    private float _lastV;
    private float _currentV;
    public Rigidbody2D Rigidbody2D;

    void Start()
    {
        _lastV = GetDeltaV(Rigidbody2D.velocity);
        StartCoroutine(CheckForThud());
    }

    float GetDeltaV(Vector3 velocity)
    {
        return velocity.x; //Needs to be a combo of up and down - if it's just magnitude. Heaps of thuds when just dropping to flat ground
        //If it's just x velocity then changing you forwards momentum to upwards momentum creates a thud....

        //I think that I'll just increase the change needed. Going up a hill probably deserves a small thud
    }


    IEnumerator CheckForThud() //Checking each frame can lead to multiple thuds
    {
        while (true)
        {
            _currentV = GetDeltaV(Rigidbody2D.velocity);
            _deltaV = _currentV - _lastV;
            _lastV = _currentV;
            if (_deltaV < - 1)
            {
                if (!GetComponent<AudioSource>().isPlaying)
                {
                    GetComponent<AudioSource>().volume = Mathf.Clamp(Mathf.Abs(_deltaV) / 6.0f, 0.0f, 1.0f); //6 is the definition - max change to alter sound
                    GetComponent<AudioSource>().Play();
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
