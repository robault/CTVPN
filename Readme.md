
# CT VPN

Circa 2008 and .Net 2.0 and Asp.Net 1.1 era, one of my first real projects. Connects remote clients to a VPN using Windows dialer (rasphone) via web service, triggered via web application. Given the time frame the Windows ras dialer feature was installed by dewfault in Windows XP. Additionally, the web service and web application were hosted on Windows Server 2003 with a default license which limited the number of client access licenses to 5, meaning at most only 5 clients can have an open VPN connection at a time.

It has been opened and upgraded to .Net 4.6.1 in Visual Studio 2019 Community Edition. The installer fearture is no longer included in newer versions of Visual Studio but you can install this extension to at least view this project in its entirety.

[Microsoft Visual Studio Installer Projects](https://marketplace.visualstudio.com/items?itemName=visualstudioclient.MicrosoftVisualStudio2017InstallerProjects)

### Setup

The project has been scrubbed but let's hypothetically say you actually want to run this (and assuming it still can in 2021), replace these values in the solution:

* <user_name> (used for app and dadtabase access)
* <password> (same ^)
* 'Company Name' (with your company name)
* <web_service_ip_or_domain_name> (Settings.settings needs edited outside Visual Studio, or chose a different editor)
* <encryption_salt> (I used a guid)

### Client Identity

Each client has an identity that stored in their local database. Make sure these are set properly for each client in VPNclient_Install.sql in the 'Insert Identity in DB' section.

* <encryped_guid>
* <encryped_name>
* <encryped_cnxName>
* <encryped_cnxUser>
* <encryped_cnxPassword>

 This even included an installer making it easy for local clients to setup. To avoid having to distribute client info, the individual identity values were hard coded in the 'Insert Identity in DB' section of VPNclient_Install.sql. Then, a client specific build was done then sent and installed at the client location.



 The security built into this application is pretty rudimentary but sufficient enough for rmeote connections meant to be temporary in nature. If I were to build something like this today the technology is drmatically different and I wouldn't hard code values like this.

### Operation

The web application and web service were transitory. I would only run them (run IIS) when I wanted to signal a client to connect the VPN back to my server. All it took was checking a box and waiting for the client to check in. The client guid would need to match what the server knew about otherwise it is ignored. Once the VPN was connected then I knew an encrpted tunnel was open for me to RDP into the machine and do my work.

### Warning

Currently does not build mostly due to the scrubbed value for <web_service_ip_or_domain_name>. I would honestly be surprised if anyone actually wanted to use this today. But if you really really do, you're on your own. At the very least replace the values stated above and work through any runtime issues upgrading from .Net 2.0 to 4.6.1. And...make sure rasphone is part of Windows 10 or modify the installer to add it. I don't know if that feature even exists anymore.

## \~la fin\~

I'm adding this to GitHub mostly as an archive of what I've worked on in the past. I wouldn't consider it fit for use and certainly not in production. Although given how prevalent ffmpeg is I don't feel as self concoius about spawning background terminal processes in code. lol
