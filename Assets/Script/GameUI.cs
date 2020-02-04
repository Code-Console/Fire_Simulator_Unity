using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine.Advertisements;
//http://u3d.as/HgR
//https://www.assetstore.unity3d.com/en/#!/content/73497
//http://u3d.as/E88
public class GameUI : MonoBehaviour {
	Animator _Animtor;
	public Transform mFireBrigade;
	Sprite mSSel, mSDsel;
	Sprite mSStarSel, mSStarDel;
	AudioSource MusicClick,MusicBG;
	InterstitialAd interstitial = null;
	// Use this for initialization
	void Start () {
		mSSel = (Sprite)Resources.Load<Sprite> ("ui/colour_btn_select");
		mSDsel  = (Sprite)Resources.Load<Sprite> ("ui/colour_btn");
		mSStarSel = (Sprite)Resources.Load<Sprite> ("ui/star00");
		mSStarDel  = (Sprite)Resources.Load<Sprite> ("ui/star01");
		MusicClick = gameObject.AddComponent<AudioSource> ();
		MusicClick.clip = (AudioClip)Resources.Load ("sound/click");

		MusicBG = gameObject.AddComponent<AudioSource> ();
		MusicBG.clip = (AudioClip)Resources.Load ("sound/game_ui");
		MusicBG.loop = true;
		if (M.setSound) {
			MusicBG.Play ();
		}

		M.Open ();

		setLevel ();
		setScreen (M.GameScreen);
		for(int i =0;i<mFireBrigade.childCount;i++){
			mFireBrigade.GetChild (i).gameObject.SetActive (i == M.PLAYER_NO);
		}
		transform.Find ("Menu/Sound/off").gameObject.SetActive (!M.setSound);
		RequestInterstitial ();

		if (www == null) {
			#if UNITY_IPHONE
			www = new WWW("http://hututusoftwares.com/Link/iphone.html");
			#elif UNITY_ANDROID
			www = new WWW("http://hututusoftwares.com/Link/android.html");
			#endif

		}
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (M.GameScreen == M.GAMEMENU) {
				setScreen (M.GAMEEXIT);
			}
			else {
				setScreen (M.GAMEMENU);
			}
		}
	}
	public void OnClickMenu(int val){
		switch (val) {
		case 0://Rate
			#if UNITY_IPHONE
			Application.OpenURL ("https://itunes.apple.com/us/developer/yogesh-bangar/id1199008030");
			#else
			Application.OpenURL ("https://play.google.com/store/apps/details?id=" + Application.identifier);
			#endif
			break;
		case 1://info
			Application.OpenURL ("http://www.hututusoftwares.com/");
			break;
		case 2://facebook
			Application.OpenURL ("https://www.facebook.com/hututusoftwares/");
			break;
		case 3://twiter
			Application.OpenURL ("https://twitter.com/hututu_games");
			break;
		case 4://G+
			Application.OpenURL ("https://plus.google.com/u/0/+Hututugames");
			break;
		case 5:
			Animator anim = transform.Find ("Menu/popbut").GetComponent<Animator> ();
			anim.SetBool ("isOpen", !anim.GetBool ("isOpen"));
			break;
		case 6://More Games
#if ANDROID
			Application.OpenURL ("https://play.google.com/store/apps/details?id=com.onedaygames24.game.trainsimulatordriver_2017");
#else
				Application.OpenURL ("https://itunes.apple.com/us/developer/yogesh-bangar/id1199008030");
			#endif
			break;
		case 7://Play
			setScreen (M.GAMESHOP);
			ShowInterstitial ();
			break;
		case 8://LeaderBoard
			break;
		case 9://Setting
			transform.Find ("Menu/SettingPopUp").GetComponent<Animator> ().SetBool ("isOpen", true);
			break;
		case 10://Achivement
			break;
		case 11://Music
			break;
		case 12://Sound
			M.setSound = !M.setSound;
			transform.Find ("Menu/Sound/off").gameObject.SetActive (!M.setSound);

			if (M.setSound) {
				MusicBG.Play ();
			} else {
				MusicBG.Pause ();
			}
			break;
		case 13://Steering
			break;
		case 14://Close
			transform.Find ("Menu/SettingPopUp").GetComponent<Animator> ().SetBool ("isOpen", false);
			break;
		case 15://Exit
			Debug.Log("Exit");
			setScreen(M.GAMEEXIT);
			break;
		}

		if (M.setSound) {
			MusicClick.Play ();
		}
	}



	public void OnClickShop(int val){
		switch (val) {
		case 0://back
			for (int i = M.FIRECOST.GetLength (0) - 1; i >= 0; i--) {
				if (M.FIRECOST [i] == 0) {
					M.PLAYER_NO = i;
					break;
				}
			}
			Debug.Log (M.PLAYER_NO);
			for(int i =0;i<mFireBrigade.childCount;i++){
				mFireBrigade.GetChild (i).gameObject.SetActive (i == M.PLAYER_NO);
			}

			setScreen (M.GAMEMENU);
			break;
		case 1://play
			Debug.Log(M.FIRECOST [M.PLAYER_NO]+"   "+M.PLAYER_NO);
			if (M.FIRECOST [M.PLAYER_NO] == 0)
				setScreen(M.GAMELEVEL);
			break;
		case 2://Left
			M.PLAYER_NO--;
			if (M.PLAYER_NO < 0) {
				M.PLAYER_NO = mFireBrigade.childCount - 1;
			}
			setShop();
			break;
		case 3://Right
			M.PLAYER_NO++;
			M.PLAYER_NO %= mFireBrigade.childCount;
			setShop();
			break;
		case 4://Updrade
			if (M.UPGEDE [M.PLAYER_NO] < 3) {
				if (M.FIRECOST [M.PLAYER_NO] != 0) {
					if (M.COINS >= M.FIRECOST [M.PLAYER_NO]) {
						M.COINS -= M.FIRECOST [M.PLAYER_NO];
						M.FIRECOST [M.PLAYER_NO] = 0;
					} else {
						transform.Find ("Shop/Massge").GetComponent<Animator> ().SetBool ("isOpen", true);
					}
				} else {
					if (M.COINS >= (M.PLAYER_NO + 1) * 200f + (M.UPGEDE [M.PLAYER_NO] + 1) * 200) {
						M.UPGEDE [M.PLAYER_NO]++;
						M.COINS -= (M.PLAYER_NO + 1) * 200 + (M.UPGEDE [M.PLAYER_NO] + 1) * 200;
					} else {
						transform.Find ("Shop/Massge").GetComponent<Animator> ().SetBool ("isOpen", true);
					}
				}
			}
			M.Save ();
			setShop();
			break;
		case 5:
			transform.Find ("Shop/Massge").GetComponent<Animator> ().SetBool ("isOpen",false);
			break;
		case 6:
			transform.Find ("Shop/Massge").GetComponent<Animator> ().SetBool ("isOpen", false);
			setScreen (M.GAMEIAP);
			break;
		case 7:
			setScreen (M.GAMEIAP);
			break;
		case 8:
			transform.Find ("IAP/Massge").GetComponent<Animator> ().SetBool ("isOpen", false);
			break;
		}
		if (M.setSound) {
			MusicClick.Play ();
		}
	}

	public void setLevel(){ 
		Transform container = transform.Find ("Location/Levels/Container").transform;
		for (int i = 0; i< container.childCount; i++) {
			for (int j = 0; j < container.GetChild (i).childCount; j++) {
				int level = (i * 6 + j);
				container.GetChild (i).GetChild (j).GetChild (0).GetComponent<Text> ().text = "Level : " + (level+1);
				container.GetChild (i).GetChild (j).GetComponent<Image> ().sprite = (Sprite)Resources.Load<Sprite> ("ui/level_"+(i*6+j));
				for(int k =0;k<container.GetChild (i).GetChild (j).GetChild (1).transform.childCount;k++){
					container.GetChild (i).GetChild (j).GetChild (1).GetChild(k).GetComponent<Image> ().sprite = M.STARS[level] > k ? mSStarSel :mSStarDel;
				}
				container.GetChild (i).GetChild (j).GetChild (2).gameObject.SetActive (M.UNLOCKLEVEL<=level);
				container.GetChild (i).GetChild (j).gameObject.AddComponent<Button> ();
				container.GetChild (i).GetChild (j).gameObject.GetComponent<Button> ().transition = Selectable.Transition.Animation;
				container.GetChild (i).GetChild (j).gameObject.GetComponent<Button> ().onClick.AddListener(() => {OnClickLevel(level);});
			}

		}

	}
	AsyncOperation asysc;
	public void OnClickLevel(int val){
		if (val == 999) {//Back
			setScreen(M.GAMESHOP);
		} else {
			Debug.Log ("OnClickLevel = " + val);
			if (M.UNLOCKLEVEL > val) {
				transform.Find ("Loading").gameObject.SetActive (true);
				M.LEVEL_NO = val;
				asysc = SceneManager.LoadSceneAsync (1);
				asysc.allowSceneActivation = false;
				StartCoroutine (ShowLoadScreen ());
				//SceneManager.LoadScene ("BlockGamePlay");
			}
		}
		if (M.setSound) {
			MusicClick.Play ();
		}
	}
	IEnumerator ShowLoadScreen()
	{
		yield return new WaitForSeconds (1);
		asysc.allowSceneActivation = true;
	}
	void setScreen(int scr){
		M.GameScreen = scr;
		if (_Animtor != null) {
			_Animtor.SetBool ("isOpen", false);
		}
		switch (M.GameScreen) {
		case M.GAMESHOP:
			_Animtor = transform.Find ("Shop").GetComponent<Animator> ();
			setShop ();
			break;
		case M.GAMELEVEL:
			_Animtor = transform.Find ("Location").GetComponent<Animator> ();
			break;
		case M.GAMEEXIT:
			_Animtor = transform.Find ("EXIT").GetComponent<Animator> ();
			break;
		case M.GAMEIAP:
			transform.Find ("IAP/Coin/Text").GetComponent<Text> ().text = M.COINS + "";
			transform.Find ("IAP/ADSFREE").gameObject.SetActive (M.isAds);
			_Animtor = transform.Find ("IAP").GetComponent<Animator> ();
			break;
		case M.GAMEMENU:
			default:
			_Animtor = transform.Find ("Menu").GetComponent<Animator> ();
			break;

		}
		_Animtor.SetBool ("isOpen", true);
		transform.Find ("Loading").gameObject.SetActive (false);
	}


	void setShop(){
		for(int i =0;i<mFireBrigade.childCount;i++){
			mFireBrigade.GetChild (i).gameObject.SetActive (i == M.PLAYER_NO);
		}

		Transform goShopPower = transform.Find ("Shop/POWER");

		goShopPower.GetChild (0).GetChild (1).GetChild (0).GetComponent<Image> ().fillAmount = .3f + M.PLAYER_NO*.08f + M.UPGEDE[M.PLAYER_NO]*.04f;
		goShopPower.GetChild (1).GetChild (1).GetChild (0).GetComponent<Image> ().fillAmount = .4f + M.PLAYER_NO*.11f+ M.UPGEDE[M.PLAYER_NO]*.03f;
		goShopPower.GetChild (2).GetChild (1).GetChild (0).GetComponent<Image> ().fillAmount = .2f + M.PLAYER_NO*.14f + M.UPGEDE[M.PLAYER_NO]*.02f;
		transform.Find ("Shop/Center/Lock").GetComponent<Image> ().enabled = (M.FIRECOST [M.PLAYER_NO] != 0);

		transform.Find ("Shop/Play").GetComponent<Image> ().enabled = true;
		if (M.UPGEDE [M.PLAYER_NO] < 3) {
			goShopPower.GetChild (3).GetComponent<Image> ().sprite = mSSel;

			if (M.FIRECOST [M.PLAYER_NO] != 0) {
				goShopPower.GetChild (3).GetChild (0).GetComponent<Text> ().text = M.FIRECOST[M.PLAYER_NO] + " $";
				goShopPower.GetChild (3).GetChild (1).GetComponent<Text> ().text = "BUY";
				transform.Find ("Shop/Play").GetComponent<Image> ().enabled = false;

			} else {
				goShopPower.GetChild (3).GetChild (0).GetComponent<Text> ().text = (M.PLAYER_NO + 1) * 200f + (M.UPGEDE [M.PLAYER_NO] + 1) * 200 + " $";
				goShopPower.GetChild (3).GetChild (1).GetComponent<Text> ().text = "UPGRADE";
			}
		} else {
			goShopPower.GetChild (3).GetComponent<Image> ().sprite = mSDsel;
			goShopPower.GetChild (3).GetChild (0).GetComponent<Text> ().text = "";
			goShopPower.GetChild (3).GetChild (1).GetComponent<Text> ().text = "FULL UPGRADE";
		}

		transform.Find ("Shop/Top/Coin/Text").GetComponent<Text> ().text = M.COINS+"";
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
	}

	public void HandleInterstitialClosed (object sender, System.EventArgs args)
	{
		MonoBehaviour.print ("HandleInterstitialClosed event received");
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
			.SetGender(Gender.Male)
			.SetBirthday(new System.DateTime(1985, 1, 1))
			.TagForChildDirectedTreatment(false)
			.AddExtra("color_bg", "9B30FF")
			.Build();
	}
	private void ShowInterstitial()
	{
		if (M.isAds) {
			if (this.interstitial.IsLoaded ()) {
				this.interstitial.Show ();
			} else {
				if (Advertisement.IsReady ("video")) {
					var options = new ShowOptions { resultCallback = HandleShowResult };
					Advertisement.Show ("video", options);
				} 
			}
		}
	}
	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log("Unity The ad was successfully shown.");
			//
			// YOUR CODE TO REWARD THE GAMER
			// Give coins etc.
			break;
		case ShowResult.Skipped:
			Debug.Log("Unity  The ad was skipped before reaching the end.");
			transform.Find ("GameOver").transform.GetComponent<Animator> ().SetBool ("isOpen", true);
			break;
		case ShowResult.Failed:
			Debug.LogError("Unity  The ad failed to be shown.");
			transform.Find ("GameOver").transform.GetComponent<Animator> ().SetBool ("isOpen", true);
			break;
		}
	}

	public static WWW www;
	public static  void MoreGame ()
	{
		if (www != null) {
			if (www.isDone) {
				string str = www.text.Split ('#') [1];
				if (str.Length > 5) {
					for (int i = 0; i < str.Length; i++) {
						Debug.Log (i+"  "+str.ToCharArray()[i]);
					}
					#if UNITY_IPHONE
					str = str;
					#else
					str = "https://play.google.com/store/apps/details?id="+str;
					#endif

					Application.OpenURL (str); 
					return;
				}
			}
		}
		Application.OpenURL ("https://play.google.com/store/apps/details?id=com.robotics.Assassinator3D");
	}
	public void OnClickExit(int val){
		switch (val) {
		case 0://Ads
			MoreGame ();
			break;
		case 1://Yes
			M.Save ();
			Application.Quit ();
			break;
		case 2://No
			setScreen (M.GAMEMENU);
			break;
		case 3://More
			#if ANDROID
			Application.OpenURL ("https://play.google.com/store/apps/developer?id=Onedaygame24");
			#else
			Application.OpenURL ("https://itunes.apple.com/us/developer/yogesh-bangar/id1199008030");
			#endif
			break;
		
			if (M.setSound) {
				MusicClick.Play ();
			}
		}
	}
}
