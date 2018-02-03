using System;

public class TimeDiff {

	public double HoursSince(String dateTime) {
		DateTime lastSkipTime = DateTime.Parse(dateTime);

		DateTime currentTime = DateTime.Now;
		TimeSpan ts = currentTime - lastSkipTime;

		return ts.TotalHours;
	}

	public int MinutesSince(String dateTime) {
		DateTime lastSkipTime = DateTime.Parse(dateTime);

		DateTime currentTime = DateTime.Now;
		TimeSpan ts = currentTime - lastSkipTime;

		return (int)ts.TotalMinutes;
	}

	public string TimeNow() {
		String dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
		return DateTime.Now.ToString(dateTimeFormat);
	}
}
