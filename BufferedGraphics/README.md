
# BufferedGraphics Example

[Example originally found here](https://docs.microsoft.com/en-us/dotnet/api/system.drawing.bufferedgraphics).

Requires the [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472) package.

## Running with Mono

* `apt install mono-complete`
* `mono-csc *.cs -r:System.Windows.Forms.dll -r:System.Drawing.dll -out:BufferedGraphicsExample.exe`
* `mono BufferedGraphicsExample.exe`

Alternatively, `make run-mono`
