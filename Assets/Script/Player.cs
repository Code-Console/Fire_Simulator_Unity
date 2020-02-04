using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour {
	
	int direction = 0;
	bool brake = false;
	bool Acc = false;

	bool rotUp = false;
	bool rotDwon = false;
	bool rotLeft = false;
	bool rotRight = false;
	public GameObject mArrow;
	public Transform mParkingPlace;
	public Transform mTarget;
	public Transform mParking;
	public Transform mGPCanvas;
	public Vector3 vWoter = new Vector3(0,0,0);
	Transform mWoter;
	public ParticleSystem mPS;
	CarController cCtrl;
	Sprite mSSel, mSDsel;
	float mEmissionRate = 0;
	int counter = 0;
	float lastTime = 0;

	bool isGameover = false;


	AudioSource MusicImpact,MusicRot,MusicWoter;

	void Start () {
		cCtrl = GetComponent<CarController> ();
		mArrow = (GameObject)Instantiate (mArrow,transform.position, transform.rotation);
		mArrow.transform.parent = transform;
		mArrow.transform.localPosition = new Vector3 (0, 5, 5);
		mSSel = (Sprite)Resources.Load<Sprite> ("ui/on");
		mSDsel  = (Sprite)Resources.Load<Sprite> ("ui/off");
		MusicImpact = gameObject.AddComponent<AudioSource> ();
		MusicImpact.clip = (AudioClip)Resources.Load ("sound/impact");

		MusicRot = gameObject.AddComponent<AudioSource> ();
		MusicRot.clip = (AudioClip)Resources.Load ("sound/pump_rotaion");

		MusicWoter = gameObject.AddComponent<AudioSource> ();
		MusicWoter.clip = (AudioClip)Resources.Load ("sound/water");


		reset ();
	}

	public void reset(){
		transform.position = new Vector3 (66, 0, -270);
		transform.rotation = Quaternion.Euler (0,90,0);
		mGPCanvas.Find ("P0").gameObject.SetActive (true);
		mGPCanvas.Find ("P1").gameObject.SetActive (false);
		mGPCanvas.Find ("P0/BottomRight/Parking").gameObject.SetActive (false);
		M.isParked = false;
		ParticleSystem.EmissionModule em = mPS.emission;
		em.rateOverTime = 5;
		mPS.gameObject.SetActive (false);
		vWoter = new Vector3(0,0,0);
		mEmissionRate = 100;
		M.GTIME = M.LEVELTIME[M.LEVEL_NO]; 
		M.STIME = Time.timeSinceLevelLoad;
		M.HTIME = 0;
		lastTime = Time.timeSinceLevelLoad;
		mTarget.gameObject.SetActive (true);
		Debug.Log ("~~~~~~~~~~~~~Gamereset "+mPS.gameObject.activeInHierarchy);
		counter = 0;
		M.DISTANCE = 0;
		mParking.transform.parent = mParkingPlace.GetChild (M.LEVEL_NO).GetChild (0).transform;
		mParking.transform.localRotation = Quaternion.Euler (0,0,0);
		mParking.transform.localPosition = new Vector3 (0,0,0);
		mParking.Find ("Direction").gameObject.SetActive (true);


		mTarget.transform.parent = mParkingPlace.GetChild (M.LEVEL_NO).transform;
		mTarget.transform.localRotation = Quaternion.Euler (0,0,0);
		mTarget.transform.localPosition = new Vector3 (0,0.02f,0);
		ParticleSystem.EmissionModule fireEM = mTarget.GetChild(0).GetComponent<ParticleSystem> ().emission;
		if (mEmissionRate > 0) {
			fireEM.rateOverTime = mEmissionRate;
			mGPCanvas.Find ("P1/TOP/FIre/Image").GetComponent<Image> ().fillAmount = mEmissionRate*.01f; 
		}
		mGPCanvas.Find ("P1/TOP/Woter/Image").GetComponent<Image> ().fillAmount = 1;
		mGPCanvas.Find ("Revive").gameObject.SetActive (false);
		ParticleSystem.VelocityOverLifetimeModule velocity = mPS.velocityOverLifetime;
		mGPCanvas.Find ("P1/BottomRight/Slider").GetComponent<Slider> ().value = (velocity.z.constant -5)*.1f;


		if (mWoter != null) {
			vWoter = new Vector3(0,0,0);
			mWoter.localRotation = Quaternion.Euler (0,vWoter.y, 0);
			mWoter.GetChild(0).GetChild(0).localRotation = Quaternion.Euler (vWoter.x, 0, 0);
		}
		isGameover = false;
		M.isWoter = 0;
		mArrow.SetActive (!M.isParked);
	}


	public void setPS(Transform trf){
		mWoter = trf;
		mPS.transform.parent = mWoter.GetChild(0).GetChild(0).transform;
		mPS.transform.localRotation = Quaternion.Euler (90,0,0);
		mPS.transform.localPosition = new Vector3 (0,0,0);
	}



	void Update () {

		if (M.GameScreen == M.GAMEPLAY) {

			float turn = Input.GetAxis ("Horizontal") * .6f;
			float Accre = Input.GetAxis ("Vertical") * 10;
			if (direction != 0) {
				turn = direction * 1.6f;
			}

			if (Acc ||brake) {
				Accre = 10;
			}

			if (M.isParked) {
				if (rotLeft || rotRight) {
					mWoter.localRotation = Quaternion.Euler (0, rotRight ? vWoter.y++ : vWoter.y--, 0);
				}
				if (rotUp || rotDwon) {
					mWoter.GetChild(0).GetChild(0).localRotation = Quaternion.Euler (rotDwon ? vWoter.x++ : vWoter.x--, 0, 0);
				}
				if(M.setSound &&(rotLeft||rotRight||rotUp||rotDwon)){
					MusicRot.Play();
				}
				cCtrl.Move (0, 0, 0, 10000);
				if (mPS.gameObject.activeInHierarchy) {
					if(M.setSound && !MusicWoter.isPlaying)
						MusicWoter.Play ();
					mGPCanvas.Find ("P1/TOP/Woter/Image").GetComponent<Image> ().fillAmount -= .001f/(M.PLAYER_NO+1+M.UPGEDE [M.PLAYER_NO]*.2f); 
					if (mGPCanvas.Find ("P1/TOP/Woter/Image").GetComponent<Image> ().fillAmount <= 0) {
						mPS.gameObject.SetActive (false);
						Revive (1);//mGPCanvas.parent.GetComponent<GameCanvas> ().setScreen (M.GAMEOVER);
					}
					ParticleSystem.Particle[] particles = new ParticleSystem.Particle[mPS.particleCount];
					int c0nt = mPS.GetParticles (particles);
					for (int i = 0; i < c0nt; i++) {
					
						if (Vector3.Distance (particles [i].position, mTarget.position) < 2) {
							ParticleSystem.EmissionModule em = mTarget.GetChild(0).GetComponent<ParticleSystem> ().emission;
							if (mEmissionRate > 0) {
								em.rateOverTime = mEmissionRate;
								mEmissionRate-=1.2f/(M.LEVEL_NO+1+M.UPGEDE [M.PLAYER_NO]*.2f);
								mGPCanvas.Find ("P1/TOP/FIre/Image").GetComponent<Image> ().fillAmount = mEmissionRate*.01f; 
								if (mEmissionRate < 1) {
									em.rateOverTime = 0;
									counter = 0;
								}
							} else {
								if (counter > 50) {
									mPS.gameObject.SetActive (false);
									mTarget.gameObject.SetActive (false);
									mGPCanvas.parent.GetComponent<GameCanvas> ().setScreen (M.GAMEWIN);
								}
							}
							break;
						}
					}
				}
				//ParticleSystem.EmissionModule em = ps.emission;
				//em.rateOverTime = 1;

			} else {
				if (Vector3.Distance (transform.position, mParking.position) < 10) {
					if (Vector3.Distance (transform.position, mParking.position) < 3) {
						mGPCanvas.Find ("P0/BottomRight/Parking").gameObject.SetActive (true);
					} else {
						mGPCanvas.Find ("P0/BottomRight/Parking").gameObject.SetActive (false);
					}

				} 

				//cCtrl.Move (turn, Accre, Accre, brake ? 1000 : 0);
				//Debug.Log((brake ?-Accre:Accre)+"   "+Acc);
				cCtrl.Move (turn, brake ?-Accre:Accre, brake ?-Accre:Accre, 0);
				mArrow.transform.LookAt (mTarget);
			}

			if (counter % 10 == 0) {
				int tim = (int)(M.GTIME - Time.timeSinceLevelLoad + M.STIME);
				if (tim < 0) {
					Revive (2);//mGPCanvas.parent.GetComponent<GameCanvas> ().setScreen (M.GAMEOVER);
				}


				mGPCanvas.Find ("TopLeft/Time/Text").GetComponent<Text> ().text = ((tim / 60 < 10 ? "0" : "") + tim / 60 + (tim % 60 < 10 ? ":0" : ":") + tim % 60) + " min";
				tim = (int)(Time.timeSinceLevelLoad - M.STIME);
				mGPCanvas.Find ("P0/BottomCenter/Meter/Sec").GetComponent<Text> ().text = ((tim / 60 < 10 ? "0" : "") + tim / 60 + (tim % 60 < 10 ? ":0" : ":") + tim % 60) + " min";
				mGPCanvas.Find ("P0/BottomCenter/Meter/spd").GetComponent<Text> ().text = (int)cCtrl.CurrentSpeed + "";
				mGPCanvas.Find ("P0/BottomCenter/Meter/Dis").GetComponent<Text> ().text = (int)(M.DISTANCE/1000)+"."+((int)(M.DISTANCE)%1000)/100+ " km";
				mGPCanvas.Find ("P0/BottomCenter/Meter/Kata").transform.rotation = Quaternion.Euler(0,0,-cCtrl.CurrentSpeed);


			}

			if (cCtrl.CurrentSpeed > 30) {
				
				M.HTIME += Time.timeSinceLevelLoad-lastTime;
			}
			lastTime = Time.timeSinceLevelLoad;
			counter++;

		}
	}
	public void onclickCar(int val){
		switch(val){
		case 0://LeftDown
			direction = -1;
			break;
		case 1://LeftUp
			direction = 0;
			break;
		case 2://RightDwon
			direction = 1;
			break;
		case 3://RightUp
			direction = 0;
			break;
		case 4://AcccDown
			cCtrl.breakUp();
			Acc = true;
			break;
		case 5://AcUp
			Acc = false;
			break;
		case 6://Rightdown
			brake = true;
			break;
		case 7://Rightup
			brake = false;
			break;


		case 8://LeftRot down Woter
			rotLeft = true;
			break;
		case 9://LeftRot Up Woter
			rotLeft = false;
			break;
		case 10://RightRot Down Woter
			rotRight = true;
			break;
		case 11://RightRot Up Woter
			rotRight = false;
			break;
		case 12://UpRot down Woter
			rotUp = true;
			break;
		case 13://uprot Down Woter
			rotUp = false;
			break;
		case 14://DownRot down Woter
			rotDwon = true;
			break;
		case 15://Downrot up Woter
			rotDwon = false;
			break;
		case 16://Parking
			mGPCanvas.Find ("P0").gameObject.SetActive (false);
			mGPCanvas.Find ("P1").gameObject.SetActive (true);
			M.isParked = true;
			mGPCanvas.Find ("P1/BottomRight/Woter").GetComponent<Image> ().sprite = mPS.gameObject.activeInHierarchy ? mSSel : mSDsel;
			Debug.Log ("Parking " + mPS.gameObject.activeInHierarchy);
			mParking.Find ("Direction").gameObject.SetActive (false);
			mArrow.SetActive (!M.isParked);
			if (M.setSound) {
				mGPCanvas.parent.GetComponent<GameCanvas> ().MusicCiran.Pause ();
				mGPCanvas.parent.GetComponent<GameCanvas> ().MusicSirenDown.Play ();
			}
			break;
		case 17://SLider
			Debug.Log( mGPCanvas.Find ("P1/BottomRight/Slider").GetComponent<Slider> ().value+"  !");
			{
				ParticleSystem.VelocityOverLifetimeModule velocity = mPS.velocityOverLifetime;
				velocity.z = 5 + mGPCanvas.Find ("P1/BottomRight/Slider").GetComponent<Slider> ().value * 10;

			}
			Debug.Log ("Slider "+mPS.gameObject.activeInHierarchy);
			break;
		case 18:
			mPS.gameObject.SetActive (!mPS.gameObject.activeInHierarchy);
			mGPCanvas.Find ("P1/BottomRight/Woter").GetComponent<Image> ().sprite = mPS.gameObject.activeInHierarchy ? mSSel : mSDsel;


			Debug.Log ("On Woter "+mPS.gameObject.activeInHierarchy);
			break;
		}
	}


	void OnCollisionEnter (Collision col) {

		Debug.Log (col.transform.tag+"   sf");
		if (col.transform.tag == "TrafficVehicle") {
			if (M.setSound) {
				MusicImpact.Play ();
			}
		}


	}


	void Revive(byte isWoter){
		M.isWoter = isWoter;
		if (isGameover) {
			mGPCanvas.parent.GetComponent<GameCanvas> ().setScreen (M.GAMEOVER);
		} else {
			mGPCanvas.parent.GetComponent<GameCanvas> ().MusicCiran.Pause ();
			M.GameScreen = M.GAMEREVIVE;
			M.STIME = Time.timeSinceLevelLoad - M.STIME;
			mGPCanvas.Find ("Revive").gameObject.SetActive (true);
			if(isWoter == 2){
				mGPCanvas.Find ("Revive/Image/Text").GetComponent<Text>().text = "WATCH A VIDEO TO GET 60 SEC.";
			}else{
				mGPCanvas.Find ("Revive/Image/Text").GetComponent<Text>().text = "WATCH A VIDEO TO REFILL WATER TANK";
			}
			isGameover = true;
		}
	}


//	void setRotationOfWoter(){
//		if (rotLeft) {
//			transform.Find ("Cylinder").localRotation *= Quaternion.Euler (0, 1, 0);
//		}
//		if (rotRight) {
//			transform.Find ("Cylinder").localRotation *= Quaternion.Euler (0, -1, 0);
//		}
//		if (rotUp) {
//			transform.Find ("Cylinder").localRotation *= Quaternion.Euler (1, 0, 0);
//		}
//		if (rotDwon) {
//			transform.Find ("Cylinder").localRotation *= Quaternion.Euler (-1, 0, 0);
//		}
//	}
}
