# 🚀 MiGente En Línea - FTP Deployment Guide (myASP.NET)

**Target Server:** win8146.site4now.net  
**Hosting:** myASP.NET (site4now.net shared hosting)  
**Database:** SQL5106.site4now.net  
**Remote Directory:** /migenteenlinea2/

---

## 📋 Quick Start

### One-Command Deployment

```powershell
cd "C:\Users\Ray\Documents\MiGenteEnlinea\MiGenteEnLinea.Clean"
.\publish-and-deploy-ftp.ps1
```

This will:
1. ✅ Build API and Web projects in Release mode
2. ✅ Publish to `C:\Publish\MiGenteEnlinea\`
3. ✅ Upload via FTP to win8146.site4now.net
4. ✅ Create deployment logs

**After upload completes**, configure IIS in myASP.NET Control Panel (see below).

---

## 🛠️ Available Deployment Commands

### Full Deployment (API + Web)
```powershell
.\publish-and-deploy-ftp.ps1
```

### Deploy API Only
```powershell
.\publish-and-deploy-ftp.ps1 -ApiOnly
```

### Deploy Web Only
```powershell
.\publish-and-deploy-ftp.ps1 -WebOnly
```

### Build Without Upload
```powershell
.\publish-and-deploy-ftp.ps1 -SkipUpload
# Creates binaries in C:\Publish\MiGenteEnlinea\ for manual review
```

### Upload Without Rebuild
```powershell
.\publish-and-deploy-ftp.ps1 -SkipBuild
# Uses existing binaries in C:\Publish\MiGenteEnlinea\
```

---

## 📂 Server Directory Structure

After deployment, your files will be organized as:

```
/migenteenlinea2/
├── api/                           # API Project
│   ├── MiGenteEnLinea.API.dll     # Main executable
│   ├── web.config                 # IIS configuration
│   ├── appsettings.Production.json # Production settings
│   ├── [~100 dependency DLLs]     # Runtime dependencies
│   ├── wwwroot/
│   │   └── uploads/
│   │       └── contratistas-fotos/
│   └── logs/                      # Application logs (created by app)
│       └── stdout_*.log           # Startup logs
│
└── web/                           # Web MVC Project
    ├── MiGenteEnLinea.Web.dll
    ├── web.config
    ├── appsettings.Production.json
    ├── wwwroot/
    │   ├── css/
    │   ├── js/
    │   ├── images/
    │   └── lib/
    └── logs/
        └── stdout_*.log
```

---

## ⚙️ IIS Configuration (myASP.NET Control Panel)

After FTP upload completes, you MUST configure IIS applications:

### Step 1: Access Control Panel
1. Login to https://member.myasp.net/
2. Select your domain/account
3. Navigate to **IIS Manager** or **Web Applications**

### Step 2: Configure API Application

**Create Application:**
- **Name:** `api` (or any name you prefer)
- **Virtual Path:** `/api`
- **Physical Path:** `/migenteenlinea2/api`
- **Application Pool:** Create new pool or use existing:
  - **.NET CLR Version:** `No Managed Code` ⚠️ CRITICAL
  - **Pipeline Mode:** `Integrated`
  - **Identity:** Default (ApplicationPoolIdentity)

**Binding:**
- If using subdomain: `api.migenteenlinea.com` → Application root
- If using path: `www.migenteenlinea.com/api` → Application in path

### Step 3: Configure Web Application

**Create Application:**
- **Name:** `web` or set as site root
- **Virtual Path:** `/` (for root) or `/web`
- **Physical Path:** `/migenteenlinea2/web`
- **Application Pool:** 
  - **.NET CLR Version:** `No Managed Code` ⚠️ CRITICAL
  - **Pipeline Mode:** `Integrated`

**Binding:**
- Domain: `www.migenteenlinea.com` → Application root

### Step 4: Set Folder Permissions

**Required Write Permissions:**

```powershell
# Via IIS Manager → Edit Permissions
# Grant IIS AppPool\[YourAppPoolName] Write access to:

