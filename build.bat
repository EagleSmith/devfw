@echo off

color 66

echo =======================================

echo = ����ϲ����� github.com/atnet/devfw =

echo =======================================

set dir=%~dp0

set megdir=%dir%\dll\

if exist "%megdir%merge.exe" (

  echo ������,���Ե�...
  cd %dir%dist\dll\

echo  /keyfile:%dir%ops.cms.snk>nul

"%megdir%merge.exe" /closed /log:%dir%dist\build_log.txt /ndebug /targetplatform:v4 /target:dll /out:%dir%dist\atnet.devfw.dll^
 AtNet.DevFw.Core.dll AtNet.DevFw.PluginKernel.dll AtNet.DevFw.Data.dll AtNet.DevFw.Template.dll AtNet.DevFw.Web.dll AtNet.DevFw.Toolkit.Data.dll
  


  echo ���!�����:%dir%dist\atnet.devfw.dll

)


pause