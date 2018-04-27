using DroneDefender.Game.Damage;
using DroneDefender.Game.Signals.Explosion;
using strange.extensions.mediation.impl;

namespace DroneDefender.Game.Views.Vehicle {
	public class HelicopterMediator : Mediator {
		// Settings
		public float StartingHealth = 300f;
		private float _health;

		// Cross Context
		[Inject] public HelicopterView View { get; set; }
		[Inject] public AddExplosionSignal AddExplosionSignal { get; set; }

		#region Base Mediator

		public override void OnRegister() {
			base.OnRegister();
			_health = StartingHealth;

			View.Init();
			UpdateHealthText();
		}

		#endregion

		private void Damage(IDamageParameter parameter) {
			_health -= parameter.Amount;

			if(_health > 0f) {
				UpdateHealthText();
				return;
			}
			enabled = false;
			View.Die();
		}

		private void UpdateHealthText() {
			View.SetTextMesh(
				$"{_health} HP",
				Constants.CommonColors.GetHealthColor(_health / StartingHealth)
			);
		}
	}
}
