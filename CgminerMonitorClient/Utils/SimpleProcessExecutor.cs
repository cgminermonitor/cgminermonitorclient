using System;
using System.Diagnostics;

namespace CgminerMonitorClient.Utils
{
    public static class SimpleProcessExecutor
    {
        public static ProcessExecutorResult Fire(string commandLine, bool runAndForget)
        {
            string processName;
            string arguments;
            CommandLineParser.SplitCommandLine(commandLine, out processName, out arguments);
            return Fire(processName, arguments, runAndForget);
        }

        public static ProcessExecutorResult Fire(string processName, string arguments, bool runAndForget)
        {
            ProcessStartInfo ps;
            if (runAndForget)
                ps = new ProcessStartInfo(processName, arguments);
            else
                ps = new ProcessStartInfo(processName, arguments)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true
                };

            Log.Instance.DebugFormat("Starting '{0}' with parameters '{1}'", processName, arguments);

            try
            {
                var p = Process.Start(ps);
                try
                {
                    if (p == null)
                        return ProcessExecutorResult.Failed(string.Format("Could not start {0} process.", processName));

                    string output = null;
                    if (!runAndForget)
                    {
                        p.StandardInput.Flush();
                        p.StandardInput.Close();
                        output = p.StandardOutput.ReadToEnd();

                        // waits for the process to exit
                        // Must come *after* StandardOutput is "empty"
                        // so that we don't deadlock because the intermediate
                        // kernel pipe is full.
                        p.WaitForExit();
                        if (p.ExitCode > 0)
                        {
                            Log.Instance.Debug("EC>0");
                            return ProcessExecutorResult.Failed(string.Format("Invoked command returned error. Process '{0}' exited with '{1}', returning: '{2}'.",
                                        processName, p.ExitCode, output));
                        }
                    }

                    var result = Consts.SuccessCommandPrefix + output;
                    Log.Instance.InfoFormat(result);
                    return ProcessExecutorResult.Succeeded(output);
                }
                finally
                {
                    if (p != null && !runAndForget)
                        ((IDisposable)p).Dispose();
                }
            }
            catch (Exception e)
            {
                return ProcessExecutorResult.Failed(e.ToString());
            }
        }
    }

    public class ProcessExecutorResult
    {
        private ProcessExecutorResult()
        {
        }

        public string Output { get; private set; }
        public bool Success { get; private set; }

        public static ProcessExecutorResult Succeeded(string message)
        {
            return new ProcessExecutorResult
            {
                Success = true,
                Output = message
            };
        }

        public static ProcessExecutorResult Failed(string message)
        {
            return new ProcessExecutorResult
            {
                Success = false,
                Output = message
            };
        }
    }
}