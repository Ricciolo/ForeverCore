# ForeverCore
ForeverCore is a small utility written in **.NET Core** for running an external process forever, allowing you to control the lifecycle **via HTTP**.

It works on any platform supported by .NET Core: **Windows**, **Linux**, **macOS**.

## Setup
Here the steps to setup the utility using bash on Linux. On Windows is pretty similar.
1. Download the release for your OS. You can find the release [here](https://github.com/Ricciolo/ForeverCore/releases/). The app is self contained and you don't need any other dependency
   ```bash
   wget https://github.com/Ricciolo/ForeverCore/releases/download/v1.0-alpha/ForeverCore-v1.0-linux-arm.zip
   ```
2. Unzip the content into a folder
   ```bash
   unzip ForeverCore-v1.0-linux-arm.zip -d ForeverCore
   ```
3. Run the utility
   ```bash
   chmod +x ForeverCore
   ./ForeverCore
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
./ForeverCore start "path to process to run and arguments"
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
