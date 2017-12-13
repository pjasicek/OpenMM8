using UnityEngine;
using System;

/// Time iteration class.
///
/// Component of the sky dome parent game object.

public class TOD_Time : MonoBehaviour
{
	/// Length of one day in minutes.
	[Tooltip("Length of one day in minutes.")]
	[TOD_Min(0f)] public float DayLengthInMinutes = 30;

	/// Progress time at runtime.
	[Tooltip("Progress time at runtime.")]
	public bool ProgressTime = true;

	/// Set the date to the current device date on start.
	[Tooltip("Set the date to the current device date on start.")]
	public bool UseDeviceDate = false;

	/// Set the time to the current device time on start.
	[Tooltip("Set the time to the current device time on start.")]
	public bool UseDeviceTime = false;

	/// Apply the time curve when progressing time.
	[Tooltip("Apply the time curve when progressing time.")]
	public bool UseTimeCurve = false;

	/// Time progression curve.
	[Tooltip("Time progression curve.")]
	public AnimationCurve TimeCurve = AnimationCurve.Linear(0, 0, 24, 24);

	/// Fired whenever the second value is incremented.
	public event Action OnSecond;

	/// Fired whenever the minute value is incremented.
	public event Action OnMinute;

	/// Fired whenever the hour value is incremented.
	public event Action OnHour;

	/// Fired whenever the day value is incremented.
	public event Action OnDay;

	/// Fired whenever the month value is incremented.
	public event Action OnMonth;

	/// Fired whenever the year value is incremented.
	public event Action OnYear;

	/// Fired whenever the sun rises.
	public event Action OnSunrise;

	/// Fired whenever the sun sets.
	public event Action OnSunset;

	private TOD_Sky sky;
	private AnimationCurve timeCurve;
	private AnimationCurve timeCurveInverse;

	/// Apply changes made to TimeCurve.
	public void RefreshTimeCurve()
	{
		TimeCurve.preWrapMode  = WrapMode.Clamp;
		TimeCurve.postWrapMode = WrapMode.Clamp;

		ApproximateCurve(TimeCurve, out timeCurve, out timeCurveInverse);

		timeCurve.preWrapMode  = WrapMode.Loop;
		timeCurve.postWrapMode = WrapMode.Loop;

		timeCurveInverse.preWrapMode  = WrapMode.Loop;
		timeCurveInverse.postWrapMode = WrapMode.Loop;
	}

	/// Apply the time curve to a time span.
	/// \param deltaTime The time span to adjust.
	/// \return The adjusted time span.
	public float ApplyTimeCurve(float deltaTime)
	{
		float time = timeCurveInverse.Evaluate(sky.Cycle.Hour) + deltaTime;
		deltaTime = timeCurve.Evaluate(time) - sky.Cycle.Hour;

		if (time >= 24)
		{
			deltaTime += ((int)time / 24) * 24;
		}
		else if (time < 0)
		{
			deltaTime += ((int)time / 24 - 1) * 24;
		}

		return deltaTime;
	}

