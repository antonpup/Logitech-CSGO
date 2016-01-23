# Logitech - CSGO game integration
This project enables game interaction between Logitech RGB keyboards, such as [Logitech G910](http://gaming.logitech.com/en-us/product/rgb-gaming-keyboard-g910) or [Logitech G410](http://gaming.logitech.com/en-us/product/rgb-tenkeyless-gaming-keyboard-g410), and [Counter-Strike: Global Offensive](http://store.steampowered.com/app/730/) via [Game Integration](https://developer.valvesoftware.com/wiki/Counter-Strike:_Global_Offensive_Game_State_Integration). Please keep in mind that this is still in development, there may be bugs and there will be new features added.

# Requirements
* [Logitech G910](http://gaming.logitech.com/en-us/product/rgb-gaming-keyboard-g910) keyboard or [Logitech G410](http://gaming.logitech.com/en-us/product/rgb-tenkeyless-gaming-keyboard-g410) keyboard
* [Counter-Strike: Global Offensive](http://store.steampowered.com/app/730/)
* Installed [Visual C++ Redistributable Packages for Visual Studio 2013](https://www.microsoft.com/en-us/download/details.aspx?id=40784)
* Installed [Microsoft .NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=30653)

# How to Install
1. First of all, make sure that "Allow games to control illumination" is enabled in Logitech Gameing Software.
2. Download the latest release from [here](https://github.com/antonpup/Logitech-CSGO/releases/latest).
3. Extract the archive anywhere on your computer.
4. Copy the "gamestate_integration_logitech.cfg" into "./steamapps/common/Counter-Strike Global Offensive/csgo/cfg/" folder
5. Run "Logitech-CSGO.exe" (Run as admin if you have any issues.)

## Run this program in the background at windows start
1. Go to the Startup folder. (For Windows 10, press Windows Key + R and enter "shell:startup")
2. Make a shortcut to the exe in that folder.
3. Edit the shortcut by right clicking it, going into properties, and add " -silent" at the end of "Target". It should look something like this: "...\Logitech-CSGO\Logitech-CSGO.exe -silent". Then press apply, and next time your windows will automatically start the program.

## Included effects
* Team-based background color
* Health indicator
* Ammo indicator
* Bomb animation
* Flashbang effect
* Static color keys
* Typing color keys (For lighting up the keyboard when you are typing in chat or console)

## Video demonstration
[![Demo 1](http://img.youtube.com/vi/-PumqhB7COU/0.jpg)](http://www.youtube.com/watch?v=-PumqhB7COU)

# F.A.Q.
* Q: Can this give me a VACation?

   A: No. This uses Valve's [Game Integration](https://developer.valvesoftware.com/wiki/Counter-Strike:_Global_Offensive_Game_State_Integration) for CSGO, which allows developers to read game information without accessing memory of the game.

* Q: How do I add my own custom effects?

   A: At the moment, you cannot. But I do plan on allowing users to customize which keys are being lit per effect.

* Q: Why are Logitech G910 and G410 only supported?

   A: Logitech G910 and G410 are the only keyboards from Logitech that allow for per-key RGB lighting effects.
   
* Q: I have found a bug. How do I report it?

   A: You can report bugs here, by creating a new Issue [here](https://github.com/antonpup/Logitech-CSGO/issues).

* Q: I wish to expand this, fixing and adding my own features.

   A: Feel free to fork this repo and make pull requests with your own code. I am open for suggestions for both features and optimization. :)
