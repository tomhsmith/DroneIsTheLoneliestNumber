using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using DroneDefender.Common.Extension;

namespace DroneDefender.Game.Ai.Action {
	public class AiRotationalMovementActionView : View, IAiAction {
		// Settings
		public float RotationSpeed = 8f; 
		public float ForwardThrust = 800f;
		public float Accuracy = 8f;

		public Signal<bool> OnCompleteSignal { get { return _onCompleteSignal; } }
		private Signal<bool> _onCompleteSignal;

		// Local components
		private Vector3 _target;
		private Rigidbody _rigidbody;

		#region Base View

		public void Init() {
			enabled = true;

			_onCompleteSignal = new Signal<bool>();
			_rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
		}

		private void FixedUpdate() {
			if (!enabled) { return; }

			DetectFall();
			LookAtTarget();
			MoveFoward();
		}

		private void LookAtTarget() {
			var remaining = _target - transform.position;
			// Create the rotation we need to be in to look at the target.
			var lookRotation = Quaternion.LookRotation(remaining);
			// Rotate us over time according to speed until we are in the required rotation.
			transform.rotation = Quaternion.Slerp(
				transform.rotation, 
				lookRotation,
				Time.deltaTime * RotationSpeed
			);
		}

		private void DetectFall() {
			float maxFall = -20f;
			float newStart = 10f;

			// Halp, im falling.
			if (transform.position.y < maxFall) {
				Debug.Log("Halp my tank is falling..");
				transform.position = new Vector3(0f, newStart, 0f);
			}
		}

		#endregion

		#region IAiAction

		public void ExecuteOn(Vector3 target) {
			_target = target;
			enabled = true;
		}

		public IAiAction Configure<T>() where T : View {
			throw new System.NotImplementedException(typeof(T).Name);
		}

		private void MoveFoward() {
			_rigidbody.AddRelativeForce(
				Vector3.forward * (Time.deltaTime * ForwardThrust),
				ForceMode.Impulse
			);
			CompleteAction();
		}

		private void CompleteAction() {
			_onCompleteSignal.Dispatch(true);
			enabled = false;
		}

		#endregion
	}
}
