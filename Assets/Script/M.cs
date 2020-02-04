using UnityEngine;
using System.Collections;



public static class M
{
	public static bool setSound = true;
	public static bool isAds = true;
	public static int []STARS = new int[]{0,0,0,0,0,0,0,0,0,0,0,0};
	public static int []UPGEDE = new int[]{0,0,0,0,0};
	public static int []FIRECOST = new int[]{0,10000,30000,50000,100000};
	public static int UNLOCKLEVEL = 1;
	public static int COINS = 1;

	public static int GameScreen;
	public  const int GAMEMENU = 0;
	public  const int GAMESHOP = 1;
	public  const int GAMELEVEL = 2;
	public  const int GAMEEXIT = 3;
	public  const int GAMEPLAY = 4;
	public  const int GAMEOVER = 5;
	public  const int GAMEWIN = 6;
	public  const int GAMEPAUSE = 7;
	public  const int GAMESTART = 8;
	public  const int GAMEIAP = 9;
	public  const int GAMEREVIVE = 10;

	public static int []LEVELTIME = new int[]{300,240,180,120,120,120,120,120,120,120,180,180};
	public static byte isWoter = 0;
	public static int PLAYER_NO = 0;
	public  const int OPPMAXSPD = 20;
	public  const int OPPMAXTORQUE = 300;
	public static bool isParked = false;
	public static int LEVEL_NO = 0;
	public static int MAXWOTER = 5000;
	public static float GTIME = 300;
	public static float STIME = 300;
	public static float HTIME = 0;
	public static float DISTANCE = 0;

	//50270001

	//ca-app-pub-7665074309496944/1640960059 InterTiatial ANDROID
	//ca-app-pub-3395412980708319/9064332115 InterTiatial IPHONE

	public static void Save ()
	{
		
		PlayerPrefs.SetInt ("a", setSound ? 1 :0);
		PlayerPrefs.SetInt ("b", isAds ? 1 :0);

		for (int i = 0; i < STARS.GetLength(0); i++) 
			PlayerPrefs.SetInt ("c"+i, STARS[i]);
		for (int i = 0; i < UPGEDE.GetLength(0); i++)
			PlayerPrefs.SetInt ("d"+i, UPGEDE[i]);
		for (int i = 0; i < FIRECOST.GetLength(0); i++)
			PlayerPrefs.SetInt ("e"+i, FIRECOST[i]);
		PlayerPrefs.SetInt ("f", UNLOCKLEVEL);
		PlayerPrefs.SetInt ("g", COINS);

		Debug.Log ("Save - >"+UNLOCKLEVEL);

	}

	public static void Open ()
	{
		
		setSound = PlayerPrefs.GetInt ("a", 1) == 1;
		isAds = PlayerPrefs.GetInt ("b", 1) ==1;

		for (int i = 0; i < STARS.GetLength(0); i++) 
			STARS[i] = PlayerPrefs.GetInt ("c"+i, STARS[i]);
		for (int i = 0; i < UPGEDE.GetLength(0); i++)
			UPGEDE[i] = PlayerPrefs.GetInt ("d"+i, UPGEDE[i]);
		for (int i = 0; i < FIRECOST.GetLength(0); i++)
			FIRECOST[i] = PlayerPrefs.GetInt ("e"+i, FIRECOST[i]);
		UNLOCKLEVEL = PlayerPrefs.GetInt ("f", UNLOCKLEVEL);
		COINS = PlayerPrefs.GetInt ("g", COINS);
		Debug.Log ("Open - >"+UNLOCKLEVEL);
	}
}
