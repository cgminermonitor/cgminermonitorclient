namespace CgminerMonitorClient.Workers
{
    public interface IWorkerDefinition
    {
        void Start(Config config);
    }
}