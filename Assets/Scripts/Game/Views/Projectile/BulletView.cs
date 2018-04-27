using DroneDefender.Common.Extension;
using DroneDefender.Game.Signals.Explosion;
using DroneDefender.Resource.Tag;
using strange.extensions.mediation.impl;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DroneDefender.Game.Views.Projectile {
	public class BulletView : View, ILaunchableFromPositionRotation {
		// Settings
		public float BulletThrust = 8000f;
		public float DieTTLS = 0f;
		public float BulletMass = 20f;
		public float BulletDrag = .01f;

		// Cross Context
		[Inject] public AddExplosionSignal AddExplosionSignal { get; set; }

		private Rigidbody _rigidbody;

		public readonly List<string> ExcludeTags = new List<string> {
			TagResources.Explosion,
			TagResources.ProjectileContainer,
			TagResources.ProjectileSpawn,
			TagResources.Drone
		};

		// Local Components
		private bool _hasFired;

		public void Init() {
			enabled = false;
		}

		private void FixedUpdate() {
			transform.LookAt(transform.position + _rigidbody?.velocity ?? Vector3.forward);
		}

		private void OnTriggerEnter(Collider other) {
			if (!_hasFired) { return; }

			Debug.Log($"Bullet Triggered by [{other.name} - {other.gameObject.tag}]");
			if (!ExcludeTags.Contains(other.gameObject.tag)) {
				AddExplosionSignal.Dispatch(transform.position, transform, DieTTLS);
			}

			enabled = false;
		}

		public GameObject Launch(Vector3 fromPosition, Quaternion startingRotation) {
			if(_hasFired) { return gameObject; }

			enabled = _hasFired = true;

			gameObject.transform.SetPositionAndRotation(fromPosition, startingRotation);

			_rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
			_rigidbody.useGravity = true;
			_rigidbody.isKinematic = false;
			_rigidbody.drag = BulletDrag;
			_rigidbody.mass = BulletMass;
			_rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

			_rigidbody.AddRelativeForce(Vector3.forward * BulletThrust, ForceMode.Impulse);

			return gameObject;
		}

	}
}


