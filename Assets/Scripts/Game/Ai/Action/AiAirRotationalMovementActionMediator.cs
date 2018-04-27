using strange.extensions.mediation.impl;

namespace DroneDefender.Game.Ai.Action {
	public class AiAirRotationalMovementActionMediator : Mediator {
		[Inject] public AiAirRotationalMovementActionView View { get; set;}

		public override void OnRegister() {
			base.OnRegister();

			View.Init();
		}
	}
}
