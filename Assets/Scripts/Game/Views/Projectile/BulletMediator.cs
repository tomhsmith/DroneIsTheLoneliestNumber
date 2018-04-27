using strange.extensions.mediation.impl;

namespace DroneDefender.Game.Views.Projectile {
	public class BulletMediator : Mediator {
		[Inject] public BulletView View { get; set; }

		public override void OnRegister() {
			base.OnRegister();

			View.Init();
		}
	}
}
