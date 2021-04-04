osql -E -i "C:\Program Files\Company Name\CTVPN Client\VPNclient_Install.sql"

XCOPY "C:\Program Files\Company Name\CTVPN Client\Rasphone\rasphone.pbk" "C:\Documents and Settings\All Users\Application Data\Microsoft\Network\Connections\Pbk\rasphone.pbk" /V /C /H /R /Y

NET START "CT VPN Service"