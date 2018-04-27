using strange.extensions.signal.impl;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DroneDefender.Common {
	public class NullableResultDictionary<TListType> : Dictionary<string, List<TListType>>
		where TListType : UnityEngine.Object {

		public Signal ItemAddedSignal = new Signal();

		public List<TListType> GetNullable(string key) {
			List<TListType> val;
			TryGetValue(key, out val);
			return val;
		}

		public TListType GetNullableFirst(string key) {
			return GetNullable(key)?.FirstOrDefault();
		}

		public NullableResultDictionary<TListType> Add(TListType item) {
			var go = item as GameObject;
			if(go != null) {
				var key = go.tag;
				var goList = GetNullable(key);
				if (goList == null) {
					base.Add(key, new List<TListType> { go as TListType });
				} else {
					goList.Add(go as TListType);
				}
			}

			ItemAddedSignal.Dispatch();
			return this;
		}
	}
}
