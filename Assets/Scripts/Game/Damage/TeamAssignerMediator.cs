using DroneDefender.Common;
using DroneDefender.Game.Constants;
using DroneDefender.Game.Views;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace DroneDefender.Game.Damage {
	public class TeamAssignerMediator : Mediator {
		public string TeamName => $"Team {View.Team}";

		[Inject] public TeamAssignerView View { get; set; }

		[Inject(InjectableTypes.TeamContainer)]
		public NullableResultDictionary<GameObject> TeamContainer { get; set; }

		public override void OnRegister() {
			base.OnRegister();

			gameObject.tag = TeamName;
			TeamContainer.Add(gameObject);

			View.Init();
		}


		internal void Damage(IDamageParameter parameter) {
			View.Health -= parameter.Amount;

			if (View.Health > 0) { return; }

			gameObject.SendMessage(MethodNames.Die, SendMessageOptions.DontRequireReceiver);
		}
	}
}
