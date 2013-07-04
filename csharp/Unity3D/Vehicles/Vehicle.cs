using UnityEngine;
using System.Collections;

public class Vehicle : ExplodableObject 
{
	
	public Vector3 maxTorque = Vector3.zero;
	public Vector3 maxPositiveThrust = Vector3.zero;
	public Vector3 maxNegativeThrust = Vector3.zero;
	public Vector3 maxPositiveImpulse = Vector3.zero;
	public Vector3 maxNegativeImpulse = Vector3.zero;
	
	public Vector3 joystick = Vector3.zero; // X= Pitch Y= yaw Z= roll
	public Vector3 thrusters= Vector3.zero; // X,Y,Z "station keeping" thrusters
	public Vector3 impulse  = Vector3.zero; // usually a big +/- Z thruster, but Y too for heli types?


	public void ResetJoystick()
	{
		joystick = Vector3.zero;
	}
	public void ResetThrusters()
	{
		thrusters = Vector3.zero;
	}
	public void ResetImpulse()
	{
		impulse = Vector3.zero;
	}
	
	public float roll  { set { joystick.z = Mathf.Clamp( value , -1.0F, +1.0F ); } get { return joystick.z; } }
	public float pitch { set { joystick.x = Mathf.Clamp( value , -1.0F, +1.0F ); } get { return joystick.x; } }
	public float yaw   { set { joystick.y = Mathf.Clamp( value , -1.0F, +1.0F ); } get { return joystick.y; } }
	public Vector3 yoke
	{
		set 
		{ 
			joystick = new Vector3(
				Mathf.Clamp( value.x , -1 , 1),
				Mathf.Clamp( value.y , -1 , 1),
				Mathf.Clamp( value.z , -1 , 1));
		}
		get
		{
			return joystick;
		}		
	}

	public float x  { set { thrusters.x = Mathf.Clamp( value , -1.0F, +1.0F ); } get { return thrusters.x; } }
	public float y  { set { thrusters.y = Mathf.Clamp( value , -1.0F, +1.0F ); } get { return thrusters.y; } }
	public float z  { set { thrusters.z = Mathf.Clamp( value , -1.0F, +1.0F ); } get { return thrusters.z; } }
	public Vector3 thrusterValues 
	{ 
		set 
		{ 
			thrusters = new Vector3(
				Mathf.Clamp( value.x , -1 , 1),
				Mathf.Clamp( value.y , -1 , 1),
				Mathf.Clamp( value.z , -1 , 1));
		}
		get
		{
			return thrusters;
		}
	}

	public float slide    { set { impulse.x = Mathf.Clamp( value , -1.0F, +1.0F ); } get { return impulse.x; } }
	public float lift     { set { impulse.y = Mathf.Clamp( value , -1.0F, +1.0F ); } get { return impulse.y; } }
	public float thrust   { set { impulse.z = Mathf.Clamp( value , -1.0F, +1.0F ); } get { return impulse.z; } }
	public Vector3 impulseValues 
	{ 
		set 
		{ 
			impulse = new Vector3(
				Mathf.Clamp( value.x , -1 , 1),
				Mathf.Clamp( value.y , -1 , 1),
				Mathf.Clamp( value.z , -1 , 1));
		}
		get
		{
			return impulse;
		}
	}

	public float forwardVelocity { get { return transform.InverseTransformDirection(rigidbody.velocity).z; }}
	public float slideVelocity   { get { return transform.InverseTransformDirection(rigidbody.velocity).x; }}
	public float liftVelocity    { get { return transform.InverseTransformDirection(rigidbody.velocity).y; }}

	void FixedUpdate()
	{ 
		rigidbody.AddRelativeTorque(Vector3.up * yaw * maxTorque.y * Time.deltaTime);
		rigidbody.AddRelativeTorque(Vector3.right * pitch * maxTorque.x * Time.deltaTime);
		rigidbody.AddRelativeTorque(Vector3.forward * roll * maxTorque.z * Time.deltaTime);

		Vector3 thrusterForce = thrusters;
		thrusterForce.x *= (thrusters.x < 0 ) ? maxNegativeThrust.x: maxPositiveThrust.x;
		thrusterForce.y *= (thrusters.y < 0 ) ? maxNegativeThrust.y: maxPositiveThrust.y;
		thrusterForce.z *= (thrusters.z < 0 ) ? maxNegativeThrust.z: maxPositiveThrust.z;

		Vector3 impulseForce = impulseValues;
		impulseForce.x *= (impulseValues.x < 0 ) ? maxNegativeImpulse.x: maxPositiveImpulse.x;
		impulseForce.y *= (impulseValues.y < 0 ) ? maxNegativeImpulse.y: maxPositiveImpulse.y;
		impulseForce.z *= (impulseValues.z < 0 ) ? maxNegativeImpulse.z: maxPositiveImpulse.z;

			
		rigidbody.AddRelativeForce(thrusterForce+impulseForce, ForceMode.Force);
	}


}
