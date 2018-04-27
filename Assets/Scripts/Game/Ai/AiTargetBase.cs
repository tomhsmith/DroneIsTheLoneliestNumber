using UnityEngine;

namespace DroneDefender.Game.Ai {
	public abstract class AiTargetBase {
		public GameObject GameObject;
		public TargetTypes TargetType;
		public PriorityLevels TargetPriority;

		public AiTargetBase() : this(new GameObject(), TargetTypes.UKNOWN) { /* Defaults for convenience. */ }

		public AiTargetBase(GameObject go, TargetTypes targetType)
			: this(go, targetType, PriorityLevels.MEDIUM) { /* Defaults for convenience. */ }

		public AiTargetBase(GameObject go, TargetTypes targetType, PriorityLevels priority) {
			GameObject = go;
			TargetType = targetType;
			TargetPriority = priority;
		}
	}
}