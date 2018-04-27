using DroneDefender.Common.Extension;
using DroneDefender.Game.Constants;
using DroneDefender.Game.Damage;
using DroneDefender.Game.Views.Explosion;
using DroneDefender.Resource.Tag;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using UnityEngine;

namespace DroneDefender.Game.Controller {
	public class DamageCommand : Command {
		// Settings
		public const int IgnoreRaycastLayer = 2;
		public float BuildingBlockDieTTLS = 10f;

		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject ContextView { get; set; }

		[Inject] public EffectsQueue MainEffectsQueue { get; set; }
		[Inject] public GameObject HitGO { get; set; }
		[Inject] public DamageParameter Parameter { get; set; }

		public override void Execute() {
			if (HitGO.tag == TagResources.BuildingMaterial) {
				if (HitGO == null) { return; }

				Parameter = new DamageParameter(1, Parameter.PlayerId);
				SendMessage(HitGO.transform.parent.gameObject);

				HitGO.transform.parent = ContextView.transform;
				HitGO.layer = IgnoreRaycastLayer;
				HitGO.tag = TagResources.UnTagged;

				var rb = HitGO.GetOrAddComponent<Rigidbody>();
				rb.isKinematic = false;
				rb.useGravity = true;

				MainEffectsQueue.Enqueue(() => {
					if (HitGO == null) { return; }

					Object.Destroy(HitGO, BuildingBlockDieTTLS);
				});


				return;
			}

			SendMessage(HitGO);
		}

		private DamageCommand SendMessage(GameObject go) {
			go.SendMessageUpwards(
				MethodNames.Damage,
				Parameter,
				SendMessageOptions.DontRequireReceiver
			);

			return this;
		}
	}
}

