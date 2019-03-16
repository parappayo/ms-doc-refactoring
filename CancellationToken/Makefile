
PROJECT?=CancellationTokenExample
FRAMEWORK?=netcoreapp2.1
CONFIGURATION?=Debug

BIN?=bin/${CONFIGURATION}/${FRAMEWORK}
OBJ?=obj/${CONFIGURATION}/${FRAMEWORK}

./${OBJ}/*.dll: *.cs
	dotnet build -c ${CONFIGURATION} ${PROJECT}.csproj

build: ./${OBJ}/*.dll

clean:
	dotnet clean -c ${CONFIGURATION}

run: build
	dotnet ./${BIN}/${PROJECT}.dll
