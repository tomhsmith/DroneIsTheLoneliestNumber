using Assets.Scripts.Game.Controller;
using DroneDefender.Common;
using DroneDefender.Game.Ai.Action;
using DroneDefender.Game.Ai.Controller;
using DroneDefender.Game.Constants.Prefab;
using DroneDefender.Game.Controller;
using DroneDefender.Game.Damage;
using DroneDefender.Game.Inputs;
using DroneDefender.Game.Model;
using DroneDefender.Game.Signals;
using DroneDefender.Game.Signals.Ai;
using DroneDefender.Game.Signals.Explosion;
using DroneDefender.Game.Views;
using DroneDefender.Game.Views.Drone;
using DroneDefender.Game.Views.Explosion;
using DroneDefender.Game.Views.Projectile;
using DroneDefender.Game.Views.UI;
using DroneDefender.Game.Views.Vehicle;
using DroneDefender.Resource.Prefab;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using System.Collections.Generic;
using UnityEngine;

namespace DroneDefender.Game {
	public class MainGameContext : MVCSContext {
		public MainGameContext(MonoBehaviour view) : base(view) { }
		public MainGameContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags) {	}

		// Unbind the default EventCommandBinder and rebind the SignalCommandBinder
		protected override void addCoreComponents() {
			base.addCoreComponents();
			injectionBinder.Unbind<ICommandBinder>();
			injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
		}

		// Override Start so that we can fire the StartSignal
		override public IContext Start() {
			base.Start();
			StartSignal startSignal = injectionBinder.GetInstance<StartSignal>();
			startSignal.Dispatch();
			return this;
		}

		protected override void mapBindings() {
			ConfigureSignals();
			ConfigureViews();
			ConfigureModels();
			ConfigurePrefabs();
			ConfigureAudioClips();
		}

		#region Configurers

		private void ConfigureSignals() {
			commandBinder.Bind<StartSignal>().To<StartCommand>().Once();

			//	Ai
			injectionBinder.Bind<AiTargetsModelChangedSignal>().ToSingleton();
			//	Commands
			commandBinder.Bind<AddExplosionSignal>().To<AddExplosionCommand>();
			commandBinder.Bind<AddSpawnSignal>().To<AddSpawnCommand>();
			commandBinder.Bind<DamageSignal>().To<DamageCommand>();
			commandBinder.Bind<AddPlayerTargetSignal>().To<AddPlayerTargetCommand>();
		}

		private void ConfigureViews() {
			// AI
			//	Player
			mediationBinder.Bind<AiPlayerView>().To<AiPlayerMediator>();
			mediationBinder.Bind<TeamAssignerView>().To<TeamAssignerMediator>();
			//	Controller
			mediationBinder.Bind<AiBasicControllerView>().To<AiBasicControllerMediator>();
			mediationBinder.Bind<AiHelicopterControllerView>().To<AiHelicopterControllerMediator>();
			//	Fire
			mediationBinder.Bind<AiRotationalFireActionView>().To<AiRotationalFireActionMediator>();
			//	Movement
			mediationBinder.Bind<AiRotationalMovementActionView>()
				.To<AiRotationalMovementActionMediator>();

			mediationBinder.Bind<AiAirRotationalMovementActionView>()
				.To<AiAirRotationalMovementActionMediator>();

			//	Drone
			mediationBinder.Bind<DroneAirBasicView>().To<DroneAirBasicMediator>();
			mediationBinder.Bind<DroneMothershipView>().To<DroneMothershipMediator>();

			//	Input
			mediationBinder.Bind<VRCameraRigView>().To<VRCameraRigMediator>();
			mediationBinder.Bind<FPSCameraRigView>().To<FPSCameraRigMediator>();
			mediationBinder.Bind<VRInputLeftView>().To<VRInputLeftMediator>();
			mediationBinder.Bind<VRInputRightView>().To<VRInputRightMediator>();

			//	Projectile
			mediationBinder.Bind<MissileView>().To<MissileMediator>();
			mediationBinder.Bind<GunView>().To<GunMediator>();
			mediationBinder.Bind<BulletView>().To<BulletMediator>();

			//	Explosion
			mediationBinder.Bind<SimpleExplosionView>().To<SimpleExplosionMediator>();
			mediationBinder.Bind<LaserView>().To<LaserMediator>();

			//	UI
			mediationBinder.Bind<IntroCanvasView>().To<IntroCanvasMediator>();

			//	Vehicle
			mediationBinder.Bind<TankView>().To<TankMediator>();
			mediationBinder.Bind<TankShellView>().To<TankShellMediator>();
			mediationBinder.Bind<HelicopterView>().To<HelicopterMediator>();
		}

		private void ConfigureModels() {
			//	Model Classes
			injectionBinder.Bind<ILocalPlayerModel>().To<LocalPlayerModel>()
				.ToName(PlayerContexts.LocalPlayer)
				.ToSingleton();

			injectionBinder.Bind<IAiTargetsModel>()
				.To<AiTargetsModel>()
				.ToSingleton();

			injectionBinder.Bind<DroneMothershipModel>()
				.To(new DroneMothershipModel())
				.ToSingleton();

			// Queue
			injectionBinder.Bind<EffectsQueue>()
				.To(new EffectsQueue())
				.ToSingleton();
			//	Containers
			injectionBinder.Bind<NullableResultDictionary<GameObject>>()
				.To(new NullableResultDictionary<GameObject>())
				.ToName(InjectableTypes.TeamContainer)
				.ToSingleton();

			injectionBinder.Bind<Queue<ExplosionQueueItem>>()
				.To(new Queue<ExplosionQueueItem>())
				.ToSingleton();
			// Prefabs
			injectionBinder.Bind<GameObject>()
				.To(Resources.Load<GameObject>(PrefabResources.PlayerTarget))
				.ToName(InjectableTypes.PlayerTarget)
				.ToSingleton();
		}

		private void ConfigurePrefabs() {
			injectionBinder.Bind<GameObject>()
				.To(Resources.Load<GameObject>(PrefabResources.ExplosionSimpleSphere))
				.ToName(InjectableTypes.Explosion)
				.ToSingleton();
		}

		private void ConfigureAudioClips() {
			injectionBinder.Bind<AudioClip>()
				.To(Resources.Load<AudioClip>(AudioPathResources.ExplosionRobotic))
				.ToName(AudioContexts.ExplosionRobotic)
				.ToSingleton();

			injectionBinder.Bind<AudioClip>()
				.To(Resources.Load<AudioClip>(AudioPathResources.IntroMustProtect))
				.ToName(AudioContexts.IntroMustProtect)
				.ToSingleton();

			injectionBinder.Bind<AudioClip>()
				.To(Resources.Load<AudioClip>(AudioPathResources.LaserBeam))
				.ToName(AudioContexts.LaserBeam)
				.ToSingleton();
		}

		#endregion
	}
}

