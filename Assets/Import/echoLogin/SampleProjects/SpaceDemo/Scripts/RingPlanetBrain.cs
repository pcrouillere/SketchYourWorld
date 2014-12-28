using UnityEngine;
using System.Collections;
using System.Collections.Generic;


class RingPlanetBrain : EchoAsteroidBelt
{
	public static EchoGameObject egoPlanet;
	public static EchoGameObject egoMoon;

	//===========================================================================
	void Start()
	{
		egoPlanet		= EchoGameObject.Find ("planet_green");
		egoMoon			= EchoGameObject.Find ("Moon");
		
		MakeAsteroidBelt();
	}

	//===========================================================================
	void Update()
	{
		egoPlanet.transform.Rotate ( new Vector3 ( 0 ,Time.deltaTime * 1.0f ,0 ) );
		egoMoon.transform.Rotate ( new Vector3 ( 0 ,Time.deltaTime * 1.5f ,0 ) );
	}
	
}
