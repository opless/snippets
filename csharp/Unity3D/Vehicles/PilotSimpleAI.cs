using UnityEngine;
using System.Collections;

public class PilotSimpleAI : Pilot
{
public bool debugLog = false;
public bool debugBreak = false;

public GameObject waypointGO;

	public  Vector3 flyTo = Vector3.zero;
	public  bool enableThrusters = true;
	public  bool enableImpulse   = false;
	public  bool enableTurn      = false;
	private Vector3 desiredLocation = Vector3.zero;

	private Vector3 lastPosition    = Vector3.zero;
	private Vector3 hoverPosition   = Vector3.zero;
	private Vector3 deltaV          = Vector3.zero;

	
	public float distancePercent = 0.5F;
	
    public delegate void State();
    public State executeState;		

	void Start()
    {
   		Debug.Log("PilotAI");

		if( vehicle == null)
		{
			Destroy (gameObject); // we don't want to exist
			Debug.Log(name+": No Vehicle, bye");
		}
        executeState = TakeOff;
        vehicle.enabled = true;

        Invoke ("ChangeRandomWaypoint", 15);

    }
    
    void ChangeRandomWaypoint()
    {
    	if(executeState == Hover)
    		flyTo =(new Vector3(Random.Range(-200,200),100+Random.Range(-95,200),Random.Range(-200,200)));
    	waypointGO.transform.position = flyTo;
    	Invoke ("ChangeWayPoint", 5); // queue next one up
    }
    
	void EngageThrusters()
	{
		Debug.Log(name+": Engaging Thrusters for destination: "+flyTo);
		executeState = SimpleThrusters;
	}
	void EngageImpulse()
	{
		Debug.Log(name+": Engaging Impulse Engines for destination: "+flyTo);
		//executeState = SimpleThrusters;
	}
	void EngageTurn()
	{
		Debug.Log(name+": Engaging Attitude Thrusters for course correction: "+flyTo);
		//executeState = SimpleThrusters;
	}
	
	void FixedUpdate()
	{
    	Transform my = vehicle.transform;
		Vector3 savedPosn = my.position;
		if(flyTo != desiredLocation)
    	{
    		desiredLocation = flyTo;
    		Debug.Log(name+": New Destination Entered. "+flyTo);
    		// figure out how we should be going there.
    		if(Vector3.Distance(my.position,desiredLocation) < 50)
    		{
    			Invoke("EngageThrusters", 1);
    		} else {
    			Invoke("EngageImpulse", 2);
    		}
		}
		deltaV = (savedPosn -lastPosition) * Time.deltaTime;
		executeState();
		lastPosition = savedPosn;
		bool reset=false;
		if(Input.GetKeyDown ("1")) { vehicle.transform.position = new Vector3(0,100,0); reset =true; } 
		if(Input.GetKeyDown ("2")) { vehicle.transform.position = new Vector3(0,250,0); reset =true; } 
		if(Input.GetKeyDown ("3")) { vehicle.transform.position = new Vector3(0,500,0); reset =true; } 
		if(Input.GetKeyDown ("4")) { vehicle.transform.position = new Vector3(80,20,750); reset =true; } 
		if(Input.GetKeyDown ("5")) { vehicle.transform.position = new Vector3(250,300,-120); reset =true; } 
		if(Input.GetKeyDown ("6")) { vehicle.transform.position = new Vector3(100,  0,-200); reset =true; } 
		if(Input.GetKeyDown ("7")) { vehicle.transform.position = new Vector3(5000,5000,5000); reset =true; } 
		if(Input.GetKeyDown ("8")) { vehicle.transform.position = new Vector3(20,0,20); reset =true; } 
		if(Input.GetKeyDown ("9")) { vehicle.transform.position = new Vector3(999,9999,-999); reset =true; } 
		if(Input.GetKeyDown ("0")) { vehicle.transform.position = new Vector3(0,0,0); reset =true; } 
		if(reset)
		{
			vehicle.transform.rotation = Quaternion.identity;
			vehicle.rigidbody.velocity = Vector3.zero;
			vehicle.rigidbody.angularVelocity = Vector3.zero;
		}
	}
	void IMPULSE(int bval)
	{
		enableImpulse=(bval!=0);
	}
	void THRUST(int bval)
	{
		enableThrusters=(bval!=0);
	}
	void TURN(int bval)
	{
		enableTurn=(bval!=0);
	}
	void ALL(int bval)
	{
		enableTurn=(bval!=0);
		enableThrusters=(bval!=0);
		enableImpulse=(bval!=0);
	}
// states
	public void TakeOff()
    {
    	Transform my = vehicle.transform;

        flyTo = my.position+ Vector3.up * 10;
        desiredLocation=flyTo; // inhibit sillyness 
        executeState    = SimpleThrusters;
    	Debug.Log(name+": TakeOff -> SimpleFlight");
    }
    public void SimpleImpulse()
 	{
    	Transform my = vehicle.transform;
    	Vector3 dest = ((desiredLocation - my.position) * distancePercent) +my.position;
		Engines(dest);
		Thrusters(dest); // for low-end slew and simple sliding
		PointAt(dest);

		
//  		Debug.DrawLine(my.position , desiredLocation , Color.magenta);
//  		Debug.DrawLine(my.position +Vector3.forward , dest+Vector3.forward , Color.green);
  		float dist = Vector3.Distance(my.position, desiredLocation); 
		if( dist  < 10 )
		{
			if( vehicle.rigidbody.velocity.magnitude  < 1 )
			{
				executeState = SimpleThrusters;
				Debug.Log(name+": SimpleImpulse achieved approx location, finetuning, with thrusters");
				vehicle.ResetImpulse();
			}
		}	
 	}
    
