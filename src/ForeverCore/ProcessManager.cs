using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ForeverCore
{
    public class ProcessManager
    {
        private Process _currentProcess;
        private bool _cancelled;
        private bool _stopped;
        private int _retriesCount;

        private readonly ProcessStartInfo _info;
        private readonly int _maxRetries;

        private TaskCompletionSource<int> _exitEvent;

        public ProcessManager(string path, string arguments, int maxRetries)
        {
            _maxRetries = maxRetries;
            _info = !String.IsNullOrWhiteSpace(arguments) ? new ProcessStartInfo(path, arguments) : new ProcessStartInfo(path);
            _info.RedirectStandardError = true;
            _info.RedirectStandardInput = true;
            _info.RedirectStandardOutput = true;
            _info.CreateNoWindow = true;
        }

        public Task<int> StartProcessAsync()
        {
            _exitEvent = new TaskCompletionSource<int>();

            StartProcessAsyncInternal();
            return _exitEvent.Task;
        }

        private void StartProcessAsyncInternal()
        {
            Task.Run(() => StartProcess());
        }

        private void StartProcess()
        {
            // Close/dispose the process
            CloseProcess();
            // Reset status
            _stopped = false;
            _cancelled = false;

            try
            {
                // Start the process
                _currentProcess = Process.Start(_info);
            }
            catch (FileNotFoundException)
            {
                Print($"Cannot find path {_info.FileName}", true);
                _exitEvent.TrySetResult(1);
                return;
            }

            Print($"Process started!{(_retriesCount > 0 ? $" Retry #{_retriesCount}" : "")}", _retriesCount > 0);

            // Show output from the process
            ReadOutputAsync(_currentProcess).GetAwaiter();

            _currentProcess.WaitForExit();

            if (_stopped)
            {
                return;
            }
            if (!_cancelled)
            {
                // Restart the process
                if (_retriesCount < _maxRetries || _maxRetries == 0)
                {
                    _retriesCount++;
                    StartProcessAsyncInternal();
                    return;
                }

                Print("Too many retries", true);
            }

            _exitEvent.TrySetResult(0);
        }

        public void Print(string message, bool error)
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = (error) ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = old;
        }

        private void CloseProcess()
        {
            if (_currentProcess == null) return;

            if (!_currentProcess.HasExited)
            {
                _currentProcess.Kill();
            }
            _currentProcess.Dispose();
            _currentProcess = null;
        }

        private async Task ReadOutputAsync(Process currentProcess)
        {
            while (!currentProcess.HasExited)
            {
                try
                {
                    string line = await currentProcess.StandardOutput.ReadLineAsync();
                    Console.WriteLine(line);
                }
                catch (Exception ex)
                {

                }
            }
        }

        public void Stop()
        {
            _stopped = true;
            CloseProcess();

            Print("Terminated!", false);
        }

        public void Start()
        {
            if (_currentProcess == null || _currentProcess.HasExited)
            {
                StartProcessAsyncInternal();
            }
        }

        public void Exit()
        {
            _cancelled = true;
            CloseProcess();

            Print("Exit!", false);
        }

        public void Restart()
        {
            Print("Restart requested", false);

            if (_stopped)
            {
                Start();
            }
            else
            {
                _retriesCount = -1;
                CloseProcess();
            }
        }
    }
}
