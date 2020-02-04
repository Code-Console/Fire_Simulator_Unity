using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine.Advertisements;
using UnityEngine.Analytics;
public class GameCanvas : MonoBehaviour {
	AudioSource MusicBG;
	AudioSource MusicClick;
	public AudioSource MusicCiran,MusicSirenDown;
	AudioSource MusicOver;
	AudioSource MusicStart;
	InterstitialAd interstitial = null;
	int TargetFrame=60;
	void Awake()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = TargetFrame;
	}
	// Use this for initialization
	void Start () {
		setScreen(M.GAMESTART);
		MusicBG = gameObject.AddComponent<AudioSource> ();
		MusicBG.clip = (AudioClip)Resources.Load ("sound/Theme");
		MusicBG.loop = true;
		if(M.setSound)
			MusicBG.Play ();

		MusicCiran = gameObject.AddComponent<AudioSource> ();
		MusicCiran.clip = (AudioClip)Resources.Load ("sound/siren_regular");
		MusicCiran.loop = true;


		MusicClick = gameObject.AddComponent<AudioSource> ();
		MusicClick.clip = (AudioClip)Resources.Load ("sound/click");

		MusicSirenDown = gameObject.AddComponent<AudioSource> ();
		MusicSirenDown.clip = (AudioClip)Resources.Load ("sound/siren_down");


		MusicOver = gameObject.AddComponent<AudioSource> ();


		MusicStart = gameObject.AddComponent<AudioSource> ();
		MusicStart.clip = (AudioClip)Resources.Load ("sound/nextlevel");
		MusicStart.Play ();

		transform.Find ("GamePause/Image/Sound/off").gameObject.SetActive (!M.setSound);
		Analytics.CustomEvent ("GameStart_" + M.LEVEL_NO);
		RequestInterstitial ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Application.targetFrameRate != TargetFrame)
			Application.targetFrameRate = TargetFrame;
		if (M.GameScreen == M.GAMEOVER) 
		{
			transform.Find ("GameOver/Watch/ray0").rotation *= Quaternion.Euler (0, 0, 1);
			transform.Find ("GameOver/Watch/ray1").rotation *= Quaternion.Euler (0, 0, -1);
		}
	}
	public void OnClickGame(int val){
		switch (val) {
		case 0://Pause
			MusicBG.Pause ();
			MusicCiran.Pause ();
			Time.timeScale = 0;
			setScreen (M.GAMEPAUSE);
			ShowInterstitial ();
			break;
		case 1://Camera
			break;
		case 2://horn
			break;
		case 3://start
			setScreen(M.GAMEPLAY);
			setStart();
			if(M.setSound)
				MusicCiran.Play();
			break;
		}
		if (M.setSound)
			MusicClick.Play ();
	}
	public void OnClickPause(int val){
		switch (val) {
		case 0://Contine
			if (M.setSound) {
				MusicBG.Play ();
				if(!M.isParked)
					MusicCiran.Play ();
			}
			Time.timeScale = 1;
			setScreen (M.GAMEPLAY);
			break;
		case 1://Restart
			Time.timeScale = 1;
			gameRestart();
			break;
		case 2://Menu
			Time.timeScale = 1;
			M.GameScreen = M.GAMEMENU;
			SceneManager.LoadScene(0);
			break;
		case 3://Music
			//M.setMusic = transform.Find("GamePause/Music").GetComponent<Toggle>().isOn;
			break;
		case 4://Sound
			//M.setMusic = transform.Find("GamePause/Sound").GetComponent<Toggle>().isOn;
			M.setSound = !M.setSound;
			transform.Find ("GamePause/Image/Sound/off").gameObject.SetActive (!M.setSound);
			Debug.Log ("M.setSound" + M.setSound);
			break;
		}
		if (M.setSound)
			MusicClick.Play ();
	}

	public void OnClickGameOver(int val){
		switch (val) {
		case 0://Shop
			M.GameScreen = M.GAMESHOP;
			SceneManager.LoadScene(0);
			break;
		case 1://Menu
			M.GameScreen = M.GAMEMENU;
			SceneManager.LoadScene(0);
			break;
		case 2://Retry
			gameRestart();
			break;
		case 3://Next Level
			//if (M.UNLOCKLEVEL > M.LEVEL_NO && M.LEVEL_NO < M.STARS.GetLength(0)-1)
				//M.LEVEL_NO++;
			//gameRestart();
			M.GameScreen = M.GAMESHOP;
			SceneManager.LoadScene(0);
			break;
		case 4:case 5://Watch
			if (Advertisement.IsReady ("rewardedVideo")) {
				var options = new ShowOptions { resultCallback = HandleShowResult };
				Advertisement.Show ("rewardedVideo", options);
				MusicBG.Pause ();

			} 
			break;
		case 6:
			M.STIME = Time.timeSinceLevelLoad - M.STIME;
			setScreen (M.GAMEOVER);
			break;

		}
	}


	void setStart(){
		M.STIME = Time.timeSinceLevelLoad;
		M.GameScreen = M.GAMEPLAY;


	}
	public void setScreen(int scr){
		M.GameScreen = scr;
		transform.Find ("GameStart").gameObject.SetActive (M.GameScreen == M.GAMESTART);
		transform.Find ("Gameplay").gameObject.SetActive (M.GameScreen == M.GAMEPLAY);
		transform.Find ("GamePause").gameObject.SetActive (M.GameScreen == M.GAMEPAUSE);
		transform.Find ("GameOver").gameObject.SetActive (M.GameScreen == M.GAMEOVER||M.GameScreen == M.GAMEWIN);
		Debug.Log ("M.GameScreen = " + M.GameScreen);
		if (M.GameScreen == M.GAMEOVER || M.GameScreen == M.GAMEWIN) {
			gameover ();
		}
	}

	public void gameRestart(){
		GameObject.Find ("Main Camera").GetComponent<ThirdPersonCamera> ().reset ();
		GameObject.Find ("PlayrerCar").GetComponent<Player> ().reset ();
		setScreen (M.GAMESTART);
		if(M.setSound)
			MusicStart.Play ();
		RequestInterstitial ();
	}
	public void gameover(){
		
		int tim = (int)(M.GTIME - Time.timeSinceLevelLoad + M.STIME);



		MusicCiran.Pause ();
		Transform goscr = transform.Find ("GameOver").transform;
		goscr.Find ("Watch/Score").GetComponent<Text> ().text = (int)(M.DISTANCE*79) + "";


		float getstars = (tim/M.GTIME)*100f;
		goscr.Find ("SCORE/UNIT/DIS").GetComponent<Text> ().text 	= (int)(M.DISTANCE/1000)+"."+((int)(M.DISTANCE)%1000)/100+ " km";
		goscr.Find ("SCORE/UNIT/TIME").GetComponent<Text> ().text 	= ((tim / 60 < 10 ? "0" : "") + tim / 60 + (tim % 60 < 10 ? ":0" : ":") + tim % 60) + " min";
		goscr.Find ("SCORE/UNIT/WOTER").GetComponent<Text> ().text 	= (int)(transform.Find ("Gameplay/P1/TOP/Woter/Image").GetComponent<Image> ().fillAmount*M.MAXWOTER)+"L";
		int tim2 = (int)M.HTIME;
		goscr.Find ("SCORE/UNIT/SPEED").GetComponent<Text> ().text 	= ((tim2 / 60 < 10 ? "0" : "") + tim2 / 60 + (tim2 % 60 < 10 ? ":0" : ":") + tim2 % 60) + " min";


		goscr.Find ("SCORE/CASH/DIS").GetComponent<Text> ().text 	= (int)(M.DISTANCE/30)+ "";
		goscr.Find ("SCORE/CASH/SPEED").GetComponent<Text> ().text 	= (tim2*40) + "";
		int total = 0;
		if (M.GAMEWIN == M.GameScreen) {
			goscr.Find ("SCORE/CASH/TIME").GetComponent<Text> ().text = (tim * 5) + "";
			goscr.Find ("SCORE/CASH/WOTER").GetComponent<Text> ().text = (int)(transform.Find ("Gameplay/P1/TOP/Woter/Image").GetComponent<Image> ().fillAmount * M.MAXWOTER*.4f) + "";
			total = (int)(M.DISTANCE / 30) + (tim * 5) + (int)(transform.Find ("Gameplay/P1/TOP/Woter/Image").GetComponent<Image> ().fillAmount * M.MAXWOTER*.4f) + (tim2 * 40);
		} else {
			goscr.Find ("SCORE/CASH/TIME").GetComponent<Text> ().text = (tim * 10) + "0";
			goscr.Find ("SCORE/CASH/WOTER").GetComponent<Text> ().text = "0";
			total = (int)(M.DISTANCE / 30) + (tim2 * 40);
		}


		goscr.Find ("SCORE/TOTAL/CASH").GetComponent<Text> ().text 	= total+ "$";

		M.COINS += total;

		goscr.Find ("Watch/totleCoin").GetComponent<Text> ().text 	= M.COINS+ "";

		goscr.Find ("Watch/title").GetComponent<Text> ().text 		= (M.GameScreen == M.GAMEWIN) ? "GAME WIN" : "GAME OVER";

		if (M.GameScreen == M.GAMEWIN) {
			if (M.UNLOCKLEVEL <= M.LEVEL_NO + 1) {
				M.UNLOCKLEVEL = M.LEVEL_NO + 2;
			}
			M.STARS [M.LEVEL_NO] = (getstars > 20) ? 5 : ((int)(getstars / 5) + 1);

			Debug.Log (M.STARS [M.LEVEL_NO] + "   ! " + M.LEVEL_NO + " ! " + M.UNLOCKLEVEL);

			MusicOver.clip = (AudioClip)Resources.Load ("sound/level_win");
			MusicOver.Play ();

			Analytics.CustomEvent ("GameWin_" + M.LEVEL_NO);

		} else {
			Analytics.CustomEvent ("GameOver_" + M.LEVEL_NO);
			if (M.setSound) {
				MusicOver.clip = (AudioClip)Resources.Load ("sound/game_over");
				MusicOver.Play ();
			}
		}

		M.isWoter = 0;
		goscr.Find ("Bottom/Next").gameObject.SetActive(M.LEVEL_NO < M.STARS.GetLength(0)-1 && M.GameScreen == M.GAMEWIN);
		M.Save ();
		ShowInterstitial ();
	}
	private void RequestInterstitial ()
	{
		#if UNITY_IPHONE
		string adUnitIdInter = "ca-app-pub-3395412980708319/9064332115";
		#else
		string adUnitIdInter = "ca-app-pub-7665074309496944/1640960059";
		#endif

		// Clean up interstitial ad before creating a new one.
		if (this.interstitial != null) {
			if (this.interstitial.IsLoaded())
			{
				return;
			}
			this.interstitial.Destroy ();
		}

		// Create an interstitial.
		this.interstitial = new InterstitialAd (adUnitIdInter);

		// Register for ad events.
		this.interstitial.OnAdLoaded += this.HandleInterstitialLoaded;
		this.interstitial.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;
		this.interstitial.OnAdOpening += this.HandleInterstitialOpened;
		this.interstitial.OnAdClosed += this.HandleInterstitialClosed;
		this.interstitial.OnAdLeavingApplication += this.HandleInterstitialLeftApplication;

		// Load an interstitial ad.
		this.interstitial.LoadAd (this.CreateAdRequest ());
	}

	public void HandleInterstitialLoaded (object sender, System.EventArgs args)
	{
		MonoBehaviour.print ("HandleInterstitialLoaded event received");
	}

	public void HandleInterstitialFailedToLoad (object sender, AdFailedToLoadEventArgs args)
	{
		MonoBehaviour.print (
			"HandleInterstitialFailedToLoad event received with message: " + args.Message);
	}

	public void HandleInterstitialOpened (object sender, System.EventArgs args)
	{
		MonoBehaviour.print ("HandleInterstitialOpened event received");
		#if UNITY_IPHONE
		MusicBG.Pause ();
		#endif

	}

	public void HandleInterstitialClosed (object sender, System.EventArgs args)
	{
		MonoBehaviour.print ("HandleInterstitialClosed event received");
		#if UNITY_IPHONE
		if(M.setSound)
		MusicBG.Play ();
		#endif
	}

	public void HandleInterstitialLeftApplication (object sender, System.EventArgs args)
	{
		MonoBehaviour.print ("HandleInterstitialLeftApplication event received");
	}
	private AdRequest CreateAdRequest()
	{
		return new AdRequest.Builder()
			.AddTestDevice(AdRequest.TestDeviceSimulator)
			.AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
			.AddKeyword("game")
			.SetBirthday(new System.DateTime(1985, 1, 1))
			.TagForChildDirectedTreatment(false)
			.AddExtra("color_bg", "9B30FF")
			.Build();
	}
	private void ShowInterstitial()
	{
		Debug.Log("~~~ShowInterstitial~~");
		if (M.isAds) {
			if (this.interstitial.IsLoaded ()) {
				this.interstitial.Show ();

			} else {
				if (Advertisement.IsReady ("video")) {
				
					Advertisement.Show ("video", null);
					#if UNITY_IPHONE
					MusicBG.Pause ();
					#endif
				} 
			}
		}
	}
	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log ("Unity The ad was successfully shown.");
			if (M.isWoter == 0) {
				M.COINS += 500;
			}
			if (M.isWoter == 1) {
				M.STIME = Time.timeSinceLevelLoad - M.STIME;
				transform.Find ("Gameplay/P1/TOP/Woter/Image").GetComponent<Image> ().fillAmount = 1;
				transform.Find ("Gameplay/Revive").gameObject.SetActive (false);
				M.GameScreen = M.GAMEPLAY;
			}
			if (M.isWoter == 2) {
				M.STIME = Time.timeSinceLevelLoad - M.STIME;
				M.GTIME += 65; 
				transform.Find ("Gameplay/Revive").gameObject.SetActive (false);
				M.GameScreen = M.GAMEPLAY;
				if(M.setSound)
					MusicCiran.Play ();
			}
			transform.Find ("GameOver/Watch/totleCoin").GetComponent<Text> ().text = M.COINS + "";
			if(M.setSound)
				MusicBG.Play ();
			
			M.Save ();
			break;
		case ShowResult.Skipped:
			Debug.Log("Unity  The ad was skipped before reaching the end.");
			transform.Find ("GameOver").transform.GetComponent<Animator> ().SetBool ("isOpen", true);
			#if UNITY_IPHONE
			if(M.setSound)
			MusicBG.Play ();
			#endif
			break;
		case ShowResult.Failed:
			Debug.LogError("Unity  The ad failed to be shown.");
			transform.Find ("GameOver").transform.GetComponent<Animator> ().SetBool ("isOpen", true);
			break;
		}
	}
}
