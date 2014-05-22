using System.Diagnostics;

namespace CgminerMonitorClient.Utils
{
    public static class SimpleProcessExecutor
    {
        public static string Fire(string commandLine)
        {
            string processName;
            string arguments;
            CommandLineParser.SplitCommandLine(commandLine, out processName, out arguments);
            return Fire(processName, arguments);
        }

        public static string Fire(string processName, string arguments)
        {
            var ps = new ProcessStartInfo(processName, arguments)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };

            Log.Instance.DebugFormat("Starting '{0}' with parameters '{1}'", processName, arguments);

            using (var p = Process.Start(ps))
            {
                if (p == null)
                    return string.Format("Could not start {0} process.", processName);
                p.StandardInput.Flush();
                p.StandardInput.Close();
                var output = p.StandardOutput.ReadToEnd();

                // waits for the process to exit
                // Must come *after* StandardOutput is "empty"
                // so that we don't deadlock because the intermediate
                // kernel pipe is full.
                p.WaitForExit();
                if (p.ExitCode > 0)
                    return string.Format("Invoked command returned error. Process '{0}' exited with '{1}', returning: '{2}'.",
                        processName, p.ExitCode, output);

                var result = Consts.SuccessCommandPrefix + output;
                Log.Instance.InfoFormat(result);
                return result;
            }
        }
    }
}