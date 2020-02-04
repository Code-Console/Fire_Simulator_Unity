using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OppCreate : MonoBehaviour {

	//public GameObject nCars;
	public GameObject nCarBody;
	public Transform nCarpath;
	// Use this for initialization
	void Start () {
		for (int k = 0; k < transform.childCount ; k++) {
			transform.GetChild (k).gameObject.AddComponent<Rigidbody> ();
			transform.GetChild (k).GetComponent<Rigidbody> ().mass = 2000;
			Opponent opp = transform.GetChild (k).GetComponent<Opponent> ();
			//opp.MaxSpeed = 30;
			opp.MaxSteerAngle = 75;
			opp.WheelColliders = new WheelCollider[4];
			for (int i = 0; i < nCarBody.transform.childCount && i < 3; i++) {
				GameObject body = (GameObject)Instantiate (nCarBody.transform.GetChild (i).gameObject);
				body.transform.parent = transform.GetChild (k).transform;
				body.transform.localPosition = new Vector3 (0, 0, 0);
				body.transform.localRotation = Quaternion.Euler (0, 0, 0);
				if (i == 0) {
					Debug.Log (transform.GetChild (k).transform.name +"~~~~~~s~~  "+body.name);
//					if (transform.GetChild (k).transform.name.Contains ("Bus")) {
//						Debug.Log (body.name);
//						body.GetComponent<BoxCollider> ().size = new Vector3 (3.2f, 1, 10.5f);
//						body.GetComponent<BoxCollider> ().center = new Vector3 (0, 1.0f, 0);
//					}
					if (transform.GetChild (k).transform.name.Contains ("Truck")) {
						Debug.Log (body.name);
						body.GetComponent<BoxCollider> ().size = new Vector3 (4f, 1, 13.5f);
						body.GetComponent<BoxCollider> ().center = new Vector3 (0, 1.3f, -2.3f);
					}
				}
				if (i == 1) {
					opp.setSenser(body.transform);
				}
				if (i == 2) {
					for (int j = 0; j < 4; j++) {
						opp.WheelColliders [j] = body.transform.GetChild (j).GetComponent<WheelCollider> ();
						if (transform.GetChild (k).transform.name.Contains ("Truck")) {
							body.transform.GetChild (j).transform.localPosition = new Vector3 (j%2==0?2.0f:-2.0f,.51f,j/2==0?4.6f:-3.7f);
						}
//						if (transform.GetChild (k).transform.name.Contains ("Bus")) {
//							body.transform.GetChild (j).transform.localPosition = new Vector3 (j%2==0?1.5f:-1.5f,.51f,j/2==0?4.6f:-3.7f);
//						}
					}
				}
			}
//			Debug.Log ("k = "+k);
		}

	}



	void Update () {
		
	}


	public void SetNearPoint(Transform trans){

		int rnd = Random.Range(0, nCarpath.childCount);
		int rnd2 = Random.Range(0, nCarpath.GetChild (rnd).childCount);

		trans.position = nCarpath.GetChild (rnd).position;

		trans.GetComponent<Opponent> ().CurrentNode = nCarpath.GetChild (rnd).GetChild (rnd2).transform;
		
	}


}
