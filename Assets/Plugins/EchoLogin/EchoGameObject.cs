using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//-------------------------------------------------------------------------------------
public class ELC
{
	public const float CELL256	= 1.0f / 256.0f;
	public const float CELL128	= 1.0f / 128.0f;
	public const float CELL64	= 1.0f / 64.0f;
	public const float CELL32	= 1.0f / 32.0f;
	public const float CELL16	= 1.0f / 16.0f;
	public const float CELL8	= 1.0f / 8.0f;
	public const float CELL4	= 1.0f / 4.0f;
	public const float CELL2	= 1.0f / 2.0f;
};


public class EchoShaderProperty
{
	public string   name;
	public int      propertyID;
	public int      type;        // 0 = float, 1 = vector4  < 0 == unused
	public float 	floatVal;
	public Vector4 	vec4Val;

}

//$-----------------------------------------------------------------------------
//@ EchoGameObject 	- The core functionality of this framework.
//&-----------------------------------------------------------------------------
[AddComponentMenu("CORE/EchoGameObject")]
public class EchoGameObject : MonoBehaviour
{
	static EchoGameObject 					egoFirst					= null;
	static EchoGameObject 					egoLast						= null;
	private static int									_echoOverride			= 0;
	public static bool									_systemInitFlag			= false;
	public static MaterialPropertyBlock 				echo_MatProperties			= null;
	private bool										_echoInit				= false;
	private List<EchoShaderProperty>					_shaderProperties;
	private EchoShaderProperty							_Color;
	private EchoShaderProperty							_echoScale;
	private EchoShaderProperty							_echoRGBA;
	private EchoShaderProperty							_echoAlpha;
	private EchoShaderProperty							_echoMix;
	private EchoShaderProperty							_echoHitMix0;
	private EchoShaderProperty							_echoHitMix1;
	private EchoShaderProperty							_echoHitMix2;
	private EchoShaderProperty							_echoHitMix3;
	private EchoShaderProperty							_echoUV;
	private EchoShaderProperty							_echoHitVector0;
	private EchoShaderProperty							_echoHitVector1;
	private EchoShaderProperty							_echoHitVector2;
	private EchoShaderProperty							_echoHitVector3;
	private EchoShaderProperty							_MainTex_ST;
	private EchoGameObject								_next;
	private EchoGameObject								_prev;
	private Vector3										_originalScale;
	private List<Vector2[]>								_echoUVSet				= new List<Vector2[]>();
	private bool										_echoHasOutline         = false;
	[HideInInspector]
	public  bool                            echo_PoolActiveList			= false; 
	[HideInInspector]
	public EchoGameObject					echoFxNext;
	[HideInInspector]
	public bool                     		echoFxFlag;
	[HideInInspector]
	public EchoGameObject[]					egoChildren;
	[HideInInspector]
	public int                              egoChildCount;
	[HideInInspector]
	public int                              echoUVSetCount;
	[HideInInspector]
	public Mesh                      		echoOutlineMesh				= null;
	[HideInInspector]
	public EchoGameObject              		egoRoot						= null;
	[HideInInspector]
	public Material[]                       echo_Mat0;
	[HideInInspector]
	public Material[]                       echo_Mat1;
	[HideInInspector]
	public int              		        echo_OutlineMatIndex;
	[HideInInspector]
	public int              		        echo_OverlayMatIndex;
	[HideInInspector]
	public SkinnedMeshRenderer              echo_skinnedMeshRenderer	= null;
	[HideInInspector]
	public Renderer              			echo_Renderer				= null;
	[HideInInspector]
	public GameObject                       echo_OutlineMeshObj;
	[HideInInspector]
	public EchoGameObject    				echoListNext;
	[HideInInspector]
	public EchoGameObject    				echoListPrev;
	[HideInInspector]
	public EchoPoolList   					echoPool					= null;
	[HideInInspector]
	public int                              renderQueue					= 0;
	[HideInInspector]
	public Transform 						cachedTransform;
	[HideInInspector]
  	public int 								meshVertCount;
	[HideInInspector]
  	public MeshFilter 						meshFilter                  = null;
	[HideInInspector]
  	public Mesh 							mesh                        = null;
	[HideInInspector]
	public int                              materialId                  = 0;
	[HideInInspector]
	public Material[]                       mainMaterials;
	public Material[]                       alternateMaterials;
	public bool             				activeAtStart				= true;     
	public bool             				rendererEnabled				= true;     
	public bool 							fixScale					= false; 	
	public bool             				addChildren					= false;
	public Material         				outlineMaterial				= null;
	public Color            				outlineColor				= new Color ( 1,1,1,1 );
	public Material         				outlineOverlayMaterial		= null;
	public Color            				outlineOverlayColor			= new Color ( 1,1,1,1 );
	public bool                             outlineChildren             = false;
	public bool                             outlineOn                   = false;

	
//$===========================================================================
//@ If you override this from the extended class, you must call base.OnDestroy first
//@ so the framework can properly remove objects from the master list.
//&===========================================================================
	virtual public void OnDestroy()
	{
		if ( _echoInit )
		{
			EchoGameObject.ListRemoveObject ( this );
		}
	}

//$===========================================================================
//@ If you override this from the extended class, you must call base.OnDisable first
//@ so the framework can properly remove objects from a pool.
//&===========================================================================
	virtual public void OnDisable()
	{
		if ( echoPool != null )
		{
			echoPool.ReturnPool ( this );
		}
	}

//$===========================================================================
//@ If you override this from the extended class, you must call base.Awake first
//@ so EchoGameObject's can be initialized by the system when needed.
//&===========================================================================
	virtual public void Awake()
	{
		if ( _echoOverride == 0 && _echoInit == false )
		{
			Init ( gameObject, fixScale, addChildren, activeAtStart, rendererEnabled );
		}
	}

//$===========================================================================
//@ This is for the scripts that extend from EchoGameObject that use Object Pools.
//@ If you override this method, it will be called on object creation at startup.
//&===========================================================================
	virtual public void EchoPoolObjectInit()
	{
	}
	
//============================================================
// this is called by framework, no user need to call this
//============================================================
	public void EchoPoolInit ( EchoPoolList ipool = null )
	{
		echoPool	= ipool;

		if ( echoPool != null )
		{
			EchoActive ( false );

			echoPool.AddNewObject ( this );
		}
	}

	
//---------------------------------------------------------------------------
//  this is automatically called when first EchoGameObject is made
//---------------------------------------------------------------------------
	public static void InitAtStartup()
	{
		_systemInitFlag		= true;
		
		_echoOverride = 1;
		egoFirst					= new GameObject().AddComponent<EchoGameObject>();
		egoLast						= new GameObject().AddComponent<EchoGameObject>();
		egoFirst.EchoActive ( false );
		egoLast.EchoActive ( false );
		egoFirst.gameObject.hideFlags 	= HideFlags.HideInHierarchy | HideFlags.HideInInspector;
		egoLast.gameObject.hideFlags 	= HideFlags.HideInHierarchy | HideFlags.HideInInspector;
		_echoOverride = 0;

		egoFirst._next	= egoLast;
		egoFirst._prev	= null;

		egoLast._next	= null;
		egoLast._prev	= egoFirst;

		echo_MatProperties = new MaterialPropertyBlock();
	}

//---------------------------------------------------------------------------
// Adds Object to master EchoGameObject list
//---------------------------------------------------------------------------
	public static void ListAddObject ( EchoGameObject iego )
	{
		iego._next				= egoLast;
		iego._prev				= egoLast._prev;

		egoLast._prev._next		= iego;
		egoLast._prev			= iego;
	}

//---------------------------------------------------------------------------
// Removes Object from master EchoGameObject list
//---------------------------------------------------------------------------
	public static void ListRemoveObject ( EchoGameObject iego )
	{
		iego._prev._next			= iego._next;
		iego._next._prev			= iego._prev;
	}

//$-----------------------------------------------------------------------------
//@ Finds any EchoGameObject, even if its inactive
//@
//@ NOTE: All EchoGameObject should be active at start. If you need them inactive,
//@ set the Active At Start EchoGameObject Option to false.
//@
//@ Parameters:
//@
//# igameobjectname - Name of the game object
//&-----------------------------------------------------------------------------
	public static EchoGameObject Find ( string igameobjectname )
	{
		EchoGameObject  ego		= null;

		for ( ego = egoFirst._next; ego != egoLast; ego = ego._next )
		{
			if ( ego.name == igameobjectname )
			{
				break;				
			}
		}

		return ( ego );
	}

//$-----------------------------------------------------------------------------
//@ Makes an EchoGameObject for
//@
//@ NOTE: All EchoGameObject should be active at start. If you need them inactive,
//@ set the Active At Start EchoGameObject Option to false.
//@
//@ Parameters:
//@
//# igameobjectname - Name of the game object
//&-----------------------------------------------------------------------------
	public static EchoGameObject Add ( GameObject igo, bool ifixScale = false, bool iaddchildren = false, Material ioutlinemat = null, Material ioverlaymat = null )
	{
		_echoOverride = 1;
		EchoGameObject ego = igo.AddComponent<EchoGameObject>();
		_echoOverride = 0;

		ego.outlineMaterial			= ioutlinemat;
		ego.outlineOverlayMaterial	= ioverlaymat;

		if ( ego )
			ego.Init ( igo, ifixScale, iaddchildren );

		return ( ego );
	}

//$-----------------------------------------------------------------------------
//@ Sets Color for Outline
//@
//@ NOTE: The GameObject needs to have the outline materials assigned in inspector
//@
//@ Parameters:
//@
//# Color - Color of outline
//&-----------------------------------------------------------------------------
	public void SetOutlineColor ( Color icolor )
	{
		outlineColor = icolor;

		if ( echo_Mat1 != null )
		{
			if ( outlineMaterial != null )
				echo_Mat1[echo_OutlineMatIndex].SetVector ("_echoGlowColor", outlineColor );
		}
	}

//$-----------------------------------------------------------------------------
//@ Sets Color for Overlay
//@
//@ NOTE: The GameObject needs to have the overlay materials assigned in inspector
//@
//@ Parameters:
//@
//# Color - Color of overlay
//&-----------------------------------------------------------------------------
	public void SetOutlineOverlayColor ( Color icolor )
	{
		outlineOverlayColor = icolor;

		if ( echo_Mat1 != null )
		{
			if ( outlineOverlayMaterial != null )
				echo_Mat1[echo_OverlayMatIndex].SetVector ("_echoGlowColor", outlineOverlayColor );
		}
	}

//==========================================================================
	public bool Init ( GameObject igo = null, bool ifixScale = false, bool iaddchildren = false, bool iactiveflag = true, bool irendererenabled = true )
	{
		bool			burnMeshScale = false;
		Vector3			scale;
		Vector3			newpos;
		int             numats = 0;
		int 			loop;
		EchoGameObject	ego;
		Transform       child;

		if ( _systemInitFlag == false )
			InitAtStartup();

		if ( igo == null )
			return ( false );

		if ( _echoInit )
			return ( false );
		
		if ( DoesParentTreeHaveFixScale ( igo.transform ) )
			return ( false );

		_echoInit = true;

		ListAddObject ( this );

		cachedTransform					= gameObject.transform;	// cache transform for speed
		
		echo_skinnedMeshRenderer		= GetComponent<SkinnedMeshRenderer>();

		if ( echo_skinnedMeshRenderer == null )
		{
			echo_Renderer = renderer;
		}
		else
		{
			echo_Renderer = echo_skinnedMeshRenderer.renderer;
		}

		if ( outlineMaterial != null || outlineOverlayMaterial != null )
		{
			_echoHasOutline = true;
		}

		if ( echo_Renderer )
		{
			_shaderProperties = new List<EchoShaderProperty>(32);

			mainMaterials = echo_Renderer.sharedMaterials;

			InitShaderProperties( mainMaterials );
			InitShaderProperties( alternateMaterials );

			if ( _echoHasOutline )
				echo_Mat0 = echo_Renderer.sharedMaterials;
			
			renderQueue = echo_Renderer.sharedMaterial.renderQueue;
		}
		
#if !UNITY_3_5
		gameObject.SetActive ( iactiveflag );
#else
		gameObject.active = iactiveflag;
#endif
	
		echoFxNext			= null;
		echoFxFlag			= false;

		if ( echo_Renderer )
			echo_Renderer.enabled		= irendererenabled;


		egoChildCount				= 0;

		_originalScale = cachedTransform.localScale;
		
		if ( ifixScale )
		{
			if ( Mathf.Abs ( 1.0f - cachedTransform.localScale.x ) > 0.0001 || Mathf.Abs ( 1.0f - cachedTransform.localScale.y ) > 0.0001 || Mathf.Abs ( 1.0f - cachedTransform.localScale.z ) > 0.0001 )
			{
				burnMeshScale = true;
			}
		}
		
		if ( burnMeshScale )
		{
			EchoBurnMeshScale();
		}

		if ( ifixScale )
			cachedTransform.localScale = new Vector3 ( 1, 1, 1 );

		_echoOverride++;

		egoChildren = new EchoGameObject[ cachedTransform.childCount ];

		for ( int index = 0; index < cachedTransform.childCount; index++ )
		{
			child = cachedTransform.transform.GetChild(index);

			ego = child.gameObject.GetComponent<EchoGameObject>();

			scale.x = child.transform.localScale.x * _originalScale.x;
			scale.y = child.transform.localScale.y * _originalScale.y;
			scale.z = child.transform.localScale.z * _originalScale.z;

			newpos.x = child.transform.localPosition.x * _originalScale.x;
			newpos.y = child.transform.localPosition.y * _originalScale.y;
			newpos.z = child.transform.localPosition.z * _originalScale.z;

			if ( ego != null )
			{
				if ( ifixScale )
				{
					child.transform.localPosition		= newpos;
					child.transform.localScale			= scale;
				}

				ego.Init ( ego.gameObject, ego.fixScale, ego.addChildren, ego.activeAtStart, ego.rendererEnabled );
			}
			else
			{
				if ( iaddchildren )
				{
					if ( ifixScale )
					{
						child.transform.localPosition		= newpos;
						child.transform.localScale			= scale;
					}
					
					ego = child.gameObject.AddComponent<EchoGameObject>();
					ego.Init ( ego.gameObject,ifixScale, true, iactiveflag, irendererenabled );
				}
				else
				{
					child.transform.localPosition		= newpos;
					child.transform.localScale			= scale;
					ProcessNormalChildren ( child.gameObject );
				}
			}

			egoChildren[egoChildCount] = ego;
			egoChildCount++;
		}

		_echoOverride--;
		
		if ( egoChildCount < 1 )
			outlineChildren = false;

		// OUTLINE and OVERLAY ==
		if ( _echoHasOutline )
		{
			GetMesh();
			
			// does it have submeshes then make an outline mesh
			if ( mesh != null && mesh.subMeshCount > 1 || gameObject.isStatic )
			{
				numats = 0;

				if ( outlineMaterial != null )
					numats++;

				if ( outlineOverlayMaterial != null )
					numats++;

				if ( numats > 0 )
				{
					echo_Mat1 = new Material[numats];

					numats = 0;

					if ( outlineMaterial != null )
					{
						echo_OutlineMatIndex = numats;
						echo_Mat1[numats] = new Material ( outlineMaterial );
						numats++;			
					}

					if ( outlineOverlayMaterial != null )
					{
						echo_OverlayMatIndex = numats;
						echo_Mat1[numats] = new Material ( outlineOverlayMaterial );
						numats++;			
					}

					MakeFullOutlineMesh(); 
				}
			}
			else
			{
				// setup outline material array
				if ( echo_Mat0 != null )
				{
					numats = echo_Mat0.Length;

					if ( outlineMaterial != null )
					{
						echo_OutlineMatIndex = numats;
						numats++;
					}

					if ( outlineOverlayMaterial != null )
					{
						echo_OverlayMatIndex = numats;
						numats++;
					}

					echo_Mat1 = new Material[numats];

					for ( loop = 0; loop < echo_Mat0.Length; loop++ )
					{
						echo_Mat1[loop] = echo_Mat0[loop];				
					}

					if ( outlineMaterial != null )
						echo_Mat1[echo_OutlineMatIndex] = new Material ( outlineMaterial );

					if ( outlineOverlayMaterial != null )
						echo_Mat1[echo_OverlayMatIndex] = new Material ( outlineOverlayMaterial );
				}

			}

			if ( echo_Mat1 != null )
			{
				if ( outlineMaterial != null )
					echo_Mat1[echo_OutlineMatIndex].SetVector ("_echoGlowColor", outlineColor );

				if ( outlineOverlayMaterial != null )
					echo_Mat1[echo_OverlayMatIndex].SetVector ("_echoGlowColor", outlineOverlayColor );
			}
		}

		// need to check parent is topmost echogameobject
		if ( echo_skinnedMeshRenderer != null )
		{
			egoRoot = this;
		}
		else
		{
			egoRoot = cachedTransform.root.GetComponent<EchoGameObject>();
			
			if ( egoRoot == null )
			{
				egoRoot = this;
			}
		}
		
		if ( outlineOn )
			Outline ( true );
		
		return ( true );
	}
	
//------------------------------------------------------------------------------
	private void GetMesh()
	{
		if ( mesh != null )
			return;
		
		meshFilter						= GetComponent<MeshFilter>() as MeshFilter;
		
		if ( meshFilter != null )
        {
            mesh = meshFilter.mesh;
			
			meshVertCount = 0;
			if ( mesh != null )
	            meshVertCount    = mesh.vertices.Length;
        }
	}

//------------------------------------------------------------------------------
	private bool DoesParentTreeHaveFixScale ( Transform itfrm )
	{
		Transform tfrm;
		bool rc = false;
		EchoGameObject ego;

		tfrm = itfrm.parent;

		while ( tfrm != null )
		{
			ego = tfrm.gameObject.GetComponent<EchoGameObject>();
			
			if ( ego != null )
			{
				// does it have parent that will fix scale ?
				// that has not been Initialized ?
				if ( ego.fixScale == true && ego._echoInit == false )
				{
					rc = true;
					break;
				}
			}

			tfrm = tfrm.parent;
		}

		return ( rc );
	}
	
//------------------------------------------------------------------------------
	private void ProcessNormalChildren ( GameObject igo )
	{
		EchoGameObject ego;
		Transform child;

		for ( int index = 0; index < igo.transform.childCount; index++ )
		{
			child = igo.transform.GetChild(index);

			ego = child.gameObject.GetComponent<EchoGameObject>();

			if ( ego != null )
				ego.Init ( ego.gameObject, ego.fixScale, ego.addChildren, ego.activeAtStart, ego.rendererEnabled );
			else
				ProcessNormalChildren ( child.gameObject );

		}
	}

//------------------------------------------------------------------------------
	public void MakeFullOutlineMesh()
	{
		int loop;
		Vector3 oldp = cachedTransform.position;

		cachedTransform.position = new Vector3 ( 0,0,0 );

		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
    	CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		
	    for ( loop = 0; loop < meshFilters.Length; loop++)
		{
	        combine[loop].mesh = meshFilters[loop].sharedMesh;
	        combine[loop].transform = meshFilters[loop].transform.localToWorldMatrix;
	    }
		
		echoOutlineMesh = new Mesh();
        echoOutlineMesh.CombineMeshes(combine,true);
		echoOutlineMesh.Optimize();
		echoOutlineMesh.RecalculateBounds();

		GameObject  go = new GameObject();
		go.name = "EchoOutlineMesh";
		go.AddComponent<MeshFilter>();
		go.AddComponent<MeshRenderer>();
		go.transform.GetComponent<MeshFilter>().mesh = echoOutlineMesh;
		go.transform.parent = cachedTransform;
		go.transform.localPosition 		= new Vector3 ( 0,0,0 );
		go.transform.localEulerAngles 	= new Vector3 ( 0,0,0 );
		go.transform.localScale 		= new Vector3 ( 1,1,1 );
#if !UNITY_3_5
		go.SetActive ( false );	
#else
		go.active = false;
#endif
		go.renderer.materials           = echo_Mat1;


 		echo_OutlineMeshObj = go;

		cachedTransform.position = oldp;
	}

//$-----------------------------------------------------------------------------
//@ Sets EchoGameObject active or inactive 
//@
//@ Parameters:
//@
//# ionoff 			- turn on or off
//# ichildrenflag  	- turn children of this object to ionoff
//&-----------------------------------------------------------------------------
	public void EchoActive ( bool ionoff )
	{
#if !UNITY_3_5
		gameObject.SetActive ( ionoff );	
#else
		gameObject.active = ionoff;

		for ( int loop = 0; loop < egoChildCount; loop++ )
		{
			egoChildren[loop].EchoActive ( ionoff );
		}
#endif
		
	}

//$-----------------------------------------------------------------------------
//@ Sets EchoGameObject renderer.enabled to value 
//@
//@ Parameters:
//@
//# ionoff 			- turn on or off
//# ichildrenflag  	- turn children of this object to ionoff
//&-----------------------------------------------------------------------------
	public void RendererSet ( bool ionoff, bool ichildrenflag = false )
	{
		echo_Renderer.enabled = ionoff;

		if ( ichildrenflag )
		{
			for ( int loop = 0; loop < egoChildCount; loop++ )
			{
				egoChildren[loop].RendererSet ( ionoff, ichildrenflag );
			}
		}
	}
	
//===========================================================================
// Scales mesh and colliders so thier localScale can be 1,1,1
//===========================================================================
	private void EchoBurnMeshScale()
	{
		int loop;
		Vector3[] newv;
		Vector3 scale;
		Vector3 newpos;
		
		GetMesh();
		
		if ( mesh == null )
			return;
		
		newv = new Vector3[mesh.vertices.Length];

		if ( collider != null )
		{
			if ( collider.GetType() == typeof(BoxCollider) )
			{
				BoxCollider c = collider as BoxCollider;

				scale.x = c.size.x * cachedTransform.localScale.x;
				scale.y = c.size.y * cachedTransform.localScale.y;
				scale.z = c.size.z * cachedTransform.localScale.z;

				newpos.x = c.center.x * cachedTransform.localScale.x;
				newpos.y = c.center.y * cachedTransform.localScale.y;
				newpos.z = c.center.z * cachedTransform.localScale.z;

				c.center 	= newpos;
				c.size 	= scale;
			}
			else if ( collider.GetType() == typeof(SphereCollider) )
			{
				SphereCollider c = collider as SphereCollider;

				scale.x = cachedTransform.localScale.x;
				scale.y = cachedTransform.localScale.y;
				scale.z = cachedTransform.localScale.z;

				newpos.x = c.center.x * cachedTransform.localScale.x;
				newpos.y = c.center.y * cachedTransform.localScale.y;
				newpos.z = c.center.z * cachedTransform.localScale.z;

				c.center 	= newpos;

				if ( scale.x > scale.y && scale.x > scale.z )
					c.radius = c.radius * scale.x;
				else if ( scale.y > scale.x && scale.y > scale.z )
					c.radius = c.radius * scale.y;
				else
					c.radius = c.radius * scale.z;

			}
			else if ( collider.GetType() == typeof(CapsuleCollider) )
			{
				CapsuleCollider c = collider as CapsuleCollider;

				scale.x = cachedTransform.localScale.x;
				scale.y = cachedTransform.localScale.y;
				scale.z = cachedTransform.localScale.z;

				switch ( c.direction )
				{
					case 0:
						c.height = c.height * scale.x;

						if ( scale.y > scale.z )
							c.radius = c.radius * scale.y;
						else 
							c.radius = c.radius * scale.z;
						break;

					case 1:
						c.height = c.height * scale.y;

						if ( scale.x > scale.z )
							c.radius = c.radius * scale.x;
						else 
							c.radius = c.radius * scale.z;
						break;

					case 2:
						c.height = c.height * scale.z;

						if ( scale.x > scale.y )
							c.radius = c.radius * scale.x;
						else 
							c.radius = c.radius * scale.y;
						break;
				}

				newpos.x = c.center.x * cachedTransform.localScale.x;
				newpos.y = c.center.y * cachedTransform.localScale.y;
				newpos.z = c.center.z * cachedTransform.localScale.z;

				c.center 	= newpos;
			}
		}

		for ( loop = 0; loop < mesh.vertices.Length; loop++ )
		{
			newv[loop].x = mesh.vertices[loop].x * cachedTransform.localScale.x;
			newv[loop].y = mesh.vertices[loop].y * cachedTransform.localScale.y;
			newv[loop].z = mesh.vertices[loop].z * cachedTransform.localScale.z;
		}	
		
		mesh.vertices = newv;
		mesh.Optimize();
		mesh.RecalculateBounds();

		cachedTransform.localScale = new Vector3 ( 1, 1, 1 );

		if ( collider != null && collider.GetType() == typeof(MeshCollider) )
		{
			MeshCollider c		= collider as MeshCollider;
			c.sharedMesh		= mesh;
		}
	}

//$===========================================================================
//@ Makes a clone of an EchoGameObject
//@
//@ Parameters:
//@
//# iactiveflag 	- sets cloned object active state
//&===========================================================================
  	public GameObject Clone ( bool iactiveflag )
	{
	  	GameObject newgo = UnityEngine.Object.Instantiate ( this ) as GameObject;
		
		newgo.layer						= gameObject.layer;
		newgo.renderer.sharedMaterial 	= renderer.sharedMaterial;
		
		return ( newgo );
	}


