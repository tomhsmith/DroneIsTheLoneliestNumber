using DroneDefender.Game.Signals.Explosion;
using DroneDefender.Game.Views.Explosion;
using DroneDefender.Resource.Prefab;
using DroneDefender.Resource.Tag;
using strange.extensions.context.api;
using strange.extensions.mediation.impl;
using System.Collections.Generic;
using UnityEngine;

namespace DroneDefender.Game.Views.Drone {
	public class DroneAirBasicView : View, ILaunchableToDestination {
		public const string ContainerName = "Drone Container";
		// Settings
		public float ImpulseMultiplier = 3f;
		public List<string> ExcludeTags = new List<string> {
			TagResources.Drone,
			TagResources.ProjectileContainer,
			TagResources.ProjectileSpawn,
			TagResources.Explosion,
		};

		// Cross context
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject ContextView { get; set; }

		[Inject] public EffectsQueue MainEffectsQueue { get; set; }
		[Inject] public AddExplosionSignal AddExplosionSignal { get; set; }

		// Local Components
		private GameObject _currentGO;
		private Rigidbody _rigidbody;
		private Vector3 _target = Vector3.down;
		private float _distanceFromTarget;
		private bool _hasLaunched;

		#region Base View

		public void Init() {
			_currentGO = Instantiate(
				Resources.Load<GameObject>(PrefabResources.DroneAirBasic), 
				transform
			);
			_currentGO.name = ContainerName;

			ConfigureComponent(ref _rigidbody, gameObject);
		}	

		private void FixedUpdate() {
			MoveHandler();
			DeathHandler();
		}

		private void OnTriggerEnter(Collider other) {
			var otherTag = other.gameObject.tag;

			if(!ExcludeTags.Contains(otherTag)) {
				Explode();
				Debug.Log($"Drone explosion Triggered by [{other.name} - {otherTag}]");
			}
		}

		#region Helpers

		private T ConfigureComponent<T>(ref T obj, GameObject go) where T : Component {
			var component = go.AddComponent<T>();

			var rb = component as Rigidbody;
			if(rb != null) {
				rb.useGravity = false;
				rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				rb.transform.parent = go.transform;
			}
			obj = component;

			return component;
		}

		private void MoveHandler() {
			// Don't let the pony outta the barn quite yet.
			if (!_hasLaunched) { return; }

			// Drop and bomb
			// TODO: Move this to an AI component.
			_rigidbody.transform.LookAt(_target);
			_rigidbody.AddRelativeForce(Vector3.forward * ImpulseMultiplier, ForceMode.Impulse);
		}

		private void DeathHandler() {
			// Check if distance from target is growing, if so explode.
			var currentDistance = (_rigidbody.transform.position - _target).magnitude;
			if (_distanceFromTarget > 5f 
				&& currentDistance > _distanceFromTarget) {
				Explode();
			}
			// Update Distance
			_distanceFromTarget = currentDistance;
		}

		#endregion

		#endregion

		#region ILaunchable

		/// <summary>
		/// Launch the specified with from and target.
		/// </summary>
		/// <param name="from">Starting vector.</param>
		/// <param name="target">End target vector.</param>
		public GameObject Launch(Vector3? from, Vector3 target) {
			if(from != null) {
				_rigidbody.transform.position = from.Value;
			}
			_target = target;

			_hasLaunched = true;
			return _currentGO;
		}

		internal void Explode() {
			if (_rigidbody == null) { return; }
			AddExplosionSignal.Dispatch(_rigidbody.transform.position, transform, 0f);
		}

		#endregion
	}
}
	