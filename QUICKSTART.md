# SmartCafe Menu Service - Quick Start Guide

## ✅ What's Implemented

The Menu Service is **fully functional** with all core features:

### API Endpoints Available

**Menu Management (Complete Workflow)**
- `POST /api/cafes/{cafeId}/menus/upsert` - ⭐ **Main endpoint** - Create/update complete menu
- `GET /api/cafes/{cafeId}/menus` - List all menus for a cafe
- `GET /api/cafes/{cafeId}/menus/{menuId}` - Get menu details
- `DELETE /api/cafes/{cafeId}/menus/{menuId}` - Delete new menu + images
- `POST /api/cafes/{cafeId}/menus/{menuId}/publish` - Publish menu
- `POST /api/cafes/{cafeId}/menus/{menuId}/activate` - Activate menu
- `GET /api/cafes/{cafeId}/menus/active` - Get active menu (public)

**Image Management**
- `POST /api/images/upload` - Upload item image (returns full + cropped URLs)

### Architecture Features
- ✅ Clean Architecture with Vertical Slices
- ✅ EF Core 10 + PostgreSQL with optimized configurations
- ✅ Azure Blob Storage integration (folder structure: `{cafeId}/{menuId}/{itemId}/`)
- ✅ Azure Service Bus event publishing
- ✅ FluentValidation for all requests
- ✅ Automatic image cropping (300x300 thumbnails)
- ✅ Time-ordered GUIDs (Guid.CreateVersion7)
- ✅ IDateTimeProvider for testable time operations
- ✅ .NET Aspire for local development

## 🚀 Running the Application

### Prerequisites
- .NET 10 SDK (RC or later)
- Docker Desktop (for PostgreSQL and Azurite)

### Step 1: Start the Application with Aspire

```powershell
# From the repository root
dotnet run --project src/SmartCafe.Menu.AppHost/SmartCafe.Menu.AppHost.csproj
```

