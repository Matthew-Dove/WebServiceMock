To run these commands you must launch PowerShell, or the Command Prompt as an administrator.
Then navigate to this directory e.g. cd "directory path goes here".

If you haven't installed the Windows Service yet, run the installService.CMD file in the PowerShell, or Command Prompt window.
If you're not sure WebMockService is installed (or if it's just stopped), run the viewWindowsServices.CMD file, find the Service, and check it's running status.
If you can't find the Service in the list, then it's most likey not installed.

To start the Service run the startService.CMD file; this Service will run until you stop it, or the machine restarts (since the default start-type is "manual").
To stop the Service from running, run the stopService.CMD file.

To uninstall the Windows Service, run the uninstallService.CMD file.

You can edit the Service's properties (such as when to startup, what user to run as, etc) by running the file viewWindowsServices.CMD, finding the Service, and hitting Right-Click => Properties.

If there are any errors reported when running the Service, they will be logged in two places.
The first place is a text file called "LogFile.txt". This file will be written to the base directory of the application (i.e. one directory above this one).
The second place is the Event Viewer. The logs can be found in Event Viewer => Windows Logs => Application, and the source will be "WebMockService".