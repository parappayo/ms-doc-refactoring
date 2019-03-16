
./obj/Debug/netcoreapp2.1/*.dll: *.cs
	dotnet build CancellationTokenExample.csproj

build: ./obj/Debug/netcoreapp2.1/*.dll

clean:
	rm -rf ./obj

run: build
	dotnet ./bin/Debug/netcoreapp2.1/CancellationTokenExample.dll
