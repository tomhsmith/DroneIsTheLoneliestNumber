using DroneDefender.Game.Model;
using DroneDefender.Game.Signals;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace DroneDefender.Game.Inputs {
	internal class VRCameraRigMediator : Mediator {
		// Cross Context
		[Inject] public VRCameraRigView View { get; set; }

		[Inject(PlayerContexts.LocalPlayer)]
		public ILocalPlayerModel LocalPlayer { get; set; }

		#region Base Mediator

		public override void OnRegister() {
			base.OnRegister();

			View.Init();
			LocalPlayer.MainCamera = View.MainCamera.GetComponent<Camera>();

			LocalPlayer.MoveSignal.AddListener(MoveHandler);
			LocalPlayer.RotateSignal.AddListener(RotateHandler);
		}

		#endregion

		#region Handlers

		private void MoveHandler(Vector3 to) {
			View.MoveHandler(to);
		}

		private void RotateHandler(Vector3 to) {
			View.RotateSignal.Dispatch(to);
		}

		#endregion
	}
}


