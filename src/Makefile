ifdef RELEASE
release:
	xbuild /nologo /p:DefineConstants="UNIX" /p:UNIX=TRUE /p:Configuration=Release /property:GenerateFullPaths=true
else 
debug:
	xbuild /nologo /p:DefineConstants="UNIX" /p:UNIX=TRUE /p:Configuration=Debug /property:GenerateFullPaths=true
endif


test: 
	nunit-console ./NoisedTests/bin/Debug/NoisedTests.dll
	
clean:
	xbuild /nologo /t:clean
