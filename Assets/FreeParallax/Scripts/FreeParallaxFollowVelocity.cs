// License: https://en.wikipedia.org/wiki/MIT_License
// Code created by Jeff Johnson & Digital Ruby, LLC - http://www.digitalruby.com
// Code is from the Free Parallax asset on the Unity asset store: http://u3d.as/bvv
// Code may be redistributed in source form, provided all the comments at the top here are kept intact

using UnityEngine;
using System.Collections;

public class FreeParallaxFollowVelocity : MonoBehaviour
{

    public FreeParallax Parallax;
    private float _velocity;
    private Vector3 _previousPosition;

    // Update is called once per frame
    void Update()
    {
        if (Parallax != null)
        {
            _velocity = -((transform.position.x - _previousPosition.x) / Time.deltaTime); //No rigid body for this task to get velocity
            _previousPosition = transform.position;
            Parallax.Speed = _velocity;
        }
    }
}
