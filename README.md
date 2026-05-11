# 🛍️ Retail POS + ERP Integration

A full-stack prototype demonstrating offline-capable POS sale entry with central ERP data synchronization.

## 🚀 Quick Start

### Prerequisites
- [.NET 8 SDK]
- [Node.js 18+]
- [Angular CLI] 
- [EF Core CLI]

### Backend
```bash
cd RetailERP.API
dotnet restore
dotnet ef database update
dotnet run
# API     → http://localhost:7229
# Swagger → http://localhost:7229/swagger
```

### Frontend
```bash
cd retail-erp-ui
npm install
ng serve --open
# App → http://localhost:4200
```

---

## 🧰 Tech Stack

| | Technology |
|---|---|
| **Backend** | ASP.NET Core 8 Web API |
| **Database** | SQLite + Entity Framework Core 8 |
| **Frontend** | Angular 19 (Standalone Components) |
| **Styling** | SCSS |
| **API Docs** | Swagger / OpenAPI |

---

## 📋 API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/products` | List all products |
| `POST` | `/api/products` | Create product |
| `GET` | `/api/products/barcode/{barcode}` | Barcode lookup |
| `POST` | `/api/sales` | Create POS sale |
| `GET` | `/api/sales/unsynced` | Get unsynced sales |
| `POST` | `/api/sync/sync-sales` | Sync batch to server |
| `POST` | `/api/sync/trigger` | Trigger local sync |

---

## 🔄 How Sync Works

```
POS Sale Created → Status: Pending
       ↓
Click "Sync Now" → Fetches all Pending/Failed sales
       ↓
POST /api/sync/sync-sales (batch payload)
       ↓
Server checks UniqueSaleId → Already exists? Skip (duplicate)
                           → New? Save + reduce stock
       ↓
Status updated → Synced ✓
```

**Retry logic** — retries up to 3 times with exponential backoff (2s → 4s → 6s).  
**Idempotency** — `UniqueSaleId` unique index prevents duplicate records on re-sync.  
**Status lifecycle** — `Pending` → `Synced` / `Failed` (with error message + attempt count).

---

## 🏗️ Project Structure

```
RetailERP.API/
├── Controllers/        # HTTP layer (thin, no logic)
├── Services/           # Business logic
├── Repositories/       # Data access (EF Core)
├── Models/             # Domain entities
├── DTOs/               # Request & Response shapes
└── Middleware/         # Global exception handling

retail-erp-ui/src/app/
├── core/
│   ├── models/         # TypeScript interfaces
│   └── services/       # HttpClient services
└── features/
    ├── dashboard/      # Stats overview
    ├── products/       # Product list + barcode lookup
    ├── sales/          # POS cart + checkout
    └── sync/           # Sync panel + results
```

---

## 💡 Design Decisions

- **SQLite** — zero-config file-based DB, perfect for offline POS simulation
- **Repository pattern** — swappable data layer (SQLite → SQL Server with no service changes)
- **DTOs** — API never exposes raw EF entities; controls exactly what goes in and out
- **UniqueSaleId** — client-generated GUID-based key for idempotent sync
- **Global exception middleware** — consistent JSON error responses across all endpoints

---

## ⚠️ Limitations

- No authentication / authorization on API endpoints
- Single store (`STORE-01`) — no multi-tenant support
- Sync is manually triggered (no background auto-sync)
- No conflict resolution for concurrent offline stock deductions
- No pagination on sales/products endpoints

---

## 🌱 Seed Data

4 products are seeded automatically on first run:

| Product | Barcode | Price | Stock |
|---|---|---|---|
| T-Shirt (M) | 8901234567890 | ৳499 | 100 |
| Jeans (32) | 8901234567891 | ৳1,299 | 50 |
| Polo Shirt (L) | 8901234567892 | ৳799 | 75 |
| Jacket (XL) | 8901234567893 | ৳2,499 | 30 |
