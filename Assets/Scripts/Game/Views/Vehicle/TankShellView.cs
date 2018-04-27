using DroneDefender.Game.Views.Explosion;
using DroneDefender.Resource.Prefab;
using DroneDefender.Resource.Tag;
using strange.extensions.mediation.impl;
using System.Collections.Generic;
using UnityEngine;

namespace DroneDefender.Game.Views.Vehicle {
	public class TankShellView : View, ILaunchableFromPositionRotation {
		public const string ContainerName = "Tank Shell Container";
		// Settings
		public float BulletSpeed = 15 * 1000f;
		public float DropPoint = -30f;
		public readonly List<string> ExcludeTags = new List<string> {
			TagResources.TankTop,
			TagResources.ProjectileSpawn,
			TagResources.ProjectileContainer,
			TagResources.Explosion,
		};

		// Cross Context
		[Inject] public EffectsQueue MainEffectsQueue { get; set; }

		// Local Components
		private GameObject _currentGO;
		private Rigidbody _rigidBody;

		private bool _isFiring;
		private bool _hasFired;

		#region Base View

		internal void Init() {
			var projectTilePrefab = Instantiate(Resources.Load<GameObject>(PrefabResources.TankShell));
			projectTilePrefab.transform.parent = transform;

			_currentGO = gameObject;
			_currentGO.name = ContainerName;

			_rigidBody = _currentGO.gameObject.AddComponent<Rigidbody>();
		}

		private void FixedUpdate() {
			if(!_hasFired && _isFiring) {
				_rigidBody.AddRelativeForce(Vector3.forward * BulletSpeed);
				_rigidBody.AddRelativeTorque(Vector3.forward * BulletSpeed);

				_hasFired = true;
				_isFiring = false;
			}

			if (_currentGO.transform.position.y < DropPoint) {
				Explode();
			}
		}

		private void OnTriggerEnter(Collider other) {
			if(!_hasFired) {
				return;
			}

			Debug.Log($"Tank Shell Triggered by [{other.name} - {other.gameObject.tag}]");
			if(!ExcludeTags.Contains(other.gameObject.tag)) {
				Explode();
			}
		}

		#endregion

		#region ILaunchable

		public GameObject Launch(Vector3 fromPosition, Quaternion startingRotation) {
			_isFiring = true;

			_currentGO.transform.position = fromPosition;
			_currentGO.transform.rotation = startingRotation;

			return _currentGO;
		}

		#endregion

		private void Explode() {
			_rigidBody.isKinematic = true;

			// TODO: Move this to explosion queue.
			var explosion = new GameObject("Explosion Top");
			explosion.transform.parent = _currentGO.transform.parent;
			explosion.transform.localPosition = _currentGO.transform.localPosition;
			explosion.AddComponent<SimpleExplosionView>();

			MainEffectsQueue.Enqueue(() => {
				Destroy(_currentGO);
			});
		}
	}
}
