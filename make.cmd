rem run from visual studio developer command prompt

msbuild dcmeditor.sln  /p:Configuration=Release /p:Platform="x86"
mkdir Deploy
xcopy bin\x86\Release\dcmeditor.exe Deploy /Y

for /R packages %%f in (*.dll) do copy %%f Deploy /Y
xcopy dcmtk\* Deploy\dcmtk\ /Y
