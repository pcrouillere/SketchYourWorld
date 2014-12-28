#pragma strict

//-----------------------------------------------------------------------------
class CubeBrain extends EchoGameObject
{
@HideInInspector
	var targetTransform:Transform;
@HideInInspector
	var speed:float;
	
	//===========================================================================
	function OnTriggerEnter ( col:Collider )
	{
		// Setting object to not active will return it to inactive pool 
		EchoActive ( false );
	}	
	
	//=========================================================================
	// Sets up this object to start moving towards a target object
	//=========================================================================
	function Shoot ( istartpos:Vector3, idirection:Vector3, itargettransform:Transform, ispeed:float )
	{
		EchoActive ( true );
		cachedTransform.position		= istartpos;
		cachedTransform.rotation		= Quaternion.LookRotation ( idirection );	
		speed							= ispeed;
		targetTransform					= itargettransform;
	}

	//=========================================================================
	// makes cube home-in on target
	//=========================================================================
	function Update ()
	{
		if ( renderer.enabled )
		{
			var qRotation:Quaternion;
		
			cachedTransform.Translate ( Vector3.forward * Time.deltaTime * speed );	

			qRotation = Quaternion.LookRotation ( targetTransform.position - cachedTransform.position );

			cachedTransform.rotation = Quaternion.Slerp ( cachedTransform.rotation, qRotation, Time.deltaTime * 0.5f );
		}
	}
}

