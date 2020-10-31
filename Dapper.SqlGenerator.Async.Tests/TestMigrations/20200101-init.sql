CREATE TABLE Migrations (Name TEXT, Date TEXT);

CREATE TABLE TestProducts (
  "id" INT PRIMARY KEY,
  "Type" INT NOT NULL,
  "Name" TEXT NULL,
  "Content" TEXT NOT NULL,
  "Value" INT NOT NULL,
  "Enum" TEXT NULL,
  "MaybeDate" TEXT NULL,
  "Date" TEXT NOT NULL,
  "MaybeGuid" BLOB NULL,
  "Guid" BLOB NULL,
  "Duration" TEXT NULL,
  "Last" INT NOT NULL
);

CREATE TABLE TestOrders (
  "OrderId" INT PRIMARY KEY NOT NULL,
  "ProductId" INT NOT NULL,
  "Count" INT NOT NULL
);
