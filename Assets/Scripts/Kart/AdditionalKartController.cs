using Dreamteck.Splines;
using UnityEngine;

namespace Kart
{
	public class AdditionalKartController : MonoBehaviour
	{
		private MainKartController _my;

		public ScaleUp scaleUp;
		public Collider kartCollider;
		public SplinePositioner Positioner { get; private set; }
		public Wagon Wagon { get; private set; }
		public KartFollow KartFollow { get; private set; }
		public Collider BoxCollider { get; private set; }
		public Passenger Passenger1{ get; private set; }
		public Passenger Passenger2{ get; private set; }

		public GameObject baseCollider;
		public Transform kartParent, characterPairsParent;

		public bool isInitialised;

		private void OnEnable()
		{
			GameEvents.ReachEndOfTrack += OnReachEndOfTrack;
		}

		private void OnDisable()
		{
			GameEvents.ReachEndOfTrack -= OnReachEndOfTrack;
		}

		private void Start()
		{
			Wagon = GetComponent<Wagon>();
			KartFollow = GetComponent<KartFollow>();

			Passenger1 = characterPairsParent.GetChild((int) UpgradeShopCanvas.only.MyCharacterSkin)
				.GetChild(0)
				.GetComponent<Passenger>();

			Passenger2 = characterPairsParent.GetChild((int) UpgradeShopCanvas.only.MyCharacterSkin)
				.GetChild(1)
				.GetComponent<Passenger>();
			
			Passenger1.gameObject.SetActive(true);
			Passenger2.gameObject.SetActive(true);
			
			kartParent.transform.GetChild((int) UpgradeShopCanvas.only.MyKartSkin).gameObject.SetActive(true);

			isInitialised = true;
			
			Positioner = GetComponent<SplinePositioner>();
			BoxCollider = GetComponent<Collider>();
			_my = GetComponent<MainKartController>();
		}

		private void OnReachEndOfTrack()
		{
			Positioner.enabled = false;
			Wagon.enabled = false;
			KartFollow.enabled = true;
		}

		public void RemoveKartsFromHere(Vector3 collisionPoint)
		{
			KartFollow.SetKartToFollow(null);

			AddedKartsManager GetMainKart()
			{
				Wagon currentFront = null;
				Wagon candidate = GetComponent<Wagon>();
			
				do
				{
					candidate = candidate.front;
					if (candidate)
						currentFront = candidate;
				} while (candidate);

				return currentFront != null ? currentFront.GetComponent<AddedKartsManager>() : null;
			}

			int GetNumberOfRearKarts()
			{
				var candidate = Wagon;

				var count = 1;
				do
				{
					candidate = candidate.back;
					if (candidate)
						count++;
				} while (candidate);

				return count;
			}
			
			Positioner.enabled = false;
			Wagon.enabled = false;
			
			GetMainKart().ExplodeMultipleKarts(GetNumberOfRearKarts(), collisionPoint);
			if (!_my) return;
			_my.PlayExplosionParticle(collisionPoint);
		}
	}
}