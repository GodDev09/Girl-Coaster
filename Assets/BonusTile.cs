using DG.Tweening;
using Kart;
using UnityEngine;

public class BonusTile : MonoBehaviour
{
	[SerializeField] private Transform leftFlag, rightFlag;
	[HideInInspector] public MeshRenderer meshRenderer;

	private static MoneyCanvas _moneyCanvas;
	
	[SerializeField] private GameObject moneyPrefab;
	
	private bool _hasEntered, _hasExited;

	private static AddedKartsManager _addedKarts;
	private static float _lowestAllowedY = -9999f;
	private bool _hasNoPassengers;

	private void Start()
	{
		meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();

		if (!_addedKarts)
			_addedKarts = GameObject.FindGameObjectWithTag("Player").GetComponent<AddedKartsManager>();
		
		if(_lowestAllowedY < -999f)
			_lowestAllowedY = GameObject.FindGameObjectWithTag("BonusRamp").GetComponent<BonusRamp>().LowestPointY - 1.8f;
		
		_moneyCanvas = GameObject.FindGameObjectWithTag("MoneyCanvas").GetComponent<MoneyCanvas>();
	}

	private void EjectPassenger(Transform myPassengerChild)
	{
		if(_hasNoPassengers) return;
		
		if (_addedKarts.PassengerCount <= 0)
		{
			GameEvents.InvokeRunOutOfPassengers();
			_hasNoPassengers = true;
			return;
		}
		
		var kartPassenger = _addedKarts.PopPassenger;

		if (_addedKarts.PassengerCount % 2 == 0)
		{
			DampCamera.only.UpdateFilledKartCount(_addedKarts.PassengerCount / 2);
			
			if(_addedKarts.AddedKartCount > 0) _addedKarts.PopKart();
		}

		kartPassenger.transform.parent = null;
		kartPassenger.transform.DORotateQuaternion(myPassengerChild.rotation, 0.5f);
		
		kartPassenger.transform.DOJump(myPassengerChild.position,
			kartPassenger.transform.position.y - _lowestAllowedY,
			1,
			1.25f).OnComplete(() =>
		{
			kartPassenger.StartDancing();
		});

		var money = Instantiate(moneyPrefab);
		money.transform.position = kartPassenger.transform.position;
		money.transform.parent = _moneyCanvas.GetMoneyDestination();
		money.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InCirc);

		money.transform.DOLocalMove(Vector3.zero, 0.25f).
			OnComplete(() =>
			{
				_moneyCanvas.ScaleMoneyImage();
				_moneyCanvas.IncreaseMoneyCount();
				money.gameObject.SetActive(false);
			});
	}

	private void OnTriggerEnter(Collider other)
	{
		if(_hasEntered) return;
		if (!other.CompareTag("Player")) return;
		var myPassengerChild = transform.GetChild(2);

		_hasEntered = true;
		
		leftFlag.DOLocalRotate(Vector3.up * -90f, 0.5f).SetEase(Ease.OutElastic);
		rightFlag.DOLocalRotate(Vector3.up * 90f, 0.5f).SetEase(Ease.OutElastic);
		
		EjectPassenger(myPassengerChild);
		
		if(AudioManager.instance)
		{
			AudioManager.instance.Play("BonusTile");
			AudioManager.instance.Play("Hype" + Random.Range(1, 5));
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(_hasExited) return;
		if (!other.CompareTag("Player")) return;
		var myPassengerChild = transform.GetChild(3);

		_hasExited = true;

		EjectPassenger(myPassengerChild);
		
		if(AudioManager.instance)
			AudioManager.instance.Play("BonusTile");
	}
}
