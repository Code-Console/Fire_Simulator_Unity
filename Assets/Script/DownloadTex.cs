
// Get the latest webcam shot from outside "Friday's" in Times Square
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class DownloadTex : MonoBehaviour {
	IEnumerator Start()
	{
		string url = "http://hututusoftwares.com/Link/ads.jpg";
		#if UNITY_IPHONE
		url = "http://hututusoftwares.com/Link/iphone.jpg";
		#endif
		WWW www = new WWW(url);
		yield return www;
		GetComponent<Image>().sprite = Sprite.Create( www.texture, new Rect(0.0f, 0.0f,  www.texture.width,  www.texture.height), new Vector2(0.5f, 0.5f), 100.0f);
	}
}

/*
 if (www == null) {
			#if UNITY_IPHONE
			www = new WWW("http://hututusoftwares.com/Link/iphone.html");
			#else
			www = new WWW("http://hututusoftwares.com/Link/android.html");
			#endif

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
 */
