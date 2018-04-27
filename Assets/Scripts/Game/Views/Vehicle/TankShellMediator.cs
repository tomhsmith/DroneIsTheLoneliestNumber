using strange.extensions.mediation.impl;

namespace DroneDefender.Game.Views.Vehicle {
	public class TankShellMediator : Mediator {
		[Inject] public TankShellView View { get; set; }

		public override void OnRegister() {
			View.Init();
		}
	}
}
