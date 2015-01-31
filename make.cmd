rem run from visual studio developer command prompt

msbuild dcmedit.sln /p:Configuration=Debug /p:Platform="Any CPU"
mkdir Deploy
xcopy bin\Debug\dcmedit.exe Deploy

rem todo copy dlls and xmls from package dirs
xcopy /S packages\* Deploy
xcopy dcmtk\* Deploy\dcmtk
xcopy img\* Deploy