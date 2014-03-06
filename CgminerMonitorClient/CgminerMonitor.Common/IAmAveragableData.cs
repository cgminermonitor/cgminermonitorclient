namespace CgminerMonitorClient.CgminerMonitor.Common
{
    public interface IAmAveragableData
    {
        void SumWithTheOther(IAmAveragableData data);
        void DivideByInt(int i);
        IAmAveragableData Clone();
    }
}