/************************************************************************************

Filename    :   OVRCamera.cs
Content     :   Interface to camera class
Created     :   January 8, 2013
Authors     :   Peter Giokaris

Copyright   :   Copyright 2013 Oculus VR, Inc. All Rights reserved.

Use of this software is subject to the terms of the Oculus LLC license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

************************************************************************************/
using UnityEngine;
using System.Runtime.InteropServices;
using MiddleVR_Unity3D;

[RequireComponent(typeof(Camera))]

//-------------------------------------------------------------------------------------
// ***** OVRCamera
//
// OVRCamera is used to render into a Unity Camera class.
// This component handles reading the Rift tracker and positioning the camera position
// and rotation. It also is responsible for properly rendering the final output, which
// also the final lens correction pass.
//
public class VRRiftCamera : MonoBehaviour
{
    [HideInInspector]
    public Vector2 _Center              = new Vector2(0.5f, 0.5f);
    [HideInInspector]
    public Vector2 _ScaleIn             = new Vector2(1.0f, 1.0f);
    [HideInInspector]
    public Vector2 _Scale               = new Vector2(1.0f, 1.0f);
    [HideInInspector]
    public Vector4 _HmdWarpParam        = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);

    // Chromatic aberration
    [HideInInspector]
    public Vector4 _ChromaticAberration = new Vector4(0.996f, 0.992f, 1.014f, 1.014f);

    public bool IsInit = false;

    public Material RenderMaterial = null;

    // * * * * * * * * * * * * *

    void SetMaterialProperties()
    {
        // Set RenderMaterial properties
        RenderMaterial.SetVector("_Center",       _Center);
        RenderMaterial.SetVector("_Scale",        _Scale);
        RenderMaterial.SetVector("_ScaleIn",      _ScaleIn);
        RenderMaterial.SetVector("_HmdWarpParam", _HmdWarpParam);

        // Chromatic aberration
        Vector4 _CA = _ChromaticAberration;
        float rSquaredCoeffR = _CA[1] - _CA[0];
        float rSquaredCoeffB = _CA[3] - _CA[2];
        _CA[1] = rSquaredCoeffR;
        _CA[3] = rSquaredCoeffB;
        RenderMaterial.SetVector("_ChromaticAberration", _CA);
    }

    // Start
    void Start()
    {
        // Anti Aliasing must be at least 2 in the Rift
        if( QualitySettings.antiAliasing < 2 )
        {
           QualitySettings.antiAliasing = 2;
        }

        Init();
    }

    protected void Init ()
    {
        if (RenderMaterial == null)
        {
            RenderMaterial = Resources.Load("OVRLensCorrectionMat", typeof(Material)) as Material;

            if (RenderMaterial == null)
            {
                MiddleVRTools.Log(-1, "[RiftCamera] Failed to load OVRLensCorrectionMat.");
            }
        }

        vrObject oculusDriver = MiddleVR.VRKernel.GetObject("OculusRift Tracker Driver");

        if (oculusDriver == null)
        {
            MiddleVRTools.Log(0, "[RiftCamera] No Oculus Rift driver found!");
            return;
        }

        float propHScreenSize = oculusDriver.GetProperty("HScreenSize").GetFloat();
        float propLensSeparationDistance = oculusDriver.GetProperty("LensSeparationDistance").GetFloat();
        float propDistortionScale = oculusDriver.GetProperty("DistortionScale").GetFloat();
        float propAR = oculusDriver.GetProperty("AspectRatio").GetFloat();
        float propK0 = oculusDriver.GetProperty("K0").GetFloat();
        float propK1 = oculusDriver.GetProperty("K1").GetFloat();
        float propK2 = oculusDriver.GetProperty("K2").GetFloat();
        float propK3 = oculusDriver.GetProperty("K3").GetFloat();

        // Get the distortion scale and aspect ratio to use when calculating distortion shader
        float distortionScale = 1.0f / propDistortionScale;
        float aspectRatio = propAR;

        float halfHSS = propHScreenSize * 0.5f;
        float halfLSD = propLensSeparationDistance * 0.5f;

        // These values are different in the Oculus SDK World Demo; Unity renders each camera to a buffer
        // that is normalized, so we will respect this rule when calculating the distortion inputs
        float normalizedWidth = 1.0f;
        float normalizedHeight = 1.0f;

        _Scale.x = (normalizedWidth / 2.0f) * distortionScale;
        _Scale.y = (normalizedHeight / 2.0f) * distortionScale * aspectRatio;
        _ScaleIn.x = (2.0f / normalizedWidth);
        _ScaleIn.y = (2.0f / normalizedHeight) / aspectRatio;
        _HmdWarpParam.x = propK0;
        _HmdWarpParam.y = propK1;
        _HmdWarpParam.z = propK2;
        _HmdWarpParam.w = propK3;

        _Center.x = 0.5f;

        if (name.Contains("Left"))
        {
            float lensOffsetLeft = (((halfHSS - halfLSD) / halfHSS) * 2.0f) - 1.0f;
            _Center.x = 0.5f + (lensOffsetLeft * 0.5f);
        }
        else if (name.Contains("Right"))
        {
            float lensOffsetRight = (( halfLSD / halfHSS) * 2.0f) - 1.0f;
            _Center.x = 0.5f + (lensOffsetRight * 0.5f);
        }

        _Center.y = 0.5f;

        if (RenderMaterial != null)
        {
            SetMaterialProperties();
        }
        else
        {
            print("NOMAT");
        }

        IsInit = true;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetMaterialProperties();

        // Draw to final destination
        if (RenderMaterial!= null)
        {
            // Render with distortion
            Graphics.Blit(source, destination, RenderMaterial);
        }
        else
        {
            // Pass through
            Graphics.Blit(source, destination);
        }
    }
}
