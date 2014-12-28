#pragma strict

class CubeManager extends EchoPoolManager
{
	static var mypool:EchoPoolList;
	static var target:EchoGameObject;
	static var time:float;
	static var sSpeed:float;
	public var speed:float = 0.5;
	
//===========================================================================
	function Start()
	{
		sSpeed = speed;
		mypool = echoObjectPool;
		target = EchoGameObject.Find ("target");
	}

//===========================================================================
	static function ShootCube()
	{
		// this will get a free object from pool
		var brain:CubeBrain = mypool.GetFree() as CubeBrain;
		
		// brain can be null if there are no free objects
		if ( brain != null ) 
			brain.Shoot ( Vector3 ( 0, 0.2, -10 ), Vector3 ( 0, 0, 1 ), target.transform, sSpeed );
	}
	
//===========================================================================
	function Update()
	{
		time += Time.deltaTime;
		
		if ( time > 1.0 )
		{
			time = 0;
			ShootCube();
		}
	}
}