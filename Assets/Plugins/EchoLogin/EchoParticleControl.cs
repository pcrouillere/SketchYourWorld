using UnityEngine;
using System.Collections;

public class EchoParticleControl : MonoBehaviour 
{
	public float cutoffDistance = 32;
	public Camera cam;
	private Transform _transform;
	private Transform _camTransform;
	ParticleSystem ps = null;
	
	void Start ()
	{
		if ( cam == null )
			return;
		
		ps = GetComponent<ParticleSystem>();
		_transform 		= transform;
		_camTransform 	= cam.transform;
		
		float dist = Vector3.Distance ( cam.transform.position, _transform.position );
		
		if ( ps != null && dist < cutoffDistance )
			ps.enableEmission = true;
		else
			ps.enableEmission = false;
	}
	
	void Update () 
	{
		if ( ps == null )
			return;
		
		float dist = Vector3.Distance ( _camTransform.position, _transform.position );
		
		if ( dist < cutoffDistance )
			ps.enableEmission = true;
		else
			ps.enableEmission = false;
	}
}
