namespace ClientSite.WASM.Shared.Storages.Model
{
    public class StoredValue<T>
    {
        public T? Value { get; set; }
        public DateTime StoredAt { get; set; }
    }
}