using DroneDefender.Game.Model;
using DroneDefender.Game.Signals;
using DroneDefender.Game.Views;
using DroneDefender.Game.Views.Explosion;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using UnityEngine;

namespace DroneDefender.Game.Controller {
	public class AddPlayerTargetCommand : Command {
		// Settings
		public float RaycastDistance = 5000f;
		public Vector3 MainCameraRaycastVector = new Vector3(.5f, .5f, 0f);

		// Cross Context
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject ContextView{ get; set; }

		[Inject(PlayerContexts.LocalPlayer)]
		public ILocalPlayerModel Player { get; set; }

		[Inject] public EffectsQueue MainEffectsQueue { get; set; }

		[Inject(InjectableTypes.PlayerTarget)]
		public GameObject PlayerTargetPrefab { get; set; }

		public override void Execute() {
			var raycastHit = GetRaycastHit();

			// No hit.
			if(raycastHit == null) { return; }

			var newTarget = GameObject.Instantiate(PlayerTargetPrefab, ContextView.transform);
			newTarget.transform.position = raycastHit.Value.point;

			MainEffectsQueue.Enqueue(() => {
				GameObject.Destroy(newTarget, Player.PlayerTargetTTLS);
			}, true);
		}

		private RaycastHit? GetRaycastHit() {
			var camera = Player.MainCamera;
			var cameraRay = camera.ViewportPointToRay(MainCameraRaycastVector);
			RaycastHit hit;

			// Did the goggles hit something?
			if(Physics.Raycast(cameraRay, out hit, RaycastDistance, ~(1 << 2))) {
				// TODO:
			}

			Player.LastRaycastHitPoint = hit.point;
			return hit;
		}
	}
}
