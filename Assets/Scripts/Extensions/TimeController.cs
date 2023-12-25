using DG.Tweening;
using UnityEngine;

public class TimeController : MonoBehaviour
{
	public static TimeController only;

	[SerializeField] private float slowedTimeScale, timeRampDownDuration = 0.5f, timeRampUpDuration = 0.5f;
	
	private const float DefaultFixedDeltaTime = 0.02f;
	private float _slowedDeltaTime;
	private bool _isTimeSlowedDown;

	private Tween _timeDeltaTween, _fixedTimeDeltaTween;
	
	private static float _defaultTimeScale = -1f;
	private bool _spedUpTime;

	private void OnDisable()
	{
		Time.timeScale = _defaultTimeScale;
	}

	private void Awake()
	{
		if (only) Destroy(gameObject);
		else only = this;
	}

	private void Start()
	{
		if(_defaultTimeScale < 0)
			_defaultTimeScale = Time.timeScale;
		
		slowedTimeScale *= _defaultTimeScale;
		
		_slowedDeltaTime = DefaultFixedDeltaTime * slowedTimeScale;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			_spedUpTime = true;
			Time.timeScale = 2f;
		}
		else if(Input.GetKeyUp(KeyCode.LeftShift))
		{
			_spedUpTime = false;
			Time.timeScale = _defaultTimeScale;
		}
	}

	public void SlowDownTime(float multiplier = 1f)
	{
		if(_spedUpTime) return;
		if(_isTimeSlowedDown) return;
		
		_isTimeSlowedDown = true;
		_timeDeltaTween.Kill();
		_fixedTimeDeltaTween.Kill();
		
		_timeDeltaTween = DOTween.To(() => Time.timeScale, value => Time.timeScale = value, slowedTimeScale * multiplier, timeRampDownDuration).SetUpdate(true);
		_fixedTimeDeltaTween = DOTween.To(() => Time.fixedDeltaTime, value => Time.fixedDeltaTime = value, _slowedDeltaTime * multiplier, timeRampDownDuration).SetUpdate(true);
	}

	public void RevertTime(bool lastEnemy = false)
	{
		if(_spedUpTime) return;
		if (!_isTimeSlowedDown) return;

		_isTimeSlowedDown = false;
		_timeDeltaTween.Kill();
		_fixedTimeDeltaTween.Kill();

		_timeDeltaTween = DOTween.To(() => Time.timeScale, value => Time.timeScale = value, _defaultTimeScale, timeRampUpDuration).SetUpdate(true);
		_fixedTimeDeltaTween = DOTween.To(() => Time.fixedDeltaTime, value => Time.fixedDeltaTime = value, DefaultFixedDeltaTime, timeRampUpDuration)
			.SetUpdate(true);
	}
}
