using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

//Code built upon code from tutorial http://blog.theknightsofunity.com/procedurally-generated-terrain-2d-infinite-runner-unity-game-tutorial/

/*
 Segments should be moved sooner by adding a buffer
 The getHeight() function should yield after each generation. 
 It should be running when it goes out of view, not before
 */

public class MeshGenerator : MonoBehaviour
{
    public Material Material;

    // the length of segment (world space)
    public float SegmentLength = 10;

    // the segment resolution (number of horizontal points)
    public int SegmentResolution = 4;

    // the size of meshes in the pool
    public int MeshCount = 4;

    // the maximum number of visible meshes. Should be lower or equal than MeshCount
    public int VisibleMeshes = 4;

    public bool FacesUp = true;
    private int _direction = 1;

    // the prefab including MeshFilter and MeshRenderer
    public MeshFilter SegmentPrefab;

    // helper array to generate new segment without further allocations
    private Vector3[] _vertexArray;

    // helper array to generate new segment without further allocations
    private float[] _newVertexArray;
    private bool _vertexArrayFull = false;
    private float _segmentPositionStep = 0;
    private int _vertexArrayIndex = 0;
    public string ChildTag; //Ceiling or Floor

    // helper array to generate new edge collider path without further allocations
    private Vector2[] _edgeColliderVertexArray;

    // the pool of free mesh filters
    private List<MeshFilter> _freeMeshFilters = new List<MeshFilter>();

    // the list of used segments
    private List<Segment> _usedSegments = new List<Segment>();

    /*
    public float Curve1Rate = 1f;
    public float Curve1Amplitude = 1f;

    public float Curve2Rate = 1.75f;
    public float Curve2Amplitude = 1f;

     */

    public float NoiseFactor = 0.4f;

    public float NoisePower = 4.0f;

    public float BaseValue = 0.01f;

    public float ChangeDistance = 0.3f;

    public float ChangeOfChange = 0.05f;

    private float _changeStep = 0;

    //public float StrenthOfChangeOfChange = 1.0f;

    void Awake()
    {
        // Create vertex array helper
        _vertexArray = new Vector3[SegmentResolution * 2];

        // Build triangles array. For all meshes this array always will
        // look the same, so I am generating it once 
        int iterations = _vertexArray.Length / 2 - 1;
        var triangles = new int[(_vertexArray.Length - 2) * 3];

        _edgeColliderVertexArray = new Vector2[SegmentResolution];

        for (int i = 0; i < iterations; ++i)
        {
            int i2 = i * 6;
            int i3 = i * 2;

            triangles[i2] = i3 + 2;
            triangles[i2 + 1] = i3 + 1;
            triangles[i2 + 2] = i3 + 0;

            triangles[i2 + 3] = i3 + 2;
            triangles[i2 + 4] = i3 + 3;
            triangles[i2 + 5] = i3 + 1;
        }

        // Create game objects (with MeshFilter) instances.
        // Assign vertices, triangles, deactivate and add to the pool.
        for (int i = 0; i < MeshCount; ++i)
        {
            MeshFilter filter = Instantiate(SegmentPrefab);

            Mesh mesh = filter.mesh;

            if (ChildTag.Length > 0)
            {
                filter.gameObject.tag = ChildTag;                
            }

            mesh.Clear();

            filter.gameObject.GetComponent<LineRenderer>().SetVertexCount(SegmentResolution);

            //Set child outline


            mesh.vertices = _vertexArray;
            mesh.triangles = triangles;

            // create a material holding the texture

            //MESH material set texture to parent
            // apply material to mesh
            //meshRenderer.sharedMaterial = Material;
            //meshRenderer.material = Material;
            //mesh.uv = uv;
            mesh.RecalculateNormals();


            filter.gameObject.SetActive(false);
            _freeMeshFilters.Add(filter);
        }
    }

    void Start()
    {
        if (!FacesUp)
        {
            _direction = -1;
        }
        if (_segmentPositionStep == 0)
        {
            _segmentPositionStep = Random.Range(1, 32768); //32768 different starting positions
        }
        _newVertexArray = new float[SegmentResolution];
        StartCoroutine(GetHeightLoop());
    }

    private void ForceFillNewVertexArray()
    {
        //Only async issue might be that some points are regenerated when they don't have to be
        //It won't generate out of index bounds it isn't running for a fixed number of times, just under segment resolution
        for (int i = _vertexArrayIndex; i < SegmentResolution; i++)
        {
            addToNewVertexArray();
        }
    }

