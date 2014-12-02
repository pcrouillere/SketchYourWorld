/* VRClusterObject
 * MiddleVR
 * (c) i'm in VR
 */

using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;

public class VRClusterObject : MonoBehaviour {
	public bool IncludeChildren = true;

	void OnEnable() {
		// Not very clear whether OnEnable will be always called before Start.
		AddClusterScripts(gameObject, IncludeChildren);
		EnableClusterScripts(gameObject, IncludeChildren);
	}

	void OnDisable() {
		DisableClusterScripts(gameObject, IncludeChildren);
	}

	void Start() {
		// Not very clear whether OnEnable will be always called before Start.
        AddClusterScripts(gameObject, IncludeChildren);
		EnableClusterScripts(gameObject, IncludeChildren);
	}

	void Update() {
	}

	/**
	 * @brief Add scripts that are needed by the synchronization with clusters.
	 *
	 * This method must be called before EnableClusterScripts.
	 */
	void AddClusterScripts(GameObject iObject, bool iChildren)
	{
		//MiddleVRTools.Log("AddCluster to " + iObject);
		//print("AddCluster to " + iObject);

		if (iObject.GetComponent<VRShareTransform>() == null)
		{
			VRShareTransform script = iObject.AddComponent<VRShareTransform>();
			script.Start();
		}

		if (iObject.GetComponent<VRApplySharedTransform>() == null)
		{
			VRApplySharedTransform script = iObject.AddComponent<VRApplySharedTransform>();
			script.Start();
		}

		if( iChildren == true )
		{
			foreach (Transform child in iObject.transform)
			{
				GameObject childObject = child.gameObject;

				//print("Child : " + childObject);
				AddClusterScripts(childObject, true);
			}
		}
	}

	private void EnableClusterScripts(GameObject iObject, bool iChildren) {
		EnableOrDisableClusterScripts(true, iObject, iChildren);
	}

	private void DisableClusterScripts(GameObject iObject, bool iChildren) {
		EnableOrDisableClusterScripts(false, iObject, iChildren);
	}

	private void EnableOrDisableClusterScripts(bool iEnabling, GameObject iObject, bool iChildren)
	{
		string enableOpStr = ( iEnabling ? "[ ] Enabling" : "[ ] Disabling" );
		MiddleVRTools.Log(enableOpStr + " cluster on " + iObject);

		VRShareTransform shareTransformObj = iObject.GetComponent<VRShareTransform>();
		if( shareTransformObj != null )
		{
			shareTransformObj.enabled = iEnabling;
			MiddleVRTools.Log(enableOpStr + " cluster on " + iObject + " with VRShareTransform.");
		}

		VRApplySharedTransform applySharedTransformObj = iObject.GetComponent<VRApplySharedTransform>();
		if( applySharedTransformObj != null )
		{
			applySharedTransformObj.enabled = iEnabling;
			MiddleVRTools.Log(enableOpStr + " cluster on " + iObject + " with VRApplySharedTransform.");
		}

		if( iChildren == true )
		{
			foreach (Transform child in iObject.transform)
			{
				GameObject childObject = child.gameObject;

				EnableOrDisableClusterScripts(iEnabling, childObject, true);
			}
		}
	}
}
