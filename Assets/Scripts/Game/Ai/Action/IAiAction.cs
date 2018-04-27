using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace DroneDefender.Game.Ai.Action {
	public interface IAiAction {
		void ExecuteOn(Vector3 targetVector);
		Signal<bool> OnCompleteSignal { get; }
		IAiAction Configure<T>() where T : View;
	}
}
