
# Appx installer
This folder contains the appx installer application that can be associated with appx packages in order to install them by double clicking. At this time you do have to manually associate appx files to this application by double clicking on one and selecting this application to use for opening.

## How it works
The installer examines the appx and extracts the certificate. It then checks if the certificate is trusted and if not, it adds it to the trusted people store. It then uses the [Package manager api](https://msdn.microsoft.com/en-us/library/windows/apps/ to actually install the application.

