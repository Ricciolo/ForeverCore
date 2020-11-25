dotnet publish -c release -r win-x86 src
dotnet publish -c release -r win-x64 src
dotnet publish -c release -r linux-x64 src
dotnet publish -c release -r linux-arm src
powershell.exe -nologo -noprofile -command "& {Add-Type -A System.IO.Compression.FileSystem;[IO.Compression.ZipFile]::CreateFromDirectory('src\ForeverCore\bin\Release\net5.0\linux-arm\publish', 'linux-arm.zip');}"
powershell.exe -nologo -noprofile -command "& {Add-Type -A System.IO.Compression.FileSystem;[IO.Compression.ZipFile]::CreateFromDirectory('src\ForeverCore\bin\Release\net5.0\linux-x64\publish', 'linux-x64.zip');}"
powershell.exe -nologo -noprofile -command "& {Add-Type -A System.IO.Compression.FileSystem;[IO.Compression.ZipFile]::CreateFromDirectory('src\ForeverCore\bin\Release\net5.0\win-x86\publish', 'win-x86.zip');}"
powershell.exe -nologo -noprofile -command "& {Add-Type -A System.IO.Compression.FileSystem;[IO.Compression.ZipFile]::CreateFromDirectory('src\ForeverCore\bin\Release\net5.0\win-x64\publish', 'win-x64.zip');}"