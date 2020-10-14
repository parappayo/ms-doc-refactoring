
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

build-mono: *.cs
	mono-csc *.cs -r:System.Windows.Forms.dll -r:System.Drawing.dll -out:${PROJECT}.exe

run-mono: build-mono
	mono ${PROJECT}.exe
