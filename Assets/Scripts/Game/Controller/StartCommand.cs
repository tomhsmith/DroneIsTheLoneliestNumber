using DroneDefender.Game.Inputs;
using DroneDefender.Game.Model;
using DroneDefender.Game.Views.Drone;
using DroneDefender.Game.Views.UI;
using DroneDefender.Resource.Tag;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using UnityEngine;

namespace DroneDefender.Game.Controller {
	public class StartCommand : Command {
		public const string VRCameraRigName = "VR Camera Rig";
		public const string FPSCameraRigName = "FPS Camera Rig";
		public const string MothershipTopName = "Mothership Top";
		public const string AITopName = "AI Top";

		// Cross Context
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject ContextView { get; set; }

		[Inject] public SettingsModel Settings { get; set; }

		public override void Execute() {
			GameObject currentGO = null;

			// Create Camera
			switch(Settings.GameMode) {
				case GameModes.VR:
					currentGO = new GameObject(VRCameraRigName);
					currentGO.AddComponent<VRCameraRigView>();
					break;
				case GameModes.FPS:
					currentGO = new GameObject(FPSCameraRigName);
					currentGO.AddComponent<FPSCameraRigView>();
					break;
				default:
					Debug.LogError($"Unrecognized game mode: {Settings.GameMode}.");
					break;
			}

			currentGO.transform.parent = ContextView.transform;

			// Intro
			var introCanvas = GameObject.FindGameObjectWithTag(TagResources.IntroCanvas);
			introCanvas.AddComponent<IntroCanvasView>().Play();

			// Mother Ship
			var go = new GameObject(MothershipTopName);
			go.AddComponent<DroneMothershipView>();
			go.transform.parent = ContextView.transform;

			// Enemy AI
			var aiGO = new GameObject(AITopName);
			aiGO.transform.parent = ContextView.transform;
			aiGO.AddComponent<AiPlayerView>();
		}
	}
}
