# Bave P7M - Digital Signature File Reader

A simple, portable Windows application to extract files from digitally signed `.p7m` archives.

## Features

- 🚀 **Portable** - Single executable file, no installation required
- 🖱️ **Drag & Drop** - Simply drag `.p7m` files onto the application window or executable
- 📁 **Automatic Organization** - Extracted files are saved to a `non firmati` subfolder
- ⚡ **Fast Processing** - Batch process multiple files at once
- 🔓 **No Signature Verification** - Extracts content without validating signatures
- 📊 **Progress Tracking** - Visual progress bar shows extraction status

## Usage

### Method 1: Drag & Drop onto Window
1. Launch `P7MExtractor.exe`
2. Drag one or more `.p7m` files onto the application window
3. Files will be automatically extracted to the `non firmati` subfolder

### Method 2: Drag & Drop onto Executable
1. Drag `.p7m` files directly onto `P7MExtractor.exe`
2. The application will launch and process the files automatically
3. Extracted files will be saved to the `non firmati` subfolder

### Output Location
Extracted files are saved in a subfolder named `non firmati` (unsigned) in the same directory as the original `.p7m` file.

**Example:**
```
C:\Documents\contract.pdf.p7m
↓
C:\Documents\non firmati\contract.pdf
```

## Building from Source

### Prerequisites
- .NET 9.0 SDK or later
- Windows OS

### Build Instructions

#### Using the Batch File
Simply run the included `build.bat` file:
```batch
build.bat
```

#### Manual Build
```powershell
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish
```

The compiled executable will be available in the `./publish` folder.

## Technical Details

- **Framework**: .NET 9.0 (Windows Forms)
- **Cryptography Library**: BouncyCastle (Portable.BouncyCastle 1.9.0)
- **Target Platform**: Windows x64
- **Deployment**: Self-contained, single-file executable

## Notes

- Only `.p7m` files are processed; other file types are ignored
- The application does not verify digital signatures
- No user confirmation is required during extraction
- Errors during extraction are silently skipped to allow batch processing to continue

## Links

- **Website**: [www.bave.info](https://www.bave.info)
- **Support**: [ko-fi.com/bave_](https://ko-fi.com/bave_)

## License

This project is licensed under the MIT License with Commons Clause - see the [LICENSE](LICENSE) file for details.

**Summary:**
- ✅ Free to use, modify, and distribute
- ✅ Attribution required (must credit Alessandro Stefani)
- ❌ Cannot be sold or used commercially without permission

**Dependencies:**
- BouncyCastle cryptography library (MIT X11 License)
