[Setup]
AppName=Coaches Comprehensive System
AppVersion=1.0
DefaultDirName={autopf}\Coaches Comprehensive System
DefaultGroupName=Coaches Comprehensive System
OutputDir=Output
OutputBaseFilename=CCS_Setup
Compression=lzma
SolidCompression=yes
ArchitecturesAllowed=x86 x64
ArchitecturesInstallIn64BitMode=x64

[Files]
Source: "src\CCS.App\bin\Release\net48\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Coaches Comprehensive System"; Filename: "{app}\CCS.App.exe"
Name: "{autodesktop}\Coaches Comprehensive System"; Filename: "{app}\CCS.App.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Code]
function InitializeSetup(): Boolean;
var
  ErrorCode: Integer;
  NetVersion: String;
begin
  Result := True;
  if not RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', NetVersion) then
  begin
    if MsgBox('This application requires .NET Framework 4.8. Do you want to download it now?', mbConfirmation, MB_YESNO) = IDYES then
    begin
      ShellExec('open', 'https://go.microsoft.com/fwlink/?LinkId=2085155', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
    end;
    Result := False;
  end;
end;