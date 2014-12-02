/* VRShortcutReload
 * MiddleVR
 * (c) i'm in VR
 */

using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;
using System;

public class VRShortcutReload : MonoBehaviour
{

    // Update is called once per frame
    void Update () 
    {
        vrKeyboard keyb = MiddleVR.VRDeviceMgr.GetKeyboard();

        // Reload Simulation (level 0)
        if (keyb != null && keyb.IsKeyToggled(MiddleVR.VRK_R) && (keyb.IsKeyPressed(MiddleVR.VRK_LSHIFT) || keyb.IsKeyPressed(MiddleVR.VRK_RSHIFT)) && (keyb.IsKeyPressed(MiddleVR.VRK_LCONTROL) || keyb.IsKeyPressed(MiddleVR.VRK_RCONTROL)))
        {
            Application.LoadLevel(0);
        }
        // Reload last loaded level
        else if (keyb != null && keyb.IsKeyToggled(MiddleVR.VRK_R) && (keyb.IsKeyPressed(MiddleVR.VRK_LSHIFT) || keyb.IsKeyPressed(MiddleVR.VRK_RSHIFT)))
        {
            Application.LoadLevel( Application.loadedLevel );
        }
    }

}