using UnityEngine;
using System.Collections;

public class ScriptableObject : MonoBehaviour 
{

 	public string Fmt(Vector3 point)
	{
		return System.String.Format(
			"({0:N3} , {1:N3} , {2:N3})",
			point.x,point.y,point.z);
	}  
	
	public void Log(object o)
	{
		Debug.Log(o);
	}

}
