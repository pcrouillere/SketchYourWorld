using UnityEngine;
using System.Collections;

public class DragBridgePiece : MonoBehaviour {


	private Collider2D target;
	private string targetTag;
	public Vector3 position;

	// Use this for initialization
	void Start () {
		targetTag = "piece"+gameObject.name.Substring (6);
		target = GameObject.Find(targetTag).collider2D;
		position = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (RayCastRazer.gRayCastRazer.selectedPuzzle == gameObject) {
			Debug.Log ("selectedpuzzle");
			if(SixenseInput.Controllers[0].GetButtonUp(SixenseButtons.BUMPER)) {
				Debug.Log ("bumperup");
				Debug.Log ("test collision : " +TestCollision(target) );
				if (TestCollision(target)) {
					Debug.Log ("testcollistion true");
					target.renderer.enabled=true; 
					SendMessageUpwards("incrementeDragCount");

					Destroy(gameObject);
				}
				else{
					gameObject.transform.position=position;
				}
				RayCastRazer.gRayCastRazer.selectedPuzzle.transform.parent = RayCastRazer.gRayCastRazer.selectedGameObjectParent;
				RayCastRazer.gRayCastRazer.selectedPuzzle=null;

			}
		}

	}

	bool TestCollision(Collider2D target) {
		return Vector2.Distance(target.transform.position, transform.position) < 1.0;
	}

}
