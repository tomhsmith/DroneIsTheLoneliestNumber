using DroneDefender.Common.Extension;
using DroneDefender.Game.Ai.Action;
using DroneDefender.Game.Views;
using strange.extensions.mediation.impl;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DroneDefender.Game.Ai.Controller {
	internal class AiBasicControllerView : View, IAiController {
		// Settings
		public string ContainerName = "Ai Controller Container";
		public float ProjectileLaunchTolerance = 500f;
		public float MovementTolerance = 100f;
		public float MovementMinimum = 50f;
		public float DieTTLS = 10f;

		public AiTargetBase CurrentTarget => _targets.FirstOrDefault();

		// Actions
		private IAiAction _movementController { get; set; }
		private IAiAction _projectileController { get; set; }

		// Local Components
		private List<AiTargetBase> _targets;
		private Vector3 _position;
		private GameObject _currentGO;
		private Rigidbody _rigidbody;
		private float _projectileRange;
		private bool _viewConfigured;
		//	IAI Controller Components
		private IControllable _controllable;

		#region Base View

		public void Init(float projectileRange = 500f) {
			_targets = new List<AiTargetBase>();
			_projectileRange = projectileRange;

			// Setup
			//	Game Object
			_currentGO = gameObject;
			_currentGO.name = ContainerName;
		}

		public AiBasicControllerView ConfigureLaunchable<T>() 
			where T : View, ILaunchableFromPositionRotation {
			_projectileController.Configure<T>();
			return this;
		}

		public AiBasicControllerView ConfigureView<T>() where T : View, IControllable {
			if(_viewConfigured) {
				Debug.LogError($"View already configured and attempted again on {GetType().Name}.");
				return this;
			}

			_viewConfigured = true;

			// Create Container
			var go = new GameObject(typeof(T).Name);
			go.transform.parent = gameObject.transform;
			go.transform.localPosition = Vector3.zero;

			// Setup
			//	View
			_controllable = go.AddComponent<T>();
			_rigidbody = _controllable.MovementGO?.GetOrAddComponent<Rigidbody>();
			//	AI
			_movementController = _controllable.MovementGO
				.AddComponent<AiRotationalMovementActionView>();
			_projectileController = _controllable.ProjectileContainerGO
				.AddComponent<AiRotationalFireActionView>();

			return this;
		}

		private void FixedUpdate() {
			// No target.
			if (CurrentTarget == null) { return; }

			// Refresh our position.
			_position = _rigidbody.transform.position;

			CalculateActionPath(CurrentTarget);
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
			var targetPosition = target?.GameObject?.transform.position;
			if(targetPosition == null) { return; }

			var remaining = targetPosition.Value - _position;

			int range = 0;
			if (target.TargetType == TargetTypes.MOVEMENT) {
				range = CheckMovementRange(remaining);

				// We found it, remove it.
				if (range == 0 || target.GameObject == null) {
					_targets.Remove(target);
					return;
				}

				// Continue the move.
				_movementController.ExecuteOn(
					(range > 0)
						? targetPosition.Value
						: -targetPosition.Value
				);
			} else if (target.TargetType == TargetTypes.ENEMY) {
				range = CheckProjectileRange(remaining);

				// Target in range!
				if (range == 0) {
					_projectileController.ExecuteOn(targetPosition.Value);
					return;
				} else {
					// Keep moving..
					_movementController.ExecuteOn(
						(range > 0)
							? targetPosition.Value
							: -targetPosition.Value
					);
				}
			}
		}

		private int CheckMovementRange(Vector3 remaining) {
			var distance = (new Vector2(remaining.x, remaining.z)).magnitude;

			if (distance > MovementTolerance) {
				return 1;
			}
			if (distance < MovementTolerance) {
				return 0;
			}

			return 0;
		}

		private int CheckProjectileRange(Vector3 remaining) {
			var range = (new Vector2(remaining.x, remaining.z)).magnitude;

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

