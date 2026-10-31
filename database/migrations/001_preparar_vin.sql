USE autotrack;

ALTER TABLE Vehiculo
    DROP INDEX Placa,
    ADD COLUMN Vin VARCHAR(17) NULL AFTER Id,
    MODIFY COLUMN ModeloId INT NULL,
    MODIFY COLUMN Placa VARCHAR(15) NULL,
    MODIFY COLUMN Anio SMALLINT NULL,
    MODIFY COLUMN TipoCombustible VARCHAR(50) NULL,
    ADD COLUMN MarcaDecodificada VARCHAR(100) NULL,
    ADD COLUMN ModeloDecodificado VARCHAR(100) NULL,
    ADD COLUMN FabricanteDecodificado VARCHAR(150) NULL,
    ADD COLUMN PaisFabricacion VARCHAR(100) NULL,
    ADD COLUMN TipoVehiculo VARCHAR(100) NULL,
    ADD COLUMN TipoCarroceria VARCHAR(100) NULL,
    ADD COLUMN EstadoDecodificacion VARCHAR(30) NOT NULL DEFAULT 'Pendiente',
    ADD COLUMN MensajeDecodificacion VARCHAR(500) NULL,
    ADD COLUMN FechaDecodificacion DATETIME NULL,
    ADD UNIQUE INDEX UQ_Vehiculo_Vin (Vin);
