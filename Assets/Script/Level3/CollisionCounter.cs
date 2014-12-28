using UnityEngine;
using System.Collections;

public class CollisionCounter : MonoBehaviour {
	public int counter;
	private ArrayList buttonList;
	public GameObject objectExit;

	// Use this for initialization
	void Start () {
		counter = 0;
		buttonList = new ArrayList ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void addButtonToTheList(GameObject button)
	{
		if(!checkIfButtonExiste(button.name))
		{
			buttonList.Add(button.name);
			counter++;
			if(counter==4)
			{
				objectExit.SetActive(true);
			}
		}
	}

	public bool checkIfButtonExiste(string buttonName)
	{
		for(int i=0;i<buttonList.Count;i++)
		{
			if(buttonList[i] == buttonName)
				return true;
		}
		return false;
	}
}
