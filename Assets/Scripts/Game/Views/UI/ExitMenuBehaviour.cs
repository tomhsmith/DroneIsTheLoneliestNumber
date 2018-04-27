using System;
using UnityEngine;

namespace DroneDefender.Game.Views.UI {
	public class ExitMenuBehaviour : KeyActivatedCanvasBehaviour {
		private void Update() {
			KeyDownHandler(ExitApplication);
		}

		protected override void KeyDownHandler(Action keyEnteredAction) {
			base.KeyDownHandler(ExitApplication);
		}

		private void ExitApplication() {
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
			Debug.LogWarning("APPLICATION.QUIT()");
		}
	}
}
