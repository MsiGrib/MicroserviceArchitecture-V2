namespace ClientSite.WASM.Shared.Storages.Model
{
    public class StoredValue<T>
    {
        public T? Value { get; init; }
        public DateTime StoredAt { get; init; }
    }
}