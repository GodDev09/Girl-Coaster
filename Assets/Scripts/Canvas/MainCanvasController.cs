using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class MainCanvasController : MonoBehaviour
{
	[SerializeField] private GameObject holdToAim, victory, defeat, nextLevel, retry, constantRetryButton, skipLevel;
	[SerializeField] private TextMeshProUGUI levelText;
	[SerializeField] private Image red, emoji;
	[SerializeField] private float emojiRotation;

	[SerializeField] private Button nextLevelButton;

	[SerializeField] private GameObject warningPanel;

	[Header("Input PassCode")] 
	[SerializeField] private string passCode;
	[SerializeField] private TMP_InputField inputPassCode;
	[SerializeField] private GameObject canvasPassCode;
	
	private Color _originalRedColor, _lighterRedColor;
	private bool _hasTapped, _hasLost;
	private Sequence _emojiSequence;
	private Tweener _redOverlayTween;
	private bool _hasWon;

	private void OnEnable()
	{
		GameEvents.KartCrash += OnObstacleCollision;
		GameEvents.MainKartCrash += OnObstacleCollision;

		GameEvents.PlayerDeath += OnGameLose;
		GameEvents.GameWin += OnGameWin;

		GameEvents.ObstacleWarningOn += CanEnableWarningPanel;
		GameEvents.ObstacleWarningOff += DontEnableWarningPanel;
	}

	private void OnDisable()
	{
		GameEvents.KartCrash -= OnObstacleCollision;
		GameEvents.MainKartCrash -= OnObstacleCollision;
		
		GameEvents.PlayerDeath -= OnGameLose;
		GameEvents.GameWin -= OnGameWin;
		
		GameEvents.ObstacleWarningOn -= CanEnableWarningPanel;
		GameEvents.ObstacleWarningOff -= DontEnableWarningPanel;
	}

	private void Awake() => DOTween.KillAll();

	void FnAfterInterstialAd()
	{
		Debug.Log("FnAfterInterstialAd");
	}
	private void Start()
	{
		inputPassCode.onEndEdit.AddListener(OnEndEditPassCode);
		
		var levelNo = PlayerPrefs.GetInt("levelNo", 1);
		levelText.text = "Level " + levelNo;

		_originalRedColor = red.color;
		_lighterRedColor = _originalRedColor;
		_lighterRedColor.a *= 0.5f;
		
		_emojiSequence = DOTween.Sequence();

		var emojiTransform = emoji.transform;
		const float duration = 0.175f;
		
		_emojiSequence.Append(emoji.DOColor(Color.white, duration));
		_emojiSequence.AppendCallback(() => emojiTransform.localScale = Vector3.zero);
		_emojiSequence.Join(emojiTransform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack));
		_emojiSequence.Append(emojiTransform.DOLocalRotate(Vector3.forward * -emojiRotation / 2, duration));
		_emojiSequence.Append(emojiTransform.DOLocalRotate(Vector3.forward * emojiRotation, duration));
		_emojiSequence.Append(emojiTransform.DOLocalRotate(Vector3.forward * -emojiRotation, duration));
		_emojiSequence.Append(emojiTransform.DOLocalRotate(Vector3.forward * emojiRotation, duration));
		_emojiSequence.Append(emojiTransform.DOLocalRotate(Vector3.forward * -emojiRotation / 2, duration));
		_emojiSequence.Append(emoji.DOColor(Color.clear, duration));
		_emojiSequence.Join(emojiTransform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack));

		_emojiSequence.SetRecyclable(true);
		_emojiSequence.SetAutoKill(false);
		_emojiSequence.Pause();
	}
	
	public void OnEndEditPassCode(string _passcode)
	{
		Debug.Log("_passcode " + _passcode);
		if (string.CompareOrdinal(_passcode, passCode) == 0)
		{
			canvasPassCode.GetComponent<Canvas>().enabled = true;

			StartCoroutine(IEClearInput());
		}
	}

	IEnumerator IEClearInput()
	{
		yield return new WaitForEndOfFrame();

		inputPassCode.text = "";
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.N)) NextLevel();
		
		if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		
		if(_hasTapped) return;
		
		if (!InputExtensions.GetFingerDown()) return;
		
		if(!EventSystem.current) { print("no event system"); return; }
		
		if(!EventSystem.current.IsPointerOverGameObject(InputExtensions.IsUsingTouch ? Input.GetTouch(0).fingerId : -1))
			TapToPlay();
	}

	private void EnableVictoryObjects()
	{
		if(defeat.activeSelf) return;
		
		victory.SetActive(true);
		nextLevelButton.gameObject.SetActive(true);
		nextLevelButton.interactable = true;
		constantRetryButton.SetActive(false);
		
		AudioManager.instance.Play("Win");
	}


	IEnumerator ShowAdvertisement()
	{
		yield return new WaitForSeconds(1);
		Debug.Log("Ads");
		FnAfterInterstialAd();
		//IronSourceAds._instance.ShowInterstialAd(FnAfterInterstialAd, "ShowAdvertisement");
	}

	private void EnableLossObjects()
	{
		if(victory.activeSelf) return;

		if (_hasLost) return;
		
		red.enabled = true;
		red.color = Color.clear;

		DOTween.Kill(red);
		red.DOColor(_originalRedColor, 1f);

		defeat.SetActive(true);
		retry.SetActive(true);
		//skipLevel.SetActive(true);
		constantRetryButton.SetActive(false);
		_hasLost = true;
		
		if(AudioManager.instance)
			AudioManager.instance.Play("Lose");
	}

	private void TapToPlay()
	{
		_hasTapped = true;
		holdToAim.SetActive(false);
		skipLevel.SetActive(false);
		
		GameEvents.InvokeTapToPlay();
		Debug.Log("Ads");
		//IronSource.Agent.displayBanner();
	}

	public void Retry()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		
		if(AudioManager.instance)
			AudioManager.instance.Play("Button");
		
		Debug.Log("Ads");
		//IronSource.Agent.displayBanner();
	}

	public void NextLevel()
	{
		UpgradeShopCanvas.only.SaveCollectedMoney();
		if (PlayerPrefs.GetInt("levelNo", 1) < SceneManager.sceneCountInBuildSettings - 1)
		{
			var x = PlayerPrefs.GetInt("levelNo", 1) + 1;
			PlayerPrefs.SetInt("lastBuildIndex", x);
			SceneManager.LoadScene(x);
		}
		else
		{
			var x = Random.Range(5, SceneManager.sceneCountInBuildSettings - 1);
			PlayerPrefs.SetInt("lastBuildIndex", x);
			SceneManager.LoadScene(x);
		}
		PlayerPrefs.SetInt("levelNo", PlayerPrefs.GetInt("levelNo", 1) + 1);
		
		if(AudioManager.instance)
			AudioManager.instance.Play("Button");
		Vibration.Vibrate(15);
		Debug.Log("Ads");
		//IronSourceAds._instance.showBanner();
	}

	private void OnObstacleCollision(Vector3 obj)
	{
		red.enabled = true;
		red.color = Color.clear;
		
		if(!_redOverlayTween.IsActive())
			_redOverlayTween = red.DOColor(_lighterRedColor, 1f).SetLoops(2, LoopType.Yoyo);
		
		_emojiSequence.Restart();
	}

	private void DontEnableWarningPanel() => DeActivateWarningPanel();

	private void CanEnableWarningPanel() => ActivateWarningPanel();

	private void DeActivateWarningPanel() => warningPanel.SetActive(false);

	private void ActivateWarningPanel() => warningPanel.SetActive(true);

	private void OnGameLose()
	{
		DOVirtual.DelayedCall(1.5f, EnableLossObjects);

		StartCoroutine(ShowAdvertisement());
		Debug.Log("Ads");
		//IronSourceAds._instance.destroyBanner();
	}

	private void OnGameWin()
	{
		if (_hasWon) return;
		
		_hasWon = true;
		DOVirtual.DelayedCall(1f, EnableVictoryObjects);
		
		StartCoroutine(ShowAdvertisement());
		Debug.Log("Ads");
		//IronSourceAds._instance.destroyBanner();
	}
}