/migenteenlinea2/api/logs/
/migenteenlinea2/api/wwwroot/uploads/
/migenteenlinea2/web/logs/
```

**Alternatively, contact myASP.NET support** to set permissions if you don't have direct access.

---

## ✅ Post-Deployment Verification

### Automated Verification Script

```powershell
cd "C:\Users\Ray\Documents\MiGenteEnlinea\MiGenteEnLinea.Clean"
.\verify-deployment.ps1
```

This checks:
- ✅ API health endpoint
- ✅ Swagger UI accessibility
- ✅ Web homepage
- ✅ Static files loading

### Manual Verification

#### 1. API Health Check
```bash
# In browser or curl:
https://api.migenteenlinea.com/health

# Expected response:
{
  "status": "Healthy",
  "timestamp": "2026-02-09T..."
}
```

#### 2. Swagger UI
```
https://api.migenteenlinea.com/

# Should display interactive API documentation
```

#### 3. Web Homepage
```
https://www.migenteenlinea.com/

# Should load landing page with MiGente branding
```

#### 4. Test Login Flow
- Navigate to `https://www.migenteenlinea.com/Auth/Login`
- Try registering a new account
- Verify activation email is sent
- Test login functionality

#### 5. Test Image Upload
- Login as Contratista
- Upload profile photo
- Verify image saves and displays

---

## 🔍 Troubleshooting

### ❌ Error: 502.5 - Process Failure

**Symptoms:**
- "HTTP Error 502.5 - Process Failure"
- "Failed to start application"

**Causes & Solutions:**

1. **ASP.NET Core Runtime not installed**
   ```
   Solution: Contact myASP.NET support
   Request: "Please install ASP.NET Core 8.0 Hosting Bundle on win8146.site4now.net"
   
   Verify on server:
   dotnet --list-runtimes
   # Should see: Microsoft.AspNetCore.App 8.0.x
   ```

2. **Application Pool misconfigured**
   ```
   Check: .NET CLR Version MUST be "No Managed Code"
   Check: Pipeline Mode = Integrated
   Fix: Reconfigure in IIS Manager
   ```

3. **web.config syntax error**
   ```
   Check: FTP to /migenteenlinea2/api/web.config
   Verify: No corruption during FTP transfer
   Fix: Re-upload web.config in binary mode
   ```

4. **Application crash on startup**
   ```
   Check logs: /migenteenlinea2/api/logs/stdout_*.log
   Look for: "The specified framework 'Microsoft.AspNetCore.App', version '8.0.0' was not found"
   ```

### ❌ Error: 500.19 - Configuration Error

**Cause:** Invalid web.config or missing IIS modules

**Solutions:**
1. Verify web.config XML is valid (no corruption)
2. Ensure URL Rewrite Module is installed (if using HTTPS redirect)
3. Check IIS application path is correct

### ❌ Error: Cannot connect to database

**Symptoms:**
- API health returns "Unhealthy"
- Endpoints return 500 errors
- Logs show SQL connection errors

**Check Connection String:**

```json
// appsettings.Production.json (on server)
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=SQL5106.site4now.net;Initial Catalog=db_a9f8ff_migentev3;User Id=db_a9f8ff_migentev3_admin;Password=Migente@1;..."
  }
}
```

**Test Database Connection:**
1. Download SQL Server Management Studio (SSMS)
2. Connect to: `SQL5106.site4now.net`
3. Login: `db_a9f8ff_migentev3_admin` / `Migente@1`
4. Verify you can access tables

**Firewall Issue:**
- Contact myASP.NET support if server IP is blocked
- Ensure SQL Server allows remote connections

### ❌ Error: 404 Not Found for API endpoints

**Symptoms:**
- `/health` returns 404
- API endpoints not found

**Causes:**
1. **IIS application not mapped correctly**
   ```
   Fix: Verify Virtual Directory /api → /migenteenlinea2/api
   ```

2. **Application pool stopped**
   ```
   Fix: Start application pool in IIS Manager
   ```

3. **Incorrect URL pattern**
   ```
   Wrong: https://api.migenteenlinea.com/health
   Right: https://api.migenteenlinea.com/api/health (if path-based)
   
   Or configure subdomain routing
   ```

