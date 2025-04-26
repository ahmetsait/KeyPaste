<div align="center">

![Icon](KeyPaste/Resources/KeyPaste.svg)  
KeyPaste  
[![Platforms](https://img.shields.io/badge/platform-windows-blue)](https://github.com/ahmetsait/KeyPaste/releases) [![Version](https://img.shields.io/github/v/release/ahmetsait/KeyPaste)](https://github.com/ahmetsait/KeyPaste/releases/latest) [![Downloads](https://img.shields.io/github/downloads/ahmetsait/KeyPaste/total)](https://github.com/ahmetsait/KeyPaste/releases) [![License](https://img.shields.io/github/license/ahmetsait/KeyPaste)](LICENSE) [![Sponsor](https://img.shields.io/badge/Sponsor-black?logo=githubsponsors)](https://github.com/sponsors/ahmetsait)
========
</div>

KeyPaste is a simple tray-icon application to copy & paste clipboard text by simulating keyboard input.
It can be useful e.g. if you're using a virtual machine and would like to copy & paste text from the host machine.

## Downloads
Pre-built binaries can be found inside [Releases](https://github.com/ahmetsait/KeyPaste/releases) section.

## Building
KeyPaste can built by opening solution file [KeyPaste.sln](KeyPaste.sln) in Visual Studio and clicking `Build`→`Build Solution` from the menu.
Alternatively, you can use MSBuild from the command line:
```
%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe KeyPaste.sln
```
## License
KeyPaste is licensed under the [MIT License](LICENSE).
