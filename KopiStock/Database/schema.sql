-- KopiStock ETS PBKK | SQLite | single warehouse (Gudang Utama)
-- All stock movements use integer grams. Never edit a saved movement.
PRAGMA foreign_keys = ON;
CREATE TABLE IF NOT EXISTS Users (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  username TEXT NOT NULL UNIQUE COLLATE NOCASE,
  password_hash TEXT NOT NULL,
  display_name TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS CoffeeTypes (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL UNIQUE COLLATE NOCASE,
  status INTEGER NOT NULL DEFAULT 1 CHECK (status IN (0,1))
);
CREATE TABLE IF NOT EXISTS Suppliers (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL,
  contact TEXT NOT NULL DEFAULT '',
  status INTEGER NOT NULL DEFAULT 1 CHECK (status IN (0,1))
);
CREATE TABLE IF NOT EXISTS CoffeeLots (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  code TEXT NOT NULL UNIQUE COLLATE NOCASE,
  coffee_type_id INTEGER NOT NULL REFERENCES CoffeeTypes(id) ON DELETE RESTRICT,
  supplier_id INTEGER NOT NULL REFERENCES Suppliers(id) ON DELETE RESTRICT,
  grade TEXT NOT NULL,
  origin TEXT NOT NULL,
  received_at TEXT NOT NULL,
  CHECK (length(trim(code)) > 0),
  CHECK (length(trim(grade)) > 0),
  CHECK (length(trim(origin)) > 0)
);
CREATE TABLE IF NOT EXISTS StockMovements (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  lot_id INTEGER NOT NULL REFERENCES CoffeeLots(id) ON DELETE RESTRICT,
  type TEXT NOT NULL CHECK (type IN ('IN','OUT')),
  quantity_g INTEGER NOT NULL CHECK (quantity_g > 0),
  happened_at TEXT NOT NULL, -- tanggal transaksi sesuai pilihan pada formulir
  created_at TEXT, -- tanggal dan jam pencatatan aktual, NULL untuk riwayat lama
  destination TEXT NOT NULL DEFAULT '',
  user_id INTEGER NOT NULL REFERENCES Users(id) ON DELETE RESTRICT
);
-- P0: each new lot receives one initial IN. Future receipts create new lots.
CREATE UNIQUE INDEX IF NOT EXISTS UX_InitialInPerLot ON StockMovements(lot_id) WHERE type='IN';
CREATE INDEX IF NOT EXISTS IX_MovementsLot ON StockMovements(lot_id, id);
CREATE INDEX IF NOT EXISTS IX_MovementsDate ON StockMovements(happened_at, id);
