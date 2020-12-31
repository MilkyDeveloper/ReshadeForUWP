<img src="https://raw.githubusercontent.com/MilkyDeveloper/dump/%F0%9F%96%BC/xbox-256.png" height="192" width="192" alt="Xbox logo" align="right" />

# Reshade for UWP

An awesome GUI wrapper that injects Reshade in UWP apps and games. It also supports Xbox Game Pass PC and Microsoft Store games.

## Installation

Just go to the [releases page](https://github.com/MilkyDeveloper/ReshadeForUWP/releases/) to install Reshade for UWP. It's a portable/bundled exe so there's no setup required whatsoever!

## Design

Image of Reshade for UWP setup that's using the UWP Acrylic effect (despite being a Win32 WPF app)
![Image of Reshade for UWP setup](https://raw.githubusercontent.com/MilkyDeveloper/dump/%F0%9F%96%BC/Reshade%20for%20UWP%2012_26_2020%209_18_07%20PM.png)

Image of Reshade's Oilify effect being applied to the Xbox Game Pass PC version of Crusader Kings 3 from the automatic setup of this app
![Image of the Reshade Oilify effect being applied to the Xbox Game Pass PC version of Crusader Kings 3](https://raw.githubusercontent.com/MilkyDeveloper/dump/%F0%9F%96%BC/Crusader%20Kings%20III%2012_26_2020%209_21_55%20PM.png)

## Usage

### Most UWP apps

Most UWP apps are standalone and don't have a launcher. For these apps, just press the ```Choose an app to launch``` button and choose your UWP app. Remember that the list is composed of the "system" UWP app names, so they have a format of:

> publisher.appname_version/architecture/other

Once you've chose the app just press ```Done selecting the app name?``` and select it again (why? tldr; it takes two different application names to launch the app and locate the directory of it). Once that's done, press ```Yes``` and then save the ```.bat``` file in whatever directory you would like too use. To launch it double-click the bat file and the game will be launched with Reshade injected. However, Reshade doesn't know where all of it's files, shader files (```.fx```) and textures are located, so clicking ```Generate Reshade.ini``` should   download all of these files, extract them, and configure the ```Reshade.ini```. Congratulations, you've configured a working Reshade install for a UWP game!

### Still not working?

Chances are you're app has a custom launcher. The manifest of the app tells it to launch one program that in turn launches *the actual game*, and this messes with the auto-detection of the exe name to inject.

Fill in the ```Auto (...)``` textbox on the top of the app using the instructions accessible by hovering your mouse over the question mark at the right. The basic rundown is:

> 1. Go to the task manager's Details tab
> 2. Scroll through and find your game's process name. Naturally, this is quite hard, so in most apps you can go to the main Processes tab of the Task Manager and right click on the main entry of your game (or expand the dropdown and find the nested processes if that doesn't work) and select Go to details. Note the text under the Name collumn.
> 3. Put that **exact** process name in the textbox. Note that this is **case-sensitive**, meaning you must preserve any capital characters. Additionaly, don't omit the **.exe** from the end of your process name. For example, Doom Eternal's process name is ```DOOMEternalx64vk.exe``` and typing ```Doometernal64VK``` in the textbox wouldn't work.
> 4. Follow the instructions from the **Most UWP apps** subheading above.

### I still don't get Reshade on my screen!

Well, you might want to scroll down and read the limitations and then post a Github Issue.

## Limitations

* Unity games may not work, but I've only tested Descenders. **EDIT:** Totally Accurate Battle Simulator works without a hitch.
* You might have some trouble getting this to work on games that are not on Steam due to the different, weird game engines that games like Minecraft Windows 10 Edition and Forza Horizon 4 may have.
* Depth buffer isn't tested **yet**. It's for-sure what I'm going to test next after I get some games downloaded to test
* You tell me in the Github Issues tab!

## License

Naturally, because Reshade is awesome (and open source!), and given the fact I love the MIT License, that's what I set it to. Just be aware of the work that was put into the making of this project and basic human ethics.
