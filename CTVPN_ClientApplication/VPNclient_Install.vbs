'CustomActionData property = [ProgramFilesFolder][Manufacturer]\[ProductName]\
strArgs = Session.Property("CustomActionData")

'Run the installation of the SQL objects
dim shell
set shell=createobject("wscript.shell")
shell.run """" & strArgs & "" & "VPNclient_Install.cmd""", 0, True
set shell=nothing

'Delete remaining setup files
dim fso
Set fso = CreateObject("Scripting.FileSystemObject") 

If fso.FileExists(strArgs & "ctvpn.exe") Then
   fso.DeleteFile strArgs & "ctvpn.exe"
End If 

If fso.FileExists(strArgs & "VPNclient_Create.sql") Then
   fso.DeleteFile strArgs & "VPNclient_Create.sql"
End If 

If fso.FileExists(strArgs & "VPNclient_Install.cmd") Then
   fso.DeleteFile strArgs & "VPNclient_Install.cmd"
End If