    // Modify this fuction to get different terrain configuration.
    private float GetHeight()
    {

        //To increase definition, add really small amounts to segmentPositionStep and then take advantage of the NoiseFactor

        float noise = Mathf.PerlinNoise(_segmentPositionStep, 0); //Whole numbers return the same result

        //Add ChangeDistance to segmentPositionStepEachFrame
        //But make it so change follows a trig curve of intensity so that you have easy sections and hard sections

        //Segment position step: How far to progress through the noise
        //_changeStep how far to progress through the changing of the size of the segment position changes

        _segmentPositionStep += ChangeDistance * (float) Math.Abs(Math.Cos(_changeStep)); //Include a value to change how hard the hard sections get??
        _changeStep += ChangeOfChange;
        noise +=1 ; //Bump <1s up
        noise = (float) Math.Pow(noise, NoisePower);
        noise *= NoiseFactor;
        return (BaseValue + noise)* _direction; //TODO: could be better. Was trying to emphasise noise changes
    }

    public IEnumerator GetHeightLoop()
    {
        while (true)
        {
            addToNewVertexArray();
            yield return 0; //returning 0 will make it wait 1 frame
        }
    }

    public void addToNewVertexArray()
    {
        if (_vertexArrayFull == false) //Might be a little bit redundant. I guess it doesn't have to do a value comparison
        {
            if (_vertexArrayIndex > SegmentResolution-1)
            {
                _vertexArrayFull = true;
            }
            else
            {
                float height = GetHeight();
                _newVertexArray[_vertexArrayIndex] = height;
                _vertexArrayIndex++;
            }
        }
    }

    void Update()
    {
        // get the index of visible segment by finding the center point world position
        Vector3 worldCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        int currentSegment = (int)(worldCenter.x / SegmentLength);

        // Test visible segments for visibility and hide those if not visible.
        for (int i = 0; i < _usedSegments.Count; )
        {
            int segmentIndex = _usedSegments[i].Index;
            if (!IsSegmentInSight(segmentIndex))
            {
                MakeSegmentNotVisible(segmentIndex);
            }
            else
            {
                // MakeSegmentNotVisible will remove the segment from the list
                // that's why I increase the counter based on that condition
                ++i;
            }
        }

        // Test neighbor segment indexes for visibility and display those if should be visible.
        for (int i = currentSegment - VisibleMeshes / 2; i < currentSegment + VisibleMeshes / 2; ++i)
        {
            if (IsSegmentInSight(i))
            {
                MakeSegmentVisible(i);
            }
        }
    }


    // This function generates a mesh segment.
    // Index is a segment index (starting with 0).
    // Mesh is a mesh that this segment should be written to.
    public void GenerateSegment(int index)
    {
        //float startPosition = index * SegmentLength;
        float step = SegmentLength / (SegmentResolution - 1);
        if (!_vertexArrayFull)
        {
            Debug.Log("ALERT: New Vertex Array not full before being called! Forcefully filling in one update:");
            ForceFillNewVertexArray();
        }
        //_vertexArrayFull will be true from above so will lock off the async func
        float yPosTop = 0;
        for (int i = 0; i < SegmentResolution; ++i)
        {


            // get the relative x position
            float xPos = step*i;

            /*
             * Maybe use a trig 
            if (i != 0 || i != SegmentResolution-1) //If it's the middle vertices they can have a random x displacement
            {
                xPos += Random.Range(-step / 2.0f, step / 2.0f);
            }
             * It just ends up making the whole thing longer or shorter and it doesn't line up
             */
            yPosTop = _newVertexArray[i];

            if (FacesUp)
            {
                // top vertex - Bumpy
                _vertexArray[i * 2] = new Vector3(xPos, yPosTop + gameObject.transform.position.y, gameObject.transform.position.z);

                // bottom vertex - Flat
                _vertexArray[i * 2 + 1] = new Vector3(xPos, gameObject.transform.position.y,
                    gameObject.transform.position.z);
            }
            else
            {
                // bottom vertex - Bumpy
                _vertexArray[i * 2 + 1] = new Vector3(xPos, yPosTop + gameObject.transform.position.y, gameObject.transform.position.z);

                // top vertex - Flat
                _vertexArray[i * 2] = new Vector3(xPos, gameObject.transform.position.y, gameObject.transform.position.z);
            }
            _edgeColliderVertexArray[i] = new Vector2(xPos , yPosTop + gameObject.transform.position.y);
        }

        _newVertexArray[0] = yPosTop; //Set first to last
        _vertexArrayIndex = 1;
        _vertexArrayFull = false; //Will lock off async function until finished
        //Prevents it from setting first value to a NEW value while this is running
        //Or if the other function is up to the second one, and this sets one, and then the second is lost
        //A few other things might be able to happen, but the condition above should freeze it off
        //What I can think of, I think the only issue is it generating more than it has to, or the generate function generating the first one AGAIN before
        //This function increases the index
        //No deadlock issues as the other function will always submit to this one

        //Also on purpose does not increase the step counts
    }

