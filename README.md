# AppxInstaller

This project was created in respose to some misconceptions and false information about the universal windows platform (UWP) apps, specifically with regards to their dependency on the windows store and suppsed inability to be distributed outside it.

This repo is devided in to two parts.

##Standalonemsi
This folder contains a powershell script that packages an appx file into an msi. It will also embed an installation script that will enable sideloading in the regist4ry, trust the application cert if it is not already trusted and deploy the application. Currently this script is a proof of concept with many of the values hardcoded, but later on it will be able to wrap any appx or appx bundle like this and read all the relevant information from the package directly.


##Appxinstaller
Appx installer is a file handler for appx files. using it, when a appx file is double clicked, a window is shown with information about the app, wether the cert is already trusted, what drive to install to and so on. This application is a work in progress and not really usable at this point.

##Miconceptions about uwp

###Non-store uwp applications require developer registration/approval

False. No registration or approval of developers is required for developing uwp apps. when publishing to the windows store, developers must register with microsoft, but even then there is no approval process of the developer. 

###Non-store uwp application must stillb e submitted to microsoft

False. An app that is not published to the store is signed locally using the signtool.exe utility from the windows sdk. Signing is done via a standard x509 certificate that can also be created locally with the makecert utility. 

###In order to install Non-store uwp apps, users must manually change their settings and accept security prompts

False. Prior to the windows 10 november update, the default setting in windows was to only install uwp apps from the store. since then the default is now to allow other apps by default. even so however, this setting is just a registry key and can be set by any app with appropriate privileges without any prompts. The standalone msi and appx installer in this repo sets these keys transparently from the user.

###Manually installing Uwp apps requires hacks/backdoors/unofficial apis.

False. The standalone installer uses standard msi functionality and the Wix toolkit using standard components. No custom code is required for the actual package installation. 

###Non-uwp application do not have access to uwp apis

Mostly false. Some Uwp apis are not available for non uwp apps, mostly relating to the store, the app model itself (like sandboxed storage) and ads, but some other significant apis are not available, like the xaml ui stack.

###Uwp applications cannot access traditional windows apis

Prior to windows 10, This was true for the most part. however with windows 10 a significant number of win32 apis was made available for uwp apps. 

### Uwp apps cannot support mods

Yes and no. Uwp apps install into a protected folder that cannot be modified by users. The platform also hash every file in the app using the signature cert and validates them agasint this hash. This means that the files that are contained in the appx cannot be modified without resigning the app. However, Apps are free to read from the users documents folder as well as custom files. In other words, mods have to be explicitly supported by the application. 

