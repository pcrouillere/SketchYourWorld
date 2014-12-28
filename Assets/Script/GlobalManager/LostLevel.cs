using UnityEngine;
using System.Collections;

public class LostLevel : MonoBehaviour {
	public string thisLevel;
	public AudioClip endLevel;
	public AudioClip loseLevel;

	void Start () {
	
	}
	
	void Update () {
		/* Use to check if the first player is falling or flying */
		if (gameObject.transform.position.y < -50 || gameObject.transform.position.y > 120 ) {
			if(!gameObject.GetComponent<AudioSource>().isPlaying){
				gameObject.GetComponent<AudioSource>().audio.clip=loseLevel;	
			gameObject.GetComponent<AudioSource>().Play ();	
			}
		}
		if (gameObject.transform.position.y < -80 || gameObject.transform.position.y > 140) {
			Application.LoadLevel(thisLevel);	
		}
	}

	void OnControllerColliderHit(ControllerColliderHit collision){
		if (collision.gameObject.tag == "Exit") {
			if(!gameObject.GetComponent<AudioSource>().isPlaying) {
				StartCoroutine(playEndLevel(collision));
			}
			
		}
		if (collision.gameObject.tag == "Die") {
			if(!gameObject.GetComponent<AudioSource>().isPlaying) {
				StartCoroutine(playLoseLevel());
			}

		}
		if (collision.gameObject.tag == "GoToPuzzle") {
			Application.LoadLevel("Level6");
				}
		if (collision.gameObject.name == "K") {
			StartCoroutine(showInterface(collision));

		}
}

	IEnumerator showInterface(ControllerColliderHit collision) {
		GameObject interfaceToShow;
		interfaceToShow = collision.gameObject.GetComponent<PanneauScript>().interfaceName;
		interfaceToShow.SetActive(true);
		yield return new WaitForSeconds(5);
		interfaceToShow.SetActive(false);
	}


	IEnumerator playEndLevel(ControllerColliderHit collision) {
		gameObject.GetComponent<AudioSource>().audio.clip=endLevel;	
		gameObject.GetComponent<AudioSource>().Play ();
		while (gameObject.GetComponent<AudioSource>().isPlaying) {
			yield return null;		
		}
		Application.LoadLevel(collision.gameObject.GetComponent<ExitLevel>().levelToLoad);
	}

	IEnumerator playLoseLevel() {
		gameObject.GetComponent<AudioSource>().audio.clip=loseLevel;	
		gameObject.GetComponent<AudioSource>().Play ();
		while (gameObject.GetComponent<AudioSource>().isPlaying) {
			yield return null;		
		}
		Application.LoadLevel(thisLevel);
	}

}