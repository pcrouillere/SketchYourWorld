using UnityEngine;
using System.Collections;

public class RayCastRazer : MonoBehaviour {
	public RaycastHit rayHit;
	public Ray ray;
	public GameObject hand;
	public GameObject onGameObject;
	public GameObject viewGameObject;
	public GameObject viewingGameObject;
	public GameObject selectedGameObject;
	public Transform selectedGameObjectParent;
	public float trans=0.1F;
	[HideInInspector]public static RayCastRazer gRayCastRazer;

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
				if (!selectedGameObject) {
						if (Physics.Raycast (hand.transform.position, hand.transform.TransformDirection (Vector3.forward), out rayHit, 3F)) {
								// Si c'est un objet qu'on veut selectionner			
						if (rayHit.collider.tag == "Selectable") {
										// Sinon et dans tous les cas le nouvel objet pré-selectionner devient le notre et on le met en une couleur voyante
										onGameObject = rayHit.collider.gameObject;
						} else if(rayHit.collider.tag == "Watch") {
										// Si on n'est plus sur un objet on lui rend sa couleur normale, sinon on ne fait rien
										viewGameObject = rayHit.collider.gameObject;
								}
						}
			// Si on ne touche rien on de pre-selectionne tout
					else {
						if (onGameObject) {
							onGameObject = null;
							
						}
				if(viewGameObject) {
					viewGameObject=null;
				}
					}
				}


		//Selectionner un objet
				if (onGameObject) {
						SelectObject ();
				} else if (viewGameObject) {
					ViewGameObject();		
		}
	}

	void ViewGameObject(){
		GameObject interfaceToShow = null;
		string interfaceName = "";
		if (SixenseInput.Controllers [0].GetButton (SixenseButtons.BUMPER)) {
			viewingGameObject = viewGameObject;
			interfaceToShow = viewingGameObject.GetComponent<PanneauScript>().interfaceName;
			interfaceToShow.SetActive(true);
		}
		if (SixenseInput.Controllers [0].GetButtonUp (SixenseButtons.BUMPER)) {
			interfaceToShow = viewingGameObject.GetComponent<PanneauScript>().interfaceName;
			interfaceToShow.SetActive(false);
			viewingGameObject = null;
				}
		
	}

	void SelectObject() {
		// lorsqu'on pre-selectionne un objet on peut choisir de le selectionner
		if (SixenseInput.Controllers [0].GetButton (SixenseButtons.BUMPER)) {
			selectedGameObject = onGameObject;
			
			if(selectedGameObject.transform.parent) {
				if (selectedGameObject.transform.parent.name != "HandR") {
					selectedGameObjectParent = selectedGameObject.transform.parent;
				}
			}
			selectedGameObject.transform.parent = hand.transform.GetChild(1).transform.parent;
			selectedGameObject.GetComponent<Rigidbody>().useGravity=false;
			selectedGameObject.GetComponent<Rigidbody>().velocity=Vector3.zero;
			
			if(SixenseInput.Controllers[0].GetButton(SixenseButtons.ONE)) {
				Vector3 pos = selectedGameObject.transform.position;
				selectedGameObject.transform.localScale += new Vector3(trans, trans, trans);
				selectedGameObject.transform.position = pos;
				pos.z+=trans*2;
				selectedGameObject.transform.position=pos;
			}
			else if(SixenseInput.Controllers[0].GetButton(SixenseButtons.THREE))
			{
				if(selectedGameObject.transform.localScale.x >= 0.3) {
					Vector3 pos = selectedGameObject.transform.position;
					selectedGameObject.transform.localScale -= new Vector3(trans, trans, trans);
					selectedGameObject.transform.position = pos;
					pos.z-=trans*2;
					selectedGameObject.transform.position=pos;
				}
				
			}
			else if(SixenseInput.Controllers[0].GetButton(SixenseButtons.TWO))
			{
				Quaternion angle = selectedGameObject.transform.rotation;
				angle.x = 0;
				angle.y =0;
				angle.z=0;
				selectedGameObject.transform.rotation = angle;
				
			}
		}
		
		if (SixenseInput.Controllers[0].GetButtonUp(SixenseButtons.BUMPER)) {
			selectedGameObject.transform.parent = selectedGameObjectParent;
			selectedGameObject.GetComponent<Rigidbody>().useGravity=true;
			selectedGameObject=null;
			
		}
	}
	}