### ❌ Images not displaying/uploading

**Symptoms:**
- Profile photos don't upload
- Existing images return 404

**Solutions:**

1. **Check folder permissions**
   ```
   Required: Write access to /migenteenlinea2/api/wwwroot/uploads/
   Contact myASP.NET support to grant IIS AppPool write permissions
   ```

2. **Verify StaticFilesBaseUrl configuration**
   ```json
   // Web appsettings.Production.json
   {
     "ApiConfiguration": {
       "StaticFilesBaseUrl": "https://api.migenteenlinea.com"
     }
   }
   ```

3. **Check CORS configuration**
   ```json
   // API appsettings.Production.json
   {
     "CorsConfiguration": {
       "AllowedOrigins": [
         "https://www.migenteenlinea.com",
         "https://migenteenlinea.com"
       ]
     }
   }
   ```

### ❌ Email not sending

**Symptoms:**
- Activation emails not received
- Password reset emails fail

**Check SMTP Configuration:**

```json
// API appsettings.Production.json
{
  "EmailSettings": {
    "SmtpServer": "mail.intdosystem.com",
    "SmtpPort": 465,
    "Username": "develop@intdosystem.com",
    "Password": "Anfeliz112322",
    "EnableSsl": true
  }
}
```

**Test SMTP:**
1. Try sending test email via API endpoint (if available)
2. Check firewall isn't blocking port 465
3. Verify SMTP credentials are correct
4. Check application logs for SMTP errors

---

## 🔄 Updating an Existing Deployment

### Quick Hotfix (Code changes only, no DB changes)

1. **Build and deploy:**
   ```powershell
   .\publish-and-deploy-ftp.ps1
   ```

2. **Restart application:** (Touch web.config or recycle app pool)
   - Option A: Via FTP, edit `web.config` (add/remove space, save)
   - Option B: myASP.NET Control Panel → Recycle Application Pool

### Database Migration Required

1. **Generate SQL migration script:**
   ```powershell
   cd MiGenteEnLinea.Clean
   dotnet ef migrations script --output migration.sql `
     --project src\Infrastructure\MiGenteEnLinea.Infrastructure
   ```

2. **Execute on production database:**
   - Connect to SQL5106.site4now.net via SSMS
   - Run migration.sql script
   - Verify no errors

3. **Deploy application code:**
   ```powershell
   .\publish-and-deploy-ftp.ps1
   ```

### Update Configuration Only (No code rebuild)

1. **Edit configuration locally:**
   - Modify `appsettings.Production.json` in `C:\Publish\MiGenteEnlinea\API\` or `\Web\`

2. **Upload via FTP:**
   - Option A: Use script: `.\publish-and-deploy-ftp.ps1 -SkipBuild`
   - Option B: Use FileZilla/WinSCP to upload single file

3. **Restart application** (touch web.config)

---

## 🔐 Security Considerations

### ⚠️ Important: Credentials in Scripts

**Current setup** has FTP credentials embedded in `publish-and-deploy-ftp.ps1` for simplicity.

**For production environments, consider:**

1. **Environment Variables:**
   ```powershell
   # Set once:
   setx FTP_USERNAME "rainiery" /M
   setx FTP_PASSWORD "Pevertiman00!" /M
   
   # Modify script to read:
   $FtpUsername = $env:FTP_USERNAME
   ```

2. **Windows Credential Manager**
3. **Azure Key Vault** (if migrating to cloud)

### 🔒 Connection String Security

**Current:** Plain-text in `appsettings.Production.json`

**Better approach:**
- Use IIS Application Settings (environment variables)
- Or Azure Key Vault integration

---

## 📊 Expected File Sizes

After deployment, expect these approximate sizes:

```
/migenteenlinea2/api/  → ~80-120 MB
  ├── DLL files        → ~60-90 MB
  ├── wwwroot/         → ~1-5 MB (grows with uploads)
  └── logs/            → ~0-50 MB (grows over time)

