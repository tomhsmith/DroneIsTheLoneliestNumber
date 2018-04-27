using DroneDefender.Game.Controller;
using DroneDefender.Game.Model;
using DroneDefender.Game.Signals;
using DroneDefender.Game.Signals.Ai;
using DroneDefender.Game.Signals.Explosion;
using DroneDefender.Game.Views;
using DroneDefender.Game.Views.Explosion;
using DroneDefender.Game.Views.UI;
using DroneDefender.Resource.Prefab;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

namespace DroneDefender.Game {
	public class SelectionScreenContext : MVCSContext {
		public SelectionScreenContext(MonoBehaviour view) : base(view) { }
		public SelectionScreenContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags) { }

		// Unbind the default EventCommandBinder and rebind the SignalCommandBinder
		protected override void addCoreComponents() {
			base.addCoreComponents();
			injectionBinder.Unbind<ICommandBinder>();
			injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
		}


		protected override void mapBindings() {
			mediationBinder.Bind<SelectionButtonView>().To<SelectionButtonMediator>();

			injectionBinder.Bind<SettingsModel>()
				.To(new SettingsModel())
				.ToSingleton()
				.CrossContext();
		}
	}
}
