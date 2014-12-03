using UnityEngine;
using System.Collections;

//$-----------------------------------------------------------------------------
//@ Script for UVSet eye movement
//@
//@ Usage
//@
//@ 1. Make an eye mesh with pupil centered on texture and mesh
//@ 2. Drag this script to the eye GameObject
//@ 3. Set UV Set Size, Uv Padding and move speed in inspector
//@ 4. Add EchoHeadController to the GameObject's head
//# 5. drag each eye to slots on Head object in inspector
//@ 
//@  NOTE: See EyeFollowUVSet project for example and EchoEyeController.cs 
//&-----------------------------------------------------------------------------
public class EchoHeadController : EchoGameObject
{
	public EchoEyeController[] eyes;
	public Transform           lookAtTransform;
	
	void LookAtTransform ( Transform itrans )
	{
		lookAtTransform = itrans;
	}

	void Start ()
	{
		int loop;
		
		for ( loop = 0; loop < eyes.Length; loop++ )
		{
			eyes[loop].LookAtTransform ( lookAtTransform );
		}
	}
}
