/* VRApplySharedTransform
 * MiddleVR
 * (c) i'm in VR
 */

using UnityEngine;
using System.Collections;

using MiddleVR_Unity3D;

public class VRApplySharedTransform : MonoBehaviour {

	private VRShareTransform m_STScript = null;
	private vrTracker        m_tracker  = null;
	private bool             m_SharedTrackerAcquired = false;

	void OnEnable() {
		ConnectToShareTransformComponent();
	}

	void OnDisable() {
		m_SharedTrackerAcquired = false;
	}

	public void Start() {
		ConnectToShareTransformComponent();
	}

	public void Update() {
		if( MiddleVR.VRClusterMgr.IsClient() )
		{
			if( !m_SharedTrackerAcquired )
			{
				// Try until we succeed because "acquire" may have
				// failed during OnEnable or OnStart due to late creation
				// of MiddleVR managers.
				AcquireSharedTracker();
			}

			if( m_SharedTrackerAcquired )
			{
				// Get rid of anything that could move the object
				//Destroy(rigidbody);

				vrVec3 pos = m_tracker.GetPosition();
				vrQuat orient = m_tracker.GetOrientation();

				Vector3 p = new Vector3(pos.x(), pos.y(), pos.z());
				Quaternion q = new Quaternion(
					(float)orient.x(), (float)orient.y(), (float)orient.z(), (float)orient.w());

				transform.position = p;
				transform.rotation = q;

				//MiddleVRTools.Log("Client applying data : " + p.z );
			}
		}
	}

	private void ConnectToShareTransformComponent() {
		if( m_SharedTrackerAcquired )
		{
			// Already initialized.
			return;
		}

		if( m_STScript == null )
		{
			m_STScript = GetComponent<VRShareTransform>();
		}

		if( m_STScript != null )
		{
			AcquireSharedTracker();
		}
		else
		{
			MiddleVRTools.Log("[X] VRApplySharedTransform failed to get the VRShareTransform script.");
		}
	}

	private void AcquireSharedTracker() {
		if( MiddleVR.VRClusterMgr != null )
		{
			if( MiddleVR.VRClusterMgr.IsClient() )
			{
				if( m_tracker == null && MiddleVR.VRDeviceMgr != null )
				{
					m_tracker = MiddleVR.VRDeviceMgr.GetTracker(m_STScript.ShareName);
					if( m_tracker != null )
					{
						MiddleVRTools.Log("[+] Acquired shared tracker " + m_tracker.GetName());
					}
					else
					{
						MiddleVRTools.Log("[X] Failed to acquire the shared tracker for \"" +
						                  m_STScript.ShareName + "\"");
					}
				}

				m_SharedTrackerAcquired = ( m_tracker != null );

				MiddleVRTools.Log(
					"[-] VRApplySharedTransform script for " + name +
					": Shared tracker acquired? " + (m_SharedTrackerAcquired ? "Yes" : "No"));
			}
		}
	}
}
