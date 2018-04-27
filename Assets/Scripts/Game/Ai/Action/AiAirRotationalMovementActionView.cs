using DroneDefender.Common.Extension;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace DroneDefender.Game.Ai.Action {
	public class AiAirRotationalMovementActionView : View, IAiAction {
		// Settings
		public float MovementSpeed = 75f;
		public float HoverForce = 45f;

		public Signal<bool> OnCompleteSignal => _onCompleteSignal;
		private Signal<bool> _onCompleteSignal { get; set; }

		// Local Components
		private Rigidbody _rigidbody;
		private Vector3? _postion;
		private Vector3 _target;

		#region Base View

		public void Init() {
			enabled = true;
			_onCompleteSignal = new Signal<bool>();

			_rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
			_postion = _rigidbody?.position;
		}

		public void FixedUpdate() {
			if(!enabled || _postion == null) { return; }

			MovementHandler();
		}

		private void MovementHandler() {
			var movement = (_target - _postion.Value).normalized * MovementSpeed;
			movement = new Vector3(movement.x, _rigidbody.position.y, movement.z);
			_rigidbody.transform.LookAt(movement);

			movement.y = HoverForce;
			_rigidbody.MovePosition(_rigidbody.position + (movement * Time.fixedDeltaTime));

			CompleteAction();
		}

		#endregion

		#region Base IAiAction

		public void ExecuteOn(Vector3 target) {
			_target = target;
			enabled = true;
		}

		public IAiAction Configure<T>() where T : View {
			throw new System.NotImplementedException(typeof(T).Name);
		}

		private void CompleteAction() {
			_onCompleteSignal.Dispatch(true);
			enabled = false;
		}

		#endregion
	}
}
