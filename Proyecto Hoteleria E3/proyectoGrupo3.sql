-- Comando para poder ver si la base de datos existe
SELECT name 
FROM sys.databases 
WHERE name = 'hoteleriaGrupo3';

-- Eliminar base de datos si es que ya existe
USE master; -- Utilizamos otra base de datos en caso de que ya usemos la que vamos a eliminar, de lo contrario dará error


-- Crear base de datos
CREATE DATABASE hoteleriaGrupo3;

-- Usar la base de datos
USE hoteleriaGrupo3;

-- Sección de creación de tablas

-- Hotel
CREATE TABLE hotel (
    hotel_id INT IDENTITY(1,1) NOT NULL,
    nombre VARCHAR(50) NOT NULL,
    descripcion VARCHAR(150),
    ubicacion VARCHAR(150) NOT NULL,
    PRIMARY KEY (hotel_id)
);

SELECT * FROM hotel;

-- Cliente
CREATE TABLE cliente (
    cliente_dpi BIGINT NOT NULL,
    nombres VARCHAR(50) NOT NULL,
    apellidos VARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL,
    telefono VARCHAR(8) NOT NULL,
    PRIMARY KEY (cliente_dpi)
);

SELECT * FROM cliente;

-- Empleado
CREATE TABLE empleado (
    empleado_id INT IDENTITY(1,1) NOT NULL,
    nombres VARCHAR(50) NOT NULL,
    apellidos VARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL,
    telefono VARCHAR(8) NOT NULL,
    fecha_nacimiento DATE NOT NULL,
    hotel_id INT NOT NULL,
    PRIMARY KEY (empleado_id),
    CONSTRAINT fk_empleado_hotel
        FOREIGN KEY (hotel_id)
            REFERENCES hotel(hotel_id)
);

INSERT INTO empleado (nombres, apellidos, email, telefono, fecha_nacimiento, hotel_id) 
VALUES ('Julio', 'Jaramillo', 'jjaramillo@gmail.com', '12345678', '2025-02-22', 1);

SELECT * FROM empleado;

-- Tipo Habitación
CREATE TABLE tipo_habitacion (
    tipo_habitacion_id INT IDENTITY(1,1) NOT NULL,
    tipo VARCHAR(50) NOT NULL,
    descripcion VARCHAR(150),
    PRIMARY KEY (tipo_habitacion_id)
);

INSERT INTO tipo_habitacion (tipo, descripcion) 
VALUES ('Individual', 'Habitación espaciosa con 3 cuartos y 2 baños completos');

INSERT INTO tipo_habitacion (tipo, descripcion) 
VALUES ('Doble', 'Habitación espaciosa con 2 cuartos y 1 baño completo');

INSERT INTO tipo_habitacion (tipo, descripcion) 
VALUES ('Suite', 'Habitación espaciosa con 1 cuarto y 2 baños completos');

SELECT * FROM tipo_habitacion;

-- Usuario
CREATE TABLE usuario (
    usuario_id INT IDENTITY(1,1) NOT NULL,
    username VARCHAR(25) NOT NULL,
    password VARBINARY(256) NOT NULL,
    empleado_id INT NOT NULL,
    PRIMARY KEY (usuario_id),
    CONSTRAINT fk_usuario_empleado 
        FOREIGN KEY (empleado_id) 
            REFERENCES empleado(empleado_id) 
);

INSERT INTO usuario (username, password, empleado_id) 
VALUES ('jjaramillo', HASHBYTES('SHA2_256', 'julio12'), 1);

SELECT * FROM usuario;

-- Habitación
CREATE TABLE habitacion (
    habitacion_id INT IDENTITY(1,1) NOT NULL,
    disponibilidad VARCHAR(2) NOT NULL,
    CONSTRAINT chk_disponibilidad CHECK (disponibilidad IN ('Sí', 'No')),
    hotel_id INT NOT NULL,
    tipo_habitacion_id INT NOT NULL,
    tipo_habitacion VARCHAR(50),
    descripcion VARCHAR(150),
    PRIMARY KEY (habitacion_id),
    CONSTRAINT fk_habitacion_tipo_habitacion
        FOREIGN KEY (tipo_habitacion_id)
            REFERENCES tipo_habitacion(tipo_habitacion_id),
    CONSTRAINT fk_habitacion_hotel
        FOREIGN KEY (hotel_id)
            REFERENCES hotel(hotel_id)
);

INSERT INTO habitacion (disponibilidad, hotel_id, tipo_habitacion_id) 
VALUES ('Sí', 1, 1);

SELECT * FROM habitacion;

-- Reservaciones
CREATE TABLE reservacion_encabezado (
    reservacion_encabezado_id INT IDENTITY(1,1) NOT NULL,
    cliente_id INT NOT NULL,
    empleado_id INT NOT NULL,
    hotel_id INT NOT NULL,
    costo DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (reservacion_encabezado_id)
);

SELECT * FROM reservacion_encabezado;

-- Reservaciones Detalle
CREATE TABLE reservacion_detalle (
    reservacion_detalle_id INT IDENTITY(1,1) NOT NULL,
    reservacion_encabezado_id INT NOT NULL,
    fecha_inicio DATE NOT NULL,
    fecha_fin DATE NOT NULL,
    PRIMARY KEY (reservacion_detalle_id),
    CONSTRAINT fk_reservacion_detalle_reservacion_encabezado
        FOREIGN KEY (reservacion_encabezado_id)
            REFERENCES reservacion_encabezado(reservacion_encabezado_id)
);

-- Sección de funciones tipo "Trigger"
CREATE TABLE reservacion_habitacion (
    reservacion_habitacion_id INT IDENTITY(1,1) NOT NULL,
    reservacion_detalle_id INT NOT NULL,
    habitacion_id INT NOT NULL,
    PRIMARY KEY (reservacion_habitacion_id),
    CONSTRAINT fk_reservacion_habitacion_detalle
        FOREIGN KEY (reservacion_detalle_id)
            REFERENCES reservacion_detalle(reservacion_detalle_id),
    CONSTRAINT fk_reservacion_habitacion_habitacion
        FOREIGN KEY (habitacion_id)
            REFERENCES habitacion(habitacion_id)
);

-- Trigger en la tabla habitación luego de insertar un registro
CREATE TRIGGER trigger_habitacion_after_insert
ON habitacion
AFTER INSERT
AS
BEGIN
    UPDATE a
    SET a.tipo_habitacion = b.tipo, 
        a.descripcion = b.descripcion
    FROM habitacion a
    INNER JOIN tipo_habitacion b ON b.tipo_habitacion_id = a.tipo_habitacion_id
    INNER JOIN inserted i ON i.habitacion_id = a.habitacion_id; -- "inserted" para aplicar solo en los nuevos registros
END;
