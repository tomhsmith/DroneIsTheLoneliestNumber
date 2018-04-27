using DroneDefender.Game.Model;
using DroneDefender.Game.Signals;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace DroneDefender.Game.Inputs {
	internal class VRInputRightMediator : Mediator {
		// Cross Context
		[Inject] public VRInputRightView View { get; set; }
		[Inject] public DroneMothershipModel Mothership { get; set; }

		[Inject(PlayerContexts.LocalPlayer)]
		public ILocalPlayerModel LocalPlayer { get; set; }

		public override void OnRegister() {
			base.OnRegister();

			View.Init();
			View.ControllerSignal.AddListener(OnControllerEvent);
		}

		private void OnControllerEvent(InputContexts context){
			Debug.Log($"{GetType().Name} received {context}");

			switch(context) {
				case InputContexts.TRIGGER_CLICKED:
					LocalPlayer.LaunchDroneSignal.Dispatch(true);
					break;
				case InputContexts.PAD_TOUCHED:
					Mothership.MoveSignal
						.Dispatch(new Vector3(
							View.TouchpadVector.x, 
							0f, 
							View.TouchpadVector.y
						));
					break;
			}
		}
	}
}

