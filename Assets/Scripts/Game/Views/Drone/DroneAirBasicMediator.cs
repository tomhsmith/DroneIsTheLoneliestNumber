using strange.extensions.mediation.impl;

namespace DroneDefender.Game.Views.Drone {
	public class DroneAirBasicMediator : Mediator {
		[Inject]
		public DroneAirBasicView View { get; set; }

		public override void OnRegister() {
			base.OnRegister();

			View.Init();
		}
	}
}
