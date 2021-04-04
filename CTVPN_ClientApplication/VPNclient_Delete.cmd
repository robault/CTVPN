NET STOP "CT VPN Service"

osql -E -i "C:\Program Files\Company Name\CTVPN Client\VPNclient_Delete.sql"

XCOPY "C:\Program Files\Company Name\CTVPN Client\Rasphone_blank\rasphone.pbk" "C:\Documents and Settings\All Users\Application Data\Microsoft\Network\Connections\Pbk\rasphone.pbk" /V /C /H /R /Y