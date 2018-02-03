using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***
 * The resource cache is to keep memory down and prevent duplication of loading of assets.
 * Whenever an asset is to be loaded, it should be loaded with the resource cache.
 */
public class ResourceCache : MonoBehaviour {

	static Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();
	static Dictionary<string, AudioClip> soundCache = new Dictionary<string, AudioClip>();

	public static GameObject Load(string name) {
		if (!cache.ContainsKey (name)) {
			GameObject loadedGameObject = Resources.Load (name) as GameObject;
			cache.Add (name, loadedGameObject);
			return loadedGameObject;
		} else {
			return cache [name];
		}
	}

	public static AudioClip LoadAudioClip(string name) {
		if (!soundCache.ContainsKey (name)) {
			AudioClip loadedAudioClip = Resources.Load (name) as AudioClip;
			soundCache.Add (name, loadedAudioClip);
			return loadedAudioClip;
		} else {
			return soundCache [name];
		}
	}

	public static GameObject Get(string name) {
		if (cache.ContainsKey(name)) {
			return cache [name];
		} else {
			print ("Error: " + name + " not found in resource cache");
			return null;
		}
	}

	public static AudioClip GetAudioClip(string name) {
		if (soundCache.ContainsKey(name)) {
			return soundCache [name];
		} else {
			print ("Error: Audio Clip, " + name + ", not found in resource cache");
			return null;
		}
	}
}