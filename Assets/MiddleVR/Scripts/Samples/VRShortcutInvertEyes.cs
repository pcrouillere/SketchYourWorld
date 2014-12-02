/* VRShortcutInvertEyes
 * MiddleVR
 * (c) i'm in VR
 */

using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;
using System;

public class VRShortcutInvertEyes : MonoBehaviour
{

    // Update is called once per frame
    void Update () 
    {
        vrKeyboard keyb = MiddleVR.VRDeviceMgr.GetKeyboard();

        // Invert eye
        if (keyb != null && keyb.IsKeyToggled(MiddleVR.VRK_I) && (keyb.IsKeyPressed(MiddleVR.VRK_LSHIFT) || keyb.IsKeyPressed(MiddleVR.VRK_RSHIFT)))
        {
            // For each vrCameraStereo, invert inter eye distance
            uint camNb = MiddleVR.VRDisplayMgr.GetCamerasNb();
            for( uint i=0 ; i<camNb ; ++i )
            {
                vrCamera cam = MiddleVR.VRDisplayMgr.GetCameraByIndex(i);
                if(cam.IsA("CameraStereo"))
                {
                    vrCameraStereo stereoCam = MiddleVR.VRDisplayMgr.GetCameraStereo(cam.GetName());
                    stereoCam.SetInterEyeDistance( -stereoCam.GetInterEyeDistance() );
                }
            }
        }
    }

}