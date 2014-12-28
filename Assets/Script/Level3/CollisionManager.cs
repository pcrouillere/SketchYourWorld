using UnityEngine;
using System.Collections;

public class CollisionManager : MonoBehaviour {

	public ArrayList collisionList;
	public bool isTranslated;
	public bool isCollide;
	public bool isDown;
	public float mass;

	// Use this for initialization
	void Start () {
		collisionList=new ArrayList();
		isTranslated = false;
		isCollide=false;
		isDown = false;
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Selectable" && !isCollide) {
			if(collision.gameObject.transform.position.y> gameObject.transform.position.y)
			{
				if (!findCube (collision.gameObject.name)) {
						isCollide=true;
						collisionList.Add (collision.gameObject.name);
						SendMessageUpwards ("addButtonToTheList", gameObject);
						translationDown ();
						GetComponent<AudioSource>().Play();
				}
			}
		} 
	}

	public bool findCube(string name)
	{
		for(int i=0;i<collisionList.Count;i++)
		{
			if(collisionList[i] == name)
				return true;
		}
		return false;
	}

	public void translationDown()
	{
		if (!isDown) {
			string name = gameObject.name.Substring(6);
			string animationName="AnimationCube"+name;
			gameObject.animation.Play (animationName);
			isDown=true;
		}
	}
}
