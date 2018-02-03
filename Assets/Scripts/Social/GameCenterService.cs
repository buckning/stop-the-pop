using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

public class GameCenterService : ISocialService {

	public delegate void LeaderboardPostCallback(bool result);

	bool authenticating = false;
	bool authenticated = false;

	public bool IsAuthenticated() {
		return authenticated;
	}

	public void Authenticate() {
		if (!authenticating) {
			authenticating = true;
			Social.localUser.Authenticate (AuthenticateUser);
			GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
		}
	}

	public void ShowLeaderboard() {
		Social.ShowLeaderboardUI();
	}

	public void ShowLeaderboard(string leaderboardId) {
		GameCenterPlatform.ShowLeaderboardUI(leaderboardId, UnityEngine.SocialPlatforms.TimeScope.AllTime);
	}

	public void PostScoreToLeaderboard(int score, string leaderboardId) {
		Debug.Log ("Posting score of " + score + " to leaderboard " + leaderboardId);
		#if UNITY_EDITOR
		Debug.Log("Mocking post to leaderboard in editor mode...");
		#else
		Social.ReportScore (score, leaderboardId, (bool success) => {	});
		#endif
	}

	public void ShowAchievements() {
		Social.ShowAchievementsUI ();
	}

	public void LoadAchievements() {
		Social.LoadAchievements (ProcessLoadedAchievements);
	}

	public string GetMyUserId() {
		return Social.localUser.id;
	}

	public void LoadLeaderboard(string leaderboardId, string userId) {
		var leaderboard = Social.CreateLeaderboard();
		leaderboard.id = leaderboardId;
		leaderboard.userScope = UserScope.Global;

		int positionInLeaderboard = -1;

		leaderboard.LoadScores ((wasLoaded) => {
			foreach (IScore score in leaderboard.scores) {
				if(score.userID == userId) {
					positionInLeaderboard = score.rank;

					Debug.Log("rank is " + score.rank);
				}
			}
		});
	}

	public void GetLeaderboardScores(string leaderboardId, Action<List<IScore>> callback) {
		var leaderboard = Social.CreateLeaderboard();
		leaderboard.id = leaderboardId;
		leaderboard.userScope = UserScope.Global;
		leaderboard.LoadScores ((wasLoaded) => {
			List<IScore> scores = new List<IScore> ();
			if (leaderboard.scores.Length > 0) {
				foreach (IScore score in leaderboard.scores) {
					scores.Add(score);
				}
			}
			callback (scores);
		});
	}

	public void UnlockAchievement(string achievementId) {
		Social.ReportProgress(achievementId, 100, (bool success) => {
			Debug.Log ("Tried to unlock "  + achievementId + " with status - " + success);
		});
	}

	void ProcessLoadedAchievements (IAchievement[] achievements) {
	}

	void AuthenticateUser(bool authResult) {
		authenticated = authResult;
		authenticating = false;
	}
}
