using UnityEngine;
using System.Collections;

// Helper functions for animation, called by GameManager

public class AnimationC : MonoBehaviour {
	
	public GameManager gm;
	public GameObject target;
	
	void stopwalk()
	{
		gameObject.animation.CrossFade("idle");
	}
	
	void stopharvest()
	{
		gameObject.animation.CrossFade("idle");
		gm.TryPickup(target.name);
	}
	
	void startwalk()
	{
		gameObject.animation.CrossFade("walking");
	}
	
}
