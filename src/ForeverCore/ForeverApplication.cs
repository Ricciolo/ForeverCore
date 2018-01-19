using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace ForeverCore
{
    public class ForeverApplication : CommandLineApplication
    {
        private const string HelpOptionName = "-?|-h|--help";

        private IWebHost _webHost;
        private ProcessManager _manager;

        public ForeverApplication()
        {
            Name = "ForeverCore";
            HelpOption(HelpOptionName);

            Console.WriteLine("###########################################");
            Console.WriteLine("| Ricciolo - ForeverCore                  |");
            Console.WriteLine("| https://github.com/ricciolo/forevercore |");
            Console.WriteLine("###########################################");
            Console.CancelKeyPress += Console_CancelKeyPress;
            
            Command("start", c =>
            {
                c.HelpOption(HelpOptionName);
                c.Description = "Run and monitor the process specified";

                CommandArgument nameArgument = c.Argument("path", "Path to the process to execute");
                CommandOption portOption = c.Option("-p |--port <port>", "The port where listen for external commands. Default is 6321", CommandOptionType.SingleValue);
                CommandOption retriesOption = c.Option("-r |--retries <n>", "How many restart the process. Default is forever", CommandOptionType.SingleValue);

                c.OnExecute(() => Start(nameArgument, portOption, retriesOption));
            });
            OnExecute(() =>
            {
                ShowHelp();
                return 0;
            });
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;

            _manager.Exit();
        }

        private int Start(CommandArgument nameArgument, CommandOption portOption, CommandOption retriesOption)
        {
            if (String.IsNullOrWhiteSpace(nameArgument.Value))
            {
                Console.WriteLine("Please specify the path");
                return 1;
            }

            // Parse options
            int port = portOption.HasValue() ? Convert.ToInt32(portOption.Value()) : 6321;
            int retries = retriesOption.HasValue() ? Convert.ToInt32(retriesOption.Value()) : 0;

            // Prepare proces manager
            string[] data = nameArgument.Value.Split(new[] { ' ' }, 2);
            _manager = new ProcessManager(data[0], data.ElementAtOrDefault(1), retries);

            // Start web server for listening commands
            StartWebServerAsync(port);
            _manager.Print($"Listening commands on http://+:{port}", false);

            // Wait the process exit
            int r = _manager.StartProcessAsync().GetAwaiter().GetResult();

            // Stop web server
            _webHost.StopAsync();

            return r;
        }

        private void StartWebServerAsync(int port)
        {
            _webHost = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .UseUrls($"http://+:{port}")
                .ConfigureServices(s => s.AddSingleton(_manager))
                .Build();
            _webHost.StartAsync().GetAwaiter();
        }       
     
    }
}
