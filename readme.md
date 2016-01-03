# FTP HTTP Clipboarder

This is a tiny app that serves the only one purpose and only for Sitecore staff:

* check if there is a file or an image in the clipboard
* upload it to `ftp://dl.sitecore.net/updater/share/7/6/` (two first chars of new guid)
* put the http link to the clipboard

#### Nuances

* The app should be configured before using: update the FtpHttpClipboarder.exe.config file
* An image is uploaded with the `NEWGUIDWITHOUTDASHES.png` file name, however a `test123.cs` file is uploaded as `/7/6/test123.76d3e70f-cbc6-421b-a72e-96e3fa49bb28`
.cs` to avoid conflicts and to preserve original name and extension.
* If the app doesn't work, check `FtpHttpClipboarder.log` file which should appear side-by-side with the executable

#### Download

Please download the latest version of the tool in [GitHub releases](https://github.com/alienlab/FtpHttpClipboarder/releases) section