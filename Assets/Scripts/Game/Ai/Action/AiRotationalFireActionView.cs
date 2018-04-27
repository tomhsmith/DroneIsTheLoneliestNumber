using DroneDefender.Game.Views;
using DroneDefender.Resource.Tag;
using strange.extensions.context.api;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace DroneDefender.Game.Ai.Action {
	class AiRotationalFireActionView : View, IAiAction {
		// Settings
		public string ProjectileContainerName = "Projectile Top";
		public float RotationSpeed = 100f;
		public float ReloadTime = 8f;

		// Cross context
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject ContextView { get; set; }

		public Signal<bool> OnCompleteSignal { get { return _onCompleteSignal; } }
		private Signal<bool> _onCompleteSignal;

		public bool CanFire { get { return _lastFireAge > ReloadTime; } }

		// Local Components
		private Transform _launcherSpawn;
		private Vector3 _target;
		private System.Type _projectileType;
		private float _lastFireAge;

		#region Base View

		public void Init() {
			_onCompleteSignal = new Signal<bool>();

			foreach (Transform child in transform) {
				if (child.gameObject.tag == TagResources.ProjectileSpawn) {
					_launcherSpawn = child;
				}
				foreach (Transform gc in child) {
					if (gc.gameObject.tag == TagResources.ProjectileSpawn) {
						_launcherSpawn = gc;
					}
				}
				break;
			}

			if(_launcherSpawn == null) {
				Debug.Log("No launcher spawn found for controller.");
				Destroy(this);
				return;
			}
		}

		private void FixedUpdate() {
			_lastFireAge += Time.fixedDeltaTime;
		}

		private void AimAtTarget() {
			var randomUpAngle = Random.Range(-25f, -8f);
			var randomLeftAngle = Random.Range(-5f, 5f);

			// point the end of the gun
			transform.LookAt(_target);
			// lets aim up a bit.. gravity..
			transform.Rotate(Vector3.right, randomUpAngle);
			transform.Rotate(Vector3.up, randomLeftAngle);
		}

		#endregion

		#region IAiAction

		public void ExecuteOn(Vector3 target) {
			// find the vector pointing from our position to the target
			_target = target;

			if (!CanFire) {
				_onCompleteSignal.Dispatch(false);
				return;
			}

			LaunchProjectile();
		}

		public IAiAction Configure<T>() where T : View {
			_projectileType = typeof(T);
			return this;
		}

		private void LaunchProjectile() {
			AimAtTarget();

			var projectile = new GameObject(ProjectileContainerName);
			projectile.transform.parent = ContextView.transform;

			var projectileView = projectile.AddComponent(_projectileType) as ILaunchableFromPositionRotation;
			projectileView.Launch(_launcherSpawn.position, _launcherSpawn.rotation);

			CompleteAction();
		}

		private void CompleteAction() {
			_lastFireAge = 0;
			_onCompleteSignal.Dispatch(true);
		}

		#endregion
	}
}
