using UnityEngine;
using System.Collections;

/*  FOR DEMO TEXTURE, yeah I had a c64 too!
LOAD “$”, 8, 1
SYS 4096
ADDR AR XR YR SP 01 EL-SIM
4096:41 4c 4c 20 57 4f 52 4b 
5004:20 4e 4f 20 50 4c 41 59 
5012:20 4d 41 4b 45 53 20 45
5020:43 48 4f 4c 4f 47 49 4e 
5028:20 41 20 44 55 4c 4c 20
5036:42 4f 59 20 41 4c 4c 20 
5044:57 4f 52 4b 20 4e 4f 20 
5052:50 4c 41 59 20 4d 41 4b 
5060:45 53 20 45 43 48 4f 4c
5068:4f 47 49 4e 20 41 20 44 
5076:55 4c 4c 20 42 4f 59 20 
5084:41 4c 4c 20 57 4f 52 4b  
5092:20 4e 4f 20 50 4c 41 59
 */

public class CameraScript : MonoBehaviour 
{
	//============================================================
	public void StartLogoAnim()
	{
		LogoBrain.AnimationStart();
	}

	//============================================================
	public void BlinkLogoOn()
	{
		LogoBrain.BlinkEchoLoginOn();
	}

	//============================================================
	public void SubtitleFadeIn()
	{
		LogoBrain.SubtitleStart();
	}

	//============================================================
	public void SubtitleFadeOut()
	{
		LogoBrain.SubtitleEnd();
	}

	//============================================================
	public void SatilliteShieldsOn()
	{
		SatilliteBrain.ShieldsUp();
		SoundFX.PlayAudioClip ( SoundFX.soundShieldOn );
		LogoBrain.myego.EchoActive ( false );
	}

	//============================================================
	public void ShieldsOnWarning()
	{
		SpaceDemoMain.TurnOnShieldHUDWarning();
	}

	//============================================================
	public void SatilliteShieldsOff()
	{
		SatilliteBrain.ShieldsDown();
		SoundFX.PlayAudioClip ( SoundFX.soundShieldDown );
	}

	//============================================================
	public void ShootMissiles1()
	{
		MissileManager.Launch ( SpaceDemoMain.missileLaunch[0].transform.position, new Vector3 ( -0.5f, 0.0f, 1.0f  ), SatilliteBrain.myego.transform, 1.25f );
		MissileManager.Launch ( SpaceDemoMain.missileLaunch[1].transform.position, new Vector3 ( 0.5f, 0.0f, 1.0f  ), SatilliteBrain.myego.transform, 1.25f );
//		MissileManager.Launch ( SpaceDemoMain.missileLaunch[2].transform.position, new Vector3 ( -0.5f, 0.0f, 1.0f  ), SatilliteBrain.myego.transform, 1.25f );
//		MissileManager.Launch ( SpaceDemoMain.missileLaunch[3].transform.position, new Vector3 ( 0.5f, 0.0f, 1.0f  ), SatilliteBrain.myego.transform, 1.25f );
	}

	//============================================================
	public void ShootMissiles2()
	{
		MissileManager.Launch ( SpaceDemoMain.missileLaunch[4].transform.position, new Vector3 ( -0.5f, 0.0f, 1.0f  ), SatilliteBrain.myego.transform, 1.25f );
		MissileManager.Launch ( SpaceDemoMain.missileLaunch[5].transform.position, new Vector3 ( 0.5f, 0.0f, 1.0f  ), SatilliteBrain.myego.transform, 1.25f );
//		MissileManager.Launch ( SpaceDemoMain.missileLaunch[6].transform.position, new Vector3 ( -0.5f, 0.0f, 1.0f  ), SatilliteBrain.myego.transform, 1.25f );
//		MissileManager.Launch ( SpaceDemoMain.missileLaunch[7].transform.position, new Vector3 ( 0.5f, 0.0f, 1.0f  ), SatilliteBrain.myego.transform, 1.25f );
	}

	//============================================================
	public void Phaser1Shoot()
	{
		Phaser.Shoot ( SpaceDemoMain.laserEmit[0], SpaceDemoMain.laserEmit[0].transform.position, SatilliteBrain.myego.transform.position - SpaceDemoMain.laserEmit[0].transform.position );
	}

	//============================================================
	public void Phaser2Shoot()
	{
		Phaser.Shoot ( SpaceDemoMain.laserEmit[1], SpaceDemoMain.laserEmit[1].transform.position,  SatilliteBrain.myego.transform.position - SpaceDemoMain.laserEmit[1].transform.position );
	}

	//============================================================
	public void Phaser3Shoot()
	{
		Phaser.Shoot ( SpaceDemoMain.laserEmit[2], SpaceDemoMain.laserEmit[2].transform.position,  SatilliteBrain.myego.transform.position - SpaceDemoMain.laserEmit[2].transform.position );
	}

	//============================================================
	public void Phaser4Shoot()
	{
		Phaser.Shoot ( SpaceDemoMain.laserEmit[3], SpaceDemoMain.laserEmit[3].transform.position,  SatilliteBrain.myego.transform.position - SpaceDemoMain.laserEmit[3].transform.position );
	}

	//============================================================
	public void FireKrylocianLaser()
	{
		KrylocianLaser.BurnThemBurnThemAll ( SatilliteBrain.myego.transform.position );
	}

	//============================================================
	public void DataPanel2On()
	{
		SpaceDemoMain.TurnOnDataPanel2();
		SoundFX.PlayAudioClip ( SoundFX.soundBeep1 );
	}

	//============================================================
	public void DataPanel2Off()
	{
		SpaceDemoMain.TurnOffDataPanel2();
		SoundFX.PlayAudioClip ( SoundFX.soundBeep2 );
	}

	//============================================================
	public void ShakeCamera1()
	{
    	EchoFXEvent.ShakeGameObject ( Camera.main.transform, 0.001f, 6.0f );
	}

	//============================================================
	public void ShakeCamera2()
	{
		EchoFXEvent efx;

    	efx = EchoFXEvent.ShakeGameObject ( Camera.main.transform, 0.008f, 5.0f );
		efx.AddFilter ( EchoFilter.SINE, 0.5f );
	}

	//============================================================
	public void WarpSound()
	{
		SoundFX.PlayAudioClip ( SoundFX.soundWarp );
	}

	//============================================================
	public void FadeToWhite()
	{
		EchoFXEvent efx;

		SpaceDemoMain.fadePlate.EchoActive(true);
		efx = EchoFXEvent.Animate_echoRGBA ( SpaceDemoMain.fadePlate, new Vector4 ( 0,0,0,0 ), new Vector4 ( 2,2,2,2 ), 3.0f );
		efx.SetEventDone ( ShowEndText );
	}

	//============================================================
	public void ShowEndText( bool iforcestop )
	{
		EchoFXEvent.Animate_echoRGBA ( SpaceDemoMain.fadePlate, new Vector4 ( 2,2,2,2 ), new Vector4 ( 0,0,0,0 ), 2.0f );

		// we should not be calling Get Component during runtime but it will do for now
		SpaceDemoMain.endText.GetComponent<EndTextBrain>().StartEndText();
	}

}
