const { app, BrowserWindow } = require('electron');
const { spawn } = require('child_process');
const path = require('path');
const net = require('net');

let mainWindow = null;
let serverProcess = null;
const SERVER_URL = 'http://localhost:5000';
const MAX_RETRIES = 30;
const RETRY_DELAY = 1000;

function checkServerReady(retries = 0) {
  return new Promise((resolve, reject) => {
    const client = net.connect({ port: 5000, host: 'localhost' }, () => {
      client.end();
      resolve(true);
    });

    client.on('error', () => {
      if (retries < MAX_RETRIES) {
        setTimeout(() => {
          checkServerReady(retries + 1).then(resolve).catch(reject);
        }, RETRY_DELAY);
      } else {
        reject(new Error('Server failed to start'));
      }
    });
  });
}

function startServer() {
  return new Promise((resolve, reject) => {
    const isDev = !app.isPackaged;
    const projectPath = path.join(__dirname, '..');

    let executablePath;
    let workingDir;

    if (isDev) {
      // Development: use the debug build
      executablePath = path.join(projectPath, 'bin/Debug/net9.0/Misuzu');
      workingDir = projectPath;
    } else {
      // Production: use the packaged self-contained executable
      const platform = process.platform;
      let runtime;
      let exeName;

      if (platform === 'win32') {
        runtime = 'win-x64';
        exeName = 'Misuzu.exe';
      } else if (platform === 'darwin') {
        runtime = 'osx-x64';
        exeName = 'Misuzu';
      } else {
        runtime = 'linux-x64';
        exeName = 'Misuzu';
      }

      executablePath = path.join(process.resourcesPath, 'backend', runtime, exeName);
      workingDir = path.join(process.resourcesPath, 'backend', runtime);
    }

    console.log('Starting ASP.NET server...');
    console.log('Executable path:', executablePath);
    console.log('Working directory:', workingDir);

    serverProcess = spawn(executablePath, [], {
      cwd: workingDir,
      env: {
        ...process.env,
        ASPNETCORE_ENVIRONMENT: isDev ? 'Development' : 'Production',
        ASPNETCORE_URLS: SERVER_URL,
        DOTNET_ENVIRONMENT: isDev ? 'Development' : 'Production'
      },
      stdio: 'inherit'
    });

    serverProcess.on('error', (err) => {
      console.error('Failed to start server:', err);
      reject(err);
    });

    serverProcess.on('exit', (code) => {
      if (code !== 0 && code !== null) {
        console.error(`Server process exited with code ${code}`);
      }
    });

    checkServerReady()
      .then(() => {
        console.log('Server is ready!');
        resolve();
      })
      .catch((err) => {
        console.error('Server failed to start:', err);
        reject(err);
      });
  });
}

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 1200,
    height: 800,
    autoHideMenuBar: true,
    webPreferences: {
      nodeIntegration: false,
      contextIsolation: true
    },
    icon: path.join(__dirname, 'icon.png')
  });

  mainWindow.loadURL(SERVER_URL);

  // Hide scrollbars
  mainWindow.webContents.on('did-finish-load', () => {
    mainWindow.webContents.insertCSS(`
      ::-webkit-scrollbar {
        display: none;
      }
      * {
        -ms-overflow-style: none;
        scrollbar-width: none;
      }
    `);
  });

  // Open DevTools in development
  if (process.env.NODE_ENV === 'development') {
    mainWindow.webContents.openDevTools();
  }

  mainWindow.on('closed', () => {
    mainWindow = null;
  });
}

app.on('ready', async () => {
  try {
    await startServer();
    createWindow();
  } catch (err) {
    console.error('Failed to start application:', err);
    app.quit();
  }
});

app.on('window-all-closed', () => {
  if (serverProcess) {
    serverProcess.kill();
  }
  app.quit();
});

app.on('activate', () => {
  if (mainWindow === null) {
    createWindow();
  }
});

process.on('exit', () => {
  if (serverProcess) {
    serverProcess.kill();
  }
});
