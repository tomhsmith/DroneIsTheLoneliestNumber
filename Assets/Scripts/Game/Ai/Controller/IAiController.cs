using UnityEngine;

namespace DroneDefender.Game.Ai.Controller {
	public interface IAiController {
		AiStandardTarget SetSingleTarget(
			GameObject go,
			TargetTypes targetType = TargetTypes.MOVEMENT,
			PriorityLevels priority = PriorityLevels.MEDIUM);

		AiStandardTarget AddTarget(
			GameObject go,
			TargetTypes targetType = TargetTypes.MOVEMENT,
			PriorityLevels priority = PriorityLevels.MEDIUM);
		
		AiTargetBase CurrentTarget { get; }

		void Die();
	}
}
