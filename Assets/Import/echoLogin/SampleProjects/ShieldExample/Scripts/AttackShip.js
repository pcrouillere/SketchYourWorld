#pragma strict

class AttackShip extends EchoPoolManager
{
	static var mypool:EchoPoolList;
	var shotSpeed:float = 0.1;
	// EnemyShips are setup in inspector
	var EnemyShips:SpaceShipBrain[]; 
	var delay:float;

	//=============================================================================
	function Update()
	{
		delay += Time.deltaTime;
		
		if ( delay > 0.5 )
		{
			delay = 0;
			Shoot();
		}
	}
	
	//===========================================================================
	function Shoot()
	{
		// this will get a free object from pool
		var brain:ShotBrain = echoObjectPool.GetFree() as ShotBrain;
		
		// brain can be null if there are no free objects
		if ( brain != null ) 
			brain.Shoot ( transform.position, Vector3 ( Random.Range ( -1,1 ), Random.Range ( -1,1 ), 1 ), EnemyShips[Random.Range(0,2)].transform, shotSpeed );
	}

}

