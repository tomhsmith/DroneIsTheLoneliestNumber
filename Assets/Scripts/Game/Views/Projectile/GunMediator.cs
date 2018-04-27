using strange.extensions.mediation.impl;

namespace DroneDefender.Game.Views.Projectile {
	public class GunMediator : Mediator {
		[Inject] public GunView View { get; set; }

		public override void OnRegister() {
			base.OnRegister();

			View.Init();
		}
	}
}
