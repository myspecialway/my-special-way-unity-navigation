using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Init : MonoBehaviour
{
	[SerializeField]
	private Navigation nav;
	
	private void Start ()
	{		
		var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		
		Debug.Log("unityplayer = " + unityPlayer);
		
		var currentActivity = unityPlayer?.GetStatic<AndroidJavaObject>("currentActivity");
		
		Debug.Log("currentActivity = " + currentActivity);
		
 
		var intent = currentActivity?.Call<AndroidJavaObject>("getIntent");
		
		Debug.Log("intent = " + intent);

		var bundle = intent?.Call<AndroidJavaObject>("getExtras");

		Debug.Log("bundle = " + intent);
		
		var destName = bundle?.Call<string> ("getCharSequence", "destination", @"""");
	
		if (string.IsNullOrEmpty(destName))
		{
			destName = "A";
		}
	
		Debug.Log("destName = " + destName);
		
		var dest = GameObject.Find(destName);

		if (dest != null)
		{
			StartCoroutine(nav.CalculateNavAndShowPath(dest.transform));
		}
	}
}
