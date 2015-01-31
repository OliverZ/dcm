rem run from visual studio developer command prompt

msbuild dcmedit.sln /p:Configuration=Debug /p:Platform="Any CPU"
mkdir Deploy
xcopy bin\Debug\dcmedit.exe Deploy

for /R packages %%f in (*.dll) do copy %%f Deploy
xcopy dcmtk\* Deploy\dcmtk\
xcopy img\* Deploy