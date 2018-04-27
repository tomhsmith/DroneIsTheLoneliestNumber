using DroneDefender.Game.Ai.Controller;
using DroneDefender.Game.Signals.Ai;
using DroneDefender.Game.Signals.Generic;
using DroneDefender.Game.Views;
using DroneDefender.Game.Views.Explosion;
using DroneDefender.Resource.Tag;
using strange.extensions.context.api;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DroneDefender.Game.Model {
	public class LocalPlayerModel : ILocalPlayerModel, IPlayer {
		// Settings
		public Vector3 MainCameraRaycastVector = new Vector3(.5f, .5f, 0f);
		public float RaycastDistance = 5000f;
		public int MaxDrones = 2;

		// Cross Context
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject ContextView { get; set; }

		[Inject(InjectableTypes.PlayerTarget)]
		public GameObject PlayerTargetPrefab { get; set; }

		[Inject] public EffectsQueue MainEffectsQueue { get; set; }

		// IPlayer
		public List<IAiController> Spawns => _spawns.ToList();
		private List<IAiController> _spawns = new List<IAiController>();

		public int Id => _id;
		private int _id = 0;

		public int TeamId => _teamId;
		private int _teamId = 0;

		public float PlayerTargetTTLS => _playerTargetTTLS;
		private float _playerTargetTTLS  = .5f;
		
		public Vector3? LastRaycastHitPoint { get; set; }

		// Signals
		public MoveSignal MoveSignal { get; private set; }
		public RotateSignal RotateSignal { get; private set; }
		public PrimaryFireSignal FireLaserSignal { get; private set; }
		public SecondaryFireSignal LaunchDroneSignal { get; private set; }
		// Command Signals
		public AddPlayerTargetSignal AddTargetSignal { get; set; }

		// Set during camera rig initialization.
		// Public setter here because of interface.
		public Camera MainCamera { get; set; }

		// Local Components
		private List<Vector3> _targetVectors;

		public LocalPlayerModel() {
			_targetVectors = new List<Vector3>();

			// Setup Local Signals
			MoveSignal = new MoveSignal();
			RotateSignal = new RotateSignal();

			FireLaserSignal = new PrimaryFireSignal();
			FireLaserSignal.AddListener(AddTarget);

			LaunchDroneSignal = new SecondaryFireSignal();
			LaunchDroneSignal.AddListener(AddTarget);

			AddTargetSignal = new AddPlayerTargetSignal();
			AddTargetSignal.AddListener(AddTarget);
		}

		private void AddTarget(bool newRaycastHit) {
			if(newRaycastHit) {
				 GetRaycastHit();
			}

			// No hit.
			if (LastRaycastHitPoint == null) { return; }

			// Add target, allows for multiple firing.
			var newTarget = GameObject.Instantiate(PlayerTargetPrefab, ContextView.transform);
			newTarget.transform.position = LastRaycastHitPoint.Value;

			MainEffectsQueue.Enqueue(() => {
				GameObject.Destroy(newTarget, _playerTargetTTLS);
			}, true);
		}

		private RaycastHit GetRaycastHit() {
			var camera = MainCamera;
			var cameraRay = camera.ViewportPointToRay(MainCameraRaycastVector);
			RaycastHit hit;

			// Did the camera center hit something?
			if (Physics.Raycast(cameraRay, out hit, RaycastDistance, ~(1 << 2))) {
				LastRaycastHitPoint = hit.point;
			} else {
				LastRaycastHitPoint = null;
			}

			return hit;
		}

		public List<Vector3> GetTargetVectors() {
			_targetVectors.Clear();

			// Too many concurrent drones lag system.
			// TODO: Implement explosion queue.
			var maxDrones = MaxDrones;
			var playerTargets = GameObject.FindGameObjectsWithTag(TagResources.PlayerTarget);

			if(!playerTargets.Any()) {
				return new List<Vector3> { LastRaycastHitPoint.Value };
			}

			// Add up until the limit.
			foreach(var target in playerTargets) {
				_targetVectors.Add(target.transform.position);
				if(--maxDrones < 1) {
					break;
				}
			}
			return _targetVectors;
		}
	}
}

