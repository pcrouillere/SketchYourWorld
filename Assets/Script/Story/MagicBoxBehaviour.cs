using UnityEngine;
using System.Collections;

public class MagicBoxBehaviour : MonoBehaviour {
	
	public GameObject theMan;
	public bool alreadyTouchIt;
	public SpriteRenderer BackgroundStory;
	public Sprite temp;
	public GameObject mainCamera;
	public GameObject blackHole;

	// Use this for initialization
	void Start () {
		alreadyTouchIt=false;
	}
	
	// Update is called once per frame
	void Update () {
		if(theMan.transform.position.y<1.37 && !alreadyTouchIt)
		{
			alreadyTouchIt=true;
			BeginTheStory();
		}
	}

	void BeginTheStory(){
		BackgroundStory.gameObject.SetActive(true);
		blackHole.gameObject.SetActive(true);
	}

}


