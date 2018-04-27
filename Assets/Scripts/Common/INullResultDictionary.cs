namespace DroneDefender.Common {
	public interface INullResultDictionarywhere<TKey, TValue> 
		where TKey : class {
		TValue this[TKey key] { get; }
	}
}