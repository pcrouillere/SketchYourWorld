/* VRSharedTransform
 * MiddleVR
 * (c) i'm in VR
 */

using UnityEngine;
using System.Collections;

using MiddleVR_Unity3D;

public class VRShareTransform : MonoBehaviour {
	static private uint g_shareID = 1;

	private uint      m_ShareID = 0;
	private vrTracker m_tracker = null;
	private bool      m_TrackerSynchronized = false;

	[HideInInspector]
	public string ShareName;

	void OnEnable() {
		DoEnableOrStart();
	}

	void OnDisable() {
		UnsynchronizeTracker();
	}

	void OnDestroy() {
		DestroyTracker();
	}

	public void Start () {
		DoEnableOrStart();
	}

	public void Update () {
		if( MiddleVR.VRClusterMgr.IsServer() )
		{
			if( !m_TrackerSynchronized )
			{
				// Try until we succeed because synchronization may have
				// failed during OnEnable or OnStart due to late creation
				// of MiddleVR managers.
				GetOrCreateTracker();
				SynchronizeTracker();
			}

			if( m_TrackerSynchronized )
			{
				Vector3 p = transform.position;
				Quaternion q = transform.rotation;

				vrVec3 pos = new vrVec3(p.x, p.y, p.z);
				vrQuat quat = new vrQuat(q.x, q.y, q.z, q.w);

				m_tracker.SetPosition(pos);
				m_tracker.SetOrientation(quat);

				//MiddleVRTools.Log("Server pushing data : " + p.z );
			}
		}
	}

	private void DoEnableOrStart() {
		// Not very clear whether OnEnable will be always called before Start.
		InitTrackerName();
		GetOrCreateTracker();
		SynchronizeTracker();
	}

	/**
	 * @brief Sets the name and the Id of the tracker.
	 *
	 * It is needed to ensure that this function is called before Addtracker.
	 */
	private void InitTrackerName() {
		if( m_ShareID == 0 )
		{
			m_ShareID = g_shareID++;
			ShareName = "S_" + m_ShareID.ToString();

			Debug.Log("[-] VRSharedTransform script for " + name +
			          ": set ShareName to \"" + ShareName + "\"");
		}
	}

	private void GetOrCreateTracker() {
		if( m_tracker != null )
		{
			return;
		}

		if( MiddleVR.VRDeviceMgr != null )
		{
			m_tracker = MiddleVR.VRDeviceMgr.GetTracker(ShareName);
			if( m_tracker == null )
			{
				// Does not exist yet so let's create it.
				m_tracker = MiddleVR.VRDeviceMgr.CreateTracker(ShareName);
				MiddleVRTools.Log("[+] Created shared tracker " + ShareName);
			}
		}
	}

	private void DestroyTracker() {
		UnsynchronizeTracker();

		if( MiddleVR.VRDeviceMgr != null && m_tracker != null )
		{
			MiddleVR.VRDeviceMgr.RemoveDevice(m_tracker);
			MiddleVR.VRDeviceMgr.DestroyDevice(m_tracker);
		}
		m_tracker = null;
	}

	private void SynchronizeTracker() {
		if( m_TrackerSynchronized )
		{
			return;
		}

		if( MiddleVR.VRClusterMgr != null && m_tracker != null )
		{
			// A tracker is added to the server and the clients.
			MiddleVR.VRClusterMgr.AddSynchronizedObject(m_tracker, 1);
			m_TrackerSynchronized = true;
			MiddleVRTools.Log("[+] Added to synchronization the shared tracker " + ShareName);
		}
	}

	private void UnsynchronizeTracker() {
		if( !m_TrackerSynchronized )
		{
			return;
		}

		if( MiddleVR.VRClusterMgr != null && m_tracker != null )
		{
			MiddleVR.VRClusterMgr.RemoveSynchronizedObject(m_tracker);
		}
		m_TrackerSynchronized = false;
		MiddleVRTools.Log("[+] Removed from synchronization the shared tracker " + ShareName);
	}
}