This will automatically start:
- ✅ PostgreSQL database (port 5432)
- ✅ pgAdmin (http://localhost:5050)
- ✅ Azurite (Azure Storage Emulator on port 10000)
- ✅ Menu API (http://localhost:5000)
- ✅ Aspire Dashboard (http://localhost:15888)

### Step 2: Apply Database Migrations

Open a new terminal:

```powershell
cd src/SmartCafe.Menu.Infrastructure
dotnet ef database update --startup-project ../SmartCafe.Menu.API
```

### Step 3: Access the Application

- **Swagger UI**: http://localhost:5000/swagger
- **Aspire Dashboard**: http://localhost:15888
- **API Base URL**: http://localhost:5000

## 📝 Usage Examples

### 1. Create a Cafe (First Time Setup)

Since menus belong to cafes, you'll need a cafe ID. You need to create a cafe first.

**Option A: Use a predefined GUID**
```
CafeId: 01234567-89ab-cdef-0123-456789abcdef
```

**Option B: Insert via SQL** (using pgAdmin at http://localhost:5050)
```sql
INSERT INTO "Cafes" ("Id", "Name", "CreatedAt", "UpdatedAt") 
VALUES (
    '01234567-89ab-cdef-0123-456789abcdef',
    'My Test Cafe',
    NOW(),
    NOW()
);
```

### 2. Upload Item Images (Before Creating Menu)

**Request**: `POST /api/images/upload`
- Content-Type: `multipart/form-data`
- Fields:
  - `cafeId`: Your cafe GUID
  - `menuId`: A new GUID for your menu (generate using any GUID tool)
  - `itemId`: A new GUID for the item (will use this in menu creation)
  - `file`: The image file (JPEG, PNG, or WebP, max 5MB)

**Response**:
```json
{
  "originalImageUrl": "https://.../cafeId/menuId/itemId/original.jpg",
  "thumbnailImageUrl": "https://.../cafeId/menuId/itemId/thumbnail.jpg"
}
```

> **Note**: The API stores relative paths in the database (e.g., `cafeId/menuId/itemId/original.jpg` and `cafeId/menuId/itemId/thumbnail.jpg`) for portability. Full URLs are constructed at runtime and returned in responses.

### 3. Create Complete Menu (Main Workflow)

**Request**: `POST /api/cafes/{cafeId}/menus/upsert`

```json
{
  "menuId": null,
  "name": "Summer Menu 2025",
  "description": "Fresh seasonal items",
  "sections": [
    {
      "sectionId": null,
      "name": "Breakfast",
      "description": "Morning favorites",
      "availableFrom": "06:00:00",
      "availableTo": "11:00:00",
      "displayOrder": 0,
      "items": [
        {
          "itemId": null,
          "name": "Avocado Toast",
          "description": "Smashed avocado on sourdough",
          "price": 12.50,
          "originalImageUrl": "https://.../original.jpg",
          "thumbnailImageUrl": "https://.../thumbnail.jpg",
          "isAvailable": true,
          "ingredients": [
            {
              "name": "Avocado",
              "canBeExcluded": false,
              "canBeIncluded": false
            },
            {
              "name": "Chili Flakes",
              "canBeExcluded": true,
              "canBeIncluded": true
            }
          ]
        }
      ]
    }
  ]
}
```

**Response**: Returns menu ID and metadata

### 4. Update Existing Menu

Same endpoint, but include the `menuId`:

```json
{
  "menuId": "your-existing-menu-id",
  "name": "Updated Summer Menu",
  ...
}
```

### 5. Publish Menu (Make it Activatable)

**Request**: `POST /api/cafes/{cafeId}/menus/{menuId}/publish`

### 6. Activate Menu (Go Live)

**Request**: `POST /api/cafes/{cafeId}/menus/{menuId}/activate`

This will:
- Deactivate the currently active menu (if any)
- Activate the new menu
- Publish events for both operations

### 7. Get Active Menu (Public Endpoint)

**Request**: `GET /api/cafes/{cafeId}/menus/active`

Returns the currently active menu with all sections and items.

## 🔍 Testing with Swagger

1. Open http://localhost:5000/swagger
2. Use the "Try it out" button on each endpoint
3. All endpoints are documented with request/response schemas

## 🗃️ Database Access

**pgAdmin**: http://localhost:5050

Connection details (auto-configured by Aspire):
- Host: localhost
- Port: 5432
- Database: menudb
- Username: postgres
- Password: (check Aspire Dashboard)

## 📊 Monitoring with Aspire Dashboard

Open http://localhost:15888 to see:
- Service status and health
- Traces and logs
- Metrics
- Resource consumption

## 🔧 Troubleshooting

### Database Migration Fails
```powershell
# Remove existing migrations and recreate
cd src/SmartCafe.Menu.Infrastructure
Remove-Item -Recurse Migrations
dotnet ef migrations add InitialCreate --startup-project ../SmartCafe.Menu.API
dotnet ef database update --startup-project ../SmartCafe.Menu.API
```

### Port Already in Use
Stop any existing instances:
```powershell
# Find process using port 5000
netstat -ano | findstr :5000

# Kill the process (replace PID with actual process ID)
taskkill /PID <PID> /F
```

### Docker Not Running
Make sure Docker Desktop is running before starting the AppHost.

## 🎯 Next Steps

After the application is running, you can:

1. **Test the complete workflow**:
   - Upload images
   - Create a new menu
   - Publish it
   - Activate it
   - Retrieve active menu

2. **Explore optional features** (see README.md TODO section):
   - Implement CloneMenu endpoint
   - Add orphan image cleanup service
   - Implement caching layer
   - Add menu versioning

3. **Production deployment**:
   - Configure Azure resources (PostgreSQL, Blob Storage, Service Bus)
   - Set up Key Vault for secrets
   - Deploy to Azure Container Apps or AKS

## 📚 Additional Documentation

- Full documentation: [README.md](README.md)
- Architecture decisions: `.github/copilot-instructions.md`

---

**Need help?** Check the Swagger UI at http://localhost:5000/swagger for interactive API documentation.
