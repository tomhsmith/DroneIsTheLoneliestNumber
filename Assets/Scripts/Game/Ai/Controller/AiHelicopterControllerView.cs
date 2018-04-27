using DroneDefender.Game.Ai.Action;
using DroneDefender.Game.Views.Drone;
using DroneDefender.Game.Views.Vehicle;
using strange.extensions.mediation.impl;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DroneDefender.Game.Ai.Controller {
	public class AiHelicopterControllerView : View, IAiController {
		public const string ContainerName = "Ai Controller";
		// Settings
		public float ProjectileLaunchTolerance = 1000f;
		public float MovementTolerance = 50f;
		public float MovementMinimum = 50f;
		public float DieTTLS = 10f;

		public AiTargetBase CurrentTarget { get { return _targets.FirstOrDefault(); } }

		// Local Components
		private List<AiTargetBase> _targets;
		private GameObject _currentGO;
		private Rigidbody _rigidbody;
		private Vector3 _position;
		private Vector3? _currentTargetPosition;
		private float _projectileRange;
		private bool _hasFired;
		private bool _viewConfigured;
		//	IAI Controller Components
		private IControllable _controllable;
		//	Actions
		private IAiAction _movementController { get; set; }
		private ILaunchableToDestination _launchable { get; set; }

		#region Base View 

		public void Init(float projectileRange = 1000f) {
			_targets = new List<AiTargetBase>();

			_projectileRange = projectileRange;

			// Setup
			//	Game Object
			_currentGO = gameObject;
			_currentGO.name = ContainerName;
		}

		private void FixedUpdate() {
			// No target.
			if (!enabled || CurrentTarget == null) { return; }

			// Refresh our position.
			_position = _rigidbody.transform.position;

			_currentTargetPosition = CurrentTarget?.GameObject?.transform.position;
			CalculateActionPath(CurrentTarget);
		}

		public AiHelicopterControllerView Configure<T>() where T : View, IControllable {
			if (_viewConfigured) {
				Debug.LogError($"View already configured and attempted again on {GetType().Name}.");
				return this;
			}
			_viewConfigured = true;

			// Create Container
			var go = new GameObject("Helicopter Top");
			go.transform.parent = gameObject.transform;
			go.transform.localPosition = Vector3.zero;

			// Setup
			//	View
			_controllable = go.AddComponent<T>();
			_rigidbody = _controllable.MovementGO.GetComponent<Rigidbody>();
			//	AI
			_movementController = _controllable.MovementGO
				.AddComponent<AiAirRotationalMovementActionView>();

			var helicopterView = _controllable as HelicopterView;
			_launchable = helicopterView?.Projectile;

			return this;
		}

		#endregion

		#region Base IAiController

		public AiStandardTarget AddTarget(
				GameObject go,
				TargetTypes targetType = TargetTypes.MOVEMENT,
				PriorityLevels priority = PriorityLevels.MEDIUM) {
			var newStandardTarget = new AiStandardTarget(go, targetType, priority);
			_targets.Add(newStandardTarget);
			return newStandardTarget;
		}

		public AiStandardTarget SetSingleTarget(
				GameObject go,
				TargetTypes targetType = TargetTypes.MOVEMENT,
				PriorityLevels priority = PriorityLevels.MEDIUM) {

			var newStandardTarget = new AiStandardTarget(go, targetType, priority);
			_targets.Insert(0, newStandardTarget);
			return newStandardTarget;
		}

		public void Die() {
			enabled = false;
			Destroy(gameObject, DieTTLS);
		}

		#endregion

		#region Controller Helpers

		private void CalculateActionPath(AiTargetBase target) {
			if (_currentTargetPosition == null) { return; }

			var targetPosition = target.GameObject.transform.position;
			var remaining = targetPosition - _position;

			// TODO: Request new target.
			if (target.GameObject == null) { return; }

			int range = 0;
			if (target.TargetType == TargetTypes.MOVEMENT || _hasFired) {
				range = CheckMovementRange(remaining);

				// We found it, remove it.
				if (range == 0 || target.GameObject == null) {
					_targets.Remove(target);
					return;
				}

				// Continue the move.
				_movementController.ExecuteOn(
					(range > 0)
						? targetPosition
						: -targetPosition
				);
			} else if (target.TargetType == TargetTypes.ENEMY) {
				range = CheckProjectileRange(remaining);

				// Target in range!
				if (range == 0) {
					_launchable?.Launch(null, target.GameObject.transform.position);
					_hasFired = true;
					return;
				} else {
					// Keep moving..
					_movementController.ExecuteOn(
						(range > 0)
							? targetPosition
							: -targetPosition
					);
				}
			}
		}

		private int CheckMovementRange(Vector3 remaining) {
			var distance = remaining.magnitude;

			if(_hasFired) { return 1; }

			if (distance > _rigidbody.transform.position.y + MovementTolerance) {
				return 1;
			}
			if (distance < _rigidbody.transform.position.y - MovementTolerance) {
				return 0;
			}

			return 0;
		}

		private int CheckProjectileRange(Vector3 remaining) {
			var range = remaining.magnitude;

			if (range > _projectileRange + ProjectileLaunchTolerance) {
				return 1;
			}
			if (range < _projectileRange - ProjectileLaunchTolerance) {
				return -1;
			}

			return 0;
		}

		#endregion
	}
}
