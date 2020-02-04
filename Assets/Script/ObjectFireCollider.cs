using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFireCollider : MonoBehaviour {

	public ParticleSystem ps;
	//public GameObject sourse;
	public float gravity = 1;
	public float speed =1;
	// Use this for initialization
	void Start () {

		change ();

	}
	
	// Update is called once per frame
//	void Update () {
//		ParticleSystem.Particle[] p = new ParticleSystem.Particle[ps.particleCount+1];
//		int l = ps.GetParticles(p);
//
//		speed = l;
//
//
//
//
//		int k = 1;
//		for (int i = 0; i < l - 1; i++) {
//			if (Mathf.Abs (p [i].position.y) < Mathf.Abs (p [k].position.y)) {
//				k = i;
//			}
//		}
//		gravity = k;
//		if(k <= l)
//			transform.position = p[k].position;
//
//
//		for(int i = 0; i < count; i++)
//		{
//			float yVel = (particles[i].lifetime / particles[i].startLifetime) * distance;
//			particles[i].velocity = new Vector3(0, yVel, 0);
//		}
//
//		particleSystem.SetParticles(particles, count);
//
//
//	}



	public float distance = 3;
	void LateUpdate0 ()
	{
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
		int count = ps.GetParticles(particles);
		for(int i = 0; i < count; i++)
		{
			float yVel = (particles[i].remainingLifetime / particles[i].startLifetime) * distance;
			particles[i].velocity = new Vector3(0, yVel, 0);
		}

//		ParticleSystem.EmissionModule em = ps.emission;
//		em.rateOverTime = 1;

		ps.SetParticles(particles, count);

	}


	void change(){
		// Get the Velocity over lifetime modult
		ParticleSystem.VelocityOverLifetimeModule snowVelocity = ps.velocityOverLifetime;

		//And to modify the value
		//ParticleSystem.MinMaxCurve rate = new ParticleSystem.MinMaxCurve();
		//rate.constantMax = 10.0f; // or whatever value you want
		snowVelocity.x = 0;
		snowVelocity.y = 0;
		snowVelocity.z = 5;
	}

}
