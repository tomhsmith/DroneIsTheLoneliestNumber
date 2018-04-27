using DroneDefender.Common.Extension;
using DroneDefender.Game.Constants;
using DroneDefender.Game.Damage;
using DroneDefender.Game.Views.Drone;
using strange.extensions.context.api;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace DroneDefender.Game.Views.Projectile {
	public class MissileView : View, ILaunchableToDestination {
		// Settings
		public float ImpulseMultiplier = 1.5f;

		// Cross Context
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject ContextView { get; set; }

		// Local Components
		private Vector3 _target;
		private Rigidbody _rigidbody;
		private float _range = float.MaxValue;
		private bool _hasLaunched;

		#region Base View

		public void Init() {
			_rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
		}

		private void FixedUpdate() {
			if(!_hasLaunched || !enabled) { return; }

			var newRange = (_target - transform.position).magnitude;
			if(newRange > _range) {
				gameObject.SendMessage(MethodNames.Damage, new DamageParameter(5, 0));
			}
			_range = newRange;

			_rigidbody.transform.LookAt(_target);
			_rigidbody.AddRelativeForce(Vector3.forward * ImpulseMultiplier, ForceMode.Impulse);
		}

		#endregion

		public GameObject Launch(Vector3? from, Vector3 target) {
			if(!enabled) { return null;	}

			_rigidbody.useGravity = false;
			_rigidbody.isKinematic = false;

			_target = target;
			_hasLaunched = true;

			transform.parent = ContextView.transform;
			return gameObject;
		}

	}
}
