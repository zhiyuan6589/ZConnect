[Setup]
AppName=ZConnect
AppVersion=1.0.0
DefaultDirName={pf}\ZConnect
DefaultGroupName=ZConnect
OutputDir=Output
OutputBaseFilename=ZConnectInstaller
SetupIconFile=E:\projects\ZConnect\Resources\logo.ico

[Files]
Source:"E:\projects\ZConnect\publish\*"; DestDir:"{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\ZConnect"; Filename: "{app}\ZConnect.exe"
Name: "{userdesktop}\ZConnect"; Filename: "{app}\ZConnect.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "创建桌面快捷方式"; GroupDescription: "附加任务："