    public void SimpleThrusters()
 	{
    	Transform my = vehicle.transform;
    	Vector3 dest = ((desiredLocation - my.position) * distancePercent) +my.position;
		Thrusters(dest); 
		
//  		Debug.DrawLine(my.position , desiredLocation , Color.magenta);
//  		Debug.DrawLine(my.position +Vector3.forward , dest+Vector3.forward , Color.green);
  		
  		float dist = Vector3.Distance(my.position, desiredLocation); 
		if( dist < 1 )
		{
			executeState = Hover;
			hoverPosition = desiredLocation;
			Debug.Log(name+": SimpleThrusters achieved stationkeeping changing to Hover");
		}
		// TODO ... same if we've not got a decent velocity after a while
		if( dist > 50 )
		{
			executeState = SimpleImpulse;
			Debug.Log(name+": SimpleThrusters cannot achieve goal changing to SimpleImpulse");
		}
 	}
 	///////////////////////////////////////////////////////////////
 	// Station Keeping !
 	public void Hover()
 	{
 		// TODO: Need to level self
    	Transform my = vehicle.transform;
    	Vector3 dest = hoverPosition;
    	if(Vector3.Distance(my.position, desiredLocation) > 2)
    	{
    		// knocked off course, correct
    		executeState    = SimpleThrusters;
			desiredLocation = hoverPosition;
			Debug.Log(name+": Hover out of tolerance ... reaquiring station keeping with SimpleTrusters");
    	}
		Thrusters(dest);
//		Vector3 myUp      =  my.position + Vector3.up  ;
//		Vector3 myForward =  my.position + Vector3.forward  ;
				
		PointAt(dest+Vector3.forward);
//		YawToFacePoint(   myForward );
///		PitchToFacePoint( myForward );
 	}
 
 



////////////////
// utilities

	public void TurnToFace(Vector3 point)
	{
		//float eta = Mathg.relativeTimeToTarget(this.gameObject,point);
		// static point, we don't need this: Vector3 targetPosition = point + (targetQ * eta);
		vehicle.yoke = Mathg.PointTowards(vehicle.transform, point, 1);
		
	}
    
    public Vector3 ThrusterRate(Vector3 point)
    {
		float eta = Mathg.relativeTimeToTarget(vehicle.gameObject, point); //, deltaV);

		//Debug.Log("ETA: "+eta);

    	Transform t= vehicle.transform;
		Vector3 futurePosition = t.position + (vehicle.rigidbody.velocity * eta);

  		//Debug.DrawLine(t.position+Vector3.forward , futurePosition +Vector3.forward, Color.blue);		
		
        Vector3 vectorToPoint = t.InverseTransformDirection(point - futurePosition);
        return vectorToPoint.normalized;

    }
    public void Thrusters(Vector3 point)
	{
		vehicle.thrusterValues =ThrusterRate(point);
	}
    public void Engines(Vector3 point)
    {
    	vehicle.impulseValues =ThrusterRate(point);
    }

    public void PointAt(Vector3 point)
    {
    	Transform my = vehicle.transform;
		Vector3   aim= Mathg.PointTowards(my, point, 1 );

		my.localRotation = my.localRotation * Quaternion.Euler(aim.x ,         0, 0);
		my.localRotation = my.localRotation * Quaternion.Euler(0        , aim.y , 0);		
		//vehicle.yoke = eulerToYoke(change);


    }


    
    public Vector3 eulerToYoke(Vector3 euler)
    {
   		Vector3 yoke = euler;

   		if(yoke.x >180) { yoke.x = yoke.x-360; }
   		else if(yoke.x < -180) { yoke.x = yoke.x+360; }

   		if(yoke.y >180) { yoke.y = yoke.y-360; }
   		else if(yoke.y < -180) { yoke.y = yoke.y+360; }
   		
   		if(yoke.z >180) { yoke.z = yoke.z-360; }
   		else if(yoke.z < -180) { yoke.z = yoke.z+360; }
   		return yoke;
    }
 

}
