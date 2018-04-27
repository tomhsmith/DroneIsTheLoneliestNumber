using DroneDefender.Game.Model;
using DroneDefender.Game.Signals;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace DroneDefender.Game.Inputs {
	public class FPSCameraRigMediator : Mediator {
		// Cross Context
		[Inject] public FPSCameraRigView View { get; set; }
		[Inject] public DroneMothershipModel MotherShip { get; set; }

		[Inject(PlayerContexts.LocalPlayer)]
		public ILocalPlayerModel LocalPlayer { get; set; }

		public override void OnRegister() {
			base.OnRegister();

			View.Init();
			View.ControllerSignal.AddListener(ControlHandler);

			LocalPlayer.MainCamera = View.MainCamera;
			LocalPlayer.MoveSignal.AddListener(MoveHandler);
		}

		#region Handlers

		private void ControlHandler(InputContexts context) {
			switch (context) {
				case InputContexts.MOUSE_0_PRESSED:
					LocalPlayer.FireLaserSignal.Dispatch(true);
					return;
				case InputContexts.MOUSE_1_PRESSED:
					LocalPlayer.LaunchDroneSignal.Dispatch(true);
					return;
				case InputContexts.SECONDARY_FORWARD_PRESSED:
					MotherShip.MoveSignal.Dispatch(Vector3.forward);
					return;
				case InputContexts.SECONDARY_BACK_PRESSED:
					MotherShip.MoveSignal.Dispatch(Vector3.back);
					return;
				case InputContexts.SECONDARY_LEFT_PRESSED:
					MotherShip.MoveSignal.Dispatch(Vector3.left);
					return;
				case InputContexts.SECONDARY_RIGHT_PRESSED:
					MotherShip.MoveSignal.Dispatch(Vector3.right);
					return;
			}
		}

		private void MoveHandler(Vector3 to) {
			View.Move(to);
		}

		#endregion
	}
}
