using UnityEngine;
using System.Collections;

public class MagicBoxBehaviour : MonoBehaviour {
	
	public GameObject theMan;
	public bool alreadyTouchIt;
	public SpriteRenderer BackgroundStory;
	public SpriteRenderer KMessage;
	public Sprite temp;
	public GameObject mainCamera;
	public GameObject blackHole;

	// Use this for initialization
	void Start () {
		alreadyTouchIt=false;
		/*theMan = GameObject.Find ("DefaultAvatar");
		temp = GetComponent<SpriteRenderer>().sprite;
		BackgroundStory = gameObject.GetComponent<SpriteRenderer> ();
		mainCamera = GameObject.Find ("Main Camera");
		blackHole = GameObject.Find ("blackhole");*/
	}
	
	// Update is called once per frame
	void Update () {
		if(theMan.transform.position.z>2.00)
		{
			changeCameraView();
		}
		if(theMan.transform.position.y<1.37 && !alreadyTouchIt)
		{
			alreadyTouchIt=true;
			BeginTheStory();
		}
	}

	void BeginTheStory(){
		BackgroundStory.gameObject.SetActive(true);
		blackHole.gameObject.SetActive(true);
		KMessage.gameObject.SetActive (true);
		blackHole.animation.Play ("blackHoleUp");
	}
	
	void changeCameraView()
	{
		/*mainCamera.transform.position.x=2.563259;
		mainCamera.transform.position.y=3.321495;
		mainCamera.transform.position.z=2.321495;*/
	}
}


