using System.Collections;
using System.Collections.Generic;

public class AchievementCache {
	List<string> cache;

	public AchievementCache() {
		cache =  new List<string>();
	}

	public void Add(string value) {
		cache.Add (value);
	}

	public bool Contains(string value) {
		return cache.Contains (value);
	}
}
