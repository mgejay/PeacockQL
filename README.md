# PEACOCKQL - HITMAN: World of Assassination Local Server Launcher

- AUTHOR: MGE
- VERSION: 1.0.0

------------------------------------------------------------
WHAT IS THIS?
------------------------------------------------------------
This is a helper launcher for Peacock, the offline/local server 
platform for HITMAN: World of Assassination. It simplifies the 
launch sequence by automatically:

1. Starting the Peacock patcher
2. Launching the Peacock local server in its own terminal window
3. Launching HITMAN: WoA from Epic Games
4. Closing the initial launcher window automatically

------------------------------------------------------------
IMPORTANT CONFIGURATION NOTE
------------------------------------------------------------
# PLEASE KEEP IN MIND:
- All directory paths in `options.json` MUST use DOUBLE BACKSLASHES.
  \Example: "C:\\\\FILES\\\\PEACOCK"

This is required so JSON string escaping behaves properly.

------------------------------------------------------------
FIRST-TIME SETUP INSTRUCTIONS
------------------------------------------------------------

1. Extract the contents of the launcher ZIP.
2. Place the folder **anywhere you want** (e.g., Desktop, Games drive).
3. On **first run**, the app will:
   - Create a `config/` folder
   - Generate `options.json` with placeholder values
   - Generate this README
   - Exit after 15 seconds

4. Open `config/options.json` in a text editor (like Notepad).
5. Fill in the correct values:

    - "PeacockFolder": The full path to your Peacock install folder
      → Example: "C:\\\\FILES\\\\PEACOCK"

    - "PatcherExe": The exact filename of the Peacock patcher
      → Example: "PeacockPatcher.exe"

    - "ServerCmd": The name of the server launcher CMD file
      → Example: "Start Server.cmd"

    - "ServerWindowTitle": What the server terminal window should be titled
      → Example: "Hitman Local Server"

    - "EpicUri": The Epic Games URI for launching HITMAN WoA
      → Default should work, but make sure Epic is installed as well as Hitman: WoA

    - "CloseDelaySeconds": How long (in seconds) to wait before closing the initial terminal window

6. Save the file.
7. Run the launcher again — it should now:
   - Patch the game
   - Start the local server
   - Launch HITMAN automatically

------------------------------------------------------------
TIPS / TROUBLESHOOTING
------------------------------------------------------------

• Make sure Peacock and HITMAN are fully set up before using this launcher.
• The Epic launch shortcut must be correct. You can find app launch shortcuts using tools like:
  - `shell:AppsFolder` in Run
  - Epic Games logs or shortcut properties
• Always keep `options.json` formatted correctly. One missing comma can cause errors.
• Don’t rename `config/` folder or move individual files out of their intended locations.

------------------------------------------------------------
KNOWN LIMITATIONS
------------------------------------------------------------

- Does not check for Peacock updates, must be handled manually.
- No error checking if the patcher or server fails, this .exe is assuming you're fully setup already.
- Terminal windows might flash briefly depending on system settings (this shouldn't really be an issue)

------------------------------------------------------------
LICENSE
------------------------------------------------------------

This launcher is licensed under GNUPL, and provided as-is for personal use with Peacock.
It is not affiliated with Peacock, IO Interactive or Epic Games.
Use at your own risk, source code is public..