/migenteenlinea2/web/  → ~60-100 MB
  ├── DLL files        → ~40-70 MB
  ├── wwwroot/         → ~10-20 MB (CSS, JS, images)
  └── logs/            → ~0-50 MB
```

---

## 🆘 Getting Help

### myASP.NET Support
- **Support Portal:** https://www.myasp.net/support/
- **Ticket System:** Available in member control panel
- **Knowledge Base:** https://www.myasp.net/support/kb/root.aspx

### Common Support Requests

1. **".NET 8.0 Runtime Installation"**
   ```
   Subject: Please install ASP.NET Core 8.0 Hosting Bundle
   Details: Server win8146.site4now.net needs Microsoft.AspNetCore.App 8.0.x
   ```

2. **"Folder Permissions"**
   ```
   Subject: Need write permissions for application folders
   Details: 
   - /migenteenlinea2/api/logs/
   - /migenteenlinea2/api/wwwroot/uploads/
   - /migenteenlinea2/web/logs/
   Grant: IIS AppPool\[MyAppPoolName] Write access
   ```

3. **"IIS Configuration Help"**
   ```
   Subject: Assistance configuring ASP.NET Core 8.0 application
   Details: Need to configure applications at /api and /web paths
   ```

---

## 📋 Pre-Deployment Checklist

- [ ] **Code Review:** All changes tested locally
- [ ] **Database Backup:** Backup production database before migrations
- [ ] **Configuration Verified:**
  - [ ] Connection string is correct
  - [ ] JWT SecretKey is production-ready
  - [ ] SMTP settings are correct
  - [ ] CORS origins match your domains
- [ ] **Build Successful:** `.\publish-and-deploy-ftp.ps1 -SkipUpload` completes without errors
- [ ] **Maintenance Window:** Notify users if expecting downtime

## 📋 Post-Deployment Checklist

- [ ] **Health Check:** `/health` endpoint returns 200
- [ ] **Swagger UI:** API documentation loads
- [ ] **Homepage:** Website loads correctly
- [ ] **Login Flow:** Can register/login successfully
- [ ] **Database Connection:** API can query database
- [ ] **File Upload:** Images can be uploaded and displayed
- [ ] **Email Sending:** Test activation email
- [ ] **Logs:** Check for errors in stdout logs
- [ ] **Performance:** Response times are acceptable
- [ ] **SSL Certificate:** HTTPS works correctly

---

## 🚀 VS Code SFTP Extension (Alternative Method)

For quick file updates without full deployment:

### Setup (One-time)

1. **Install Extension:**
   - Press `Ctrl+Shift+X`
   - Search "SFTP" by Natizyskunk
   - Click Install

2. **Configuration files created:**
   - `.vscode/sftp-api.json` → API configuration
   - `.vscode/sftp-web.json` → Web configuration

### Usage

1. **Open Command Palette:** `Ctrl+Shift+P`
2. **Type:** `SFTP: Config`
3. **Select configuration** (api or web)
4. **Quick commands:**
   - `SFTP: Upload Folder` → Upload entire directory
   - `SFTP: Download File` → Download from server
   - `SFTP: Sync Local → Remote` → Sync changes

**⚠️ Caution:** 
- Configuration files contain credentials
- Already added to `.gitignore` (do not commit!)
- Use only for quick updates, not full deployments

---

## 📞 Next Steps

### After First Successful Deployment:

1. **Setup Monitoring:**
   - Configure health check monitoring (UptimeRobot, Pingdom)
   - Setup log aggregation if needed
   - Enable email alerts for critical errors

2. **Performance Optimization:**
   - Enable response compression in IIS
   - Configure output caching for static files
   - Monitor database query performance

3. **Backup Strategy:**
   - Schedule regular database backups
   - Keep last 3 deployment packages locally
   - Document rollback procedures

4. **Documentation:**
   - Document any server-specific configurations
   - Note any manual IIS settings made
   - Keep list of installed server components

---

**🎉 You're ready to deploy!**

Run `.\publish-and-deploy-ftp.ps1` and watch the magic happen! 🚀
