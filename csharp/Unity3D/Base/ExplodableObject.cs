using UnityEngine;
using System.Collections;

public class ExplodableObject : ScriptableObject 
{

	public GameObject explosionPrefab = null;


	public void ExplodeNow()
	{
		Instantiate(explosionPrefab, transform.position, transform.rotation);
		Destroy (gameObject);
	}

}
