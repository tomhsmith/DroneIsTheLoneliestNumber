using DroneDefender.Game.Model;
using DroneDefender.Game.Signals;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace DroneDefender.Game.Inputs {
	internal class VRInputLeftMediator : Mediator {
		// Settings
		public float MovementReductionDelta = 2f;
		public float VerticalMovementMultiplier = 4f;

		// Cross Context
		[Inject] public VRInputLeftView View { get; set; }

		[Inject(PlayerContexts.LocalPlayer)]
		public ILocalPlayerModel LocalPlayer { get; set; }

		#region Base Mediator

		public override void OnRegister() {
			base.OnRegister();

			View.Init();
			View.ControllerSignal.AddListener(OnControllerEvent);
		}

		private void OnControllerEvent(InputContexts context){
			Debug.Log($"{GetType().Name} received {context}.");

			// Trigger
			if (View.TriggerPressed) {
				LocalPlayer.FireLaserSignal.Dispatch(true);
			}

			// Touchpad
			if(context == InputContexts.PAD_TOUCHED) {
				SendTouchpadSignals(View.Gripped);
			}
		}

		#endregion

		#region Helpers

		private void SendTouchpadSignals(bool isGripped) {
			if(View.TouchpadVector.magnitude < View.TouchpadSensitivity) {
				return;
			}

			Vector3 hVector, 
				vVector;

			// Graipping causes lateral movement.
			if(isGripped) {
				hVector = Vector3.right * View.TouchpadVector.x / MovementReductionDelta;
				vVector = Vector3.up * View.TouchpadVector.y / MovementReductionDelta;
				LocalPlayer.MoveSignal.Dispatch(hVector + vVector);
			} else {
				hVector = Vector3.up * View.TouchpadVector.x / MovementReductionDelta * VerticalMovementMultiplier;
				vVector = Vector3.forward * View.TouchpadVector.y / MovementReductionDelta;

				LocalPlayer.RotateSignal.Dispatch(hVector);
				LocalPlayer.MoveSignal.Dispatch(vVector);
			}
		}

		#endregion
	}
}

