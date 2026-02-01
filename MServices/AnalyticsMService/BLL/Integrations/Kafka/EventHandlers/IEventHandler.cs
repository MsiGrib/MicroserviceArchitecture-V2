namespace BLL.Integrations.Kafka.EventHandlers
{
    public interface IEventHandler
    {
        public Task HandleAsync(string message);
    }
}