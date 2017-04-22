using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	[HideInInspector]
	public GameObject master;
	void Start(){
		StartCoroutine(life());
	}
	IEnumerator life(){	
		yield return new WaitForSeconds(1);
		Destroy(gameObject);
	}	
}
