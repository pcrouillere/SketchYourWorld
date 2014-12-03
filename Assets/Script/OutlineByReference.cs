using UnityEngine;
using System.Collections;

public class OutlineByReference : MonoBehaviour {
	private EchoGameObject ego;
	
	public Material outlineMat;
	public Color outlineColor = new Color ( 1,0,0,1 );
	public Material overlayMat;
	public Color overlayColor = new Color ( 1,0,0,1 );
	// Use this for initialization
	void Start () {
		ego = EchoGameObject.Add ( gameObject, true, true, outlineMat, overlayMat );
		ego.SetOutlineColor ( outlineColor );
		ego.SetOutlineOverlayColor ( overlayColor );
	}
	
	// Update is called once per frame
	void Update () {
		if (RayCastRazer.gRayCastRazer.selectedGameObject == gameObject || RayCastRazer.gRayCastRazer.onGameObject == gameObject) {
						ego.Outline (true);		
				} else {
			ego.Outline(false);
				}
	}
}
