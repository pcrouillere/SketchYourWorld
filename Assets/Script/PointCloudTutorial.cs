using UnityEngine;
using System.Collections;

public class PointCloudTutorial : MonoBehaviour {
	public GameObject particule;
	public GameObject myCube;
	public GameObject myBridge;
	public GameObject placeObject;
	public bool hover=false;

	void OnFingerDown( FingerDownEvent e ) 
	{	
		if (GlobalManager.gManager.drawingMenu.activeSelf) {
					hover = true;
					Debug.Log( e.Finger + " Down at " + e.Position + " on object:" + e.Selection );
				}

	}
	void OnCustomGesture( PointCloudGesture gesture ) 
	{
		Debug.Log( "Recognized custom gesture: " + gesture.RecognizedTemplate.name + 
		          ", match score: " + gesture.MatchScore + 
		          ", match distance: " + gesture.MatchDistance );
		if (gesture.MatchScore>0.40) {
			if (gesture.RecognizedTemplate.name=="Cube") {
				Debug.Log( "C'est un cube!" );
				GameObject cubeSpawn = (GameObject)Instantiate(myCube, new Vector3(
					placeObject.transform.position.x, placeObject.transform.position.y,
					placeObject.transform.position.z), transform.rotation);
				cubeSpawn.SetActive(true);
			}
			if (gesture.RecognizedTemplate.name=="Pont") {
				Debug.Log( "C'est un pont!" );
				GameObject bridgeSpawn = (GameObject)Instantiate(myBridge, new Vector3(
					placeObject.transform.position.x, placeObject.transform.position.y+100,
					placeObject.transform.position.z), transform.rotation);
				bridgeSpawn.SetActive(true);
			}

		}
	}
	void OnFingerUp(FingerUpEvent e) {
		hover = false;
	}

	void Update( ) 
	{
		if (hover) {
			particule.transform.position = GlobalManager.gManager.interfaceCamera.GetComponent<Camera>().ScreenToWorldPoint(
				new Vector3(Input.mousePosition.x, Input.mousePosition.y, 3));

			}
	}
}
