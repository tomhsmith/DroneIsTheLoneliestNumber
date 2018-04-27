using UnityEngine;

namespace DroneDefender.Game.Ai {
	public class AiStandardTarget : AiTargetBase {

		public AiStandardTarget(GameObject go, TargetTypes targetType) 
			: base(go, targetType) {
		}

		public AiStandardTarget(GameObject go, TargetTypes targetType, PriorityLevels priority) 
			: base(go, targetType, priority) {
		}
	}
}
