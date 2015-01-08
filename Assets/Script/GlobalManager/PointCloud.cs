using UnityEngine;
using System.Collections;

public class PointCloud : MonoBehaviour {
	public GameObject particule;
	public GameObject myCube;
	public GameObject myBridge;
	public GameObject myPipe;
	public GameObject mynewHouse;
	public GameObject myRock;
	public GameObject myGrass;
	public GameObject placeObject;
	public bool hover=false;
	[HideInInspector]public static PointCloud gPointCloud;

	void Awake() {
		gPointCloud = this;
	}

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

		if (gesture.MatchScore>0.05) {
			gameObject.GetComponent<AudioSource>().Play();
			if (gesture.RecognizedTemplate.name=="Cube") {
				Debug.Log( "C'est un cube!" );
				CreateObjectDrawed(myCube);
			}

			if (gesture.RecognizedTemplate.name=="Pont") {
				Debug.Log( "C'est un pont!" );
				CreateObjectDrawed(myBridge);

			}
			if (Application.loadedLevelName=="YourWorld") {
				if (gesture.RecognizedTemplate.name=="Maison") {
					Debug.Log( "C'est une Maison!" );
					CreateObjectDrawed(mynewHouse);
					
				}
				if (gesture.RecognizedTemplate.name=="Pipe") {
					Debug.Log( "C'est un pipe!" );
					CreateObjectDrawed(myPipe);
					
				}
				if (gesture.RecognizedTemplate.name=="Rock") {
					Debug.Log( "C'est un rock!" );
					CreateObjectDrawed(myRock);
					
				}
				if (gesture.RecognizedTemplate.name=="Grass") {
					Debug.Log( "C'est un rock!" );
					CreateObjectDrawed(myGrass);
					
				}
			}

		}
	}
	void OnFingerUp(FingerUpEvent e) {
		hover = false;
	}

	void Update( ) 
	{
			particule.transform.position = GlobalManager.gManager.interfaceCamera.GetComponent<Camera>().ScreenToWorldPoint(
				DrawHand(GameObject.Find("HandR").transform.position));
	}

	public Vector3 DrawHand(Vector3 pos){
			Vector3 posScreen = Camera.main.WorldToScreenPoint (pos);
			posScreen.z = 3;
		return posScreen;
		}

	void CreateObjectDrawed(GameObject objectName) {
		GameObject objectSpawn = (GameObject)Instantiate(objectName, new Vector3(
			placeObject.transform.position.x, placeObject.transform.position.y,
			placeObject.transform.position.z), transform.rotation);
		objectSpawn.SetActive(true);
	}
}
