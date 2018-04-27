using DroneDefender.Game.Ai;

namespace DroneDefender.Game.Model {
	public interface IAiTargetsModel {
		AiStandardTarget NextTarget { get; }
		AiStandardTarget AddTarget(AiStandardTarget target);
	}
}