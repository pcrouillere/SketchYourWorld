using UnityEngine;
using System.Collections;

public class PointCloudTutorial : MonoBehaviour {
	public GameObject pioupiou;
	public bool hover=false;

	void OnFingerDown( FingerDownEvent e ) 
	{
		hover = true;
		Debug.Log( e.Finger + " Down at " + e.Position + " on object:" + e.Selection );
	}
	void OnCustomGesture( PointCloudGesture gesture ) 
	{
		Debug.Log( "Recognized custom gesture: " + gesture.RecognizedTemplate.name + 
		          ", match score: " + gesture.MatchScore + 
		          ", match distance: " + gesture.MatchDistance );
	}
	void OnFingerUp(FingerUpEvent e) {
		hover = false;
	}

	void Update(  ) 
	{
		if (hover) {
			pioupiou.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

			}
	}
}
