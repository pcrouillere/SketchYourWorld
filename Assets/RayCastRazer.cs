using UnityEngine;
using System.Collections;

public class RayCastRazer : MonoBehaviour {
	public RaycastHit rayHit;
	public Ray ray;
	public GameObject hand;
	public GameObject onGameObject;
	public Color colorOnGameObject;
	public GameObject selectedGameObject;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
				ray = camera.ScreenPointToRay (SixenseInput.Controllers [0].Position);

		// si on a selectionné un objet on ne peut pas en selectionner un autre
				if (!selectedGameObject) {
						if (Physics.Raycast (hand.transform.position, hand.transform.TransformDirection (Vector3.forward), out rayHit, 3F)) {
								// Si c'est un objet qu'on veut selectionner			
								if (rayHit.collider.name == "brick") {
										// si un objet était déjà pré-selectionner on lui rend sa véritable couleur
										if (onGameObject) {
												onGameObject.renderer.material.color = colorOnGameObject;
										}
										// Sinon et dans tous les cas le nouvel objet pré-selectionner devient le notre et on le met en une couleur voyante
										onGameObject = rayHit.collider.gameObject;
										colorOnGameObject = onGameObject.renderer.material.color;
										onGameObject.renderer.material.color = Color.black;
								} else {
										// Si on n'est plus sur un objet on lui rend sa couleur normale, sinon on ne fait rien
										if (onGameObject) {
												onGameObject.renderer.material.color = colorOnGameObject;
												onGameObject = null;
										}
								}
						}
			// Si on ne touche rien on de pre-selectionne tout
					else {
						if (onGameObject) {
							onGameObject.renderer.material.color = colorOnGameObject;
							onGameObject = null;
						}
					}
				}


		//Selectionner un objet
				if (onGameObject) {
						// lorsqu'on pre-selectionne un objet on peut choisir de le selectionner
						if (SixenseInput.Controllers [0].GetButton (SixenseButtons.BUMPER)) {
								Debug.Log ("bumper");	
								selectedGameObject = onGameObject;
						}
			if (SixenseInput.Controllers[0].GetButtonUp(SixenseButtons.BUMPER)) {
				selectedGameObject=null;
			}
				}
		}
								/* if (SixenseInput.Controllers [1].JoystickX > 0.5) {
										onGameObject.transform.Translate (Vector3.right * 0.5F, Camera.main.transform);
								}
								else if(Input.GetKey("l")){
				transform.Translate(Vector3.forward*speed, Camera.main.transform);
			}
			else if(Input.GetKey("k")){
				transform.Translate(Vector3.left*speed, Camera.main.transform);
			}
			else if(Input.GetKey("j")){
				transform.Translate(Vector3.back*speed, Camera.main.transform);
			}
			else if(Input.GetKey("d")) {
				transform.Rotate(Vector3.back);
			}
			else if(Input.GetKey("t")) {
				transform.Rotate(Vector3.forward);
			}
			else if(Input.GetKey("v")) {
				Debug.Log("+");
				transform.localScale += new Vector3(0.1F, 0.1F, 0.1F);
			}
			else if(Input.GetKey("w")) {
				Debug.Log("-");
				if(transform.localScale.x >= 0.3) {
					transform.localScale -= new Vector3(0.1F, 0.1F, 0.1F);
				}
			} */
}
