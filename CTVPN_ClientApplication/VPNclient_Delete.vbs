'CustomActionData property = [ProgramFilesFolder][Manufacturer]\[ProductName]\
strArgs = Session.Property("CustomActionData")

'Run the installation of the SQL objects
dim shell
set shell=createobject("wscript.shell")
shell.run """" & strArgs & "" & "VPNclient_Delete.cmd""", 0, True
set shell=nothing