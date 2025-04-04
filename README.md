Basic system for managing transshipment warehouse. Windows Platform Application (WPF).Now using asynchronic connection to Azure SQL.
Using: 
  - Azure SQL
  - .net 9.0
  - Microsoft Entra ID
  - MahApps.Metro for UI https://github.com/MahApps/MahApps.Metro
  - WPF
Features:
  - Storage position: add & list avaiable storage positions
  - Supplier: add a new supplier
  - Package: calculating valume
  - can show supplier adress real-time
    
Future featurs planned (TODO):
  - list of suppliers & shipments
  - proper new version release with secrets management implemented
  - loading bar & "wake up call" for connecting to the Azure server at launch with bigger timeout set
  - UNIT testing...
  - barcode generator
  - Serilog for logging exceptions
  - basic user settings: custom color scheme, language
  - possibly more

Pending fixes for next version:
  - IČO: proper format
  - button for adding another supplier not working properly
  - input check for storage positions
