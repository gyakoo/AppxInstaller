This repostitory was created in respose to some misconceptions and incorrect information about the universal windows platform (UWP) apps, specifically with regards to their dependency on the windows store and suppsed inability to be distributed outside it.

This repostitory is devided in to two parts:

## StandaloneMsi
This folder contains a powershell script, `Generate-AppxMsi.ps1`, that packages an appx file into an msi. It will also embed an installation script that will enable sideloading in the registry, trust the application cert if it is not already trusted and deploy the application. Currently this script is a proof of concept with hardcoded values, but later on it will be able to wrap any appx or appx bundle like this and read all the relevant information from the package directly.


## Appxinstaller
Appx installer is a file handler for appx files. Using it, when a appx file is double clicked, a window is shown with information about the app. The user can then click install to install the app.

## Miconceptions about uwp

#### Non-store uwp applications require developer registration/approval

**False**. No registration or approval of developers is required for developing uwp apps. When publishing to the windows store, developers must register with microsoft, but even then there is no approval process of the developer. 

#### Non-store uwp application must still be submitted to Microsoft

**False**. An app that is not published to the store is signed locally using the signtool.exe utility from the windows sdk. Signing is done via a standard x509 certificate that can also be created locally with the makecert utility included in the Windows Sdk. 

#### In order to install Non-store uwp apps, users must manually change their settings and accept security prompts

**False**. Prior to the windows 10 november update, the default setting in windows was to only install uwp apps from the store. Since then the default is now to allow other apps by default. Even so however, this setting is just a registry key and can be set by any app with appropriate privileges without any prompts. The standalone msi and appx installer in this repo sets these keys transparently from the user. When trusting an untrusted certificate, a uac prompt is required but this is not related to the universal windows platform.

#### Manually installing Uwp apps requires hacks/backdoors/unofficial apis.

**False**. The standalone installer uses standard msi functionality and the Wix toolkit using standard components. No custom code is required for the actual package installation. In powershell the [Add-AppxPackage](https://technet.microsoft.com/en-us/library/ dn448376.aspx) cmdlet is used and from code, the [Package manager api](https://msdn.microsoft.com/en-us/library/windows/apps/windows.management.deployment.packagemanager.aspx) can be used.

#### Non-uwp application do not have access to Uwp apis

**Mostly false**. Some Uwp apis are not available for non uwp apps, mostly relating to the store, the app model itself (like sandboxed storage) and ads, but some other significant apis are available, like the xaml ui stack.

#### Uwp applications cannot access traditional windows apis

**Prior to windows 10**, This was true for the most part. However with windows 10 a significant number of win32 apis was made available for uwp apps. 

#### Uwp apps cannot support mods

**Yes and no**. Uwp apps install into a protected folder that cannot be modified by users. The platform also hash every file in the app using the signature cert and validates them agasint this hash. This means that the files that are contained in the appx cannot be modified without resigning the app. However, Apps are free to read from the users documents folder as well as custom files. In other words, *mods have to be explicitly supported by the application*. 

