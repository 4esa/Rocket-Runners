using System;
using UnityEngine;
using System.Collections;
using System.Security.Permissions;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.CrossPlatformInput.PlatformSpecific;

[System.Serializable]
public class MovementController : MonoBehaviour
{
    public ThrustSFXController ThrustSfxController;
    public TrailController TrailController;

    public float JumpSpeed = 10;
    public float FallSpeed = 10;
    public float MaxThrustTime = 1;
    public float MaxVerticalSpeed = 4;

    public float MovementAcceleration = 10;
    public float MaxHorizontalIncreaseSpeed = 8;

    public float MaxBootJumpThrustTime = 0.2f;
    public float BoostJumpThrustSpeed = 10;

    private float _thrustTimer = 0;

    private bool _jumping = false;
    private bool _falling = false;

    public string JumpName;
    public string FallName;

    float _maxThrust = 0;

    void Start()
    {
        //ThrustSfxController = gameObject.GetComponentInChildren<ThrustSFXController>();
        //TrailController = gameObject.GetComponentInChildren<TrailController>();

        _maxThrust = JumpSpeed > FallSpeed ? JumpSpeed : FallSpeed;
        _maxThrust = BoostJumpThrustSpeed * 0.75f;
    }




    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Floor"))
        {
            ResetJump();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool jumpDown;
        bool fallDown;

        try
        {
            jumpDown = Input.GetButton(JumpName);
            fallDown = Input.GetButton(FallName);
        }
        catch (ArgumentException)
        {
            jumpDown = false;
            fallDown = false;
        }

        float thrustSpeed = 0;

        if (jumpDown)
        {
            thrustSpeed = Jump();
        }
        else
        {
            _jumping = false;
            if (fallDown)
            {
                thrustSpeed = Fall();
            }
        }




        if (thrustSpeed > 0)
        {
            thrustSpeed = Mathf.Clamp(thrustSpeed / _maxThrust, 0, 1);
            //Making it relative to 1 for volume -- Boost will go over 50% and be clamped
            ThrustSfxController.Thrust(thrustSpeed);
            //TrailController.Thrust();

        }
        else
        {
            ThrustSfxController.ReleaseThrust();
            //TrailController.ReleaseThrust();
        }

        /*
        if (Input.GetKeyUp("space"))
        {
            //_jumpEnabled = false; //Hasn't jumped yet if touching
            print("space key was released");
        }
         * */

        Move();
    }

    void ResetJump()
    {
        _thrustTimer = 0;
        //_jumpEnabled = true;
    }

    float Fall()
    {
        if (_thrustTimer < MaxThrustTime && GetComponent<Rigidbody2D>().velocity.y > -MaxVerticalSpeed)
        {
            ReduceJumpTime();
            GetComponent<Rigidbody2D>().AddForce(Vector3.down * FallSpeed);
            return FallSpeed;
        }
        else
        {
            return 0;
        }
    }

    float Jump()
    {
        if (GetComponent<Rigidbody2D>().velocity.y < MaxVerticalSpeed)
        {
            if (_thrustTimer < MaxThrustTime)
            {
                ReduceJumpTime();
                if (_thrustTimer < MaxBootJumpThrustTime) //Bigger jump from ground position
                {
                    GetComponent<Rigidbody2D>().AddForce(Vector3.up * BoostJumpThrustSpeed);
                    return BoostJumpThrustSpeed;

                }
                else
                {
                    GetComponent<Rigidbody2D>().AddForce(Vector3.up * JumpSpeed);
                    return JumpSpeed;
                }
            }
            else
            {
                return 0;
            }
        }
        else
        {
            Debug.DrawLine(gameObject.transform.position, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.5f, gameObject.transform.position.z), Color.black);
            return 0;
        }
    }

    void ReduceJumpTime()
    {
        _thrustTimer += Time.deltaTime;
    }

    void Move()
    {
        if (GetComponent<Rigidbody2D>().velocity.x < MaxHorizontalIncreaseSpeed) //It won't increase it if it is at that this point, but it can go faster
        {
            GetComponent<Rigidbody2D>().AddForce(transform.right * MovementAcceleration);
        }
        else
        {
            Debug.DrawLine(gameObject.transform.position, new Vector3(gameObject.transform.position.x + 0.5f, gameObject.transform.position.y, gameObject.transform.position.z), Color.black);
        }
    }
}
