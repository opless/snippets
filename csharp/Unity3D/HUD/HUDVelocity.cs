using UnityEngine;
using System.Collections;

public class HUDVelocity : MonoBehaviour {

	public GameObject player;
	public  float updateInterval = 0.25F;
	 
	private float accum   = 0; // FPS accumulated over the interval
	private float timeleft; // Left time for current interval

	Vector3 lastP = Vector3.zero;

	void Start()
	{
	    if( !guiText )
	    {
	        Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
	        enabled = false;
	        return;
	    }
	    timeleft = updateInterval;  
	    lastP =player.rigidbody.velocity;
	}
	 
	void Update()
	{
	    timeleft -= Time.deltaTime;
	    accum += Time.timeScale/Time.deltaTime;
	
	    // Interval ended - update GUI text and start new interval
	    if( timeleft <= 0.0 )
	    {
			float velocity = player.rigidbody.velocity.magnitude;
			Vector3 accel = lastP - player.rigidbody.velocity;
			guiText.text = System.String.Format("Velocity: {0:F2} {1}\nTime: {2}", 
				velocity,
				Vec("Velocity",player.rigidbody.velocity)+
				Vec("Orientation",player.transform.localEulerAngles)+
				Vec("AngVel(W)",player.rigidbody.angularVelocity)+
				Vec("AngVel(L)",player.rigidbody.rotation*player.rigidbody.angularVelocity)+
				Vec("Accel", accel)+
				System.String.Format("Dist: {0:N3}", player.transform.position.y - 100)
				,
				Time.time);
			lastP =  player.rigidbody.velocity;
		}
	}
	string Vec(string name,Vector3 point)
	{
		return System.String.Format(
			"{0}: ({1:N3} , {2:N3} , {3:N3})\n",
			name,point.x,point.y,point.z);
	}
}