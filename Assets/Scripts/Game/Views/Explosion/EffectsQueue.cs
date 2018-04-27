using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DroneDefender.Game.Views.Explosion {
	public class EffectsQueue {
		public int ChunkSize = 50;
		public int ChannelSize = 200;
		public int ChannelCount { get { return _channels.Sum(x => x.Count); } }

		private readonly LinkedList<Action>[] _channels;
		private readonly LinkedList<int> _channelInsertNumbers;

		public EffectsQueue() {
			_channels = new LinkedList<Action>[ChannelSize];

			Parallel.For(0, ChannelSize, i => {
				_channels[i] = new LinkedList<Action>();
			});

			_channelInsertNumbers = new LinkedList<int>();
		}

		public void ExecuteNextEffectChunk() {
			if(_channelInsertNumbers == null || !_channelInsertNumbers.Any()) {
				return;
			}

			var chunk = _channelInsertNumbers.Take(ChunkSize).ToArray();

			foreach(var i in chunk) {
				var channel = _channels[i];
				_channelInsertNumbers.RemoveFirst();

				var firstEffect = channel.First();
				channel.RemoveFirst();

				firstEffect?.Invoke();
			}
		}
		
		public void Enqueue(Action newEffect, bool priority = false) {
			var channel = UnityEngine.Random.Range(0, ChannelSize);
			var currentChannel = _channels[channel];

			// This effect prioritiezed in it's channel.
			if (priority) {
				currentChannel.AddFirst(newEffect);
			} else {
				currentChannel.AddLast(newEffect);
			}

			// It will consume the next spot in it's channel.
			// so this is added to eventually queue less priority item.
			// TODO: Problem.. it could bump another prority item..
			_channelInsertNumbers.AddLast(channel);
		}
	}
}
