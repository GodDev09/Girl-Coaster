using UnityEngine;

namespace Kart
{
	public class MainKartController : MonoBehaviour
	{
		public KartTrackMovement TrackMovement { get; private set; }
		public Wagon Wagon { get; private set; }
		public TrainEngine TrainEngine { get; private set; }
		public KartFlyMovement FlyMovement { get; private set; }
		public AddedKartsManager AddedKartsManager { get; private set; }
		public PlayerAudio PlayerAudio { get; private set; }
		public Collider BoxCollider { get; private set; }
		public KartCounter KartCounter { get; private set; }

		public Passenger Passenger1{ get; private set; }
		public Passenger Passenger2{ get; private set; }
		
		public GameObject kartParent, characterPairsParent;

		public Collider kartCollider;
		public Fever fever;

		public bool isInitialised;
		[SerializeField] private ParticleSystem explosionParticle;

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
			TrackMovement = GetComponent<KartTrackMovement>();
			Wagon = GetComponent<Wagon>();
			TrainEngine = GetComponent<TrainEngine>();
			FlyMovement = GetComponent<KartFlyMovement>();
			AddedKartsManager = GetComponent<AddedKartsManager>();
			BoxCollider = GetComponent<Collider>();
			PlayerAudio = GetComponent<PlayerAudio>();
			KartCounter = GetComponent<KartCounter>();
			
			Passenger1 = characterPairsParent.transform.GetChild((int) UpgradeShopCanvas.only.MyCharacterSkin)
				.GetChild(0)
				.GetComponent<Passenger>();
			
			Passenger2 = characterPairsParent.transform.GetChild((int) UpgradeShopCanvas.only.MyCharacterSkin)
				.GetChild(1)
				.GetComponent<Passenger>();

			kartParent.transform.GetChild((int) UpgradeShopCanvas.only.MyKartSkin).gameObject.SetActive(true);
			characterPairsParent.transform.GetChild((int) UpgradeShopCanvas.only.MyCharacterSkin).gameObject.SetActive(true);
			
			isInitialised = true;
		}
		
		public void PlayExplosionParticle(Vector3 collisionPoint)
		{
			var expParticle = Instantiate(explosionParticle);
			expParticle.transform.position = collisionPoint;
			expParticle.Play();
		}
		
		private void OnReachEndOfTrack()
		{
			TrackMovement.StopFollowingTrack();
			Wagon.enabled = false;
			TrainEngine.enabled = false;
		}
	}
}