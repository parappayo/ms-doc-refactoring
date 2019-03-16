
./obj: *.cs
	dotnet build CancellationTokenExample.csproj

build: ./obj

clean:
	rm -rf ./obj

run: build
	dotnet ./bin/Debug/netcoreapp2.1/CancellationTokenExample.dll
