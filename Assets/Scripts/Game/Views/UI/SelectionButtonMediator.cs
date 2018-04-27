using DroneDefender.Common.Extension;
using DroneDefender.Game.Model;
using DroneDefender.Game.Signals;
using strange.extensions.context.api;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DroneDefender.Game.Views.UI {
	public class SelectionButtonMediator : Mediator {
		// Cross Context
		[Inject] public SelectionButtonView View { get; set; }

		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject ContextView { get; set; }

		[Inject] public SettingsModel Settings { get; set; }

		public override void OnRegister() {
			base.OnRegister();

			View.Init();
			gameObject.GetComponent<Button>().onClick.AddListener(OnClick);
		}

		private void OnClick() {
			enabled = false;

			switch(gameObject.tag) {
				case "VR":
					Settings.GameMode = GameModes.VR;
					break;
				case "FPS":
					Settings.GameMode = GameModes.FPS;
					break;
			}

			ContextView.transform.DestroyChildren();

			// Load level
			SceneManager.LoadScene(1, LoadSceneMode.Additive);
			Debug.Log($"UI: Clicked on {gameObject.name}.");
		}
	}
}
