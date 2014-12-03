using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EventType for shader effects
public enum EchoET
{
	SCROLL_ECHOUV,
	SCROLL_SECTION_ECHOUV,
	CELLANIM_ECHOUV,
	CELLANIM_UVSET,
	CELLANIM_BLEND,
	RANDOM_ECHOUV,
	ANIMATE_TILING,
	SHIELD_ON,
	SHIELD_OFF,
	SHIELD_DOWN,
	SHIELD_HIT,
	FUSION,
	ECHORGBA,	
	ECHOALPHA,
	ECHOSCALE,
	DISSOLVE,
	RANDOM_MIX,
	BLINK_ECHOUV,
	BLINK_UVSET,
	SHAKE_OBJECT,
	CUSTOM,
};

public enum EchoFilter
{
	LINEAR,
	SINE,
	SQUARED,
	CUBED,
	SQRT,
};

//$-----------------------------------------------------------------------------
//@ Controls the effect events and requires the EchoFXEventManager prefab to be in scene.
//&-----------------------------------------------------------------------------
public class EchoFXEvent : EchoLinkedList
{
	private static int      _ticks 							= 0;
	public static int 		poolListID = -1;  // make one of this for each class extended from EchoLinkedList
	static float 			fxPer;       // percent done
	static float 			fxRatio;     // ratio of effect 0-1
	static float 			fxNum1;
	static float 			fxNum2;
	static EchoGameObject	listFirst;
	static EchoGameObject	listLast;
	public EchoET 			eventType;
	public EchoGameObject	ego;
	float                   fxStartDelay;
	int 					fxStage;
	Vector4 				fxUVAdd;
	Vector4 				fxUVBase;
	float 					fxTime;
	float 					fxDuration;
	float 					fxArg1;
	float 					fxArg2;
	float 					fxArg3;
	int 					fxIArg1;
	int 					fxIArg2;
	Transform      			fxTransform;
  	Vector3         		fxPosHold;
  	Vector4         		fxScale1;
  	Vector4         		fxScale2;
	int             		fxShieldHitID;
	Vector4                 fxRGBA1;
	Vector4                 fxRGBA2;
	Vector4                 fxUV1;
	Vector4                 fxUV2;
	Vector2                 fxTile1;
	Vector2                 fxTile2;
	Vector2                 fxTileBase;
	int                     fxAnimReps;
	float                   fxAnimCurCell;
	int                   	fxAnimCurCellOld;
	float                   fxAnimCurCellDir;
	int                     fxAnimCellCount;
	int                     fxAnimCellStart;
	float                   fxAcc;
	bool                   	forceStop;
	bool                   	fxOver;
	float                   fxPeriod;
	float                   fxAmplitude;
	float                   fxOffset;
	int                     fxUse_MainTex_ST;
	EchoFilter              echoFilter = EchoFilter.LINEAR;
	int                     fxTempNum;
	float                   fxSpeed;
	float                   fxSpeedCur;
	float                   fxDamp;
	float                   fxFrameCur;
	float                   fxFrameFirst;
	float                   fxFrameLast;
	int                   	fxDirection;

