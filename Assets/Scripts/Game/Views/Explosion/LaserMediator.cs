using strange.extensions.mediation.impl;

namespace DroneDefender.Game.Views.Explosion {
	public class LaserMediator : Mediator {
		[Inject] public LaserView View { get; set; }

		public override void OnRegister() {
			base.OnRegister();

			View.Init();
		}
	}
}

