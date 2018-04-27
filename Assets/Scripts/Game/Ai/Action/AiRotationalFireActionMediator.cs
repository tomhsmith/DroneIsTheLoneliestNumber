using strange.extensions.mediation.impl;

namespace DroneDefender.Game.Ai.Action {
	class AiRotationalFireActionMediator : Mediator {
		[Inject] public AiRotationalFireActionView View { get; set; }

		public override void OnRegister() {
			base.OnRegister();

			View.Init();
		}
	}
}