    private bool IsSegmentInSight(int index)
    {
        Vector3 worldLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 worldRight = Camera.main.ViewportToWorldPoint(new Vector3(6, 0, 0));


        //Designed for going left to right here:
        //Add in buffer to change preemptively
        //worldRight.x += 5;
        //worldLeft.x -= 5;

        // check left and right segment side
        float x1 = (index * SegmentLength);
        float x2 = x1 + SegmentLength;

        return x1 <= worldRight.x && x2 >= worldLeft.x;
    }

    //We will need to check if a segment is currently visible, so we don’t use more than one MeshFilters to render the single segment.
    private bool IsSegmentVisible(int index)
    {
        return SegmentCurrentlyVisibleListIndex(index) != -1;
    }

    private int SegmentCurrentlyVisibleListIndex(int index)
    {
        for (int i = 0; i < _usedSegments.Count; ++i)
        {
            if (_usedSegments[i].Index == index)
            {
                return i;
            }
        }

        return -1;
    }

    private struct Segment //We’re using struct instead of class because creating new struct does not generate any garbage.
    {
        public int Index { get; set; }
        public MeshFilter MeshFilter { get; set; }
    }

    private void MakeSegmentVisible(int index)
    {
        if (!IsSegmentVisible(index))
        {

            //TODO: It seems that the first mesh filter is never being taken even if necessary. Don't think I caused this. 

            // get from the pool
            int meshIndex = _freeMeshFilters.Count - 1;
            MeshFilter filter = _freeMeshFilters[meshIndex];
            _freeMeshFilters.RemoveAt(meshIndex);

            // position
            filter.transform.position = new Vector3(index * SegmentLength, 0, 0);

            //Generate - MUST BE AFTER PLACEMENT for line drawing
            RegenerateSegment(filter, index); //TODO: ChangeAmplitude to when leaving 

            // make visible
            filter.gameObject.SetActive(true);

            // register as visible segment
            var segment = new Segment();
            segment.Index = index;
            segment.MeshFilter = filter;

            _usedSegments.Add(segment);
        }
    }

    private void RegenerateSegment(MeshFilter filter, int index)
    {
        // generate
        Mesh mesh = filter.mesh;
        GenerateSegment(index); //Changes vertex array
        //No need to send a ref to the mesh. Can do it from outside since _vertexArray is global 
        mesh.vertices = _vertexArray;

        Vector2[] uvs = new Vector2[_vertexArray.Length];
        int i = 0;
        while (i < uvs.Length)
        {
            uvs[i] = new Vector2(_vertexArray[i].x, _vertexArray[i].y);
            i++;
        }
        mesh.uv = uvs;
        filter.gameObject.GetComponent<EdgeCollider2D>().points = _edgeColliderVertexArray;

        for (i = 0; i < _edgeColliderVertexArray.Length; i++)
        {
            filter.gameObject.GetComponent<LineRenderer>().SetPosition(i, 
                new Vector3(
                    _edgeColliderVertexArray[i].x + filter.transform.position.x,
                    _edgeColliderVertexArray[i].y,
                    gameObject.transform.position.z
                    ));
            //It has to be in a loop anyway so that the x values can be updated with the individual mesh x values
        }
        mesh.RecalculateBounds(); // need to recalculate bounds, because mesh can disappear too early
        mesh.RecalculateNormals();
    }

    private void MakeSegmentNotVisible(int index)
    {
        if (IsSegmentVisible(index))
        {
            int listIndex = SegmentCurrentlyVisibleListIndex(index);
            Segment segment = _usedSegments[listIndex];
            _usedSegments.RemoveAt(listIndex);

            MeshFilter filter = segment.MeshFilter;

            filter.gameObject.SetActive(false);

            _freeMeshFilters.Add(filter);
        }
    }

}
