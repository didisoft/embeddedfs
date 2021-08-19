del .\EmbeddedFS\VSdoc\*.* /s/q/f
"c:\Program Files (x86)\VSdocman\VSdocmanCmdLine.exe" /vs 2019 /operation compileProject /profile "default" "c:\Projects\EmbeddedFS\EmbeddedFS\EmbeddedFS.csproj"
copy .\EmbeddedFS\VSdoc\EmbeddedFS.chm C:\Projects\EmbeddedFS\Setup\EmbeddedFS.chm /y

