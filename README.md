Basic system for managing transshipment warehouse. Windows Platform Application (WPF).Now using asynchronic connection to Azure SQL.
Using: 
  - Azure SQL
  - .net 9.0
  - Microsoft Entra ID
  - MahApps.Metro for UI https://github.com/MahApps/MahApps.Metro
  - WPF

Features:
  - Storage position: add & list available storage positions
  - Supplier: add a new supplier
  - Package: calculating valume
  - can show supplier address real-time
    
Future features planned (TODO):
  - list of suppliers & shipments
  - proper new version release with secrets management implemented 
  - UNIT testing...
  - basic user settings: custom color scheme, language
  - possibly more

Pending fixes for the next version: 
  - barcode generator - IN PROGRESS: ███████▒▒▒ [75%]
  - Localization Support
  - Loading bar & "wake up call" for connecting to the Azure server at launch with bigger timeout set
    
DONE ✓

8.4.2025
  - Serilog for logging exceptions ✓

6.4.2025 
  - PSČ (Postcode): proper format ✓
  - button for adding another supplier not working properly ✓
  - input check for storage positions ✓
