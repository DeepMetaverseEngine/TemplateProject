@echo off

SET TargetDir=%1

SET SRC_ASSETS1=%~dp0..\..\DeepCore\DeepCore.Unity.Assets
SET SRC_ASSETS2=%~dp0..\..\DeepCore\DeepMetaGame.UnityAssets

SET DST_ASSETS=%~dp0..\..\..\_Temp_Unity\Assets
SET DST_RUNTIME=%~dp0..\..\..\GameEditor\bin_unity\MyUnity_Data\Managed

echo ----------------------------------------------------------------------------------
echo - AOT Assets TO: %DST_ASSETS%
xcopy /Y    %SRC_ASSETS1%\Assets\*    %DST_ASSETS%\
xcopy /Y    %SRC_ASSETS2%\Assets\*    %DST_ASSETS%\

echo ----------------------------------------------------------------------------------
echo - AOT DLL TO: %DST_ASSETS%\Plugins\Library\
xcopy /Y /EXCLUDE:%~dp0copydll.exclude.unity.txt    %TargetDir%*.dll    %DST_ASSETS%\Plugins\Library\
xcopy /Y /EXCLUDE:%~dp0copydll.exclude.unity.txt    %TargetDir%*.pdb    %DST_ASSETS%\Plugins\Library\

echo ----------------------------------------------------------------------------------
if EXIST %DST_RUNTIME% (
echo - RUNTIME £º¸´ÖÆDLLµ½Runtime
xcopy /Y/E /EXCLUDE:%~dp0copydll.exclude.unity.txt    %TargetDir%*.dll    %DST_RUNTIME%
del /F %DST_RUNTIME%\DeepCore.UnityEditor.dll
)
echo ----------------------------------------------------------------------------------
