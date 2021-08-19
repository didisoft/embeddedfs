rem "c:\Program Files (x86)\VSdocman\VSdocmanCmdLine.exe" /vs 2010 /operation compileProject /profile "helpviewer" "c:\Projects\PGPNet\PGPNet\DidiSoft.Pgp.csproj"
"c:\Program Files (x86)\VSdocman\VSdocmanCmdLine.exe" /vs 2015 /operation compileSolutionToSingle /profile "helpviewer" "c:\Projects\PGPNet\PGPNetDoc\PGPNetDoc.sln"
copy .\PGPNetDoc\VSdoc\PGPNetDoc.mshc .\PGPNetDoc\Help\DidiSoft.mshc /y