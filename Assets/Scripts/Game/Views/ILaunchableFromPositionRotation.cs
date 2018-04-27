using UnityEngine;

namespace DroneDefender.Game.Views {
	public interface ILaunchableFromPositionRotation {
		GameObject Launch(Vector3 fromPosition, Quaternion startingRotation);
	}
}
