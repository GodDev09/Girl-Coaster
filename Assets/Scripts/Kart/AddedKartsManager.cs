using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Kart
{
	public class AddedKartsManager : MonoBehaviour
	{
		[SerializeField] private GameObject kartPrefab;
		[SerializeField] private float forceMultiplier, upForce, sideForce;
		[SerializeField] private float passengerJumpDelayStep;

		private List<AdditionalKartController> AddedKarts { get; set; }

		[SerializeField] private List<Passenger> _availablePassengers;
		private MainKartController _my;

		private static bool _isInKartCollisionCooldown;
		private static int _audioIndex;

		public int AddedKartCount => AddedKarts.Count;
		public int PassengerCount => _availablePassengers.Count;

		public bool isObstacleMainKart;
		[SerializeField] private GameObject additionalObstacleKartPrefab;
		[SerializeField] private int totalObstacleAdditionalKarts = 5;
		

		public Passenger PopPassenger
		{
			get
			{
				var x = _availablePassengers[^1];
				_availablePassengers.RemoveAt(_availablePassengers.Count - 1);
				return x;
			}
		}

		private void OnEnable()
		{
			GameEvents.MainKartCrash += OnObstacleCollision;
		}

		private void OnDisable()
		{
			GameEvents.MainKartCrash -= OnObstacleCollision;
		}

		private void Start()
		{
			_my = GetComponent<MainKartController>();

			AddedKarts = new List<AdditionalKartController>();
			void InitList()
			{
				_availablePassengers = new List<Passenger>
				{
					//add main kart passengers
					_my.Passenger1, _my.Passenger2
				};
			}
			
			Tween checker = null;
			checker = DOVirtual.DelayedCall(0.05f, () =>
			{
				if (!_my.isInitialised) return;

				//init kart passengers
				InitList();
				checker.Kill();
			}).SetLoops(-1);

			if (isObstacleMainKart)
			{
				kartPrefab = additionalObstacleKartPrefab;
				SpawnKarts(totalObstacleAdditionalKarts);
			}
		}

		public void SpawnKarts(int kartsToSpawn)
		{
			var pitch = 0.9f;
			DOVirtual.DelayedCall(0.15f, () =>
			{
				SpawnNewKart();

				if (isObstacleMainKart || !AudioManager.instance) return;
				
				AudioManager.instance.Play("AddKart", -1f, pitch);
				pitch += 0.1f;
			}).SetLoops(kartsToSpawn);
		}

		public void PopKart()
		{
			var kartToPop = AddedKarts[^1];

			kartToPop.baseCollider.SetActive(false);
			kartToPop.gameObject.name += " fallen";
			kartToPop.KartFollow.SetKartToFollow(null);
			kartToPop.kartCollider.gameObject.SetActive(true);

			var direction = kartToPop.transform.forward;
			direction = direction.normalized;

			kartToPop.BoxCollider.enabled = false;
			kartToPop.Positioner.enabled = false;

			kartToPop.kartCollider.gameObject.SetActive(true);
			kartToPop.kartCollider.attachedRigidbody.isKinematic = false;
			kartToPop.kartCollider.attachedRigidbody.AddForce(direction * forceMultiplier + Vector3.up * upForce +
															  Vector3.left * sideForce, ForceMode.Impulse);

			AddedKarts.RemoveAt(AddedKarts.Count - 1);
			if (AddedKartCount > 0) AddedKarts[^1].Wagon.back = null;
		}

		public void MakePassengersJump(float duration)
		{
			var delay = 0f;

			for (var index = 0; index < _availablePassengers.Count; index++)
			{
				if (index % 2 == 0) delay += passengerJumpDelayStep;
				
				_availablePassengers[index].MakePassengerJump(duration, delay);
			}
			GameEvents.InvokeJumpInBathTub();
		}

		public void ExplodeMultipleKarts(int number, Vector3 collisionPoint)
		{
			for (var i = 0; i < number; i++) ExplodeRearKart(collisionPoint);
			
			CameraFxController.only.ScreenShake(5f);
		}

		private void SpawnNewKart()
		{
			var newKart = Instantiate(kartPrefab, transform.parent).GetComponent<AdditionalKartController>();
			
			if(!isObstacleMainKart) DOVirtual.DelayedCall(0.1f, newKart.scaleUp.ScaleMeUp);

			Tween checker = null;
			checker = DOVirtual.DelayedCall(0.05f, () =>
			{
				if (!newKart.isInitialised) return;

				//add new kart passengers
				_availablePassengers.Add(newKart.Passenger1);
				_availablePassengers.Add(newKart.Passenger2);

				if (AddedKarts.Count == 0)
				{
					_my.Wagon.back = newKart.Wagon;
					newKart.Wagon.Setup(_my.Wagon);
					newKart.KartFollow.SetKartToFollow(transform);
				}
				else
				{
					AddedKarts[^1].Wagon.back = newKart.Wagon;
					newKart.Wagon.Setup(AddedKarts[^1].Wagon);
					newKart.KartFollow.SetKartToFollow(AddedKarts[^1].Wagon.transform);
				}

				AddNewKart(newKart);
				checker.Kill();
			}).SetLoops(-1);
		}

		private void ExplodeRearKart(Vector3 collisionPoint, bool explodeMultipleKarts = false)
		{
			var kartToPop = AddedKarts[^1];

			kartToPop.gameObject.name += " fallen";
			kartToPop.tag = "Untagged";
			kartToPop.baseCollider.SetActive(false);
			kartToPop.KartFollow.SetKartToFollow(null);
			kartToPop.kartCollider.transform.parent = null;
			
			var direction = collisionPoint - kartToPop.transform.position;
			direction = direction.normalized;

			var perpendicular = direction;
			perpendicular.x = -direction.z;
			perpendicular.z = direction.x;

			kartToPop.BoxCollider.enabled = false;
			kartToPop.Positioner.enabled = false;

			var directionMultiplier = (Random.value > 0.5f ? 1f : -1f);
			
			var attachedRigidbody = kartToPop.kartCollider.attachedRigidbody;
			attachedRigidbody.isKinematic = false;
			attachedRigidbody.AddForce(
				direction * forceMultiplier + Vector3.up * upForce  * (explodeMultipleKarts ? 0.5f : 1f) +
				Vector3.left * sideForce * directionMultiplier, ForceMode.Impulse);

			kartToPop.transform.DOScale(kartToPop.transform.lossyScale * 1.5f, 0.15f);
			
			attachedRigidbody.AddTorque(perpendicular * forceMultiplier + Vector3.left * sideForce * directionMultiplier, ForceMode.Impulse);

			RemoveKartFromRear();
			if (AddedKartCount > 0) AddedKarts[^1].Wagon.back = null;

			_availablePassengers.RemoveAt(_availablePassengers.Count - 1);
			_availablePassengers.RemoveAt(_availablePassengers.Count - 1);

			if (AudioManager.instance)
				AudioManager.instance.Play("Death" + ((++_audioIndex % 4) + 1));
		}

		private void ExplodeMainKart(Vector3 collisionPoint)
		{
			var direction = collisionPoint - transform.position;
			direction = direction.normalized;

			var perpendicular = direction;
			perpendicular.x = -direction.z;
			perpendicular.z = direction.x;

			_my.BoxCollider.enabled = false;
			_my.kartCollider.isTrigger = false;

			var attachedRigidbody = _my.kartCollider.attachedRigidbody;
			attachedRigidbody.isKinematic = false;
			attachedRigidbody.AddForce(direction * forceMultiplier + Vector3.up * upForce + Vector3.left * upForce, ForceMode.Impulse);
			attachedRigidbody.AddTorque(perpendicular * forceMultiplier + Vector3.left * upForce, ForceMode.Impulse);

			if (AudioManager.instance) AudioManager.instance.Play("Death" + ((++_audioIndex % 4) + 1));
		}

		private void AddNewKart(AdditionalKartController newKart)
		{
			AddedKarts.Add(newKart);
			_my.KartCounter.UpdateText(AddedKarts.Count + 1, true);
		}

		private void RemoveKartFromRear()
		{
			AddedKarts.RemoveAt(AddedKarts.Count - 1);
			_my.KartCounter.UpdateText(AddedKarts.Count + 1, false);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!other.CompareTag("PickUpPlatform")) return;
			
			var pickUpPlatform = other.GetComponent<PickupPlatform>();
			var kartPassenger1 = _my.Passenger1;
			var kartPassenger2 = _my.Passenger2;
			pickUpPlatform.JumpOnToTheKart(kartPassenger1.transform, kartPassenger2.transform);
			
			other.enabled = false; 
		}

		private void OnObstacleCollision(Vector3 collisionPoint)
		{
			if(isObstacleMainKart) return;
			if(_isInKartCollisionCooldown) return;

			_isInKartCollisionCooldown = true;
			DOVirtual.DelayedCall(0.1f, () => _isInKartCollisionCooldown = false);

			if (AddedKarts.Count > 0)
				ExplodeRearKart(collisionPoint);
			else
			{
				ExplodeMainKart(collisionPoint);
				GameEvents.InvokePlayerDeath();
			}

			CameraFxController.only.ScreenShake(5f);
			_my.PlayExplosionParticle(collisionPoint);
			TimeController.only.SlowDownTime();
			DOVirtual.DelayedCall(0.75f, () => TimeController.only.RevertTime());
		}
	}
}