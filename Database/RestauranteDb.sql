IF DB_ID('RestauranteDb') IS NULL
BEGIN
    CREATE DATABASE RestauranteDb;
END
GO

USE RestauranteDb;
GO

IF OBJECT_ID('dbo.PedidoDetalle', 'U') IS NOT NULL DROP TABLE dbo.PedidoDetalle;
IF OBJECT_ID('dbo.Pedido', 'U') IS NOT NULL DROP TABLE dbo.Pedido;
IF OBJECT_ID('dbo.MovimientoStock', 'U') IS NOT NULL DROP TABLE dbo.MovimientoStock;
IF OBJECT_ID('dbo.Plato', 'U') IS NOT NULL DROP TABLE dbo.Plato;
IF OBJECT_ID('dbo.TipoPlato', 'U') IS NOT NULL DROP TABLE dbo.TipoPlato;
IF OBJECT_ID('dbo.Cliente', 'U') IS NOT NULL DROP TABLE dbo.Cliente;
IF OBJECT_ID('dbo.ConfiguracionFacturacion', 'U') IS NOT NULL DROP TABLE dbo.ConfiguracionFacturacion;
GO

CREATE TABLE dbo.Cliente
(
    IdCliente INT IDENTITY(1,1) NOT NULL,
    Cedula VARCHAR(20) NULL,
    Nombres VARCHAR(100) NOT NULL,
    Apellidos VARCHAR(100) NOT NULL,
    Telefono VARCHAR(20) NULL,
    Email VARCHAR(120) NULL,
    Direccion VARCHAR(200) NULL,
    Activo BIT NOT NULL CONSTRAINT DF_Cliente_Activo DEFAULT (1),
    FechaRegistro DATETIME NOT NULL CONSTRAINT DF_Cliente_FechaRegistro DEFAULT (GETDATE()),
    CONSTRAINT PK_Cliente PRIMARY KEY (IdCliente),
    CONSTRAINT UQ_Cliente_Cedula UNIQUE (Cedula),
    CONSTRAINT CK_Cliente_Nombres CHECK (LEN(LTRIM(RTRIM(Nombres))) > 0),
    CONSTRAINT CK_Cliente_Apellidos CHECK (LEN(LTRIM(RTRIM(Apellidos))) > 0)
);
GO

CREATE TABLE dbo.TipoPlato
(
    IdTipoPlato INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(50) NOT NULL,
    Descripcion VARCHAR(200) NULL,
    Activo BIT NOT NULL CONSTRAINT DF_TipoPlato_Activo DEFAULT (1),
    CONSTRAINT PK_TipoPlato PRIMARY KEY (IdTipoPlato),
    CONSTRAINT UQ_TipoPlato_Nombre UNIQUE (Nombre),
    CONSTRAINT CK_TipoPlato_Nombre CHECK (LEN(LTRIM(RTRIM(Nombre))) > 0)
);
GO

CREATE TABLE dbo.Plato
(
    IdPlato INT IDENTITY(1,1) NOT NULL,
    IdTipoPlato INT NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(500) NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL CONSTRAINT DF_Plato_Stock DEFAULT (0),
    Imagen VARCHAR(200) NULL,
    Activo BIT NOT NULL CONSTRAINT DF_Plato_Activo DEFAULT (1),
    FechaRegistro DATETIME NOT NULL CONSTRAINT DF_Plato_FechaRegistro DEFAULT (GETDATE()),
    CONSTRAINT PK_Plato PRIMARY KEY (IdPlato),
    CONSTRAINT FK_Plato_TipoPlato FOREIGN KEY (IdTipoPlato) REFERENCES dbo.TipoPlato(IdTipoPlato),
    CONSTRAINT UQ_Plato_Nombre UNIQUE (Nombre),
    CONSTRAINT CK_Plato_Nombre CHECK (LEN(LTRIM(RTRIM(Nombre))) > 0),
    CONSTRAINT CK_Plato_Precio CHECK (Precio >= 0),
    CONSTRAINT CK_Plato_Stock CHECK (Stock >= 0)
);
GO

