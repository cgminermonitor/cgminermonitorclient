namespace CgminerMonitorClient.Workers
{
    public interface IWorkerDefinition
    {
        void Start(object config);
    }
}