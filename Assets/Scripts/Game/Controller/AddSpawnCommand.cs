using DroneDefender.Common;
using DroneDefender.Game.Ai.Controller;
using DroneDefender.Game.Model;
using DroneDefender.Game.Signals.Ai;
using DroneDefender.Game.Views;
using DroneDefender.Game.Views.Vehicle;
using DroneDefender.Resource.Tag;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DroneDefender.Game.Controller {
	public class AddSpawnCommand : Command {
		public const string ContainerName = "AI Top";

		// Cross Context
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject ContextView { get; set; }

		[Inject(InjectableTypes.TeamContainer)]
		public NullableResultDictionary<GameObject> TeamContainer { get; set; }

		[Inject] public IPlayer Player { get; set; }
		[Inject] public SpawnTypes SpawnType { get; set; }
		[Inject] public Vector3 Location { get; set; }
		[Inject] public List<GameObject> Waypoints { get; set; }

		// Local Components
		private List<GameObject> _targets;

		public override void Execute() {
			var go = new GameObject(ContainerName);
			go.transform.parent = ContextView.transform;
			go.transform.position = Location;

			_targets = TeamContainer.GetNullable(TagResources.Team00)
				?? new List<GameObject>();

			if (SpawnType == SpawnTypes.TANK) {
				AddTank(go);
			} else if( SpawnType == SpawnTypes.HELICOPTER) {
				AddHelicopter();
			}
		}        

		private void AddTank(GameObject go) {
			var aiController = go.AddComponent<AiBasicControllerView>();
			aiController.ConfigureView<TankView>().ConfigureLaunchable<TankShellView>();

			foreach (var waypoint in Waypoints) {
				aiController.AddTarget(waypoint);
			}
			Player.Spawns.Add(aiController);

			// TODO: Add better ai then random targeting.
			aiController.AddTarget(GetRandomTarget(), Ai.TargetTypes.ENEMY);
		}

		private void AddHelicopter() {
			var heliGo = new GameObject("Helicopter Top");
			heliGo.transform.parent = ContextView.transform;
			heliGo.transform.position = Location + (Vector3.up * 400);

			var helicopterView = heliGo.AddComponent<AiHelicopterControllerView>()
				.Configure<HelicopterView>();

			Player.Spawns.Add(helicopterView);

			helicopterView.AddTarget(GetRandomTarget(), Ai.TargetTypes.ENEMY);
		}

		private GameObject GetRandomTarget() {
			if (!_targets.Any()) { return null; }

			return _targets[Random.Range(0, _targets.Count - 1)];
		}
	}
}