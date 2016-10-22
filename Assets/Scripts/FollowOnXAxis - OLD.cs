using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Debug = UnityEngine.Debug;

public class FollowOnXAxisOLD : MonoBehaviour
{
    public float XAdjust = 0;
    GameObject[] _gameObjectsToFollow;
    public GameObject FastestPlayer = null;
    private float _offsetScreen = 0.15f;
    public string Tag = "Player";
    private float _velocity; //Not sure why this works when it hasn't been inititialised to anything
    private int _stepMax = 720;

    private float _lastTargetX;
    private float _newTargetX;

    private float _distanceFromTarget;



    public bool SmoothMode = false;

    private int _step;



    void Start()
    {
        _gameObjectsToFollow = GameObject.FindGameObjectsWithTag(Tag);
        _step = 1;
    }

    float GetCameraOffset()
    {
        float leftX = gameObject.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        return gameObject.GetComponent<Camera>().ViewportToWorldPoint(new Vector3((_offsetScreen), 0, 0)).x - leftX;
    }

    void FixedUpdate() //LateUpdate and Update were the reason it was flickering so much. Changing to FixedUpdate fixed so much. Makes delta targetX (flat ground) the same
    {
        {
            /*
             * It was so jumpy or too slow, always lagging behind for always having it. Now used for activated smoothmode
             * 
             */

            if (SmoothMode) //Should go until target is within bounds before swapping back to not smooth
            {

                //Debug.Log("dTargetX: " + (GetCameraTargetX() - _newTargetX) + " St: " + _step);

                _newTargetX = GetCameraTargetX();// + GetCameraOffset(); 
                float time = _step / (float) _stepMax;
                if (_step < _stepMax)
                {

                    //By using current position instead of previous position it can speed up or slow down depending on how much left it has to go I think

                    //Vector3 updatedTarget = new Vector3(Mathf.SmoothStep(gameObject.transform.position.x, _newTargetX, time), gameObject.transform.position.y, gameObject.transform.position.z);
                    //Vector3 newPosition = new Vector3(Mathf.Lerp(gameObject.transform.position.x, _newTargetX, time),
                    //gameObject.transform.position.y, gameObject.transform.position.z); //It keeps setting itself forward and back in this
                    //Vector3 updatedTarget = new Vector3(_lastTargetX + (intervalDistance*step), gameObject.transform.position.y, gameObject.transform.position.z);
                    //Vector3 newTarget = new Vector3(Mathf.SmoothDamp(_lastTargetX, _newTargetX, ref _velocity, SmoothTime), gameObject.transform.position.y, gameObject.transform.position.z);


                    //Update a quarter each step
                    
                    float distance = _newTargetX - gameObject.transform.position.x;
                    float newX = gameObject.transform.position.x + (distance / 8f);
                    Vector3 newPosition = new Vector3(newX, gameObject.transform.position.y, gameObject.transform.position.z);
                    

                    gameObject.transform.position = newPosition;
                    _step++;
                }

                _distanceFromTarget = Mathf.Abs(GetComponent<Camera>().WorldToViewportPoint(new Vector2(_newTargetX, 0.0f)).x - 0.5f);

                if (_step >= _stepMax || _distanceFromTarget < 0.01f )
                {
                    _step = 1;
                    SmoothMode = false;
                }
               
                Debug.DrawLine(new Vector3(_newTargetX, 0, 0), new Vector3(_newTargetX, 1, 0), Color.red);
            }
            else
            {
                _newTargetX = GetCameraTargetX();
                transform.position = new Vector3(_newTargetX, gameObject.transform.position.y, gameObject.transform.position.z);                
            }



        }
    }


    private float GetCameraTargetX()
    {

        float averageX = 0;
        FastestPlayer = _gameObjectsToFollow[0];
        averageX += _gameObjectsToFollow[0].transform.position.x;
        for (int i = 1; i < _gameObjectsToFollow.Length; i++)
        {
            averageX += _gameObjectsToFollow[i].transform.position.x;
            if (_gameObjectsToFollow[i].transform.position.x > FastestPlayer.transform.position.x)
            {
                FastestPlayer = _gameObjectsToFollow[i];
            }
        }
        averageX = averageX / _gameObjectsToFollow.Length;

        //Put camera between average of all players, but if the fastest player is over % limit, look at them
        //How does a player get out of that then?


        //float maxX = middleX + offsetDistance;

        //float swapToAverageX = middleX - offsetDistance;

        Debug.DrawLine(new Vector3(averageX, 3, 0), new Vector3(averageX, -3, 0), Color.yellow);

        //Debug.DrawLine(new Vector3(maxX, 3, 0), new Vector3(maxX, -3, 0), Color.magenta);

        float targetX;

        //Average is always greater than middleX so it always defaults to target if I have the condition middleX
        //It probably needs to work with states rather than a frame by frame basis to be smoother

        /*
        if (FastestPlayer.transform.position.x >= maxX && averageX <= swapToAverageX) //If the player is in front of the percentage mark
	    {
            targetX = FastestPlayer.transform.position.x - offsetDistance;
	    }
	    else
	    {
            targetX = averageX + offsetDistance;
	    }

         */


        targetX = averageX;// + GetCameraOffset(); //Seems to be find if not set directly to average, but back a bit, then characters can die still

        //Debug.DrawLine(new Vector3(swapToAverageX, 1, 0), new Vector3(swapToAverageX, -1, 0), Color.gray);

        Debug.DrawLine(new Vector3(targetX, 2, 0), new Vector3(targetX, -2, 0), Color.white);


        /*

	    float newX;
        if (FastestPlayer.transform.position.x > maxX + 0.5) //If the player is in front of the percentage mark
	    {
            //Set the camera so the fastest player is in the middle
            newX = FastestPlayer.transform.position.x;

            //Get the offset of how far behind the camera should be to keep player at maxX
            float offset = rightX - maxX + 1;
            newX = newX + offset;
            Debug.Log("Follow");
	    }
	    else 
	    {


            newX = averageX; //Set camera to the average of all
            Debug.Log("Average");
  
	    }
        */
        //The fastest player should only ever be able to get to (real world of percentLimit)
        //Unless that player is greater than that limit, it should be following the average, but maybe that means it will jump

        //Vector3 targetPosition = new Vector3(targetX, gameObject.transform.position.y, gameObject.transform.position.z);

        return targetX;

        //if a player dies then focus on the first player and move to them slowly
    }
}
