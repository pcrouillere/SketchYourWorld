/* VRFPSInputController
 * (c) i'm in VR
 */

using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;
using System;

[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(MouseLook))]
[RequireComponent(typeof(FPSInputController))]
public class VRFPSInputController : MonoBehaviour
{
    public string ReferenceNode = "HandNode";
    public  bool  Strafe = false;

    private bool  m_SearchedRefNode = false;

    // Use this for initialization
    void Start()
    {
        // Disable MouseLook
        GetComponent<MouseLook>().enabled = false;

        // Disable FPSInputController
        GetComponent<FPSInputController>().enabled = false;

        GameObject Wand = GameObject.Find("VRWand");

        if (Wand != null)
        {
            Wand.GetComponent<VRWandNavigation>().enabled = false;
            MiddleVRTools.Log("[ ] VRFPSInputController deactivated VRWandNavigation. Make sure you set the VR Root Node to the First Person Controller.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CharacterMotor motor = GetComponent<CharacterMotor>();

        vrKeyboard keyb = null;

        if (MiddleVR.VRDeviceMgr.GetKeyboard() != null)
        {
            keyb = MiddleVR.VRDeviceMgr.GetKeyboard();
        }

        GameObject refNode = GameObject.Find(ReferenceNode);

        float speed = 0.0f;
        float speedR = 0.0f;

        float forward = 0.0f;

        // Choosing active vertical axis

        // First test Unity's inputs
        if (Math.Abs(Input.GetAxis("Vertical")) > 0)
        {
            forward = Input.GetAxis("Vertical");
        }

        // Then test MiddleVR's keyboard
        if (keyb != null)
        {
            if (keyb.IsKeyPressed(MiddleVR.VRK_UP))
            {
                forward = 1.0f;
            }

            if (keyb.IsKeyPressed(MiddleVR.VRK_DOWN))
            {
                forward = -1.0f;
            }
        }


        float wandVertical = MiddleVR.VRDeviceMgr.GetWandVerticalAxisValue();

        // Finally, the Wand will have precedence over everything
        if (Math.Abs( wandVertical ) > 0.1f)
        {
            forward = wandVertical;
        }

        // Computing speed
        if (Math.Abs(forward) > 0.1) speed = forward * Time.deltaTime * 30;

        //print("Speed: " + speed);


        // Choosing active horizontal axis
        float rotation = 0.0f;

        // First test Unity's inputs
        if (Math.Abs(Input.GetAxis("Horizontal")) > 0)
        {
            rotation = Input.GetAxis("Horizontal");
        }

        // Then test MiddleVR's keyboard
        if (keyb != null)
        {
            if (keyb.IsKeyPressed(MiddleVR.VRK_LEFT))
            {
                rotation = -1.0f;
            }

            if (keyb.IsKeyPressed(MiddleVR.VRK_RIGHT))
            {
                rotation = 1.0f;
            }
        }

        float wandHorizontal = MiddleVR.VRDeviceMgr.GetWandHorizontalAxisValue();

        if (Math.Abs(wandHorizontal) > 0.1f)
        {
            rotation = wandHorizontal;
        }

        if (Math.Abs(rotation) > 0.1) speedR = rotation * Time.deltaTime * 50;

        Vector3 directionVector = new Vector3(0, 0, speed);

        if (Strafe)
        {
            directionVector.x = speedR;
        }
        else
        {
            transform.Rotate(Vector3.up, speedR);
        }

        if (refNode == null)
        {
            if (m_SearchedRefNode == false)
            {
                MiddleVRTools.Log("[ ] Didn't find reference node " + ReferenceNode);
                m_SearchedRefNode = true;
            }

            motor.inputMoveDirection = directionVector;
        }
        else
        {
            motor.inputMoveDirection = refNode.transform.TransformDirection(directionVector);

            //print(motor.inputMoveDirection);
        }

        bool jump = false;

        if (Input.GetButton("Jump") == true)
        {
            jump = true;
        }

        if (keyb != null)
        {
            if (keyb.IsKeyPressed(MiddleVR.VRK_SPACE))
            {
                jump = true;
            }
        }

        if (MiddleVR.VRDeviceMgr.IsWandButtonToggled(1) == true)
        {
            jump = true;
        }

        motor.inputJump = jump;
    }


}
