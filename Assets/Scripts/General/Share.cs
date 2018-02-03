using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class Share : MonoBehaviour {
	public string title;
	public string bodyText;

	public void SaveAndShareImage() {
		#if UNITY_IOS
		SocialServiceManager.GetInstance ().UnlockAchievement ("sharingiscaring");
		#endif
		#if UNITY_ANDROID
		SocialServiceManager.GetInstance ().UnlockAchievement (GPGSIds.achievement_sharing_is_caring);
		#endif

		StartCoroutine (ShareImageBackground ());
	}

	IEnumerator ShareImageBackground() {
		string destination = Path.Combine(Application.persistentDataPath, "test.png");
		SaveImage (destination);
		ShareImage (destination, title, 
			bodyText);
		yield return null;
	}

	void ShareImage(string imageLocation, string titleText, string bodyText) {
		#if UNITY_IOS && !UNITY_EDITOR
		CallSocialShareAdvanced(bodyText, titleText, "", imageLocation);
		#endif

		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
		intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
		AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
		AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse","file://" + imageLocation);
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), bodyText);
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), titleText);
		intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
		AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

		currentActivity.Call("startActivity", intentObject);
		#endif
	}

	void SaveImage(string destination) {
		Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
		screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height),0,0);
		screenTexture.Apply();
		byte[] dataToSave = screenTexture.EncodeToPNG();
		File.WriteAllBytes(destination, dataToSave);
	}

	#if UNITY_IOS
	public struct ConfigStruct
	{
		public string title;
		public string message;
	}

	[DllImport ("__Internal")] private static extern void showAlertMessage(ref ConfigStruct conf);

	public struct SocialSharingStruct
	{
		public string text;
		public string url;
		public string image;
		public string subject;
	}

	[DllImport ("__Internal")] private static extern void showSocialSharing(ref SocialSharingStruct conf);

	public static void CallSocialShare(string title, string message)
	{
		ConfigStruct conf = new ConfigStruct();
		conf.title  = title;
		conf.message = message;
		showAlertMessage(ref conf);
	}


	public static void CallSocialShareAdvanced(string defaultTxt, string subject, string url, string img)
	{
		SocialSharingStruct conf = new SocialSharingStruct();
		conf.text = defaultTxt;
		conf.url = url;
		conf.image = img;
		conf.subject = subject;

		showSocialSharing(ref conf);
	}
	#endif
}
