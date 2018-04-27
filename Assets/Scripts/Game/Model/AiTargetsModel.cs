using DroneDefender.Game.Ai;
using DroneDefender.Game.Signals.Ai;
using System.Collections.Generic;

namespace DroneDefender.Game.Model {
	internal class AiTargetsModel : IAiTargetsModel {
		[Inject] public AiTargetsModelChangedSignal TargetsChangedSignal { get; set; }

		private List<AiStandardTarget> _targets;

		public AiStandardTarget NextTarget {
			get {
				if (_targets.Count < 1) {
					return null;
				}
				return _targets[0];
			}
		}

		public AiTargetsModel() {
			_targets = new List<AiStandardTarget>();
		}

		public AiStandardTarget AddTarget(AiStandardTarget target) {
			_targets.Add(target);

			TargetsChangedSignal.Dispatch();
			return target;
		}
	}
}