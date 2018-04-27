using DroneDefender.Game.Damage;
using DroneDefender.Game.Signals.Explosion;
using DroneDefender.Resource.Tag;
using strange.extensions.mediation.impl;
using System.Collections.Generic;
using UnityEngine;

namespace DroneDefender.Game.Views.Projectile {
	public class MissileMediator : Mediator {
		[Inject] public MissileView View { get; set; }
		[Inject] public AddExplosionSignal AddExplosionSignal { get; set; }

		private List<string> _excludeTags = new List<string> {
			TagResources.ControllableMover,
			TagResources.UnTagged
		};

		public override void OnRegister() {
			base.OnRegister();

			View.Init();
		}

		private void OnTriggerEnter(Collider other) {
			if (_excludeTags.Contains(other.tag)) {
				return;
			}

			Damage(new DamageParameter(5f, 0));
		}

		private void Damage(IDamageParameter parameter) {
			enabled = false;
			AddExplosionSignal.Dispatch(transform.position, transform, 0f);
		}
	}
}
