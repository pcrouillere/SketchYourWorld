using UnityEngine;
using System.Collections;

//$-----------------------------------------------------------------------------
//@ Extend from this to make an Asteroid belt around an object
//@
//@ Usage
//@
//@ 1. Extend from this class and place script on an object
//@ 2. Drag an asteroid prefab to Asteroid_Prefab slot in inspector
//@ 3. Set Astroid Count, Radius and Width
//@ 4. Press RUN to see your new Asteroid belt that has one drawcall
//@ 
//@  NOTE: See space demo for script example on "planet_green" object
//&-----------------------------------------------------------------------------
public class EchoAsteroidBelt : EchoGameObject
{
	public GameObject       asteroid_prefab1 	= null;
	public GameObject       asteroid_prefab2 	= null;
	public float            asteroid_maxscale   = 1;
	public int              asteroid_count		= 128;
	public float            asteroid_radius 	= 32;
	public float            asteroid_width 		= 8;
	public static GameObject asteroid_parent 	= null;
	
	public void MakeAsteroidBelt()
	{
		int loop;
		float per;
		float angle;
		float x;
		float y;
		Vector3 apos;
		MeshFilter pmf;
		int index = 0;
		float scale;
		
		if ( asteroid_maxscale < 1 )
			asteroid_maxscale = 1;
		
		asteroid_parent = null;
		
		// make parent that will hold mesh of all asteroids
		asteroid_parent 		= new GameObject();
		asteroid_parent.name 	= "AsterParent";
		pmf 					= asteroid_parent.AddComponent<MeshFilter>();
		asteroid_parent.AddComponent<MeshRenderer>();
		asteroid_parent.renderer.sharedMaterial = asteroid_prefab1.renderer.sharedMaterial;
		
#if !UNITY_3_5
		asteroid_parent.SetActive ( true );
#else
		asteroid_parent.active = true;
#endif

		// make asteroids
		for ( loop = 0; loop < asteroid_count; loop++ )
		{
			per = (float)loop / (float)asteroid_count;
			
			angle = per * Mathf.PI * 2;
			
			x = Mathf.Sin ( angle ) * Random.Range ( asteroid_radius, asteroid_radius + asteroid_width );
    		y = Mathf.Cos ( angle ) * Random.Range ( asteroid_radius, asteroid_radius + asteroid_width );
			
			apos = new Vector3 ( x, 0, y );
			
			GameObject aster;
			
			if ( Random.Range ( 0, 100 ) < 70 || asteroid_prefab2 == null )
			{
				aster = (GameObject)Instantiate(asteroid_prefab1 );
				aster.renderer.sharedMaterial = asteroid_prefab1.renderer.sharedMaterial;
			}
			else
			{
				aster = (GameObject)Instantiate(asteroid_prefab2 );
				aster.renderer.sharedMaterial = asteroid_prefab2.renderer.sharedMaterial;
			}
			
			aster.transform.parent 			= asteroid_parent.transform;
			aster.transform.localPosition 	= apos;
			aster.transform.rotation 		= Random.rotation;
			if ( Random.Range ( 0, 100 ) < 75 )
				scale = Random.Range ( 1.0f, asteroid_maxscale );
			else
				scale = 1.0f;
			aster.transform.localScale 		= new Vector3 ( scale, scale, scale );

#if !UNITY_3_5
		aster.SetActive ( true );
#else
		aster.active = true;
#endif
		}

		pmf = asteroid_parent.GetComponent<MeshFilter>();

		// combine into one mesh
		MeshFilter[] meshFilters = asteroid_parent.GetComponentsInChildren<MeshFilter>();
    	CombineInstance[] combine = new CombineInstance[meshFilters.Length];

	    for ( loop = 0; loop < meshFilters.Length; loop++)
		{
			if ( meshFilters[loop].sharedMesh != null )
			{
		        combine[index].mesh = meshFilters[loop].sharedMesh;
		        combine[index].transform = meshFilters[loop].transform.localToWorldMatrix;
			
				Destroy ( meshFilters[loop].transform.gameObject );
								
//#if !UNITY_3_5
//		meshFilters[loop].transform.gameObject.SetActive ( false );
//#else
//		meshFilters[loop].transform.gameObject.active = false;
//#endif
				index++;
			}
	    }
		
		if ( index < meshFilters.Length )
		{
	    	CombineInstance[] finalCombine = new CombineInstance[index];
			
			for ( loop = 0; loop < index; loop++ )
				finalCombine[loop] = combine[loop];
			
			combine = finalCombine;
		}
		
		if ( pmf )
		{
			pmf.mesh = new Mesh();
	        pmf.mesh.CombineMeshes(combine,true);
			pmf.mesh.Optimize();
			pmf.mesh.RecalculateBounds();
		}

		asteroid_parent.transform.parent				= cachedTransform;
		asteroid_parent.transform.localPosition			= new Vector3 ( 0,0,0 );
		asteroid_parent.transform.localEulerAngles		= new Vector3 ( 0,0,0 );
		asteroid_parent.transform.localScale			= new Vector3 ( 1,1,1 );
	}

}