	//-----------------------------------------------------------------------
	private void SetMaterialsRenderQueueOutline ( Material[] imats, int iqueue )
	{
		for ( int loop = 0; loop < imats.Length; loop++ )
		{
			if ( loop == echo_OutlineMatIndex )
				imats[loop].renderQueue = iqueue-1;
			else if ( loop == echo_OverlayMatIndex )
				imats[loop].renderQueue = iqueue+1;
			else
				imats[loop].renderQueue = iqueue;
		}
	}

	//-----------------------------------------------------------------------
	private void SetMaterialsRenderQueue ( Material[] imats, int irenderqueue )
	{
		for ( int loop = 0; loop < imats.Length; loop++ )
		{
			imats[loop].renderQueue = irenderqueue;
		}
	}

//$===========================================================================
//@ This adds a shield sphere object to an existing EchoGameObject.
//@ This should be for testing only.
//@
//@ Parameters:
//@
//# imat  		 	- Material with the echoLogin shield shader on it
//# iscale          - Object scale
//&===========================================================================
	public void Outline ( bool flag )
	{
		int loop;
		EchoGameObject child;

		// if we had to make a 2nd outline mesh
		if ( echoOutlineMesh != null )
		{
			if ( flag )
			{
#if !UNITY_3_5
				echo_OutlineMeshObj.SetActive ( true );	
#else
				echo_OutlineMeshObj.active = true;
#endif

				echo_OutlineMeshObj.renderer.materials           = echo_Mat1;
				SetMaterialsRenderQueueOutline ( echo_OutlineMeshObj.renderer.materials, 2449 );
				SetMaterialsRenderQueue ( echo_Renderer.materials, 2449 );
			}
			else
			{
				echo_Renderer.sharedMaterials = echo_Mat0;

#if !UNITY_3_5
				echo_OutlineMeshObj.SetActive ( false );	
#else
				echo_OutlineMeshObj.active = false;
#endif
			}

			return;		
		}

		if ( flag )
		{
			if ( echo_Renderer )
			{
				echo_Renderer.materials = echo_Mat1;
				SetMaterialsRenderQueueOutline ( echo_Renderer.materials, 2449 );
			}
		}
		else
		{
			if ( echo_Renderer )
			{
				echo_Renderer.sharedMaterials = echo_Mat0;
			}
		}

		if ( outlineChildren )
		{
			for ( loop = 0; loop < egoRoot.egoChildCount; loop++ )
			{
				child = egoRoot.egoChildren[loop];

				if ( flag )
				{
					if ( child.echo_Renderer )
					{
						child.echo_Renderer.materials = echo_Mat1;
						SetMaterialsRenderQueueOutline ( child.echo_Renderer.materials, 2449 );
					}
				}
				else
				{
					if ( child.echo_Renderer )
					{
						child.echo_Renderer.sharedMaterials = echo_Mat0;
					}
				}
			}
		}
	}
	
//$===========================================================================
//@ This method swaps main Material and Alternate Material
//@ NOTE: Alternate material must be setup in inspector.
//&===========================================================================
	public void SwapMaterial()
	{
		if ( alternateMaterials == null )
			return;

		ShaderPropertiesSubmit();
		
		if ( materialId == 0 )
		{
			materialId = 1;
			echo_Renderer.sharedMaterials = alternateMaterials;
		}
		else
		{
			materialId = 0;
			echo_Renderer.sharedMaterials = mainMaterials;
		}
	}
	
	
//$===========================================================================
//@ This adds a shield sphere object to an existing EchoGameObject.
//@ This should be for testing only.
//@
//@ Parameters:
//@
//# imat  		 	- Material with the echoLogin shield shader on it
//# iscale          - Object scale
//&===========================================================================
	public EchoGameObject AddShield ( Material imat = null, float iscale = 1.2f )
	{
		GameObject 		shield;
		EchoGameObject 	egoShield;
		Vector3 		scale;
		SphereCollider  sc;
		Rigidbody  		rb;
		
		GetMesh();

		scale.x = Mathf.Abs ( mesh.bounds.max.x - mesh.bounds.min.x ) * iscale;
		scale.y = Mathf.Abs ( mesh.bounds.max.y - mesh.bounds.min.y ) * iscale;
		scale.z = Mathf.Abs ( mesh.bounds.max.z - mesh.bounds.min.z ) * iscale;

		shield = GameObject.CreatePrimitive ( PrimitiveType.Sphere );			
		shield.transform.localScale		= scale;
		shield.renderer.sharedMaterial	= imat;

		egoShield = new EchoGameObject();
		egoShield.Init ( shield , false, false, true );
		egoShield.EchoBurnMeshScale();

		egoShield.transform.position			= cachedTransform.position;
		egoShield.transform.parent				= cachedTransform;

		sc = shield.GetComponent<SphereCollider>();

		if ( sc == null )
			sc = shield.AddComponent<SphereCollider>();

		sc.isTrigger 	= true;

		rb = shield.AddComponent<Rigidbody>();
		rb.useGravity	= false;
		rb.isKinematic	= true;

		return ( egoShield );
	}
	
//$===========================================================================
//@ Convert a pixel value to UV space
//@
//@ Parameters:
//@
//# pixelpos	- pixel position or length in pixels
//# textsize    - size of texture in pixels
//&===========================================================================
	public float Coord2UV ( float pixelpos, float textsize )
	{
		return ( pixelpos / textsize );
	}

//$===========================================================================
//@ Convert a X pixel value to U space
//@
//@ Parameters:
//@
//# pixelpos	- x pixel position or width in pixels
//# imat    	- material
//&===========================================================================
	public float X2U ( float pixelpos, Material imat )
	{
		return ( pixelpos / (float)imat.mainTexture.width );
	}

//$===========================================================================
//@ Convert a Y pixel value to V space
//@
//@ Parameters:
//@
//# pixelpos	- y pixel position or height in pixels
//# imat    	- material
//&===========================================================================
	public float Y2U ( float pixelpos, Material imat )
	{
		return ( pixelpos / (float)imat.mainTexture.height );
	}

//$===========================================================================
//@ Makes a UV set offset from base UV
//@
//@ NOTE: When using UV Sets, mesh should have as few triangles as possible
//@
//@ Parameters:
//@
//# addu	 	- U offset
//# addv        - V offset
//&===========================================================================
	public void UVCellMake ( float addu, float addv )
	{
		int loop;
		Vector2[] uvs;

		GetMesh();

		if ( mesh == null )
			return;
			
		uvs = new Vector2[mesh.vertices.Length];

		for ( loop = 0; loop < mesh.vertices.Length; loop++ )
		{
			uvs[loop] = new Vector2 ( mesh.uv[loop].x + addu, mesh.uv[loop].y - addv );
		}

		_echoUVSet.Add ( uvs );
		echoUVSetCount++;
	}

//$===========================================================================
//@ Makes a grid of UV sets to be used for cell animation
//@
//@ NOTE: When using UV Sets, mesh should have as few triangles as possible
//@
//@ Parameters:
//@
//# addu	 	- U offset of each cell  
//# addv        - V offset of each cell
//# iwidth      - cellwidth
//# icount      - number of cells
//&===========================================================================
	public void UVSetMakeJustify ( int iwidth, int iheight, float uvpadding )
	{
		int loopx;
		int loopy;
		float u;
		float v;
		float uMin;
		float vMin;
		float uMax;
		float vMax;
		float offset;
		float addu;
		float addv;
		float width;

		GetMesh();

		if ( mesh == null )
			return;

		uMin = 9999;
		vMin = 9999;
		uMax = -9999;
		vMax = -9999;
		
		// find UV extents
		for ( loopx = 0; loopx < mesh.vertices.Length; loopx++ )
		{
			u = mesh.uv[loopx].x;
			v = mesh.uv[loopx].y;
			
			if ( u < uMin )
				uMin = u;

			if ( v < vMin )
				vMin = v;

			if ( u > uMax )
				uMax = u;

			if ( v < vMax )
				vMax = v;
		}

		if ( vMin < uMin )
		{
			offset = vMin-uvpadding;
			width = ( offset)*2.0f;
		}
		else
		{
			offset = uMin-uvpadding;
			width = ( offset)*2.0f;
		}
		
		addu = width / (float)iwidth;
		addv = width / (float)iheight;

		for ( loopy = 0; loopy < iheight; loopy++ )
		{
			for ( loopx = 0; loopx < iwidth; loopx++ )
			{
				UVCellMake ( ( (float)loopx * addu ) - offset, ( (float)loopy * addv ) - offset );
			}
		}
	}
	
//$===========================================================================
//@ Makes a grid of UV sets to be used for cell animation
//@
//@
//@ NOTE: When using UV Sets, mesh should have as few triangles as possible
//@
//@ Parameters:
//@
//# addu	 	- U offset of each cell  
//# addv        - V offset of each cell
//# iwidth      - cellwidth
//# icount      - number of cells
//&===========================================================================
	public void UVSetMake ( float addu, float addv, int iwidth, int icount )
	{
		int loop;
		int x;
		int y;

		for ( loop = 0; loop < icount; loop++ )
		{
			x = loop % iwidth;
			y = loop / iwidth;

			UVCellMake ( x * addu, y * addv );
		}
	}

//$===========================================================================
//@ Makes UV sets to be used for smooth horizontal scrolling
//@
//@
//@ NOTE: When using UV Sets, mesh should have as few triangles as possible
//@
//@ Parameters:
//@
//# addu	 	- U offset of each cell  
//# addv        - V offset of each cell
//# iwidth      - cellwidth
//# icount      - number of cells
//&===========================================================================
	public void UVSetMakeScrollH ( float uwidth, int usteps )
	{
		int loop;
		float adduv;
		float curuv = 0.0f;
		
		adduv = uwidth / usteps;

		for ( loop = 0; loop < usteps; loop++ )
		{
			UVCellMake ( curuv, 0.0f );
			curuv+=adduv;
		}
	}
	
//$===========================================================================
//@ Makes UV sets to be used for smooth vertical scrolling
//@
//@
//@ NOTE: When using UV Sets, mesh should have as few triangles as possible
//@
//@ Parameters:
//@
//# addu	 	- U offset of each cell  
//# addv        - V offset of each cell
//# iwidth      - cellwidth
//# icount      - number of cells
//&===========================================================================
	public void UVSetMakeScrollV ( float vheight, int vsteps )
	{
		int loop;
		float adduv;
		float curuv = 0.0f;
		
		adduv = vheight / vsteps;

		for ( loop = 0; loop < vsteps; loop++ )
		{
			UVCellMake ( 0.0f, curuv );
			curuv+=adduv;
		}
	}

//$===========================================================================
//@ Sets a new UV set to mesh
//@
//@ NOTE: you must have setup UV sets with UVSetMake before calling this
//@
//@ Parameters:
//@
//# index	 	- index of uv set  
//&===========================================================================
	public void UVSet ( int index )
	{
		if ( mesh )
			mesh.uv = _echoUVSet[index];	
	}
	
//===========================================================================
// Add new shader property to List ( no user need to call this )
//===========================================================================
	private EchoShaderProperty AddShaderProperty ( Material isharedMat, string sname )
	{
		EchoShaderProperty esp = null;
		EchoShaderProperty espl;
		
		if ( isharedMat.HasProperty ( sname ) )
		{
			for ( int loop = 0; loop < _shaderProperties.Count; loop++ )
			{
				espl = _shaderProperties[loop];
				
				if ( espl.name == sname )
				{
					esp = espl;
					break;
				}
			}

			if ( esp == null )
			{
				esp 			= new EchoShaderProperty();
				esp.name        = sname;
				esp.propertyID 	= Shader.PropertyToID ( sname );
				_shaderProperties.Add ( esp ); 
			}
		}

		 return ( esp );
	}

//$===========================================================================
//@ Adds a new Float Per Object Shader Property
//@
//@ Parameters:
//@
//# isharedMat	- Material to use Property  
//# sname	 	- Name of Shader Property  
//&===========================================================================
	public EchoShaderProperty AddShaderPropertyFloat ( Material isharedMat, string sname )
	{
		EchoShaderProperty esp = AddShaderProperty ( isharedMat, sname );

		if ( esp != null )
		{
			esp.type     = 0;
			esp.floatVal = isharedMat.GetFloat ( sname );
		}

		return ( esp );
	}

//$===========================================================================
//@ Adds a new Vector4 Per Object Shader Property
//@
//@ Parameters:
//@
//# isharedMat	- Material to use Property  
//# sname	 	- Name of Shader Property  
//&===========================================================================
	public EchoShaderProperty AddShaderPropertyVector4 ( Material isharedMat, string sname )
	{
		EchoShaderProperty esp = AddShaderProperty ( isharedMat, sname );

		if ( esp != null )
		{
			esp.type     = 1;
			esp.vec4Val = isharedMat.GetVector ( sname );
		}

		return ( esp );
	}

//===========================================================================
// Init all the shader properties for this object  ( no user need to call this )
//===========================================================================
	public void InitShaderProperties ( Material[] imats )
	{
		Material mat;
		int loop;

		if ( imats == null )
			return;
		
		for ( loop = 0; loop < imats.Length; loop++ )
		{
			mat = imats[loop];
			//-------
			_MainTex_ST  		= AddShaderPropertyVector4 ( mat, "_MainTex_ST" );
			_Color  			= AddShaderPropertyVector4 ( mat, "_Color" );
			_echoRGBA			= AddShaderPropertyVector4 ( mat, "_echoRGBA" );
			_echoAlpha			= AddShaderPropertyFloat ( mat, "_echoAlpha" );
			_echoMix			= AddShaderPropertyFloat ( mat, "_echoMix" );
			_echoUV				= AddShaderPropertyVector4 ( mat, "_echoUV" );
			_echoScale			= AddShaderPropertyVector4 ( mat, "_echoScale" );
			_echoHitMix0		= AddShaderPropertyFloat ( mat, "_echoHitMix0" );
			_echoHitMix1		= AddShaderPropertyFloat ( mat, "_echoHitMix1" );
			_echoHitMix2		= AddShaderPropertyFloat ( mat, "_echoHitMix2" );
			_echoHitMix3		= AddShaderPropertyFloat ( mat, "_echoHitMix3" );
			_echoHitVector0 	= AddShaderPropertyVector4 ( mat, "_echoHitVector0" );
			_echoHitVector1 	= AddShaderPropertyVector4 ( mat, "_echoHitVector1" );
			_echoHitVector2 	= AddShaderPropertyVector4 ( mat, "_echoHitVector2" );
			_echoHitVector3 	= AddShaderPropertyVector4 ( mat, "_echoHitVector3" );
		}
	}

//$-----------------------------------------------------------------------------
//@ Submits this object's shader properties
//&-----------------------------------------------------------------------------
	public void ShaderPropertiesSubmit()
	{
		EchoShaderProperty espl;
		
		if ( echo_Renderer == null )
			return;
		
		echo_MatProperties.Clear();
		
		for ( int loop = 0; loop < _shaderProperties.Count; loop++ )
		{
			espl = _shaderProperties[loop];

			switch ( espl.type )
			{
				case 0:
					echo_MatProperties.AddFloat ( espl.propertyID, espl.floatVal );
					break;

				case 1:
					echo_MatProperties.AddVector ( espl.propertyID, espl.vec4Val );
					break;

				default:
					break;
			}
		}

		echo_Renderer.SetPropertyBlock ( echo_MatProperties );
	}

//$=============================================================================
//@ Sets the Tiling value of shaders with _MainTexST property
//@
//@ Parameters:
//@
//# itile  	    - Vector2 of the tiling value to set
//&-----------------------------------------------------------------------------
	public void ShaderSetTiling ( Vector2 itile )
	{
		if ( _MainTex_ST == null )
			return;	

		_MainTex_ST.vec4Val.x 	= itile.x;
		_MainTex_ST.vec4Val.y 	= itile.y;
	}

//$=============================================================================
//@ Sets the Offset value of shaders with _MainTexST property
//@
//@ Parameters:
//@
//# ioffset  	    - Vector2 of the tiling value to set
//&-----------------------------------------------------------------------------
	public void ShaderSetOffset ( Vector2 ioffset )
	{
		if ( _MainTex_ST == null )
			return;	

		_MainTex_ST.vec4Val.z 	= ioffset.x;
		_MainTex_ST.vec4Val.w 	= ioffset.y;
	}

//$=============================================================================
//@ Sets the alpha value of shaders with the _echoAlpha property
//@
//@ Parameters:
//@
//# ialpha      - Float alpha value 0-1
//&-----------------------------------------------------------------------------
	public void ShaderSet_echoAlpha ( float ialpha )
	{
		if ( _echoAlpha == null )
			return;

		_echoAlpha.floatVal		= ialpha;
	}

//$=============================================================================
//@ Sets the RGBA value of shaders with the _echoRGBA property
//@
//@ Parameters:
//@
//# irgba  	    - Vector4 of RGBA values 0-2
//&-----------------------------------------------------------------------------
	public void ShaderSet_echoRGBA ( Vector4 irgba )
	{
		if ( _echoRGBA == null )
			return;

		_echoRGBA.vec4Val 	= irgba;
	}
	