CREATE TABLE dbo.ConfiguracionFacturacion
(
    IdConfiguracion INT IDENTITY(1,1) NOT NULL,
    IvaPorcentaje DECIMAL(5,2) NOT NULL CONSTRAINT DF_ConfiguracionFacturacion_Iva DEFAULT (12.00),
    ServicioPorcentaje DECIMAL(5,2) NOT NULL CONSTRAINT DF_ConfiguracionFacturacion_Servicio DEFAULT (10.00),
    Activo BIT NOT NULL CONSTRAINT DF_ConfiguracionFacturacion_Activo DEFAULT (1),
    FechaRegistro DATETIME NOT NULL CONSTRAINT DF_ConfiguracionFacturacion_FechaRegistro DEFAULT (GETDATE()),
    CONSTRAINT PK_ConfiguracionFacturacion PRIMARY KEY (IdConfiguracion),
    CONSTRAINT CK_ConfiguracionFacturacion_Iva CHECK (IvaPorcentaje >= 0),
    CONSTRAINT CK_ConfiguracionFacturacion_Servicio CHECK (ServicioPorcentaje >= 0)
);
GO

CREATE TABLE dbo.Pedido
(
    IdPedido INT IDENTITY(1,1) NOT NULL,
    IdCliente INT NOT NULL,
    FechaPedido DATETIME NOT NULL CONSTRAINT DF_Pedido_FechaPedido DEFAULT (GETDATE()),
    Estado VARCHAR(20) NOT NULL CONSTRAINT DF_Pedido_Estado DEFAULT ('Pendiente'),
    Subtotal DECIMAL(10,2) NOT NULL CONSTRAINT DF_Pedido_Subtotal DEFAULT (0),
    IvaPorcentaje DECIMAL(5,2) NOT NULL CONSTRAINT DF_Pedido_IvaPorcentaje DEFAULT (12.00),
    IvaValor DECIMAL(10,2) NOT NULL CONSTRAINT DF_Pedido_IvaValor DEFAULT (0),
    ServicioPorcentaje DECIMAL(5,2) NOT NULL CONSTRAINT DF_Pedido_ServicioPorcentaje DEFAULT (10.00),
    ServicioValor DECIMAL(10,2) NOT NULL CONSTRAINT DF_Pedido_ServicioValor DEFAULT (0),
    Total DECIMAL(10,2) NOT NULL CONSTRAINT DF_Pedido_Total DEFAULT (0),
    CONSTRAINT PK_Pedido PRIMARY KEY (IdPedido),
    CONSTRAINT FK_Pedido_Cliente FOREIGN KEY (IdCliente) REFERENCES dbo.Cliente(IdCliente),
    CONSTRAINT CK_Pedido_Estado CHECK (Estado IN ('Pendiente', 'Pagado', 'Anulado')),
    CONSTRAINT CK_Pedido_Valores CHECK (Subtotal >= 0 AND IvaValor >= 0 AND ServicioValor >= 0 AND Total >= 0)
);
GO

CREATE TABLE dbo.PedidoDetalle
(
    IdPedidoDetalle INT IDENTITY(1,1) NOT NULL,
    IdPedido INT NOT NULL,
    IdPlato INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,
    CONSTRAINT PK_PedidoDetalle PRIMARY KEY (IdPedidoDetalle),
    CONSTRAINT FK_PedidoDetalle_Pedido FOREIGN KEY (IdPedido) REFERENCES dbo.Pedido(IdPedido),
    CONSTRAINT FK_PedidoDetalle_Plato FOREIGN KEY (IdPlato) REFERENCES dbo.Plato(IdPlato),
    CONSTRAINT CK_PedidoDetalle_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CK_PedidoDetalle_Valores CHECK (PrecioUnitario >= 0 AND Subtotal >= 0)
);
GO

CREATE TABLE dbo.MovimientoStock
(
    IdMovimientoStock INT IDENTITY(1,1) NOT NULL,
    IdPlato INT NOT NULL,
    IdPedido INT NULL,
    TipoMovimiento VARCHAR(20) NOT NULL,
    Cantidad INT NOT NULL,
    StockAnterior INT NOT NULL,
    StockNuevo INT NOT NULL,
    FechaMovimiento DATETIME NOT NULL CONSTRAINT DF_MovimientoStock_Fecha DEFAULT (GETDATE()),
    Observacion VARCHAR(200) NULL,
    CONSTRAINT PK_MovimientoStock PRIMARY KEY (IdMovimientoStock),
    CONSTRAINT FK_MovimientoStock_Plato FOREIGN KEY (IdPlato) REFERENCES dbo.Plato(IdPlato),
    CONSTRAINT FK_MovimientoStock_Pedido FOREIGN KEY (IdPedido) REFERENCES dbo.Pedido(IdPedido),
    CONSTRAINT CK_MovimientoStock_Tipo CHECK (TipoMovimiento IN ('Entrada', 'Salida', 'Ajuste')),
    CONSTRAINT CK_MovimientoStock_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CK_MovimientoStock_Stock CHECK (StockAnterior >= 0 AND StockNuevo >= 0)
);
GO

