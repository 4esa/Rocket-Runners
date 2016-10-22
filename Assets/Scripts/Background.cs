using UnityEngine;

//This script controls the scrolling of the background
public class Background : MonoBehaviour
{
	public float speed = 0.1f;			//Speed of the scrolling

    public int materialIndex = 0;
    public Vector2 uvAnimationRate = new Vector2(1.0f, 0.0f);
    public string textureName = "_MainTex";
    Vector2 uvOffset = Vector2.zero;

	void Update ()
	{
        uvOffset += (uvAnimationRate * Time.deltaTime);
		//Keep looping between 0 and 1
		float y = Mathf.Repeat (Time.time * speed, 1);
		//Create the offset
		Vector2 offset = new Vector2 (0, y);
        Debug.Log(offset);
		//Apply the offset to the material
		//GetComponent<Renderer>().sharedMaterial.SetTextureOffset ("_MainTex", offset);
        GetComponent<Renderer>().materials[materialIndex].SetTextureOffset(textureName, uvOffset);
	}
}