#pragma strict

public var prefab:GameObject;

function Start () 
{
	var loopx:int;
	var loopy:int;
	var go:GameObject;

#if !UNITY_3_5
			prefab.SetActive ( true );
#else
			prefab.active = true;
#endif
	
	for ( loopy = 0; loopy < 8; loopy++ )
	{
		for ( loopx = 0; loopx < 12; loopx ++ )
		{
			go	= UnityEngine.Object.Instantiate ( prefab ) as GameObject;
			
			// Unity seems to need this line below, or wont batch but only sometimes ?
			go.renderer.sharedMaterial = prefab.renderer.sharedMaterial;
			
			
			go.transform.localPosition = Vector3 ( ( loopx-5.5 ) * 70.0, ( loopy-3.5 ) * 64.0, prefab.transform.localPosition.z );
		}
	}

#if !UNITY_3_5
			prefab.SetActive ( false );
#else
			prefab.active = false;
#endif


}

