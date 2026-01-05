Outdated, badly needs huge refactor and overhaul which makes sense cause it was project for learning basics of C# programming.  

Basic system for managing transshipment warehouse. Windows Platform Application (WPF). Now using an asynchronous connection to Azure SQL.
Using: 
  - Azure SQL
  - .net 9.0
  - Microsoft Entra ID
  - MahApps.Metro for UI https://github.com/MahApps/MahApps.Metro
  - WPF
  - ZXing .NET for generating barcodes https://github.com/micjahn/ZXing.Net
  - Serilog for logging exceptions https://github.com/serilog/serilog

Features:
  - Storage position: add & list available storage positions
  - Supplier: add a new supplier, list of suppliers, supplier detail
  - Package: calculating valume
  - can show supplier address real-time
    
Future features planned (TODO):
  - list of suppliers & shipments
  - proper new version release with secrets management implemented 
  - UNIT testing...
  - basic user settings: custom color scheme, language
  - possibly more

Pending fixes for the next version: 
  - code cleanup, warnings removal
  - Localization Support
    
DONE ✓

21.4.2025
- Supplier detail - User Control, which will show details about the currently selected supplier from the supplier list 

20.4.2025
  - List of Suppliers 

14.4.2025
  - Barcode generator ✓
  - Loading bar & "wake up call" for connecting to the Azure server at launch with a bigger timeout set ✓

8.4.2025
  - Serilog for logging exceptions ✓

6.4.2025 
  - PSČ (Postcode): proper format ✓
  - button for adding another supplier not working properly ✓
  - input check for storage positions ✓
