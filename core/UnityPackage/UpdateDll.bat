pushd %~dp0

SET SRC=..\TemplateTable.Net35\bin\Release
SET SRC_JSON=..\..\plugins\TemplateTable.Json.Net35\bin\Release
SET SRC_PROTOBUF=..\..\plugins\TemplateTable.Protobuf.Net35\bin\Release
SET DST=.\Assets\Middlewares\TemplateTable
SET PDB2MDB=..\..\tools\unity3d\pdb2mdb.exe
SET SFK=..\..\tools\sfk\sfk.exe

%PDB2MDB% "%SRC%\TemplateTable.dll"
%PDB2MDB% "%SRC_JSON%\TemplateTable.Json.dll"
%PDB2MDB% "%SRC_PROTOBUF%\TemplateTable.Protobuf.dll"

COPY /Y "%SRC%\TemplateTable.dll*" %DST%
COPY /Y "%SRC_JSON%\TemplateTable.Json.dll*" %DST%
COPY /Y "%SRC_PROTOBUF%\TemplateTable.Protobuf.dll*" %DST%

REM Remove public token of referenced Newtonsoft.Json from TemplateTable.Json.dll
%SFK% rep %DST%\TemplateTable.Json.dll -pat -bin /0830ad4fe6b2a6aeed/000000000000000000/ -yes

popd
