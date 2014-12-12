using UnityEngine;
using System.Collections;

public class RayCastRazerMenu : MonoBehaviour {
	public RaycastHit rayHit;
	public Ray ray;
	public GameObject hand;
	public GameObject onButton;
	public float trans=0.1F;
	[HideInInspector]public static RayCastRazerMenu gRayCastRazer;
	
	// Use this for initialization
	void Awake() {
		gRayCastRazer = this;
	}
	
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		ray = camera.ScreenPointToRay (SixenseInput.Controllers [0].Position);
		// si on a selectionné un objet on ne peut pas en selectionner un autre
			if (Physics.Raycast (hand.transform.position, hand.transform.TransformDirection (Vector3.forward), out rayHit, 3F)) {
						// Si c'est un objet qu'on veut selectionner			
						if (rayHit.collider.tag == "ButtonClick") {
								// Sinon et dans tous les cas le nouvel objet pré-selectionner devient le notre et on le met en une couleur voyante
								onButton = rayHit.collider.gameObject;
						}
				}
			// Si on ne touche rien on de pre-selectionne tout
			else {
				if (onButton) {
					onButton = null;
				}
			}		
		
		//Selectionner un objet
		if (onButton) {
			ClickOnButton();
		} 
	}
	
	void ClickOnButton(){
		string levelToLoad = onButton.GetComponent<ButtonMenuScript> ().levelToLoad;
		if(SixenseInput.Controllers[0].GetButtonDown(SixenseButtons.TRIGGER)) {
			Application.LoadLevel(levelToLoad);
		}
	}
}