	public Vector4 ShaderGet_echoRGBA()
	{
		if ( _echoRGBA == null )
			return ( new Vector4 ( 0, 0, 0, 0 ) );

		return ( _echoRGBA.vec4Val );
	}

//$=============================================================================
//@ Sets the Color value of shaders with the _Color property
//@
//@ Parameters:
//@
//# icolor     - Color in ( r,g,b,a )
//&-----------------------------------------------------------------------------
	public void ShaderSet_Color ( Color icolor )
	{
		if ( _Color == null )
			return;

		_Color.vec4Val 	= icolor;
	}

//$=============================================================================
//@ Sets the Scale value of shaders with the _echoScale property
//@
//@ Parameters:
//@
//# iscale     - Scale ( x, y, z, 1 )  
//&-----------------------------------------------------------------------------
	public void ShaderSet_echoScale ( Vector4 iscale )
	{
		if ( _echoScale == null )
			return;

		_echoScale.vec4Val 	= iscale;
	}

//$=============================================================================
//@ Sets the UV for shaders that have the _echoUV or MainTex_ST property
//@
//@ Parameters:
//@
//# iuv  		- UV as Vector4 ( u1, v1, u2, v2 )  
//# itype       - 0 = using echoLogin shaders, 1 = use _MainTex_ST for offset
//&-----------------------------------------------------------------------------
	public Vector4 ShaderSet_echoUV ( Vector4 iuv, int itype )
	{
		if ( _echoUV == null )
			return ( iuv );

		if ( iuv.x > 1.0f )
			iuv.x -= 1.0f;
		if ( iuv.x < 0.0f )
			iuv.x += 1.0f;

		if ( iuv.y > 1.0f )
			iuv.y -= 1.0f;
		if ( iuv.y < 0.0f )
			iuv.y += 1.0f;

		if ( itype == 0 )
		{
			if ( iuv.z > 1.0f )
				iuv.z -= 1.0f;
			if ( iuv.z < 0.0f )
				iuv.z += 1.0f;

			if ( iuv.w > 1.0f )
				iuv.w -= 1.0f;
			if ( iuv.w < 0.0f )
				iuv.w += 1.0f;

			_echoUV.vec4Val = iuv;
		}
		else
		{
			_MainTex_ST.vec4Val.z = iuv.x;
			_MainTex_ST.vec4Val.w = iuv.y;
		}

		return ( iuv );
	}


//$=============================================================================
//@ Sets the UV for shaders that have the _echoUV as cells for cell animation
//@
//@ Parameters:
//@
//# cellnum         - Position in cell grid to show
//# iuvcellwidth    - Width of cell in UV space
//# iuvcellheiught  - Height of cell in UV space
//# icolumns        - Number of columns across in cell grid
//# iuvset          - Which UV set to use (1, 2 or zero for both)    
//&-----------------------------------------------------------------------------
	public void ShaderSetCell_echoUV ( int cellnum, float iuvcellwidth, float iuvcellheight, int icolumns, int iuvset = 1 )
	{
		if ( _echoUV == null )
			return;

		if ( iuvset == 0 || iuvset == 1 )
		{
			_echoUV.vec4Val.x = ( cellnum % icolumns ) * iuvcellwidth;
			_echoUV.vec4Val.y = 1.0f - ( cellnum / icolumns ) * iuvcellheight;
		}

		if ( iuvset == 0 || iuvset == 2 )
		{
			_echoUV.vec4Val.z = ( cellnum % icolumns ) * iuvcellwidth;
			_echoUV.vec4Val.w = 1.0f - ( cellnum / icolumns ) * iuvcellheight;
		}

	}

//$=============================================================================
//@ Sets the mix value for shaders that have _echoMix property
//@
//@ Parameters:
//@
//# imix        - Mix amount (purpose varys with shader-see shader docs)
//&-----------------------------------------------------------------------------
	public void ShaderSet_echoMix ( float imix )
	{
		if ( _echoMix == null )
			return;

		_echoMix.floatVal		= imix;
	}

//$=============================================================================
//@ Sets the Hit Mix value for shaders that have _echoHitMix0-3 property
//@
//@ Parameters:
//@
//# ihitid       - Hit number to use (can be 0-3 in shield shader)
//# imix         - Mix amount (purpose varys with shader-see shader docs)
//&-----------------------------------------------------------------------------
	public void ShaderSet_echoHitMix ( int ihitid, float imix )
	{
		if ( _echoHitMix0 == null )
			return;

		switch ( ihitid )
		{
			case 0:
				_echoHitMix0.floatVal	= imix;
				break;

			case 1:
				_echoHitMix1.floatVal	= imix;
				break;

			case 2:
				_echoHitMix2.floatVal	= imix;
				break;

			case 3:
				_echoHitMix3.floatVal	= imix;
				break;

			default:
				break;
		}
	}

//$=============================================================================
//@ Sets _hitVector0-3 property and turns on Hit Mode
//@
//@ NOTE: This is only used on Shield Shader right now.
//@
//@ Parameters:
//@
//# ivec   	   - Direction of the hit effect
//# ihitnum    - 0-3
//&-----------------------------------------------------------------------------
	public void ShaderSet_echoHitVectorOn ( Vector3 ivec, int ihitnum )
	{
		if ( _echoHitVector0 == null )
			return;
		
		ivec = renderer.worldToLocalMatrix * ivec;
		
		switch ( ihitnum )
		{
			case 0:
				_echoHitVector0.vec4Val.x		= ivec.x; 
				_echoHitVector0.vec4Val.y		= ivec.y; 
				_echoHitVector0.vec4Val.z		= ivec.z; 
				_echoHitVector0.vec4Val.w		= 1.0f; 
				break;

			case 1:
				_echoHitVector1.vec4Val.x		= ivec.x; 
				_echoHitVector1.vec4Val.y		= ivec.y; 
				_echoHitVector1.vec4Val.z		= ivec.z; 
				_echoHitVector1.vec4Val.w		= 1.0f; 
				break;

			case 2:
				_echoHitVector2.vec4Val.x		= ivec.x; 
				_echoHitVector2.vec4Val.y		= ivec.y; 
				_echoHitVector2.vec4Val.z		= ivec.z; 
				_echoHitVector2.vec4Val.w		= 1.0f; 
				break;

			case 3:
				_echoHitVector3.vec4Val.x		= ivec.x; 
				_echoHitVector3.vec4Val.y		= ivec.y; 
				_echoHitVector3.vec4Val.z		= ivec.z; 
				_echoHitVector3.vec4Val.w		= 1.0f; 
				break;

			default:
				break;
		}
	}

//$=============================================================================
//@ Turns off a HitVector
//@
//@ NOTE: This is only used on Shield Shader right now.
//@
//@ Parameters:
//@
//# ihitnum    - 0-3
//&-----------------------------------------------------------------------------
	public void ShaderSet_echoHitVectorOff ( int ihitnum )
	{
		if ( _echoHitVector0 == null )
			return;

		switch ( ihitnum )
		{
			case 0:
				_echoHitVector0.vec4Val.x		= 0.0f; 
				_echoHitVector0.vec4Val.y		= 0.0f; 
				_echoHitVector0.vec4Val.z		= 0.0f; 
				_echoHitVector0.vec4Val.w		= 0.0f; 
				break;

			case 1:
				_echoHitVector1.vec4Val.x		= 0.0f; 
				_echoHitVector1.vec4Val.y		= 0.0f; 
				_echoHitVector1.vec4Val.z		= 0.0f; 
				_echoHitVector1.vec4Val.w		= 0.0f; 
				break;

			case 2:
				_echoHitVector2.vec4Val.x		= 0.0f; 
				_echoHitVector2.vec4Val.y		= 0.0f; 
				_echoHitVector2.vec4Val.z		= 0.0f; 
				_echoHitVector2.vec4Val.w		= 0.0f; 
				break;

			case 3:
				_echoHitVector3.vec4Val.x		= 0.0f; 
				_echoHitVector3.vec4Val.y		= 0.0f; 
				_echoHitVector3.vec4Val.z		= 0.0f; 
				_echoHitVector3.vec4Val.w		= 0.0f; 
				break;

			default:
				break;
		}
	}
}

