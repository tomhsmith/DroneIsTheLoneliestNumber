using DroneDefender.Game.Damage;
using strange.extensions.mediation.impl;

namespace DroneDefender.Game.Views.Vehicle {
	public class TankMediator : Mediator {
		// Settings
		public float StartingHealth = 500f;

		[Inject] public TankView View { get; set; }

		// Local Components
		private float _health;

		public override void OnRegister() {
			base.OnRegister();
			_health = StartingHealth;

			View.Init();
			UpdateHealthText();
		}

		private void Damage(IDamageParameter damage) {
			if (!enabled) return;

			_health -= damage.Amount;

			if(_health > 0) {
				UpdateHealthText();
				return;
			}

			View.Die();
			enabled = false;
		}

		private void UpdateHealthText() {
			View.SetTextMesh(
				$"{_health} HP", 
				Constants.CommonColors.GetHealthColor(_health / StartingHealth)
			);
		}
	}
}

