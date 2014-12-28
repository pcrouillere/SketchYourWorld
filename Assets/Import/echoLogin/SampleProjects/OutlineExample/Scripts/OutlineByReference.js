#pragma strict

private var ego:EchoGameObject;

public var outlineMat:Material;
public var outlineColor:Color = new Color ( 1,0,0,1 );
public var overlayMat:Material;
public var overlayColor:Color = new Color ( 1,0,0,1 );

function Start()
{
	ego = EchoGameObject.Add ( gameObject, true, true, outlineMat, overlayMat );
	ego.SetOutlineColor ( outlineColor );
	ego.SetOutlineOverlayColor ( overlayColor );
	//ego.Outline(true);
}
function Update() {
	
}

function OnMouseDown()
{
	ego.Outline(true);
}
	
function OnMouseUp()
{
	ego.Outline(false);
}
