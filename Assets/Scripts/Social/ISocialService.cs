using System;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

public interface ISocialService {
	bool IsAuthenticated();
	void Authenticate();
	void PostScoreToLeaderboard(int score, string leaderboardId);
	void ShowLeaderboard();
	void ShowLeaderboard(string leaderboardId);
	void LoadAchievements ();
	void ShowAchievements ();
	void UnlockAchievement (string achievementId);
	string GetMyUserId();
	void GetLeaderboardScores (string leaderboardId, Action<List<IScore>> callback);
}
