using strange.extensions.mediation.impl;

namespace DroneDefender.Game.Ai.Controller {
	class AiHelicopterControllerMediator : Mediator {
		[Inject] public AiHelicopterControllerView View { get; set; }

		public override void OnRegister() {
			base.OnRegister();

			View.Init();
		}
	}
}
