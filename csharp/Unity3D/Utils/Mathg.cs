using UnityEngine;
using System.Collections;

public class Mathg
{
	public static float relativeTimeToTarget(   GameObject origin, GameObject target )
	{
		Vector3 oP,oV,tP,tV,pV;
		if(origin.rigidbody != null)
			oV = origin.rigidbody.velocity;
		else
			oV = Vector3.zero;
		oP = origin.transform.position;
		
		if(target.rigidbody != null)
			tV = target.rigidbody.velocity;
		else
			tV = Vector3.zero;
		tP = target.transform.position;
		
		return relativeTimeToTarget(oP,oV,tP,tV, 0);
	}
	public static float relativeTimeToTarget( GameObject origin, Vector3 target )
	{
		return relativeTimeToTarget( origin.transform.position, origin.rigidbody.velocity, target, Vector3.zero, 0);
	}
	public static float relativeTimeToTarget( GameObject origin, Vector3 target ,Vector3 deltaV)
	{
		Vector3 oP = origin.transform.position;
		Vector3 oV = origin.rigidbody.velocity;// + 2*deltaV ;
		// what would our eta be if we were constant velocity
//		float eta =  relativeTimeToTarget( oP , oV, target, Vector3.zero, 0);
		// 
		
		return relativeTimeToTarget( oP, oV, target, deltaV, 0);
	}	
	
	public static float relativeTimeToTarget(   Vector3 originPosn, Vector3 originVel, 
												Vector3 targetPosn, Vector3 targetVel, float pVel)
	{
		Vector3 diffPosn = originPosn - targetPosn;
		Vector3 diffVel  = originVel  - targetVel ;
		if(targetVel.sqrMagnitude == 0)
			diffVel =originVel;
		return Mathg.timeToTarget(diffPosn, diffVel,pVel);
	}
	public static float timeToTarget(Vector3 vTargetPosn, Vector3 vTargetVelocity, float projectileVelocity)
	{
	
		float a = Vector3.Dot(vTargetVelocity,vTargetVelocity) - (projectileVelocity*projectileVelocity);
		float b = 2*Vector3.Dot(vTargetPosn, vTargetVelocity);
		float c = Vector3.Dot(vTargetPosn,vTargetPosn);
		
		float d = b*b - 4*a*c;


		float t = 0;
		float u = 0;
		float tt = 0;
		float tu = 0;
		float r = 1;
	    if (d >= 0)
	    {
	        tt = (-b + Mathf.Sqrt(d)) / (2*a);
	        tu = (-b - Mathf.Sqrt(d)) / (2*a);
	        
	        t = (tt < 0) ? System.Single.PositiveInfinity : tt;
			u = (tu < 0) ? System.Single.PositiveInfinity : tu;
			
			r = Mathf.Min(t,u); 
   		 }
   		/*
		// Debug.Log(
			System.String.Format(
				"tPosn: {0} tVel:{1} mVel:{2}({10}) a:{3} b:{4} c:{5} d:{6} t:{7} u:{8} r:{9}", 
				vTargetPosn,vTargetVelocity,projectileVelocity,
				a,b,c,d,
				tt,tu,r));
		*/
		if( System.Single.IsNaN(r) )
		{
			r = 0.0001F; // sufficently small number
//			Debug.Log("ERK - NaN");
		}
		if( System.Single.IsInfinity(r) )
		{
			r = 0.0001F; // sufficently large number
//			Debug.Log("ERK - INF");
		}
		return r;
	}
	public static Vector3 PointTowardsQuaternionDOESNTWORK(Transform self, Vector3 targetPosition, float clampTo)
	{
		Vector3 desiredAim = targetPosition - self.position; 		
		Vector3 up         = ( self.rotation * Vector3.up).normalized;
		Vector3 forward    = ( self.rotation * Vector3.forward).normalized;
		Quaternion desiredRotation = Quaternion.identity;
		desiredRotation.SetFromToRotation(forward, desiredAim);
		Quaternion allowedRotation = Quaternion.Lerp(self.rotation,desiredRotation, Time.deltaTime );
		Quaternion localRotation   = self.rotation * allowedRotation; // combine to get local rotation
		Vector3 ret = localRotation.eulerAngles;
		if (System.Single.IsNaN(ret.x)) ret.x = 0;
		if (System.Single.IsNaN(ret.y)) ret.y = 0;
		if (System.Single.IsNaN(ret.z)) ret.z = 0;
		return ret;		
	}
	
	
	
		
	public static Vector3 PointTowards(Transform self, Vector3 targetPosition, float clampTo)
	{
		Vector3 desiredAim = ( targetPosition - self.position ).normalized; 
		Vector3 up         = ( self.rotation * Vector3.up).normalized;
		Vector3 right      = ( self.rotation * Vector3.right).normalized;
		Vector3 forward    = ( self.rotation * Vector3.forward).normalized;

		Vector3 axis = Vector3.Cross(forward ,desiredAim  ).normalized;
		float angle  = Mathf.Acos( Mathg.Clamp(Vector3.Dot(desiredAim , forward),-1,1) );
		
//		if(Mathf.Abs(angle) < 3)
//			return Vector3.zero;
			
		float amount_to_pitch = angle * Vector3.Dot(axis , right);
		float amount_to_yaw   = angle * Vector3.Dot(axis , up);
//		float amount_to_roll  = angle * Vector3.Dot(axis , forward); 

		amount_to_pitch = Mathg.Clamp(amount_to_pitch,-clampTo,clampTo);
		amount_to_yaw   = Mathg.Clamp(amount_to_yaw,  -clampTo,clampTo)  ;
 //		amount_to_roll  = Mathg.Clamp(amount_to_roll, -clampTo,clampTo)  ;
		return new Vector3( amount_to_pitch, amount_to_yaw, 0);//amount_to_roll); 
	}
	public static Vector3 PointTowardsXXXXXX(Transform self, Vector3 targetPosition, float clampTo)
	{
	
		float torqueFactor=1;
            Vector3 forward = self.forward;
            Vector3 direction = targetPosition  - self.position;
 
            float distance = (targetPosition - self.position).magnitude;
 
            Vector3 axis = Vector3.Cross(forward, direction);
            float angle = Vector3.Angle(forward, direction);
            self.rigidbody.AddTorque(axis * angle * torqueFactor * Mathf.Atan(distance * 0.01f));
            
 		return new Vector3(0,0,0);
	}
	public static float Clamp(float amount, float min, float max)
	{
		if(System.Single.IsInfinity(amount))
		{
			return System.Single.IsNegativeInfinity(amount) ? min : max;
		}
		if(System.Single.IsNaN(amount))
			return 0;
		if(amount > max) return max;
		if(amount < min) return min;
		return amount;
	}

}
