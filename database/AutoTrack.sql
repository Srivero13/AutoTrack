CREATE DATABASE IF NOT EXISTS autotrack CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE autotrack;

CREATE TABLE IF NOT EXISTS Marca (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(80) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS Modelo (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    MarcaId INT NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    CONSTRAINT UQ_Modelo_Marca_Nombre UNIQUE (MarcaId, Nombre),
    CONSTRAINT FK_Modelo_Marca FOREIGN KEY (MarcaId) REFERENCES Marca(Id)
);

CREATE TABLE IF NOT EXISTS Vehiculo (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Vin VARCHAR(17) NOT NULL,
    ModeloId INT NULL,
    Placa VARCHAR(15) NULL,
    Anio SMALLINT NULL,
    KilometrajeActual INT NOT NULL DEFAULT 0,
    TipoCombustible VARCHAR(50) NULL,
    MarcaDecodificada VARCHAR(100) NULL,
    ModeloDecodificado VARCHAR(100) NULL,
    FabricanteDecodificado VARCHAR(150) NULL,
    PaisFabricacion VARCHAR(100) NULL,
    TipoVehiculo VARCHAR(100) NULL,
    TipoCarroceria VARCHAR(100) NULL,
    EstadoDecodificacion VARCHAR(30) NOT NULL DEFAULT 'Pendiente',
    MensajeDecodificacion VARCHAR(500) NULL,
    FechaDecodificacion DATETIME NULL,
    Activo BOOLEAN NOT NULL DEFAULT TRUE,
    FechaRegistro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT UQ_Vehiculo_Vin UNIQUE (Vin),
    CONSTRAINT FK_Vehiculo_Modelo FOREIGN KEY (ModeloId) REFERENCES Modelo(Id),
    CONSTRAINT CK_Vehiculo_Kilometraje CHECK (KilometrajeActual >= 0)
);

CREATE TABLE IF NOT EXISTS CargaCombustible (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    VehiculoId INT NOT NULL,
    Fecha DATETIME NOT NULL,
    Kilometraje INT NOT NULL,
    Litros DECIMAL(10,2) NOT NULL,
    PrecioPorLitro DECIMAL(10,2) NOT NULL,
    Total DECIMAL(12,2) NOT NULL,
    CONSTRAINT FK_Carga_Vehiculo FOREIGN KEY (VehiculoId) REFERENCES Vehiculo(Id),
    CONSTRAINT CK_Carga_Kilometraje CHECK (Kilometraje >= 0),
    CONSTRAINT CK_Carga_Litros CHECK (Litros > 0),
    CONSTRAINT CK_Carga_Precio CHECK (PrecioPorLitro > 0)
);

INSERT IGNORE INTO Marca (Id, Nombre) VALUES
(1, 'Jetour'),
(2, 'Toyota'),
(3, 'Mitsubishi');

INSERT IGNORE INTO Modelo (Id, MarcaId, Nombre) VALUES
(1, 1, 'X70 Plus'),
(2, 2, 'Corolla'),
(3, 3, 'Lancer');
