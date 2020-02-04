using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour {

	[Header ("Movement Variables")]
	public Transform CurrentNode; //Current node this vehicle is moving to
	public GameObject UsedSpawnPoint; //The spawnpoint we used for this vehicle
	public float MaxSteerAngle; //Max steering angle for this vehicle
	public WheelCollider[] WheelColliders; //Reference to our wheelcolliders
	private float CurrentTorque;
	public float MaxBrakeTorque = 2150f; //Max brake torque
	private float CurrentBrakeTorque;
	public float CurrentSpeed; 

	public bool isBraking = false; 
	public bool isSlowingDown = false;
	private float BrakeDistance; 
	public GameObject[] BackLights; //Reference to both backlights

	[Header ("Sensor Variables")]
	public float SensorLength;
	[SerializeField]private Transform FrontMiddleSensor;
	[SerializeField]private Transform FrontLeftSensor;
	[SerializeField]private Transform FrontRightSensor;


	float MaxSpeed = M.OPPMAXSPD; //Max speed this vehicle is supposed to go
	public float MaxTorque = 800f; //Max torque for our wheels

	void Start()
	{
//		if (isServer) {
//			CurrentNode = UsedSpawnPoint.GetComponent<Traffic_SpawnPoint> ().FirstNode; //Get the first node from our used spawnpoint
//		}
		MaxTorque = M.OPPMAXTORQUE;
	}

	// Update is called once per frame
	void FixedUpdate () {
		//if (isServer) 
		{ //Call these function only if we are the host client
			if (SensorLength > 2) {
				Sensors ();
			}

			if (WheelColliders == null) {
				return;

			}
			if (WheelColliders.GetLength(0) == 0) {
				return;
			}
			VehicleMove ();
			CheckForWayPoints ();
		}


	}

	void Sensors()
	{
		

		BrakeDistance = 5f; 
		RaycastHit Hit;
		//FrontMiddleSensor
		if (Physics.Raycast (FrontMiddleSensor.transform.position, FrontMiddleSensor.transform.forward, out Hit, SensorLength)) {
			if (Hit.collider.transform.root.tag == "TrafficVehicle" || Hit.collider.transform.root.tag == "Player") { //If our sensor hits a player or other traffic vehicle
				isSlowingDown = true; //Slow our speed down
				StartCoroutine (SlowDownVehicle (1f)); 
				if ((Vector3.Distance (Hit.point,FrontMiddleSensor.position) < BrakeDistance|| Hit.collider.transform.root.tag == "Player") && !isBraking) { //If we are getting close to the other vehicle, brake
					transform.parent.GetComponent<OppCreate>().SetNearPoint(transform);
					isBraking = true; //Brake
					StartCoroutine (StopVehicle (1f, false)); //Tell our vehicle to stop
				}
			}
		}
		Debug.DrawLine (FrontMiddleSensor.position, FrontMiddleSensor.position + FrontMiddleSensor.transform.forward * SensorLength); //Draw our sensor raycast line when in the editor
		//FrontLeftSensor
		if (Physics.Raycast (FrontLeftSensor.transform.position, FrontLeftSensor.transform.forward, out Hit, SensorLength)) {
			if (Hit.collider.transform.root.tag == "TrafficVehicle" || Hit.collider.transform.root.tag == "Player") { //If our sensor hits a player or other traffic vehicle
				isSlowingDown = true; //Slow our speed down
				StartCoroutine (SlowDownVehicle (1f));
				if ((Vector3.Distance (Hit.point,FrontLeftSensor.position) < BrakeDistance|| Hit.collider.transform.root.tag == "Player") && !isBraking) { //If we are getting close to the other vehicle, brake
					isBraking = true; //Brake
					StartCoroutine (StopVehicle (1f, false)); //Tell our vehicle to stop
				}
			}
		}
		Debug.DrawLine (FrontLeftSensor.position, FrontLeftSensor.position + FrontLeftSensor.transform.forward * SensorLength); //Draw our sensor raycast line when in the editor
		//FrontRightSensor
		if (Physics.Raycast (FrontRightSensor.transform.position, FrontRightSensor.forward, out Hit, SensorLength)) {
			if (Hit.collider.transform.root.tag == "TrafficVehicle" || Hit.collider.transform.root.tag == "Player") { //If our sensor hits a player or other traffic vehicle
				isSlowingDown = true; //Slow our speed down
				StartCoroutine (SlowDownVehicle (1f));
				if ((Vector3.Distance (Hit.point,FrontRightSensor.position) < BrakeDistance|| Hit.collider.transform.root.tag == "Player") && !isBraking) { //If we are getting close to the other vehicle, brake
					isBraking = true; //Brake
					StartCoroutine (StopVehicle (1f, false)); //Tell our vehicle to stop
				}
			}
		}
		Debug.DrawLine (FrontRightSensor.position, FrontRightSensor.position + FrontRightSensor.transform.forward * SensorLength); //Draw our sensor raycast line when in the editor

	}


	void VehicleMove()
	{
				//Calulate the current speed
		CurrentSpeed = 2 * Mathf.PI * WheelColliders[2].radius * WheelColliders[2].rpm * 60 / 1000;

		if (CurrentSpeed < MaxSpeed && !isBraking && !isSlowingDown) { //If this vehicle isnt yet at his top speed and isnt braking or slowing down
			if(transform.position.y > 0)
				CurrentTorque = MaxTorque*10;
			else
				CurrentTorque = MaxTorque;
		} else { 
			CurrentTorque = 0f;
		}

		//Apply torque to the back wheels
		WheelColliders [0].motorTorque = CurrentTorque; 
		WheelColliders [1].motorTorque = CurrentTorque;
		WheelColliders [2].motorTorque = CurrentTorque; 
		WheelColliders [3].motorTorque = CurrentTorque;

		Vector3 relativeVector = transform.InverseTransformPoint (CurrentNode.position); //Calculate our relative vector between our position and the next node
		float newSteer = (relativeVector.x / relativeVector.magnitude) * MaxSteerAngle; //Turn the steering wheels towards the next node while keeping the max steer angle in mind


		//Apply the new steer angle
		WheelColliders[0].steerAngle = newSteer;
		WheelColliders[1].steerAngle = newSteer;

		RpcRemoteVehicleMove (CurrentTorque, newSteer);
	}


	public void RpcRemoteVehicleMove(float torque, float steer) //Apply the motortorque and steerangle on this vehicle for all clients so the vehicle behaves better on their instances 
	{
		WheelColliders [2].motorTorque = torque;
		WheelColliders [3].motorTorque = torque;

		WheelColliders[0].steerAngle = steer;
		WheelColliders[1].steerAngle = steer;
	}

	void CheckForWayPoints ()
	{
		if(Vector3.Distance (transform.position, CurrentNode.position) < 5f && !isBraking){ //If we are a set distance from the next node and we arent braking, get the next node to move to
			GetNextNode ();
		}
	}

	void GetNextNode() //Get the next node to move to
	{
		//Debug.Log (CurrentNode.parent.name +"   " +CurrentNode.name);
		if (CurrentNode.GetComponent<PathFinder> ().Stop == true) { //If this is a stopping position
			isBraking = true; 
			StartCoroutine (StopVehicle (5f, true));
			return; //Dont go further into this function
		}
		CurrentNode = CurrentNode.GetComponent<PathFinder> ().NextNodes [Random.Range(0,CurrentNode.GetComponent<PathFinder> ().NextNodes.Count)]; //Get the next node
		//Debug.Log (CurrentNode.parent.name +" ~~~~~~~~~~~~~~~~~~~~~  " +CurrentNode.name);
	}

	IEnumerator StopVehicle(float WaitTime, bool SetNextNode)
	{
		CurrentBrakeTorque = MaxTorque;
		WheelColliders [0].brakeTorque = CurrentBrakeTorque;
		WheelColliders [1].brakeTorque = CurrentBrakeTorque;
		WheelColliders [2].brakeTorque = CurrentBrakeTorque;
		WheelColliders [3].brakeTorque = CurrentBrakeTorque;
		foreach (GameObject HL in BackLights) {  //Get every backlight from the backlights array
			HL.SetActive (true); //Set them as active
		}
		RpcRemoteStopVehicle (CurrentBrakeTorque, true);
		yield return new WaitForSeconds (WaitTime); //Stop time
		if(SetNextNode == true)
		{
			CurrentNode = CurrentNode.GetComponent<PathFinder> ().NextNodes [Random.Range(0,CurrentNode.GetComponent<PathFinder> ().NextNodes.Count)]; //Get the next node
		}
		isBraking = false; //We dont need to be braking anymore
		foreach (GameObject HL in BackLights) {  //Get every backlight from the backlights array
			HL.SetActive (false); //Set them as active
		}
		CurrentBrakeTorque = 0f;
		//Apply the brake torque on all wheels
		WheelColliders [0].brakeTorque = CurrentBrakeTorque;
		WheelColliders [1].brakeTorque = CurrentBrakeTorque;
		WheelColliders [2].brakeTorque = CurrentBrakeTorque;
		WheelColliders [3].brakeTorque = CurrentBrakeTorque;
		RpcRemoteStopVehicle (CurrentBrakeTorque, false);
	}


	public void RpcRemoteStopVehicle(float brakeTorque, bool BackLight) //Apply the braketorque and backlights stance to all clients so braking looks better and other clients can see the brake lights on traffic vehicles
	{
		WheelColliders [0].brakeTorque = brakeTorque;
		WheelColliders [1].brakeTorque = brakeTorque;
		WheelColliders [2].brakeTorque = brakeTorque;
		WheelColliders [3].brakeTorque = brakeTorque;
		foreach (GameObject HL in BackLights) {  //Get every backlight from the backlights array
			HL.SetActive (BackLight); //Invert the current stance of the brakelights
		}
	}

	IEnumerator SlowDownVehicle(float SlowDownTime)
	{
		MaxSpeed = 5; //Cap the maxspeed
		yield return new WaitForSeconds (SlowDownTime); //Slowdown time
		isSlowingDown = false;
		MaxSpeed = M.OPPMAXSPD; //Reset the capspeed to its initial value
	}
	public void setSenser(Transform trf){
		FrontLeftSensor = trf.GetChild (0);
		FrontMiddleSensor = trf.GetChild (1);
		FrontRightSensor = trf.GetChild (2);
		SensorLength = 3;
	}
}
