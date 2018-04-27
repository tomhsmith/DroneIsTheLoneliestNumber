using DroneDefender.Game.Damage;
using DroneDefender.Game.Signals.Explosion;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace DroneDefender.Game.Views.Explosion {
	public class SimpleExplosionMediator : Mediator {
		// Settings
		public float DPS = 150f;

		// Cross Context
		[Inject] public SimpleExplosionView View { get; set; }
		[Inject] public DamageSignal DamageSignal { get; set; }

		public override void OnRegister() {
			base.OnRegister();

			View.Init();
		}

		private void OnTriggerEnter(Collider other) {
			if (other == null || other.gameObject == null) {
				return;
			}

			DamageSignal.Dispatch(other.gameObject, new DamageParameter(DPS, 0));
		}
	}
}

