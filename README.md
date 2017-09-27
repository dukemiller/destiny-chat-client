# destiny-chat-client

A re-implementation of the [destiny.gg integrated chat](https://www.destiny.gg/bigscreen) websocket front-end client using windows native controls as a stand-alone executable. This is mostly an exercise in using the controls, personal use, and for fun.

**\* [Find the download link for the latest version here.](https://github.com/dukemiller/destiny-chat-client/releases/latest)**  

**How to login:**  
Only tested successfully on **Firefox**, there are a few tricks to how your browser will finally store your cookies, usually they're accessable in your firefox profile in some sort of cache or database. The best bet would be to go to [the chatroom](https://www.destiny.gg/bigscreen) and login, then while it's open you open the chat program and attempt to sign in.

**Full-Disclosure on Privacy**:     
I don't have an official endpoint for logging in and generating a token, so I search for the user's stored cookies for 'sid' and 'rememberme' key values. [This is the code that I do that in](destiny-chat-client/Services/CookieFinderService.cs). This information is **stored for later use in plain text** in the user settings stored at *%UserProfile%/appdata/local/destiny_chat_client/settings.json*. **This information is your login to the chat and should not be shared.**  

## Screenshot
![chat display](http://i.imgur.com/QoYWutZ.png)

----------

## Details

**Implemented**:  
\- Logging in and sending/receiving messages in chat  
\- User mentions and custom mentions  
\- Integrated emotes (text into chat), user flair  
\- Tab autocomplete for usernames and emotes  
\- User highlighting by clicking their username in chat, either in the message chunk or username  
\- The dialogs and most (~80%) of their functionality (settings, user list, emotes)  
\- Tray icon functionality  
\- Greentext  
\- Load history on connect  

**Missing/To Be Implemented**:  
\- /Me  
\- Basically any chat commands in general  
\- Unhandled server error states  
\- Server response issues and reconnection  
\- Emote sizing and animations  
\- Middle-click scrollwheel glyph  
\- Drag selecting multiple message text  

**Problems**:  
There's a pretty significant memory leak that I found in stress testing to the point that I manually call garbage collection periodically to help keep the issue in check. On busy discussion times with a lot of messages after long use, the upper bound of memory use shown in Task Manager is ~310 MB, with the average staying near ~250MB. At around 3 million messages posted, the application can crash. I'm looking more into fixing the problem.

**Other notes**:  
I'm open to any issues raised noting missing features, and checking out pull requests.

----------

## Build & Run

**Requirements:** [nuget.exe](https://dist.nuget.org/win-x86-commandline/latest/nuget.exe) on PATH, Visual Studio 2017 and/or C# 7.0 Roslyn Compiler  
**Optional:** Devenv (Visual Studio 2017) on PATH  

```
git clone https://github.com/dukemiller/destiny-chat-client.git
cd destiny-chat-client
nuget install destiny-chat-client\packages.config -OutputDirectory packages
```  

**Building with Devenv (CLI):** ``devenv destiny-chat-client.sln /Build``  
**Building with Visual Studio:**  Open (Ctrl+Shift+O) "destiny-chat-client.sln", Build Solution (Ctrl+Shift+B)

A "destiny-chat-client.exe" artifact will be created in the parent destiny-chat-client folder.

------------

## Image, Emote and Icon notice and license
I use the icons [provided here](https://github.com/destinygg/website/tree/master/assets/icons/icons) and emoticons [provided here](https://github.com/destinygg/website/tree/master/assets/emotes/emoticons). For the icon, I use the image [provided here](https://github.com/destinygg/website/blob/master/assets/web/img/destinygg.png). I have not modified the material in any way, outside of resizing the icon image. The designs including all CSS and images by <http://www.destiny.gg/> unless otherwise noted, is licensed under a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License. <http://creativecommons.org/licenses/by-nc-nd/3.0/deed.en_US>. 
