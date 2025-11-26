# Misuzu Desktop App

This is an Electron wrapper for the Misuzu language learning tracker web application. It allows you to run Misuzu as a self-contained desktop application.

## Prerequisites

- [Node.js](https://nodejs.org/) (version 18 or higher)
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

## Setup

1. Navigate to the electron-app directory:
   ```bash
   cd electron-app
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

## Running the Application

To start the desktop application:

```bash
npm start
```

This will:
1. Start the ASP.NET Blazor server in the background
2. Open an Electron window displaying the application
3. Automatically connect to http://localhost:5000

When you close the Electron window, the ASP.NET server will automatically shut down.

## Building Distributable Packages

To build distributable packages for different platforms:

### Linux
```bash
npm run build:linux
```
This creates AppImage and .deb packages.

### Windows
```bash
npm run build:win
```
This creates an NSIS installer and a portable executable.

### macOS
```bash
npm run build:mac
```
This creates a .dmg and .zip package.

### All Platforms
```bash
npm run build
```

Built packages will be available in the `dist` directory.

## How It Works

The Electron application:
1. Spawns the ASP.NET application as a child process using `dotnet run`
2. Waits for the server to be ready on port 5000
3. Opens a browser window pointing to the local server
4. Manages the lifecycle of both processes

## Customization

### Window Size
Edit `main.js` and modify the `width` and `height` properties in the `createWindow()` function:

```javascript
mainWindow = new BrowserWindow({
  width: 1200,  // Change this
  height: 800,  // Change this
  // ...
});
```

### Server Port
If you need to change the port, modify the `SERVER_URL` constant in `main.js`:

```javascript
const SERVER_URL = 'http://localhost:5000';  // Change port here
```

And update the port in the net.connect call:

```javascript
const client = net.connect({ port: 5000, host: 'localhost' }, () => {
  // Change port here too
});
```

### Application Icon
Place your icon file as `icon.png` in the electron-app directory. For better results across platforms, you can also provide:
- `icon.ico` for Windows
- `icon.icns` for macOS

## Troubleshooting

### Server fails to start
- Ensure .NET 9 SDK is installed: `dotnet --version`
- Check that port 5000 is not already in use
- Check the terminal output for any ASP.NET errors

### Blank window
- Wait a few seconds for the server to fully start
- Check the terminal for any errors
- Try opening http://localhost:5000 in a regular browser to verify the server is running

### Building fails
- Ensure all npm dependencies are installed: `npm install`
- For Windows builds on Linux, you may need Wine installed
- For macOS builds, you must be running on macOS
