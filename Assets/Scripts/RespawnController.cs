using UnityEngine;
using System.Collections;

public class RespawnController : MonoBehaviour
{

    public Camera Cam;
    public float BaseDelay = 1.0f;
    public float CheckInterval = 0.5f;
    public float DelayOutOfScreen = 2.0f;

    void Start()
    {
        if (Cam == null)
        {
            Cam = Camera.main;
        }
        StartCoroutine(CheckRespawn());
    }

    IEnumerator CheckRespawn() //Not a thread. Thread safe not an issue
    {
        while (true)
        {
            GameObject fastestPlayer = Cam.GetComponent<FollowOnXAxis>().FastestPlayer;
            if (fastestPlayer != null)
            {
                Vector3 posOnScreen = Cam.WorldToViewportPoint(gameObject.transform.position);
                if (posOnScreen.x < 0.0)
                { //TODO: Also include a boundary check with player size and check collision with other 
                    Cam.GetComponent<FollowOnXAxis>().PlayersToFollow.Remove(gameObject);
                    yield return new WaitForSeconds(DelayOutOfScreen);
                    Vector3 newPosition = fastestPlayer.transform.position;
                    Vector2 newVelocity = fastestPlayer.GetComponent<Rigidbody2D>().velocity;
                    float delay = BaseDelay / newVelocity.x;
                    yield return new WaitForSeconds(delay);
                    gameObject.transform.position = newPosition;
                    gameObject.GetComponent<Rigidbody2D>().velocity = newVelocity;
                    Cam.GetComponent<FollowOnXAxis>().PlayersToFollow.Add(gameObject);
                    Cam.GetComponent<FollowOnXAxis>().SetSmooth();
                }
            }
            yield return new WaitForSeconds(CheckInterval);
        }
    }
}
