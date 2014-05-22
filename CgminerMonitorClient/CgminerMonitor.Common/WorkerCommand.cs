using CgminerMonitorClient.Utils;
using Newtonsoft.Json;

namespace CgminerMonitorClient.CgminerMonitor.Common
{
    public class WorkerCommand
    {
        [JsonProperty("i")]
        public string Id { get; set; }

        [JsonProperty("n")]
        public string HandlerKey { get; set; }

        [JsonProperty("v")]
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", HandlerKey, Value);
        }
    }

    public class WorkerCommandResponse
    {
        protected WorkerCommandResponse()
        {
        }

        [JsonProperty("i")]
        public string CommandId { get; set; }

        [JsonProperty("s")]
        public WorkerCommandResponseStatus Status { get; set; }

        [JsonProperty("m")]
        public string Message { get; set; }

        public static WorkerCommandResponse Success(string commandId, string message)
        {
            return new WorkerCommandResponse
            {
                CommandId = commandId,
                Status = WorkerCommandResponseStatus.Success,
                Message = message
            };
        }

        public static WorkerCommandResponse Warning(string commandId, string message)
        {
            return new WorkerCommandResponse
            {
                CommandId = commandId,
                Status = WorkerCommandResponseStatus.Warning,
                Message = message
            };
        }

        public static WorkerCommandResponse Failure(string commandId, string message)
        {
            return new WorkerCommandResponse
            {
                CommandId = commandId,
                Status = WorkerCommandResponseStatus.Failure,
                Message = message
            };
        }

        public static WorkerCommandResponse NotAllowed(string commandId)
        {
            return new WorkerCommandResponse
            {
                CommandId = commandId,
                Status = WorkerCommandResponseStatus.NotAllowedByWorker,
            };
        }

        public static WorkerCommandResponse Unknown(string commandId)
        {
            return new WorkerCommandResponse
            {
                CommandId = commandId,
                Status = WorkerCommandResponseStatus.UnknownCommand,
            };
        }

        public static WorkerCommandResponse FromProcessExecutionResult(string commandId, ProcessExecutorResult result)
        {
            if (result.Success)
                return Success(commandId, result.Output);
            return Failure(commandId, result.Output);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Status, Message);
        }
    }

    public enum WorkerCommandResponseStatus
    {
        Success = 1,
        Warning = 2,
        Failure = 3,
        NotAllowedByWorker = 4,
        UnknownCommand = 5,
    }
}