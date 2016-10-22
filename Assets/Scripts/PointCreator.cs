using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions.Comparers;
using Random = UnityEngine.Random;

public class PointCreator : MonoBehaviour
{

    public Camera Cam;
    public GameObject Point;

    public float DistanceToKeepFromCamera = 20; //How far to instantiate ahead, does not affect frequency
    public float BaseJumpAheadDistance = 6;
    public float RandomJumpAheadDistance = 4;
    public float PaddingToTerrain = 1;
    public float ChanceToSpawn = 0.5f;


    /*
     Outline:
     * If gameObj is within x distance to camera, jump ahead a fixed distance + a random value
     *      On Jump, get a range of the y value of the top piece + padding and the y value of the bottom piece + padding
     *          Inst a new point within that range
     */

    // Use this for initialization
    void Start()
    {
        if (Cam == null)
        {
            Cam = Camera.main;
        }
        gameObject.transform.position = new Vector3(Cam.transform.position.x, Cam.transform.position.y, 0);
    }

    void JumpX()
    {
        var distance = gameObject.transform.position.x + BaseJumpAheadDistance + Random.Range(0, RandomJumpAheadDistance);
        gameObject.transform.position = new Vector3(distance, gameObject.transform.position.y, gameObject.transform.position.z);
    }

    float GetY()
    {
        //More meshes means slower distance calculation but better placement: On planes containing hills

        //Can't simpy drag in object of plane because it won't know which plane is where. Plane is instantiated on the fly
        //Need to look at specific tag too so it can differentiate what is floor and what is ceiling

        //Could be more detailed and go to nearest vertex, but might not be necessary. Could use raycast. It would use more processing power
        GameObject ceiling = FindClosestOnXWithTag("Ceiling", gameObject.transform.position);
        GameObject floor = FindClosestOnXWithTag("Floor", gameObject.transform.position);

        Vector3 ceilingVertexPosition = FindClosestVertex(ceiling, gameObject.transform.position);
        Vector3 floorVertexPosition = FindClosestVertex(floor, gameObject.transform.position);

        //TODO It currently is getting the wrong game object when search for the position because it is only looking at the left hand x value

        float newY;

        //Check if there is enough padding for placement
        if (floorVertexPosition.y + PaddingToTerrain > ceilingVertexPosition.y - PaddingToTerrain)
        //If floor placing is greater than ceiling placement jump again
        {
            Debug.Log("Failed to place. Lower: " + (floorVertexPosition.y + PaddingToTerrain) + ". Upper: " +
                      (ceilingVertexPosition.y - PaddingToTerrain));
            newY = Single.NaN; //Return failure
        }
        else
        {
            //Snap to middle between two points for better placement
            gameObject.transform.position = 
                new Vector3( (ceilingVertexPosition.x + floorVertexPosition.x)/2.0f,
                    gameObject.transform.position.y,
                    gameObject.transform.position.z);
            newY = Random.Range(floorVertexPosition.y + PaddingToTerrain, ceilingVertexPosition.y - PaddingToTerrain);
        }
        Debug.DrawLine(ceilingVertexPosition, gameObject.transform.position, Color.green, 10, false);
        Debug.DrawLine(floorVertexPosition, gameObject.transform.position, Color.green, 10, false);
        return newY;
    }

    Vector3 FindClosestVertex(GameObject go, Vector3 anchor) //TODO: Not really working currently
    {
        Mesh mesh = go.GetComponent<MeshFilter>().mesh;
        Vector3 closestVertex = new Vector3();
        float closestSqrDistance = Mathf.Infinity;

        foreach (Vector3 vertex in mesh.vertices)
        {
            Vector3 diff = anchor - (vertex + go.transform.position);
            if (diff.sqrMagnitude < closestSqrDistance)
            {
                closestSqrDistance = diff.sqrMagnitude; //Whole distance needs to be checked because of the base vertex
                closestVertex = vertex;
            }
        }
        //Update the x value with the game objects x on return. It worked previously since it only used the y value, but now it uses the x too
        return new Vector3(
            closestVertex.x + go.transform.position.x,
            closestVertex.y,
            closestVertex.z);
    }


    GameObject FindClosestOnXWithTag(String gameObjectTag, Vector3 anchor) //Faster because only compares x value. THIS ONLY WORKS FOR SEGMENTS
        //This only works for left origin objects as it corrects the origin to middle x. Previously it would compare with left anchor of object
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag(gameObjectTag);
        float closestDistance = Mathf.Infinity;
        GameObject closestGameObject = null;
        foreach (GameObject go in gos)
        {
            float diffX = Math.Abs(anchor.x - (go.transform.position.x + (go.GetComponent<MeshFilter>().mesh.bounds.size.x)/2.0f));
            if (diffX < closestDistance)
            {
                closestDistance = diffX;
                closestGameObject = go;
            }
        }
        return closestGameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(gameObject.transform.position.x - Cam.transform.position.x) < DistanceToKeepFromCamera)
        {
            JumpX();
            //Removed the jump and try again continually mechanic because it would jump too far where it wasn't generated and always fail
            if (Random.value < ChanceToSpawn)
            {
                float newY = GetY();
                if (!float.IsNaN(newY))
                {
                    Vector3 pointPosition = new Vector3(gameObject.transform.position.x, newY,
                        gameObject.transform.position.z);
                    Quaternion quaternion = new Quaternion();
                    Instantiate(Point, pointPosition, quaternion);
                }
            }
        }
    }
}
