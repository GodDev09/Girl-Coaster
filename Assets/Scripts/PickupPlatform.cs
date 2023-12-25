using DG.Tweening;
using UnityEngine;

public class PickupPlatform : MonoBehaviour
{
	[SerializeField] private Transform PassengerPairs;
	[SerializeField] private float jumpHeight;
	public Transform PassengerL { get; private set; }
	public Transform PassengerR { get; private set; }

	private void Start()
	{
		PassengerR = PassengerPairs.GetChild(0).GetChild((int) UpgradeShopCanvas.only.MyCharacterSkin);
		PassengerL = PassengerPairs.GetChild(1).GetChild((int) UpgradeShopCanvas.only.MyCharacterSkin);
		PassengerR.gameObject.SetActive(true);
		PassengerL.gameObject.SetActive(true);
	}

	public void JumpOnToTheKart(Transform kartPassenger1,Transform kartPassenger2)
	{
		var temp = 0f;

		DOTween.To(() => temp, value => temp = value, 1f, 0.5f)
			.SetEase(Ease.InOutCubic)
			.OnUpdate(() =>
			{
				PassengerL.transform.position = Vector3.Lerp(PassengerL.position, kartPassenger1.position, temp);
				PassengerR.transform.position = Vector3.Lerp(PassengerR.position, kartPassenger2.position, temp);
			});

		PassengerL.DOMoveY(PassengerL.position.y + jumpHeight, 0.2f)
			.SetEase(Ease.OutExpo)
			.OnComplete(() => 
				PassengerL.DOMoveY(kartPassenger1.position.y, 0.2f)
					.SetEase(Ease.InExpo)
					.OnComplete(() =>
					{
						//change animator state to cheering, dont disable and enable
						kartPassenger1.gameObject.SetActive(true);
						PassengerL.gameObject.SetActive(false);
					}));
		
		PassengerR.DOMoveY(PassengerL.position.y + jumpHeight, 0.2f)
			.SetEase(Ease.OutExpo)
			.OnComplete(() => 
				PassengerL.DOMoveY(kartPassenger2.position.y, 0.2f)
					.SetEase(Ease.InExpo)
					.OnComplete(() =>
					{
						kartPassenger2.gameObject.SetActive(true);
						PassengerR.gameObject.SetActive(false);
					}));
	}
}