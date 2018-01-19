# ForeverCore
ForeverCore is a small utility written in **.NET Core** for running an external process forever, allowing you to control the lifecycle **via HTTP**.

It works on any platform supported by .NET Core: **Windows**, **Linux**, **macOS**.

## Setup
Here the steps to setup the utility using bash. 
1. Install the .NET Core SDK versione 2 or above, following the instructions you can [find here](https://www.microsoft.com/net/download/windows). 
2. Clone this repository
   ```bash
   mkdir ForeverCore
   git clone https://github.com/Ricciolo/ForeverCore.git
   ```
3. Compile the code
   ```bash
   cd ForeverCore/src
   dotnet publish -c release -o output
   ```
4. Run the utility
   ```bash
   cd output
   dotnet ForeverCore.dll
   ```
## Usage
The utility use .NET Core CLI usage style.
```
Usage: ForeverCore [options] [command]

Options:
  -?|-h|--help  Show help information

Commands:
  start  Run and monitor the process specified

Use "ForeverCore [command] --help" for more information about a command.
```
The only available command is **start**
```bash
dotnet ForeverCore.dll "path to process to run"
```
Options are:
* **-r**: number of retries. If the process crash, the utility restarts it automatically. Default value is infinite (zero)
* **-p**: port number where listen for HTTP commands. Default value is 6321
## HTTP commands
Assuming your are on the same machine where utility is running, call *http://localhost:6321/[command]* via GET (ex. using the browser). Commands are:
* **stop**: stop the process keeping the utility up and waiting for a new command;
* **start**: start the process again if you previously stopped it using the stop command;
* **restart**: force the process to stop and start again;
* **exit**: force the process to stop and the utility to quit.

The following is a tipical utility output:
```
###########################################

 Ricciolo - ForeverCore
 https://github.com/ricciolo/forevercore

###########################################
Listening commands on http://+:6321
Process started!
...Demo process output...
Terminated!
Process started!
...Demo process output...
Restart requested
Process started!
...Demo process output...
```
