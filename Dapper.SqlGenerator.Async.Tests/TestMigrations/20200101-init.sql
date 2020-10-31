CREATE TABLE Migrations (Name TEXT, Date TEXT);

CREATE TABLE TestProducts (
  "Id" INTEGER PRIMARY KEY,
  "Type" INTEGER NOT NULL,
  "Name" TEXT NULL,
  "Content" TEXT NOT NULL,
  "Value" INTEGER NOT NULL,
  "Enum" TEXT NULL,
  "MaybeDate" TEXT NULL,
  "Date" TEXT NOT NULL,
  "MaybeGuid" BLOB NULL,
  "Guid" BLOB NULL,
  "Duration" TEXT NULL,
  "Last" INTEGER NOT NULL
);

CREATE TABLE TestOrders (
  "OrderId" INTEGER PRIMARY KEY,
  "ProductId" INTEGER NOT NULL,
  "Count" INTEGER NOT NULL
);
