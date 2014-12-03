using UnityEngine;
using System.Collections;

class FPS
{
	public static float  fps					= 60;
	public static float  updateInterval			= 0.5f;
	public static float  accum					= 0.0f; 
	public static float  frames					= 0; 
	public static float  timeleft				= 0.5f; 

	// found this on net somewhere
	public static void ProcessInUpdate()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;

		if ( timeleft <= 0.0f )
		{
			fps = accum/frames;
			timeleft = updateInterval;
			accum = 0.0f;
			frames = 0;
		}
	}
}



