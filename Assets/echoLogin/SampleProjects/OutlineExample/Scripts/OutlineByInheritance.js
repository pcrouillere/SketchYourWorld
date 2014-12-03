#pragma strict

class OutlineByInheritance extends EchoGameObject
{
	function OnMouseDown()
	{
		Outline(true);
	}
		
	function OnMouseUp()
	{
		Outline(false);
	}
}

