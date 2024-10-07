Introduction
------------
The software is used to create a *.cache file for Warhammer 40,000: Space Marine 2 *.pak files.
It can also create a *.header file for zipped audio files.

This is achived by reading each file's header information in the acrhive file and recreating the .cache/.header file base on thar information.

Requirements
------------
Requires .Net Framework 4.8

Usage
-----
Drag and drop *.pak or *.zip file(s) on the executable for it to create corresponing *.cache or *.header file.
The created file will be in the same directory of the dropped file. 
If a *.cache or *.header file already exists for the file, the existing is backed up before the new one is created. 

Disclaimer
----------
Although this tool does not modify the *.pak or *.header files, and creates backup of the existing *.cache file.
It's till recommened to backup your original files before using the tool.

If the original files are lost or unrecoveralbe in any way, it's advisable to verity integrity of files on Steam or reinstall the game.

This tool does not bypass any anti-cheat mechanism of the game and does not guarantee that the recreated *.cache
will not trigger an AVF error.

Known Issues
------------
Recreating the *.cache file for default_animation.pak file will does work correctlty with the current version. 
It will trigger a crash on start up, I'd advise not to craete one for the default_animation.pak file with this tool with the currect version.
Cache file header structure for default_animation.pak is seems to be different than the other pak files.

There may have some issues if using it on a pak file that are modified in some other way. 
If you're using zip archiving tools like 7zip or WinRar, it should be fine.
