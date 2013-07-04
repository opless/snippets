using UnityEngine;
using System.Collections;

public class HUDGUI : MonoBehaviour 
{
	public GameObject target;
	public string     function;
	public int        val;
	public string     key;
	
	void Update()
	{
		if(Input.GetKeyUp(key))
		{
			target.SendMessage(function,val,SendMessageOptions.DontRequireReceiver);
		}
	}

}
