using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimController : MonoBehaviour
{

    private bool _inAir = true;
    private List<Collider2D> _colliders = new List<Collider2D>();

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.tag.Equals("Floor"))
        {
            _colliders.Add(collider2D);
            _inAir = false;
            GetComponent<Animator>().speed = 1;
        }
    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.tag.Equals("Floor")) //NULLIFY IF TOUCHNG ANOTHER FLOOR
        {
            _colliders.Remove(collider2D);
        }
        if (_colliders.Count < 1) //If touching no other floors, then consider in air (fixes problem when transitioning between two floors. It would become air after it only just set itself to collide in the next mesh)
        {
            _inAir = true;
        }
    }

    public void AtJumpFrame() //Once it hits the jump frame and is in the air, pause the animation - finishes the current half it's on
    {
        if (_inAir)
        {
            GetComponent<Animator>().speed = 0;
        }
    }

}
