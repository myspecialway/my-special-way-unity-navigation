using Navigation;
using UnityEngine;

public class Init : MonoBehaviour
{
	[SerializeField]
	private Navigator navigator;

	private void Start()
	{
		var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		
		var currentActivity = unityPlayer?.GetStatic<AndroidJavaObject>("currentActivity");
		
		var intent = currentActivity?.Call<AndroidJavaObject>("getIntent");
		
		var bundle = intent?.Call<AndroidJavaObject>("getExtras");

		var destName = bundle?.Call<string> ("getCharSequence", "destination", null);
	
		if (string.IsNullOrEmpty(destName))
		{
			destName = "C1";
		}
	
		Debug.Log("destName = " + destName);
		
		var dest = GameObject.Find(destName);

		if (dest != null)
		{
			StartCoroutine(navigator.NavigateTo(dest.transform));
		}
	}
}
