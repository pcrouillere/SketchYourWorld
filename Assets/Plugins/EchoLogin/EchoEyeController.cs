using UnityEngine;
using System.Collections;

//$-----------------------------------------------------------------------------
//@ Class for UVSet eye movement
//@
//@ Usage
//@
//@ 1. Make an eye mesh with pupil centered on texture and mesh
//@ 2. Drag this script to the eye GameObject
//@ 3. Set UV Set Size, Uv Padding and move speed in inspector
//@ 4. Add EchoHeadController to the GameObject's head
//# 5. drag each eye to slots on Head object in inspector
//@ 
//@  NOTE: See EyeFollowUVSet project for example and EchoHeadController.cs
//&-----------------------------------------------------------------------------
public class EchoEyeController : EchoGameObject 
{
	public int eyeUvSetSize    = 33;
	public float uvPadding     = 0.1f;
	public float moveSpeed     = 1.0f;
	private Transform 	_lookAt;
	private int   		_xSet;
	private int   		_ySet;
	private int   		_xSetStart;
	private int   		_ySetStart;
	private int   		_xSetEnd;
	private int   		_ySetEnd;
	private float 		_AngleX1;
	private float 		_AngleX2;
	private float 		_AngleY1;
	private float 		_AngleY2;
	private float 		_xLength;
	private float 		_yLength;
	private float   	_movePer;
	private float   	_moveTime;
	private float   	_moveTimeDest;
	private bool    	_lookFound;
	private bool    	_looking;
	private Vector3 	_oldPos;
	
	void Start ()
	{
		int loop;
		float x;
		float y;
		float z;
		float minX;
		float minY;
		float minZ;
		float maxX;
		float maxZ;
		float maxY;
		
		_lookFound 	= false;
		_looking   	= false;
		_xSetEnd 	= eyeUvSetSize / 2;
		_ySetEnd 	= _xSetEnd;
		_xSetStart 	= _xSetEnd;
		_ySetStart 	= _xSetEnd;
		_xSet		= _xSetEnd;
		_ySet		= _xSetEnd;
		
		if ( uvPadding < 0.0f )
			uvPadding = 0.0f;

		if ( uvPadding > 0.3f )
			uvPadding = 0.3f;
		
		minX = 9999;
		minY = 9999;
		minZ = 9999;
		maxX = -9999;
		maxY = -9999;
		maxZ = -9999;
		
		UVSetMakeJustify ( eyeUvSetSize, eyeUvSetSize, uvPadding );
		
		for ( loop = 0; loop < meshVertCount; loop++ )
		{
			x = mesh.vertices[loop].x; 
			y = mesh.vertices[loop].y;
			z = mesh.vertices[loop].z;

			if ( x < minX )
				minX = x;

			if ( y < minY )
				minY = y;

			if ( z < minZ )
				minZ = z;

			if ( x > maxX )
				maxX = x;

			if ( y > maxY )
				maxY = y;
			
			if ( z > maxZ )
				maxZ = z;
		}

		maxZ = ( minZ + maxZ ) / 2.0f;
		
		_AngleX1 = Mathf.Atan2 ( minX, maxZ ) * 180.0f / Mathf.PI; 
		_AngleY1 = Mathf.Atan2 ( minY, maxZ ) * 180.0f / Mathf.PI; 

		_AngleX2 = Mathf.Atan2 ( maxX, maxZ ) * 180.0f / Mathf.PI; 
		_AngleY2 = Mathf.Atan2 ( maxY, maxZ ) * 180.0f / Mathf.PI; 

	}
	
	public void LookAtTransform ( Transform itrans )
	{
		_lookAt = itrans;
	}
	
	private void LookAt ( Vector3 ipos )
	{
		float xAngle;
		float yAngle;
		float per;
		
		if ( ipos == _oldPos )
			return;
		
		_oldPos = ipos;
		
		ipos = cachedTransform.InverseTransformPoint ( ipos );
		
		if ( ipos.z > 0 )
		{
			_looking 	= true;
			_lookFound 	= true;
			
			xAngle = Mathf.Atan2 ( ipos.x, ipos.z ) * 180.0f / Mathf.PI; 
			yAngle = Mathf.Atan2 ( ipos.y, ipos.z ) * 180.0f / Mathf.PI;
			
			xAngle = Mathf.Clamp ( xAngle, _AngleX1, _AngleX2 );
			yAngle = Mathf.Clamp ( yAngle, _AngleY1, _AngleY2 );
			
			per = Mathf.InverseLerp ( _AngleX1, _AngleX2, xAngle );
			_xSetEnd = (int)( ( (float)eyeUvSetSize - 1.0f ) * per );
			
			per = Mathf.InverseLerp ( _AngleY1, _AngleY2, yAngle );
			_ySetEnd = (int)( ( (float)eyeUvSetSize - 1.0f ) * per );
			
			_xSetStart = _xSet;
			_ySetStart = _ySet;
			
			_moveTimeDest = moveSpeed;
			_moveTime = moveSpeed * 0.1f;
		}
		else
		{
			if ( _lookFound )
			{
				_looking 		= true;
				_lookFound 		= false;
				_moveTimeDest 	= moveSpeed;
				_moveTime 		= 0;
				
				_xSetStart 	= _xSet;
				_ySetStart 	= _ySet;
				_xSetEnd 	= eyeUvSetSize / 2;
				_ySetEnd 	= _xSetEnd;
			}
		}
	}
	
	void Update()
	{
		if ( _lookAt == null )
			return;
		
		LookAt  ( _lookAt.position );
		
		if ( _looking )
		{
			_moveTime += Time.deltaTime;
			_movePer = _moveTime /_moveTimeDest;
			
			_xSet = (int)Mathf.Lerp ( _xSetStart, _xSetEnd, _movePer );
			_ySet = (int)Mathf.Lerp ( _ySetStart, _ySetEnd, _movePer );
			
			UVSet ( _xSet + ( _ySet * eyeUvSetSize ) );
			
			if ( _movePer >= 1.0f )
				_looking = false;
		}
	}
}
