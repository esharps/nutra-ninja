using UnityEngine;
using System.Collections;
using Windows.Kinect;

/// <summary>
/// Increments or decrements the player's score based
/// on whether the player has sliced a healthy/unhealthy
/// GameObject
/// @author Emily Sharp
/// </summary>
public class CollisionManager : MonoBehaviour {

	public int goodFoodScoreScheme = 10;
	public int badFoodScoreScheme = -10;
    private Vector2 lastPosition;

    public GameObject splashIcon;

    public GameObject splatIconSushi;
    public GameObject splatIconDarkGreen;
    public GameObject splatIconOrange;
    public GameObject splatIconWhite;
    public GameObject splatIconRed;
    public GameObject splatIconYellow;
    public GameObject splatIconLime;

	// Keep track of number of objects player has slashed
	// Accessible to other classes
	public static int objectsSlashed;

	void Start()
	{
		objectsSlashed = 0;
	}

	// Handles player collision with GameObjects
	// TODO Object is replaced by a "splat" icon and fades out
	void OnTriggerEnter2D(Collider2D col)
	{
        // In DODGE gameplay mode
        if (col.gameObject.tag == "BadFood")
        {
            ScoreManager.currentScore += badFoodScoreScheme;
            Destroy(col.gameObject);

            //TODO Instantiate X symbol
        }
        // In SLASH gameplay mode
        else {

            lastPosition = transform.position;
            objectsSlashed += 1;
            ScoreManager.currentScore += goodFoodScoreScheme;

            if (col.gameObject.tag == "SplatDarkGreen")
            {
                Destroy(col.gameObject);
                GameObject splat = Instantiate(splatIconDarkGreen, lastPosition, Quaternion.identity) as GameObject;
            }
            if (col.gameObject.tag == "SplatLime")
            {
                Destroy(col.gameObject);
                GameObject splat = Instantiate(splatIconLime, lastPosition, Quaternion.identity) as GameObject;
            }
            if(col.gameObject.tag == "SplatOrange")
            {
                Destroy(col.gameObject);
                GameObject splat = Instantiate(splatIconOrange, lastPosition, Quaternion.identity) as GameObject;
            }
            if (col.gameObject.tag == "SplatRed")
            {
                Destroy(col.gameObject);
                GameObject splat = Instantiate(splatIconRed, lastPosition, Quaternion.identity) as GameObject;
            }
            if (col.gameObject.tag == "SplatWhite")
            {
                Destroy(col.gameObject);
                GameObject splat = Instantiate(splatIconWhite, lastPosition, Quaternion.identity) as GameObject;
            }
            if (col.gameObject.tag == "SplatSushi")
            {
                Destroy(col.gameObject);
                GameObject splat = Instantiate(splatIconSushi, lastPosition, Quaternion.identity) as GameObject;
            }
            if (col.gameObject.tag == "SplatYellow")
            {
                Destroy(col.gameObject);
                GameObject splat = Instantiate(splatIconOrange, lastPosition, Quaternion.identity) as GameObject;
            }
        }
	
		
	}

    IEnumerator fadeOut(float s)
    {
        yield return new WaitForSeconds(s);
    }
}
