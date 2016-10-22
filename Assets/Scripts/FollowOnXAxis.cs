/*
 There is an older version of this file in the folder. There has been so much deleted and changed in this script
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Debug = UnityEngine.Debug;

public class FollowOnXAxis : MonoBehaviour
{
    public float XAdjust = 0;
    public List<GameObject> PlayersToFollow = new List<GameObject>();
    public GameObject FastestPlayer = null;
    private float _offsetScreen = 0.30f;
    public string Tag = "Player";

    private float _deltaPos = 0;

    private float _newTargetX;

    private float _distanceFromTarget;

    private bool _smoothMode = false;

    public int StepMax = 1200;
    private int _step;


    void Start()
    {
        PlayersToFollow = new List<GameObject>(GameObject.FindGameObjectsWithTag(Tag));
    }

    float GetCameraOffset()
    {
        float leftX = gameObject.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        return gameObject.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(_offsetScreen, 0, 0)).x - leftX;
    }

    public void SetSmooth()
    {
        _step = 0; //Important to restart smooth phase
        _smoothMode = true;
    }

    void FixedUpdate() //LateUpdate and Update were the reason it was flickering so much. Changing to FixedUpdate fixed so much. Makes delta targetX (flat ground) the same
    {
        {
            /*
             * Smoothmode is only activated so that it is not always jumping around but keeps a precise target
             */

            //Always in smooth mod
            _deltaPos = GetCameraTargetX() - _newTargetX;

            _newTargetX = GetCameraTargetX(); 
                //Update a quarter each step

            float distance = _newTargetX - gameObject.transform.position.x;
            float newX = gameObject.transform.position.x + (distance / 16.0f);
            Vector3 newPosition = new Vector3(newX, gameObject.transform.position.y, gameObject.transform.position.z);


            Debug.DrawLine(new Vector3(newPosition.x, 0.5f, 0), new Vector3(newPosition.x, -0.5f, 0), Color.red);

            gameObject.transform.position = newPosition;
            
            

            //EXITS SMOOTHMODE NICELY
            //I think it all came down to me liking how the other progressed at the beginning.
            /*
            if (_smoothMode) //Should go until target is within bounds before swapping back to not smooth
            {

                _newTargetX = GetCameraTargetX(); 
                float time = _step / (float)StepMax;
                if (_step < StepMax)
                {

                    //By using current position instead of previous position it can speed up or slow down depending on how much left it has to go I think
                    Vector3 newPosition = new Vector3(Mathf.Lerp(gameObject.transform.position.x, _newTargetX, time),
                    gameObject.transform.position.y, gameObject.transform.position.z); //It keeps setting itself forward and back in this

                    gameObject.transform.position = newPosition;
                    _step++;
                }

                if (_step >= StepMax)
                {
                    _step = 1;
                    _smoothMode = false;
                }

                Debug.DrawLine(new Vector3(_newTargetX, 0, 0), new Vector3(_newTargetX, 1, 0), Color.red);
            }
            else
            {
                _newTargetX = GetCameraTargetX();
                transform.position = new Vector3(_newTargetX, gameObject.transform.position.y, gameObject.transform.position.z);
            }
            */

        }
    }


    private float GetCameraTargetX()
    {

        //Async warning: do not delete a player half way through this!

        float averageX = 0;
        if (PlayersToFollow.Count == 0) //Somehow no players alive, camera target where it is. Shouldn't be possible anyway
        {
            return gameObject.transform.position.x;
        }
        FastestPlayer = PlayersToFollow[0];
        averageX += PlayersToFollow[0].transform.position.x;
        for (int i = 1; i < PlayersToFollow.Count; i++)
        {
            averageX += PlayersToFollow[i].transform.position.x;
            if (PlayersToFollow[i].transform.position.x > FastestPlayer.transform.position.x)
            {
                FastestPlayer = PlayersToFollow[i];
            }
        }
        averageX = averageX / PlayersToFollow.Count;

        Debug.DrawLine(new Vector3(averageX, 3, 0), new Vector3(averageX, -3, 0), Color.yellow);


        var targetX = averageX + GetCameraOffset();

        Debug.DrawLine(new Vector3(targetX, 2, 0), new Vector3(targetX, -2, 0), Color.white);
        return targetX;
    }
}