CREATE INDEX IX_Plato_IdTipoPlato ON dbo.Plato(IdTipoPlato);
CREATE INDEX IX_Plato_Activo ON dbo.Plato(Activo);
CREATE INDEX IX_Pedido_IdCliente ON dbo.Pedido(IdCliente);
CREATE INDEX IX_Pedido_FechaPedido ON dbo.Pedido(FechaPedido);
CREATE INDEX IX_PedidoDetalle_IdPedido ON dbo.PedidoDetalle(IdPedido);
CREATE INDEX IX_PedidoDetalle_IdPlato ON dbo.PedidoDetalle(IdPlato);
CREATE INDEX IX_MovimientoStock_IdPlato ON dbo.MovimientoStock(IdPlato);
GO

INSERT INTO dbo.TipoPlato (Nombre, Descripcion)
VALUES
('Entrada', 'Platos pequenos para iniciar el pedido'),
('Sopa', 'Sopas disponibles'),
('Plato fuerte', 'Platos principales'),
('Postre', 'Postres disponibles'),
('Bebida', 'Bebidas frias o calientes');
GO

INSERT INTO dbo.ConfiguracionFacturacion (IvaPorcentaje, ServicioPorcentaje)
VALUES (12.00, 10.00);
GO

INSERT INTO dbo.Plato (IdTipoPlato, Nombre, Descripcion, Precio, Stock, Imagen)
VALUES
((SELECT IdTipoPlato FROM dbo.TipoPlato WHERE Nombre = 'Entrada'), 'Empanada de Morocho', 'Empanada tradicional ecuatoriana rellena de carne y vegetales.', 2.50, 20, 'empanada-de-morocho.jpg'),
((SELECT IdTipoPlato FROM dbo.TipoPlato WHERE Nombre = 'Entrada'), 'Bolon de Verde', 'Bolon de verde con queso o chicharron.', 3.00, 20, 'bolón-de-verde.jpg'),
((SELECT IdTipoPlato FROM dbo.TipoPlato WHERE Nombre = 'Sopa'), 'Caldo de Bolas de Verde', 'Sopa tradicional con bolas de verde rellenas.', 4.50, 15, 'caldo-bolas-verde.jpg'),
((SELECT IdTipoPlato FROM dbo.TipoPlato WHERE Nombre = 'Sopa'), 'Caldo de Gallina', 'Caldo de gallina criolla con papa y hierbas.', 4.00, 15, 'caldo-de-gallina.jpg'),
((SELECT IdTipoPlato FROM dbo.TipoPlato WHERE Nombre = 'Plato fuerte'), 'Arroz Marinero', 'Arroz preparado con mariscos y especias.', 8.50, 12, 'arroz-marinero.jpg'),
((SELECT IdTipoPlato FROM dbo.TipoPlato WHERE Nombre = 'Plato fuerte'), 'Churrasco', 'Carne a la plancha acompanada de guarniciones.', 6.00, 12, 'churrasco.jpg'),
((SELECT IdTipoPlato FROM dbo.TipoPlato WHERE Nombre = 'Postre'), 'Dulce de Higos', 'Postre tradicional de higos en almibar.', 2.00, 18, 'dulce-de-higos.jpg'),
((SELECT IdTipoPlato FROM dbo.TipoPlato WHERE Nombre = 'Postre'), 'Espumilla', 'Postre ecuatoriano batido con pulpa de fruta.', 1.50, 18, 'espumilla.jpg'),
((SELECT IdTipoPlato FROM dbo.TipoPlato WHERE Nombre = 'Bebida'), 'Jugo Natural', 'Jugo natural del dia.', 1.75, 30, NULL),
((SELECT IdTipoPlato FROM dbo.TipoPlato WHERE Nombre = 'Bebida'), 'Gaseosa', 'Bebida gaseosa personal.', 1.50, 30, NULL);
GO