	public delegate void 		EventStart();
	public event EventStart		fxEventStart;
	public delegate void 		EventDone( bool forcestop );
	public event EventDone 		fxEventDone;
	public delegate bool 		EventCustom ( EchoGameObject iego, float ipercentdone, float iratio, float itime );
	public event EventCustom	fxEventCustom;
	public delegate void 		EventProgress ( float ipercentdone, int stage );
	public event EventProgress 	fxEventProgress;
	private bool                _fxEventProgressFlag;

//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	public EchoFXEvent ()
	{
	    _fxEventProgressFlag	= false;
		fxStartDelay			= 0.0f;
		fxStage					= -1;
		forceStop				= false;
		fxEventDone				= EventDoneNull;
		fxEventCustom			= EventCustomNull;
		fxEventProgress			= EventProgressNull;
		fxEventStart            = EventStartNull;
		fxUVBase				= new Vector4 ( 0.0f, 0.0f, 0.0f, 0.0f );

		// Setup each item for this classes PoolList
		PoolListItemInit ( poolListID );

		// Add to the "not in use" pool list
		PoolAddListInactive ( this );
	}

//--------------------------------------------------------------------------
// Allocate the objects to be used for Events
//--------------------------------------------------------------------------
	public static void PoolAlloc ( int iNumPoolObjs = 32, bool idynamicadd = false )
	{
		if ( poolListID < 0 )
		{
			// Get ID to a new EchoLinkedList pool
			poolListID = PoolListAdd();
			
			if ( idynamicadd )
				NewObjectCallback ( poolListID, DynamicAddObj );
	
			while ( iNumPoolObjs-- > 0 )
			{
				new EchoFXEvent();
			}
		}
	}
	
//--------------------------------------------------------------------------
	public static void DynamicAddObj()
	{
		new EchoFXEvent();
	}

//--------------------------------------------------------------------------
	public static void ProcessAllInUpdate()
	{
		EchoLinkedList epl;
		EchoLinkedList first;
		EchoLinkedList last;
		
		if ( poolListID < 0 )
			return;

		first	= PoolGetFirstActive ( poolListID );
		last	= PoolGetLastActive ( poolListID );

		_ticks++;

		listFirst 	= null;
		
		// loop thru active events 
		for ( epl = first; epl != last; epl = epl.active_next )
		{
			epl.ProcessInUpdate();
		}

		// now process any gameobject submits
		while ( listFirst !=  null )
		{
			listFirst.ShaderPropertiesSubmit();
			listFirst.echoFxFlag = false;
			listFirst = listFirst.echoFxNext;			
		}
	}

//==========================================================================
	private void AddToEgoList()
	{
		if ( ego.echoFxFlag == false )
		{
			ego.echoFxFlag = true;

			if ( listFirst == null )
			{
				listFirst				= ego;
				listFirst.echoFxNext	= null;	
				listLast				= ego;
			}
			else
			{
				listLast.echoFxNext		= ego;
				listLast				= ego;
				listLast.echoFxNext		= null;
			}
		}
	}

//$=============================================================================
//@ Sets UV Events to use MainText_ST UV inplace of _echoUV
//@
//@ NOTE: This lets you use UV events on non echoLogin shaders.
//@ 
//&=============================================================================
	public void Use_MainTex_ST ()
	{
		fxUse_MainTex_ST = 1;
	}

//$=============================================================================
//@ Adds a filter to an existing EchoFXEvent
//@
//@ Parameters:
//@
//# ifilter    - Can be EchoFilter.LINEAR, EchoFilter.SINE, EchoFilter.SQUARED, EchoFilter.CUBED or EchoFilter.SQRT 
//# iperiod    - Period of SINE filter
//# iamplitude - Amplitude of SINE filter
//# ioffset    - Offset of SINE filter
//&=============================================================================
	public void AddFilter ( EchoFilter ifilter,  float iperiod = 0.25f, float iamplitude = 1.0f, float ioffset = 0.0f )
	{
		echoFilter		= ifilter;
		fxAmplitude		= iamplitude;
		fxPeriod		= iperiod;
		fxOffset        = ioffset;
	}

//$=============================================================================
//@ Sets a start delay on an existing EchoFXEvent
//@
//@ Parameters:
//@
//# idelay 		- Delay, in seconds, before the event begins  
//# icallback   - Optional callback that gets called when event first begins
//&=============================================================================
	public void StartDelay ( float idelay, EventStart icallback = null )
	{
		fxStartDelay = idelay;		

		if ( icallback != null )
			fxEventStart = icallback;
	}

//$=============================================================================
//@ Sets a callback that is triggered when an event starts
//@
//@ Parameters:
//@
//# icallback - Method that gets called when the event is done.
//&=============================================================================
	public void SetEventStart ( EventStart icallback )
	{
		fxEventStart = icallback;
	}

//$=============================================================================
//@ Sets a callback that is triggered when an event ends
//@
//@ Parameters:
//@
//# icallback - Method that gets called when the event is done.
//&=============================================================================
	public void SetEventDone ( EventDone icallback )
	{
		fxEventDone = icallback;
	}

//$=============================================================================
//@ Sets a callback that gets triggered each frame that the event is active
//@
//@ Parameters:
//@
//# icallback - Method that will recieve the progress information.
//&=============================================================================
	public void SetEventProgress ( EventProgress icallback )
	{
		fxEventProgress = icallback;
	}

//$=============================================================================
//@ Sets up a custom user event
//@
//@ Parameters:
//@
//# iego		- EchoGameObject that will have this EchoFXEvent
//# itime 	    - Start time 
//# iduration   - Duration of the effect
//# icallback   - Callback that processes this effect
//&=============================================================================
	public static EchoFXEvent StartEventCustom ( EchoGameObject iego, float itime, float iduration, EventCustom icallback )
	{
		EchoFXEvent efx;

		efx		= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;

		if ( efx != null )
		{
			efx.echoFilter		= EchoFilter.LINEAR;
			efx.fxAmplitude		= 1.0f;
			efx.fxPeriod		= 1.0f;
			efx.fxOffset        = 0.0f;
			efx.eventType		= EchoET.CUSTOM;
			efx.ego             = iego;
			efx.fxDuration		= iduration;
			efx.fxTime			= itime;
			efx.fxEventCustom	= icallback;
		}

		return ( efx );
	}

//===========================================================================
	private void EventStartNull()
	{
	}

//===========================================================================
	private void EventDoneNull( bool forcestop )
	{
	}

//===========================================================================
	private void EventProgressNull ( float ipercentdone, int stage )
	{
	}

//===========================================================================
	private bool EventCustomNull ( EchoGameObject iego, float ipercentdone, float iratio, float itime  )
	{
		return ( true );
	}

//$=============================================================================
//@ Changes UV Scroll Speed on CellAnimate_UVSet events
//@
//@ Parameters:
//@
//# ifps	- Positive values are forward, negative reverse , 0 Stops
//&===========================================================================
	public void UVSetSpeed ( float ifps )
	{
		fxSpeed = ifps;	

		if ( Mathf.Approximately ( fxDamp , 0.0f ) )
		{
			fxSpeedCur	= fxSpeed;
		}
	}

//$=============================================================================
//@ Set the dampening effect for UV Scrolling when reverseing or stoping
//@
//@ Parameters:
//@
//# idamp	- percent of speed, 0 == instant 
//@ 
//&===========================================================================
	public void UVSetDamp ( float idamp )
	{
		fxDamp = Mathf.Abs ( fxSpeed ) * idamp;
	}

//$=============================================================================
//@ Change the duration of an event once it has been made/started
//@
//@ Parameters:
//@
//# iduration	- new duration
//@ 
//@ NOTE:  passing zero will end the event
//&===========================================================================
	public void EventDuration ( float iduration )
	{
    	fxDuration = iduration;
	}

//$-----------------------------------------------------------------------------
//@ Performs Cell Animation on an EchoGameObject using pre-allocated UV sets
//@
//@ Parameters:
//@
//# iego  		 	- EchoGameObject that will have this EchoFXEvent
//# istartcell      - uvset index to start animation on
//# icellcount      - Total number of cells\uvsets in this animation
//# itimepercell    - The time to show each cell
//# ireps           - How many times to repeat this animation. 0 = forever
//&-----------------------------------------------------------------------------
	public static EchoFXEvent CellAnimate_UVSet ( EchoGameObject iego, int istartcell, int icellcount, float ifpsspeed = 32.0f, int ireps = 0, float ifxduration = 0.0f )
	{
		EchoFXEvent efx;

		efx	= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;
		
		if ( efx != null )
		{
			efx.fxSpeedCur		= ifpsspeed;

			if ( ifpsspeed < 0 )
				ifpsspeed		= -ifpsspeed;				

			efx.echoFilter				= EchoFilter.LINEAR;
			efx.eventType				= EchoET.CELLANIM_UVSET;
			efx.ego						= iego;
			efx.fxUse_MainTex_ST        = 0;
			efx.fxAnimReps				= ireps;
			efx.fxTime					= 0.0f;  
			efx.fxDuration				= ifxduration;
			efx.fxFrameCur				= istartcell;
			efx.fxFrameFirst			= istartcell;
			efx.fxFrameLast				= istartcell + icellcount -1;
			efx.fxSpeed        			= ifpsspeed;
			efx.fxDamp   				= 0;
		}

		return ( efx );
	}


//$-----------------------------------------------------------------------------
//@ Blinks Randomly between 2 UV sets
//@
//@ Parameters:
//@
//# iego 			- EchoGameObject that will have this EchoFXEvent
//# iuvset1      	- UV set for first frame 
//# iuvset2      	- UV set for second frame 
//# ifxduration 	- Duration of this effect. 0.0f = forever
//&-----------------------------------------------------------------------------
  public static EchoFXEvent Blink_UVSet ( EchoGameObject iego, int iuvset1, int iuvset2, float ifxduration )
	{
		EchoFXEvent efx;

		efx						= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;
		efx.eventType			= EchoET.BLINK_UVSET;
			
		efx.ego					= iego;  
		efx.fxUse_MainTex_ST    = 0;
		efx.fxTime				= 0;
		efx.fxDuration			= ifxduration;

		efx.fxIArg1				= iuvset1;
		efx.fxIArg2				= iuvset2;

		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Animates the UV coordinates of an echoLogin Shader
//@
//@ Parameters:
//@
//# iego  		- EchoGameObject that will have this EchoFXEvent
//# iuvAdd      - Value added to the UV coordinates of each frame 
//# ifxduration - Duration of this effect. 0.0f means forever
//&-----------------------------------------------------------------------------
	public static EchoFXEvent Scroll_echoUV ( EchoGameObject iego, Vector4 iuvAdd, float ifxduration = 0.0f )
	{
		EchoFXEvent efx;

		efx				= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;

		if ( efx != null )
		{
			efx.echoFilter				= EchoFilter.LINEAR;
			efx.eventType				= EchoET.SCROLL_ECHOUV;
			efx.ego						= iego;
			efx.fxUse_MainTex_ST        = 0;
			efx.fxUVAdd					= iuvAdd;
			efx.fxTime					= 0;
			efx.fxDuration				= ifxduration;
			efx.fxAnimCurCellDir        = 1.0f;
		}


		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Scrolls an object's UV from iuvstart to iuvend
//@
//@ Parameters:
//@
//# iego  		- EchoGameObject that will have this EchoFXEvent
//# iuvstart    - UV position on texture to start
//# iuvend      - UV position to end scroll
//# ireps       - Number of repetitions to scroll. 0 = forever
//# ifxduration - Duration of one repetition of a scroll 
//&-----------------------------------------------------------------------------
	public static EchoFXEvent ScrollSection_echoUV ( EchoGameObject iego, Vector4 iuvstart, Vector4 iuvend, int ireps, float ifxduration = 1.0f )
	{
		EchoFXEvent efx;

		efx	= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;
		
		if ( efx != null )
		{
			efx.echoFilter				= EchoFilter.LINEAR;
			efx.eventType				= EchoET.SCROLL_SECTION_ECHOUV;
			efx.ego						= iego;
			efx.fxUse_MainTex_ST        = 0;
			efx.fxUV1					= iuvstart;
			efx.fxUV2					= iuvend;
			efx.fxTime					= 0;
			efx.fxAnimReps				= ireps;
			efx.fxDuration				= ifxduration;
		}

		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Performs Cell Animation on an EchoGameObject
//@
//@ Parameters:
//@
//# iego  		 	- EchoGameObject that will have this EchoFXEvent
//# iuvcellwidth    - Width of the cell in UV space 
//# iuvcellheight   - Height of the cell in UV space
//# icolumns        - Number of cells horizontally
//# icellcount      - Total number of cells in this animation
//# itimepercell    - The time to show each cell
//# ireps           - How many times to repeat this animation. 0 = forever
//&-----------------------------------------------------------------------------
	public static EchoFXEvent CellAnimate_echoUV ( EchoGameObject iego, float iuvcellwidth, float iuvcellheight, int icolumns, int istartcell, int icellcount, float itimepercell, int ireps = 1 )
	{
		EchoFXEvent efx;

		efx	= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;
		
		if ( efx != null )
		{
			efx.echoFilter				= EchoFilter.LINEAR;
			efx.eventType				= EchoET.CELLANIM_ECHOUV;
			efx.ego						= iego;
			efx.fxUse_MainTex_ST        = 0;
			efx.fxArg1					= iuvcellwidth;
			efx.fxArg2					= iuvcellheight;
			efx.fxIArg1					= icolumns;
	//		efx.fxIArg2					= irows;
			efx.fxAnimReps				= ireps;
			efx.fxAnimCellCount			= icellcount;
			efx.fxTime					= itimepercell + 1.0f;  
			efx.fxDuration				= itimepercell;
			efx.fxAnimCurCell			= istartcell;
			efx.fxAnimCurCellDir        = 1;
			efx.fxAnimCellStart			= istartcell;
		}

		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Performs Cell Animation on an EchoGameObject
//@
//@ Parameters:
//@
//# iego  		 	- EchoGameObject that will have this EchoFXEvent
//# iuvcellwidth    - Width of the cell in UV space 
//# iuvcellheight   - Height of the cell in UV space
//# icolumns        - Number of cells horizontally
//# icellcount      - Total number of cells in this animation
//# itimepercell    - The time to show each cell
//# ireps           - How many times to repeat this animation. 0 = forever
//&-----------------------------------------------------------------------------
	public static EchoFXEvent CellAnimateBlend_echoUV ( EchoGameObject iego, float iuvcellwidth, float iuvcellheight, int icolumns, int istartcell, int icellcount, float itimepercell, int ireps = 1 )
	{
		EchoFXEvent efx;

		efx	= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;
		
		if ( efx != null )
		{
			efx.echoFilter				= EchoFilter.LINEAR;
			efx.eventType				= EchoET.CELLANIM_BLEND;
			efx.ego						= iego;
			efx.fxUse_MainTex_ST        = 0;
			efx.fxArg1					= iuvcellwidth;
			efx.fxArg2					= iuvcellheight;
			efx.fxIArg1					= icolumns;
	//		efx.fxIArg2					= irows;
			efx.fxAnimReps				= ireps;
			efx.fxAnimCellCount			= icellcount;
			efx.fxTime					= itimepercell + 1.0f;  
			efx.fxDuration				= itimepercell;
			efx.fxAnimCurCell			= istartcell;
			efx.fxAnimCurCellDir        = 1;
			efx.fxAnimCellStart			= istartcell;
		}

		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Blinks Randomly between 2 UV sets
//@
//@ Parameters:
//@
//# iego 			- EchoGameObject that will have this EchoFXEvent
//# iuvoffset1      - UV offset from UV origin for first frame 
//# iuvoffset2      - UV offset from UV origin for second frame 
//# ifxduration 	- Duration of this effect. 0.0f = forever
//&-----------------------------------------------------------------------------
  public static EchoFXEvent Blink_echoUV ( EchoGameObject iego, Vector4 iuvoffset1, Vector4 iuvoffset2, float ifxduration )
	{
		EchoFXEvent efx;

		efx						= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;
		efx.eventType			= EchoET.BLINK_ECHOUV;
			
		efx.ego					= iego;  
		efx.fxUse_MainTex_ST    = 0;
		efx.fxTime				= 0;
		efx.fxDuration			= ifxduration;

		efx.fxUV1				= iuvoffset1;
		efx.fxUV2				= iuvoffset2;

		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Generates Random UV coordinates
//@
//@ Parameters:
//@
//# iego  		 	- EchoGameObject that will have this EchoFXEvent
//# ifxduration 	- Duration of this effect. 0.0f = forever
//&-----------------------------------------------------------------------------
	public static EchoFXEvent Random_echoUV ( EchoGameObject iego, float ifxduration = 0.0f )
	{
		EchoFXEvent efx;

		efx	= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;
		
		if ( efx != null )
		{
			efx.echoFilter				= EchoFilter.LINEAR;
			efx.eventType				= EchoET.RANDOM_ECHOUV;
			efx.ego						= iego;
			efx.fxUse_MainTex_ST        = 0;
			efx.fxUVAdd					= new Vector4 ( 0, 0, 0, 0 );
			efx.fxTime					= 0;
			efx.fxDuration				= ifxduration;
		}
		
		return ( efx );
	}


//$-----------------------------------------------------------------------------
//@ Animates Shader Tiling values from itilestart to itileend
//@
//@ Parameters:
//@
//# iego  		 	- EchoGameObject that will have this EchoFXEvent
//# itilestart      - Tiling Start value
//# itileend      	- Tiling End value
//# ifxduration 	- Duration of this effect. 0.0f = forever
//&-----------------------------------------------------------------------------
	public static EchoFXEvent Animate_ShaderTiling ( EchoGameObject iego, Vector2 itilestart, Vector2 itileend, float ifxduration = 0.0f )
	{
		EchoFXEvent efx;

		efx				= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;

		if ( efx != null )
		{
			efx.echoFilter		= EchoFilter.LINEAR;
			efx.eventType		= EchoET.ANIMATE_TILING;
			efx.ego				= iego;
			efx.fxTile1			= itilestart;
			efx.fxTile2			= itileend;
			efx.fxTime			= 0;
			efx.fxDuration		= ifxduration;
		}

		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Animates Scale of Object using a shader that has the _echoScale property
//@
//@ Parameters:
//@
//# iego  		 	- EchoGameObject that will have this EchoFXEvent
//# iscalestart     - Scaling Start value
//# iscaleend      	- Scaling End value
//# ifxduration 	- Duration of this effect. 0.0f = forever
//&-----------------------------------------------------------------------------
	public static EchoFXEvent Animate_echoScale ( EchoGameObject iego, Vector3 iscalestart, Vector3 iscaleend, float ifxduration = 0.0f )
	{
		EchoFXEvent efx;

		efx	= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;

		if ( efx != null )
		{
			efx.echoFilter		= EchoFilter.LINEAR;
			efx.eventType		= EchoET.ECHOSCALE;
			efx.ego				= iego;
			
			efx.fxScale1.x		= iscalestart.x;
			efx.fxScale1.y		= iscalestart.y;
			efx.fxScale1.z		= iscalestart.z;
			efx.fxScale1.w		= 1.0f;
			
			efx.fxScale2.x		= iscaleend.x;
			efx.fxScale2.y		= iscaleend.y;
			efx.fxScale2.z		= iscaleend.z;
			efx.fxScale2.w		= 1.0f;
			
			efx.fxTime			= 0;
			efx.fxDuration		= ifxduration;
		}

		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Animates Alpha of Object using a shader that has the _echoAlpha property
//@
//@ Parameters:
//@
//# iego  		 	- EchoGameObject that will have this EchoFXEvent
//# ialphastart     - Alpha Start value
//# ialphaend      	- Alpha End value
//# ifxduration 	- Duration of this effect. 0.0f = forever
//&-----------------------------------------------------------------------------
	public static EchoFXEvent Animate_echoAlpha ( EchoGameObject iego, float ialphastart, float ialphaend, float ifxduration = 0.0f )
	{
		EchoFXEvent efx;

		efx	= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;

		if ( efx != null )
		{
			efx.echoFilter		= EchoFilter.LINEAR;
			efx.eventType		= EchoET.ECHOALPHA;
			efx.ego				= iego;
			efx.fxArg1			= ialphastart;
			efx.fxArg2			= ialphaend;
			efx.fxTime			= 0;
			efx.fxDuration		= ifxduration;
		}

		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Animates RGBA of Object using a shader that has the _echoRGBA property
//@
//@ Parameters:
//@
//# iego  		 	- EchoGameObject that will have this EchoFXEvent
//# ialphastart     - Alpha Start value
//# ialphaend      	- Alpha End value
//# ifxduration 	- Duration of this effect. 0.0f = forever
//&-----------------------------------------------------------------------------
	public static EchoFXEvent Animate_echoRGBA ( EchoGameObject iego, Vector4 irgba1, Vector4 irgba2, float ifxduration = 1.0f )
	{
		EchoFXEvent efx;

		efx				= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;

		if ( efx != null )
		{
			efx.echoFilter	= EchoFilter.LINEAR;
			efx.eventType	= EchoET.ECHORGBA;
			efx.ego			= iego;
			efx.fxTime      = 0;
			efx.fxDuration  = ifxduration;
			efx.fxRGBA1     = irgba1;
			efx.fxRGBA2     = irgba2;
		}

		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Starts a Shield Hit effect on the EchoLogin Shield Shader
//@
//@ Parameters:
//@
//# iego  		 	- EchoGameObject that will have this EchoFXEvent
//# ihitid          - 0-3 id of the hit of which there can be up to 4 at one time
//# idirection      - Direction vector of the hit
//# iminalpha       - Minimum alpha of the hit effect
//# imaxalpha       - Maximum alpha of the hit effect
//# ifxduration 	- Duration of this effect. 0.0f = forever
//&-----------------------------------------------------------------------------
	public static EchoFXEvent ShieldHit_echoShader ( EchoGameObject iego, int ihitid, Vector3 idirection, float iminalpha, float imaxalpha, float ifxduration = 0.0f )
	{
		EchoFXEvent efx;

		efx		= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;

		if ( efx != null )
		{
			efx.echoFilter			= EchoFilter.LINEAR;
			efx.eventType			= EchoET.SHIELD_HIT;
			efx.ego					= iego;
			efx.fxArg1				= iminalpha;
			efx.fxArg2				= imaxalpha;
			efx.fxTime				= 0;
			efx.fxDuration			= ifxduration;
			efx.fxShieldHitID 		= ihitid;

			efx.ego.ShaderSet_echoHitVectorOn ( idirection, efx.fxShieldHitID );
		}


		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Starts a Shield Turning On type effect on the EchoLogin Shield Shader
//@
//@ Parameters:
//@
//# iego  		 	- EchoGameObject that will have this EchoFXEvent
//# imaxalpha       - Maximum alpha of the hit effect
//# ifxduration 	- Duration of this effect. 0.0f = forever
//&-----------------------------------------------------------------------------
	public static EchoFXEvent ShieldOn_echoShader ( EchoGameObject iego,  float imaxalpha, float ifxduration = 1.0f )
	{
		EchoFXEvent efx;

		efx				= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;

		if ( efx != null )
		{
			iego.EchoActive ( true );
			efx.echoFilter		= EchoFilter.LINEAR;
			efx.eventType		= EchoET.SHIELD_ON;
			efx.ego				= iego;

			efx.fxStage			= 0;
			efx.fxTime			= 0;
			efx.fxDuration		= ifxduration;
			efx.fxArg1			= imaxalpha;
		}

		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Starts a Shield Turning Off type effect on the EchoLogin Shield Shader
//@
//@ Parameters:
//@
//# iego  		 	- EchoGameObject that will have this EchoFXEvent
//# imaxalpha       - Maximum alpha of the hit effect
//# ifxduration 	- Duration of this effect. 0.0f = forever
//&-----------------------------------------------------------------------------
	public static EchoFXEvent ShieldOff_echoShader ( EchoGameObject iego, float imaxalpha, float ifxduration = 1.0f )
	{
		EchoFXEvent efx;

		efx		= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;

		if ( efx != null )
		{
			efx.echoFilter		= EchoFilter.LINEAR;
			efx.eventType		= EchoET.SHIELD_OFF;
			efx.ego				= iego;

			efx.fxStage			= 0;
			efx.fxTime			= 0;
			efx.fxDuration		= ifxduration;
			efx.fxArg1			= imaxalpha;
		}

		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Starts a Shield Down/Destroyed type effect on the EchoLogin Shield Shader
//@
//@ Parameters:
//@
//# iego  		 	- EchoGameObject that will have this EchoFXEvent
//# imaxalpha       - Maximum alpha of the hit effect
//# ifxduration 	- Duration of this effect. 0.0f = forever
//&-----------------------------------------------------------------------------
	public static EchoFXEvent ShieldDown_echoShader ( EchoGameObject iego, float imaxalpha, float ifxduration = 2.0f )
	{
		EchoFXEvent efx;

		efx								= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;

		if ( efx != null )
		{
			efx.echoFilter		= EchoFilter.LINEAR;
			efx.eventType		= EchoET.SHIELD_DOWN;
			efx.ego				= iego;

			efx.fxStage			= 0;
			efx.fxTime			= 0;
			efx.fxDuration		= ifxduration;
			efx.fxArg1			= imaxalpha;
		}

		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Starts a Fusion effect for the EchoLogin Fusion Shader
//@
//@ Parameters:
//@
//# iego  		 	- EchoGameObject that will have this EchoFXEvent
//# istart          - Start size of effect
//# iend          	- End size of effect
//# ijitter         - Amount of random jitters while effect is active
//# ifxduration 	- Duration of this effect. 0.0f = forever
//&-----------------------------------------------------------------------------
	public static EchoFXEvent Fusion_echoShader ( EchoGameObject iego, float istart, float iend, float ijitter, float ifxduration = 1.0f )
	{
		EchoFXEvent efx;

		efx				= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;

		if ( efx != null )
		{
			efx.eventType	= EchoET.FUSION;
			efx.ego      	= iego;
			efx.fxArg1    	= istart;
			efx.fxArg2    	= iend;
			efx.fxArg3    	= ijitter;
			efx.fxTime      = 0;
			efx.fxDuration  = ifxduration;
		}

		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Sets Random mix values for shaders that have the _echoMix property
//@
//@ Parameters:
//@
//# iego  		 	- EchoGameObject that will have this EchoFXEvent
//# ifxduration 	- Duration of this effect. 0.0f = forever
//&-----------------------------------------------------------------------------
	public static EchoFXEvent Random_echoMix ( EchoGameObject iego, float ifxduration = 0.0f )
	{
		EchoFXEvent efx;

		efx				= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;
		efx.eventType	= EchoET.RANDOM_MIX;
		efx.ego			= iego;

		efx.fxTime      = 0;
		efx.fxDuration  = ifxduration;
		efx.fxArg1      = 0.0f;
		efx.fxArg2      = 1.0f;

		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Starts Dissolve effect for any echoLogin Dissolve Shaders
//@
//@ Parameters:
//@
//# iego  		 	- EchoGameObject that will have this EchoFXEvent
//# imix1           - Start of dissolve (-0.5 is all Off)
//# imix2           - Start of dissolve ( 1.5 is all On )
//# ifxduration 	- Duration of this effect. 0.0f = forever
//&-----------------------------------------------------------------------------
	public static EchoFXEvent Dissolve_echoShader ( EchoGameObject iego, float imix1 = -0.5f, float imix2 = 1.5f, float ifxduration = 1.0f )
	{
		EchoFXEvent efx;

		efx				= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;
		efx.eventType	= EchoET.DISSOLVE;
		efx.ego			= iego;

		efx.fxTime      = 0;
		efx.fxDuration  = ifxduration;
		efx.fxArg1      = imix1;
		efx.fxArg2      = imix2;

		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Starts shaking event on a gameObject 
//@
//@ Parameters:
//@
//# itramsform      - Transform of this GameObject
//# istrength       - Amount to shake object
//# ifxduration 	- Duration of this effect. 0.0f = forever
//&-----------------------------------------------------------------------------
	public static EchoFXEvent ShakeGameObject ( Transform itransform, float istrength, float ifxduration )
	{
		EchoFXEvent efx;

		efx				= PoolGetFreeInactive ( poolListID ) as EchoFXEvent;
		efx.eventType	= EchoET.SHAKE_OBJECT;

		efx.fxTime      = 0;
		efx.fxDuration  = ifxduration;

		efx.fxTransform = itransform;
		efx.fxPosHold   = itransform.position;
		efx.fxArg1      = istrength;

		return ( efx );
	}

//$-----------------------------------------------------------------------------
//@ Stops the specified event
//@
//@ Parameters:
//@
//# iefx  		 - EchoFXEvent reference of event
//&-----------------------------------------------------------------------------
	public static void StopEvent ( EchoFXEvent iefx )
	{
		if ( iefx != null )
			iefx.forceStop = true;
	}

//$-----------------------------------------------------------------------------
//@ Stops All the active events 
//&-----------------------------------------------------------------------------
	public static void StopAllEvents ( )
	{
		EchoLinkedList 	ell;
		EchoFXEvent 	efx;
		EchoLinkedList 	first;
		EchoLinkedList 	last;
		
		if ( poolListID < 0 )
			return;

		first	= PoolGetFirstActive ( poolListID );
		last	= PoolGetLastActive ( poolListID );

		// loop thru active events and move them to inactive
		for ( ell = first; ell != last; ell = efx.active_next )
		{
			efx = ell as EchoFXEvent;
			efx.forceStop = true;
		}
	}

//===========================================================================
	private void ProcessAnimation()
	{
		if ( !Mathf.Approximately ( fxSpeed, fxSpeedCur ) )
		{
			if ( fxSpeedCur > fxSpeed )
			{
				fxSpeedCur -= ( fxDamp * Time.deltaTime );

				if ( fxSpeedCur < fxSpeed )
					fxSpeedCur = fxSpeed;

			}
			else if ( fxSpeedCur < fxSpeed )
			{
				fxSpeedCur += ( fxDamp * Time.deltaTime  );

				if ( fxSpeedCur > fxSpeed )
					fxSpeedCur = fxSpeed;
			}
		}

		fxFrameCur += ( fxSpeedCur * Time.deltaTime ) ;

		if ( fxFrameCur > fxFrameLast )
		{
			fxFrameCur = fxFrameFirst + ( fxFrameCur - fxFrameLast + 1.0f );

			if ( fxAnimReps > 0 )
			{
				fxAnimReps--;
				if ( fxAnimReps <= 0 )
				{
					fxOver	= true;
				}
			}
		}
		else if ( fxFrameCur < fxFrameFirst )
		{
			fxFrameCur = fxFrameLast - ( fxFrameFirst - fxFrameCur );

			if ( fxAnimReps > 0 )
			{
				fxAnimReps--;
				if ( fxAnimReps <= 0 )
				{
					fxOver	= true;
				}
			}
		}
	}

//===========================================================================
	public override void ProcessInUpdate()
	{
		if ( fxStartDelay > 0.0 )
		{
			fxStartDelay -= Time.deltaTime;

			if ( fxStartDelay < 0.0 )
			{
				fxEventStart();
				fxEventStart = EventStartNull;
			}
			else
				return;
		}

		fxTime += Time.deltaTime;

		fxPer = 0.0f;

		if ( forceStop )
			fxPer				= 1.0f;

		if ( fxDuration > 0.0f )
		{
			fxPer = fxTime / fxDuration;	
			
			if ( forceStop || fxPer > 1.0f )
				fxPer				= 1.0f;

			switch ( echoFilter )
			{
				case EchoFilter.SINE:
					fxRatio = Mathf.Sin ( fxOffset + ( Mathf.PI * 2.0f * fxPer * fxPeriod ) ) * fxAmplitude;
					break;

				case EchoFilter.SQUARED:
					fxRatio = fxPer * fxPer;
					break;

				case EchoFilter.CUBED:
					fxRatio = fxPer * fxPer * fxPer;
					break;

				case EchoFilter.SQRT:
					fxRatio = Mathf.Sqrt ( fxPer );
					fxRatio = Mathf.Sqrt ( fxRatio );
					fxRatio = Mathf.Sqrt ( fxRatio );
					break;

				default:
					fxRatio = fxPer;
					break;
			}
		}


		if ( _fxEventProgressFlag )
			fxEventProgress ( fxPer, fxStage );

		switch ( eventType )
		{
			case EchoET.BLINK_UVSET:
				if ( fxTime < fxDuration )
                {
                  	if ( _ticks % 3 == 0 )
                  	{
                        if ( Random.Range ( 0, 100 ) < 50 )
                        {
							ego.UVSet ( fxIArg1 );
                        }
                        else
                        {
							ego.UVSet ( fxIArg2 );
                        }
                    }
                }
                else
                {
					ego.UVSet ( fxIArg2 );
					fxOver = true;
        	    }
				break;

			case EchoET.CELLANIM_UVSET:
				ego.UVSet ( (int)fxFrameCur );

				ProcessAnimation();

				if ( fxPer >= 1.0f )
				{
					fxOver	= true;
				}
				break;

			case EchoET.BLINK_ECHOUV:
				if ( fxTime < fxDuration )
                {
                  	if ( _ticks % 3 == 0 )
                  	{
                        if ( Random.Range ( 0, 100 ) < 50 )
                        {
							ego.ShaderSet_echoUV ( fxUV1, fxUse_MainTex_ST );
                        }
                        else
                        {
							ego.ShaderSet_echoUV ( fxUV2, fxUse_MainTex_ST );
                        }
                    }
                }
                else
                {
					ego.ShaderSet_echoUV ( fxUV2, fxUse_MainTex_ST );
					fxOver = true;
        	    }

				AddToEgoList();
				break;


			case EchoET.SHAKE_OBJECT:
				if ( fxPer <= 1.0 )
                {
					fxArg2 = Mathf.Lerp ( 0.0f, fxArg1, fxRatio );

		            fxTransform.position =  new Vector3 ( fxPosHold.x + Random.Range ( -fxArg2, fxArg2 ), fxPosHold.y + Random.Range ( -fxArg2, fxArg2 ), fxPosHold.z + Random.Range ( -fxArg2, fxArg2 ) );
                }
				else
                {
		            fxTransform.position		=  fxPosHold;
					fxOver						= true;
                }
              	break;

			case EchoET.RANDOM_MIX:
				if ( _ticks % 3 == 0 )
				{
					fxNum1 = Mathf.Lerp ( fxArg1, fxArg2, Random.Range ( fxArg1, fxArg2 ) );

					ego.ShaderSet_echoMix ( fxNum1 );
				}

				if ( fxPer >= 1.0f )
				{
					fxOver	= true;
				}

				AddToEgoList();
				break;			

			case EchoET.DISSOLVE:
				fxNum1 = Mathf.Lerp ( fxArg1, fxArg2, fxPer );

				ego.ShaderSet_echoMix ( fxNum1 );

				if ( fxPer >= 1.0f )
				{
					fxOver	= true;
				}

				AddToEgoList();
				break;			

			case EchoET.SCROLL_ECHOUV:
				if ( fxAnimCurCellDir < 0 )
					fxUVBase -= fxUVAdd * Time.deltaTime;

				if ( fxAnimCurCellDir > 0 )
					fxUVBase += fxUVAdd * Time.deltaTime;

				fxUVBase = ego.ShaderSet_echoUV ( fxUVBase, fxUse_MainTex_ST );

				if ( fxPer >= 1.0f )
				{
					fxOver	= true;
				}

				AddToEgoList();
				break;			

			case EchoET.SCROLL_SECTION_ECHOUV:
				fxUVBase = Vector4.Lerp ( fxUV1, fxUV2, fxPer );

				fxUVBase = ego.ShaderSet_echoUV ( fxUVBase, fxUse_MainTex_ST );

				if ( fxPer >= 1.0f )
				{
					fxTime = 0.0f;

					if ( fxAnimReps > 0 )
					{
						fxAnimReps--;
						if ( fxAnimReps <= 0 )
						{
							fxOver	= true;
						}
					}
				}

				AddToEgoList();
				break;			

			case EchoET.CELLANIM_ECHOUV:
				if ( fxPer >= 1.0 )
				{
					fxNum1 = ( (int)fxAnimCurCell % fxIArg1 ) * fxArg1;
					fxNum2 = 1.0f - ( (int)fxAnimCurCell / fxIArg1 ) * fxArg2;

					fxUVBase.x = fxNum1;
					fxUVBase.y = fxNum2;
					fxUVBase.z = fxNum1;
					fxUVBase.w = fxNum2;

					ego.ShaderSet_echoUV ( fxUVBase, fxUse_MainTex_ST );

					fxTime = 0.0f;
					fxAnimCurCell++;

					if ( fxAnimCurCell >= fxAnimCellCount )
					{
 						fxAnimCurCell = fxAnimCellStart;

						if ( fxAnimReps > 0 )
						{
							fxAnimReps--;
							if ( fxAnimReps <= 0 )
							{
								fxOver	= true;
							}
						}
					}

					AddToEgoList();
				}
				break;

			case EchoET.CELLANIM_BLEND:
				if ( fxPer >= 1.0 )
				{
					ego.ShaderSet_echoMix ( 0 );

					fxNum1 = ( (int)fxAnimCurCell % fxIArg1 ) * fxArg1;
					fxNum2 = 1.0f - ( (int)fxAnimCurCell / fxIArg1 ) * fxArg2;

					fxUVBase.x = fxNum1;
					fxUVBase.y = fxNum2;

					fxTempNum = (int)fxAnimCurCell + 1;
					if ( fxTempNum >= fxAnimCellCount )
 						fxTempNum = fxAnimCellStart;

					fxNum1 = ( fxTempNum % fxIArg1 ) * fxArg1;
					fxNum2 = 1.0f - ( fxTempNum / fxIArg1 ) * fxArg2;

					fxUVBase.z = fxNum1;
					fxUVBase.w = fxNum2;

					ego.ShaderSet_echoUV ( fxUVBase, fxUse_MainTex_ST );

					fxTime = 0.0f;
					fxAnimCurCell++;

					if ( fxAnimCurCell >= fxAnimCellCount )
					{
 						fxAnimCurCell = fxAnimCellStart;

						if ( fxAnimReps > 0 )
						{
							fxAnimReps--;
							if ( fxAnimReps <= 0 )
							{
								fxOver	= true;
							}
						}
					}

				}
				else
				{
					ego.ShaderSet_echoMix ( fxPer );
				}

				AddToEgoList();
				break;

			case EchoET.RANDOM_ECHOUV:
				fxUVAdd.x = Random.Range ( 0.0f, 1.0f );
				fxUVAdd.y = Random.Range ( 0.0f, 1.0f );
				fxUVAdd.z = Random.Range ( 0.0f, 1.0f );
				fxUVAdd.w = Random.Range ( 0.0f, 1.0f );

				ego.ShaderSet_echoUV ( fxUVAdd, fxUse_MainTex_ST );

				if ( fxPer >= 1.0f )
				{
					fxOver						= true;
				}

				AddToEgoList();
				break;			

			case EchoET.ANIMATE_TILING:
				if ( fxPer >= 1.0 )
				{
					fxTileBase = new Vector2( 1.0f, 1.0f );				
					ego.ShaderSetTiling ( fxTileBase );
					fxOver = true;
				}
				else
				{
					fxTileBase = Vector2.Lerp ( fxTile1, fxTile2, fxRatio );				
					ego.ShaderSetTiling ( fxTileBase );
				}

				AddToEgoList();
				break;

			case EchoET.SHIELD_HIT:
				ego.renderer.enabled 	= true;
			
				fxNum1 = Mathf.Lerp ( 0.15f, 0.25f, Random.Range ( 0.0f, 1.0f ) );
				fxNum2 = Random.Range ( fxArg1, fxArg2 );

				ego.ShaderSet_echoHitMix ( fxShieldHitID, fxNum1 );
				ego.ShaderSet_echoAlpha ( fxNum2 );

				// turn hit off
				if ( fxPer >= 1.0f )
				{
					ego.ShaderSet_echoHitMix ( fxShieldHitID, 0.0f );
					ego.ShaderSet_echoAlpha ( 0.0f );
					ego.ShaderSet_echoHitVectorOff ( fxShieldHitID );
					ego.renderer.enabled = false;

					fxOver						= true;
				}

				AddToEgoList();
				break;			

			case EchoET.SHIELD_ON:
				ego.renderer.enabled 	= true;

				if ( fxPer >= 1.0f )
				{
					ego.ShaderSet_echoAlpha ( 0.0f );
					ego.renderer.enabled	= false;
					fxOver						= true;
				}
				else
				{
					fxNum1 = Mathf.Sin ( fxPer * Mathf.PI ) * fxArg1;

					ego.ShaderSet_echoAlpha ( fxNum1  );
				}

				AddToEgoList();
				break;			

			case EchoET.SHIELD_OFF:
				ego.renderer.enabled 	= true;

				switch ( fxStage )
				{
					case 0:
					case 1:
					case 2:
						fxPer = fxTime / ( fxDuration * 0.2f );				

						if ( fxPer >= 1.0f )
						{
							fxTime = 0.0f;
							fxStage++;
						}
						else
						{
							ego.ShaderSet_echoAlpha ( Mathf.PingPong ( fxPer, fxArg1 ) );
						}
						break;

					case 3:
						fxPer = fxTime / fxDuration;				

						if ( fxPer >= 1.0f )
						{
							ego.ShaderSet_echoAlpha (  0.0f );
							ego.renderer.enabled		= false;
							ego.EchoActive ( false );
							fxOver						= true;
							fxStage						= -1;
						}
						else
						{
							ego.ShaderSet_echoAlpha ( Mathf.Sin ( fxPer * Mathf.PI ) * fxArg1 );
						}
						break;
				}

				AddToEgoList();
				break;			

			case EchoET.SHIELD_DOWN:
				ego.renderer.enabled 	= true;

				if ( fxPer >= 1.0f )
				{
					ego.ShaderSet_echoAlpha ( 0.0f );
					ego.EchoActive ( false );
					ego.renderer.enabled		= false;
					fxOver						= true;
					fxStage						= -1;
				}
				else
				{
					if ( Random.Range ( 0, 100 ) < 40 && fxStage == 0 )
						ego.ShaderSet_echoAlpha ( Mathf.Sin ( fxPer * Mathf.PI ) * fxArg1 );
					else
					{
						if ( fxStage == 0 )
							ego.ShaderSet_echoAlpha (  0.0f );

						fxStage = ( fxStage + 1 ) % 2;							
					}
				}

				AddToEgoList();
				break;			

			case EchoET.FUSION:
				ego.ShaderSet_echoHitMix ( 0, Mathf.Lerp ( fxArg1, fxArg2, fxRatio + Random.Range ( 0.0f, fxArg3 ) ) );

				if ( fxPer >= 1.0f )
				{
					fxOver = true;
				}

				AddToEgoList();
				break;

			case EchoET.ECHORGBA:
				ego.ShaderSet_echoRGBA ( Vector4.Lerp ( fxRGBA1, fxRGBA2, fxRatio ) );

				if ( fxPer >= 1.0f )
				{
					fxOver = true;
				}

				AddToEgoList();
				break;			

			case EchoET.ECHOSCALE:
				ego.ShaderSet_echoScale ( Vector4.Lerp ( fxScale1, fxScale2, fxRatio ) );

				if ( fxPer >= 1.0f )
				{
					fxOver = true;
				}

				AddToEgoList();
				break;			


			case EchoET.ECHOALPHA:
				ego.ShaderSet_echoAlpha ( Mathf.Lerp ( fxArg1, fxArg2, fxRatio ) );

				if ( fxPer >= 1.0f )
				{
					fxOver = true;
				}

				AddToEgoList();
				break;			

			case EchoET.CUSTOM:
				fxOver = fxEventCustom ( ego, fxPer, fxRatio, fxTime );

				if ( fxOver || forceStop )
					fxEventCustom = EventCustomNull;

				if ( ego != null )
					AddToEgoList();
				break;

		}

		if ( forceStop || fxOver )
		{
			PoolMoveActive2Inactive ( this );
			fxEventDone(forceStop);
			fxEventDone			= EventDoneNull;
			forceStop			= false;	
			fxOver				= false;		
		}
	}
}

