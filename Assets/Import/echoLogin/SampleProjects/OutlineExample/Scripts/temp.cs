using UnityEngine;
using System.Collections;

public class temp : MonoBehaviour {
	public EchoGameObject ego;

	public Material outlineMat;
	public Color outlineColor = new Color ( 1,0,0,1 );
	public Material overlayMat;
	public Color overlayColor = new Color ( 1,0,0,1 );


	// Use this for initialization
	void Start () {
		ego = EchoGameObject.Add ( gameObject, true, true, outlineMat, overlayMat );
		
		ego.SetOutlineColor ( outlineColor );
		ego.SetOutlineOverlayColor ( overlayColor );
		
		ego.Outline(true);
		
	}
	
}