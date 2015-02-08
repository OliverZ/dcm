rem run from visual studio developer command prompt

msbuild dcmeditor.sln /p:Configuration=Debug /p:Platform="Any CPU"
mkdir Deploy
xcopy bin\Debug\dcmeditor.exe Deploy /Y

for /R packages %%f in (*.dll) do copy %%f Deploy /Y
xcopy dcmtk\* Deploy\dcmtk\ /Y
xcopy img\* Deploy /Y