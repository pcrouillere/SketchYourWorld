using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(CharacterMotor))]
public class Move : MonoBehaviour {
	float speed = 5.0f;
	float speedRotate = 100.0f;
	private CharacterMotor motor;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		move();
	}
	
	
	void Awake () {
		motor = GetComponent<CharacterMotor>();
	}
	
	void move()
	{
		if (!GlobalManager.gManager.drawingMenu.activeSelf || !GlobalManager.gManager.canMove) {

						Vector3 directionVector = new Vector3 (SixenseInput.Controllers [0].JoystickX, 0, SixenseInput.Controllers [0].JoystickY);
			
						if (directionVector != Vector3.zero) {
								// Get the length of the directon vector and then normalize it
								// Dividing by the length is cheaper than normalizing when we already have the length anyway
								float directionLength = directionVector.magnitude;
								directionVector = directionVector / directionLength;
				
								// Make sure the length is no bigger than 1
								directionLength = Mathf.Min (1, directionLength);
				
								// Make the input vector more sensitive towards the extremes and less sensitive in the middle
								// This makes it easier to control slow speeds when using analog sticks
								directionLength = directionLength * directionLength;
				
								// Multiply the normalized direction vector by the modified length
								directionVector = directionVector * directionLength;
						}
			
						// Apply the direction to the CharacterMotor
						motor.inputMoveDirection = transform.rotation * directionVector;
						motor.inputJump = SixenseInput.Controllers [1].GetButton (SixenseButtons.THREE);

				} 		
		//Camera.main.transform.Rotate(Vector3.up * speedRotate *  SixenseInput.Controllers[1].JoystickX  * Time.deltaTime);
		//Camera.main.transform.Rotate(Vector3.right * speedRotate *  SixenseInput.Controllers[1].JoystickY  * Time.deltaTime);
		//transform.Rotate(Vector3.up * speedRotate *  SixenseInput.Controllers[1].JoystickX  * Time.deltaTime);
	}	

			/* Debug.Log("something");	
			if ( SixenseInput.Controllers[0].GetButtonDown(SixenseButtons.START) ) {
				Debug.Log("start");
			}
			else if ( SixenseInput.Controllers[0].GetButtonDown(SixenseButtons.JOYSTICK) ) {
				Debug.Log("joystick");
			}
			else if ( SixenseInput.Controllers[0].GetButtonDown(SixenseButtons.BUMPER) ) {
				Debug.Log("bumper");
			}
			else if ( SixenseInput.Controllers[0].GetButtonDown(SixenseButtons.ONE) ) {
				Debug.Log("one");
			}			
			else if ( SixenseInput.Controllers[0].GetButtonDown(SixenseButtons.TWO) ) {
				Debug.Log("2");
			}			
			else if ( SixenseInput.Controllers[0].GetButtonDown(SixenseButtons.THREE) ) {
				Debug.Log("3");
			}			
			else if ( SixenseInput.Controllers[0].GetButtonDown(SixenseButtons.FOUR) ) {
				Debug.Log("4");
			}
			else if ( SixenseInput.Controllers[0].GetButtonDown(SixenseButtons.TRIGGER) ) {
				Debug.Log("trigger");
			}

			Debug.Log(SixenseInput.Controllers[0].JoystickX); 
*/

		}