	/// Add hours and fractions of hours to the current time.
	/// \param hours The hours to add.
	/// \param adjust Whether or not to apply the time curve.
	public void AddHours(float hours, bool adjust = true)
	{
		if (UseTimeCurve && adjust) hours = ApplyTimeCurve(hours);

		var dateTimeOld = sky.Cycle.DateTime;
		var dateTimeNew = dateTimeOld.AddHours(hours);

		sky.Cycle.DateTime = dateTimeNew;

		if (dateTimeNew.Year > dateTimeOld.Year)
		{
			if (OnYear   != null) OnYear();
			if (OnMonth  != null) OnMonth();
			if (OnDay    != null) OnDay();
			if (OnHour   != null) OnHour();
			if (OnMinute != null) OnMinute();
			if (OnSecond != null) OnSecond();
		}
		else if (dateTimeNew.Month > dateTimeOld.Month)
		{
			if (OnMonth  != null) OnMonth();
			if (OnDay    != null) OnDay();
			if (OnHour   != null) OnHour();
			if (OnMinute != null) OnMinute();
			if (OnSecond != null) OnSecond();
		}
		else if (dateTimeNew.Day > dateTimeOld.Day)
		{
			if (OnDay    != null) OnDay();
			if (OnHour   != null) OnHour();
			if (OnMinute != null) OnMinute();
			if (OnSecond != null) OnSecond();
		}
		else if (dateTimeNew.Hour > dateTimeOld.Hour)
		{
			if (OnHour   != null) OnHour();
			if (OnMinute != null) OnMinute();
			if (OnSecond != null) OnSecond();
		}
		else if (dateTimeNew.Minute > dateTimeOld.Minute)
		{
			if (OnMinute != null) OnMinute();
			if (OnSecond != null) OnSecond();
		}
		else if (dateTimeNew.Second > dateTimeOld.Second)
		{
			if (OnSecond != null) OnSecond();
		}

		double oldHour = dateTimeOld.TimeOfDay.TotalHours;
		double newHour = dateTimeNew.TimeOfDay.TotalHours;

		if (oldHour < sky.SunriseTime && newHour >= sky.SunriseTime)
		{
			if (OnSunrise != null) OnSunrise();
		}

		if (oldHour < sky.SunsetTime && newHour >= sky.SunsetTime)
		{
			if (OnSunset != null) OnSunset();
		}
	}

	/// Add seconds and fractions of seconds to the current time.
	/// \param seconds The seconds to add.
	/// \param adjust Whether or not to apply the time curve.
	public void AddSeconds(float seconds, bool adjust = true)
	{
		AddHours(seconds / 3600f);
	}

	private void CalculateLinearTangents(Keyframe[] keys)
	{
		for (int i = 0; i < keys.Length; i++)
		{
			var key = keys[i];

			if (i > 0)
			{
				var prev = keys[i-1];
				key.inTangent = (key.value - prev.value) / (key.time - prev.time);
			}

			if (i < keys.Length-1)
			{
				var next = keys[i+1];
				key.outTangent = (next.value - key.value) / (next.time - key.time);
			}

			keys[i] = key;
		}
	}

	private void ApproximateCurve(AnimationCurve source, out AnimationCurve approxCurve, out AnimationCurve approxInverse)
	{
		const float minstep = 0.01f;

		var approxCurveKeys   = new Keyframe[25];
		var approxInverseKeys = new Keyframe[25];

		float time = -minstep;
		for (int i = 0; i < 25; i++)
		{
			time = Mathf.Max(time + minstep, source.Evaluate(i));

			approxCurveKeys[i]   = new Keyframe(i, time);
			approxInverseKeys[i] = new Keyframe(time, i);
		}

		CalculateLinearTangents(approxCurveKeys);
		CalculateLinearTangents(approxInverseKeys);

		approxCurve   = new AnimationCurve(approxCurveKeys);
		approxInverse = new AnimationCurve(approxInverseKeys);
	}

	protected void Awake()
	{
		sky = GetComponent<TOD_Sky>();

		if (UseDeviceDate)
		{
			sky.Cycle.Year  = DateTime.Now.Year;
			sky.Cycle.Month = DateTime.Now.Month;
			sky.Cycle.Day   = DateTime.Now.Day;
		}

		if (UseDeviceTime)
		{
			sky.Cycle.Hour = (float)DateTime.Now.TimeOfDay.TotalHours;
		}

		RefreshTimeCurve();
	}

	protected void FixedUpdate()
	{
		if (ProgressTime && DayLengthInMinutes > 0)
		{
			const float oneDayInMinutes = 60 * 24;

			float timeFactor = oneDayInMinutes / DayLengthInMinutes;

			AddSeconds(Time.fixedDeltaTime * timeFactor);
		}
	}
}
