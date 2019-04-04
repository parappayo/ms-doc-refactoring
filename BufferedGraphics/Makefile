
PROJECT?=BufferedGraphicsExample
FRAMEWORK?=net472
CONFIGURATION?=Debug

BIN?=bin/${CONFIGURATION}/${FRAMEWORK}
OBJ?=obj/${CONFIGURATION}/${FRAMEWORK}

./${OBJ}/*.dll: *.cs *.csproj
	dotnet build -c ${CONFIGURATION} ${PROJECT}.csproj

build: ./${OBJ}/*.dll

clean:
	dotnet clean -c ${CONFIGURATION}

test:
	dotnet test

run: build
	./${BIN}/${PROJECT}.exe
