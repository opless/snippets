using UnityEngine;
using System.Collections;

public class PilotWaypointAI : PilotSimpleAI
{

	Vector3 finalWayPoint = Vector3.zero;
	

	// Use this for initialization
	void Start() 
	{
		Debug.Log("PilotWaypointAI");
		if( vehicle == null)
		{
			Destroy (gameObject); // we don't want to exist
			Debug.Log(name+": No Vehicle, bye");
		}
        executeState = TakeOff;
        vehicle.enabled = true;

        Invoke ("Initialise", 15);
	
	}
	void  Initialise()
	{
		Debug.Log("Initialise");
		plotTo  = new Vector3( 0 , 100 , 100);
	}
	public Vector3 plotTo
	{
		get { return finalWayPoint; }
		set { finalWayPoint = value; Invoke ("PlotCourse", 0.1F); }
	}
	void PlotCourse()
	{
		Debug.Log("PlotCourse");
		// Cast ray to destination
		Vector3 direction = vehicle.transform.rotation * Vector3.forward ;
		if(vehicle.rigidbody.velocity.sqrMagnitude > 1)
			direction = vehicle.rigidbody.velocity.normalized;
		flyTo = ComputeCourse(vehicle.transform.position , finalWayPoint , direction);
		Invoke ("PlotCourse", 0.1F);
	}
	
	bool flip = false;
	void DrawBlue(Vector3 A,Vector3 B)
	{
		Debug.DrawLine(A , B, flip ? Color.blue : Color.cyan);
		flip = !flip;
	}
	Vector3 ComputeCourse(Vector3 origin, Vector3 destination, Vector3 direction)
	{
		float sphereRadius = 10;
		
		Vector3 max =vehicle.collider.bounds.max;
		
		float radius = (max - origin).magnitude * 1.25F; // plus 25% :p
		Vector3 source = (destination - origin).normalized * radius ;
		
		// first point directly at the destination
		Vector3 point = Raycast( source , destination ) ;
		if(IsCloseTo( point ,destination, 20))
		{
		DrawBlue(source,point);
			Log("woo");
			return destination;
		}
		
		// ok, lets try pointing to 50% of the distance to point, and going right for the rest of the distance :)
		Vector3 relativePoint = (point - source);
		Vector3 tryPoint      = (relativePoint/2) + (Vector3.right*relativePoint.magnitude/2) + source;
		
		point = Raycast( tryPoint , destination );
		if(IsCloseTo( point, destination, 20))
		{
			Log("seeking...");
			DrawBlue(tryPoint,point);
			return point;
		}
		
		Log("meh, back up a bit"); 
		point =origin - (vehicle.transform.rotation*(Vector3.forward*3 - Vector3.up));
		DrawBlue(vehicle.transform.position,point);		
		return point;
		
	}
	bool IsCloseTo( Vector3 A , Vector3 B, float radius )
	{
		return (B - A).sqrMagnitude < (radius*radius);
	}
	Vector3 Raycast(Vector3 source, Vector3 destination)	
	{
		RaycastHit hit;
		if (Physics.Raycast(source, destination,out  hit,Mathf.Infinity,0)) 
		{
			return hit.point;
		}		
		return destination;
	}
	
	
}
