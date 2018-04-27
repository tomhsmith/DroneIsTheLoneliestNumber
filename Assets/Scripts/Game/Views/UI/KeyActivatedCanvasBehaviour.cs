using DroneDefender.Common;
using DroneDefender.Common.Extension;
using System;
using System.Linq;
using UnityEngine;

namespace DroneDefender.Game.Views.UI {
	public class KeyActivatedCanvasBehaviour : MonoBehaviour {
		// Settings
		public KeyCode ActivatorKeyCode = KeyCode.Escape;

		// Local Components
		private Canvas _canvas;
		private NullableResultDictionary<GameObject> _children;
		private bool _keyEntered;

		private void Awake() {
			_canvas = gameObject.GetComponent<Canvas>();
			_children = transform.ChildrenByTagDictionary();

			MakeVisible(false);
		}

		private void Update() {
			KeyDownHandler();
		}

		protected virtual void KeyDownHandler(Action keyEnteredAction = null) {
			// Only registering down.
			if(!Input.anyKeyDown) { return; }

			var keyDown = Input.GetKeyDown(ActivatorKeyCode);

			// Not our key, reset and abort.
			if(!keyDown) {
				if (_keyEntered) {
					MakeVisible(_keyEntered = false);
				}
				return;
			}
			
			// We have key, quit or show.
			if (_keyEntered) {
				keyEnteredAction?.Invoke();
				MakeVisible(_keyEntered = false);
			} else {
				MakeVisible(_keyEntered = true);
			}
		}

		private void MakeVisible(bool to) {
			_children?.SelectMany(x => x.Value).ToList()
				.ForEach(x => x.gameObject.SetActive(to));
		}
	}
}
