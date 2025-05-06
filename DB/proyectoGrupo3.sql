-- Comando para poder ver si la base de datos existe
SELECT name 
FROM sys.databases 
WHERE name = 'hoteleriaGrupo3';

-- Eliminar base de datos si es que ya existe
use master;
drop database if exists hoteleriaGrupo3;

-- Crear base de datos
create database hoteleriaGrupo3;
use hoteleriaGrupo3;

-- Hotel
create table hotel(
    hotel_id int identity(1,1) not null,
    nombre varchar(50) not null,
    descripcion varchar(150),
    ubicacion varchar(150) not null,
    primary key (hotel_id)
);

-- Cliente
create table cliente(
    cliente_dpi bigint not null,
    nombres varchar(50) not null,
    apellidos varchar(50) not null,
    email varchar(100) not null,
    telefono varchar(8) not null,
    primary key (cliente_dpi)
);

-- Rol del empleado
create table rol(
	rol_id int identity(1,1) not null,
	tipo_rol varchar(50) not null,
	primary key (rol_id)

);

-- Empleado
create table empleado(
    empleado_id int identity(1,1) not null,
    nombres varchar(50) not null,
    apellidos varchar(50) not null,
    email varchar(100) not null,
    telefono varchar(8) not null,
    fecha_nacimiento date not null,
    hotel_id int not null,
    primary key (empleado_id),
    constraint fk_empleado_hotel
        foreign key (hotel_id)
            references hotel(hotel_id)
);

-- Tipo Habitaci�n
create table tipo_habitacion(
    tipo_habitacion_id int identity(1,1) not null,
    tipo varchar(50) not null,
    descripcion varchar(150),
    primary key (tipo_habitacion_id)
);

-- Usuario
create table usuario(
    usuario_id int identity(1,1) not null,
    username varchar(25) not null,
    --password varbinary(64) not null,  -- Se cambi� a 64 para SHA-256
	password varchar(50) not null,
    empleado_id int not null,
	rol_id int not null,
    primary key (usuario_id),
    constraint fk_usuario_empleado 
        foreign key (empleado_id) 
            references empleado(empleado_id),
	constraint fk_empleado_rol
        foreign key (rol_id)
            references rol(rol_id)
);

-- Habitaci�n
create table habitacion(
    habitacion_id int identity(1,1) not null,
    disponibilidad varchar(2) not null,
    constraint chk_disponibilidad CHECK (disponibilidad IN ('S�', 'No')),
    hotel_id int not null,
    tipo_habitacion_id int not null,
    primary key (habitacion_id),
    constraint fk_habitacion_tipo_habitacion
        foreign key (tipo_habitacion_id)
            references tipo_habitacion(tipo_habitacion_id),
    constraint fk_habitacion_hotel
        foreign key (hotel_id)
            references hotel(hotel_id)
);

-- Reservaciones Encabezado
create table reservacion(
    reservacion_id int identity(1,1) not null,
    cliente_dpi bigint not null,  -- Se corrige
    empleado_id int not null,
    hotel_id int not null,
	fecha_inicio date not null,
    fecha_fin date not null,
    costo decimal(10,2) not null,
    primary key (reservacion_id),
    constraint fk_reservacion_encabezado_cliente
        foreign key (cliente_dpi)
            references cliente(cliente_dpi),
	constraint fk_reservacion_encabezado_empleado
		foreign key(empleado_id)
			references empleado(empleado_id),
	constraint fk_reservacion_hotel
		foreign key (hotel_id)
			references hotel(hotel_id)
);


-- Insertar datos en la tabla hotel
INSERT INTO hotel (nombre, descripcion, ubicacion) VALUES
('Hotel Central', 'Hotel de lujo en el centro de la ciudad', 'Ciudad de Guatemala'),
('Hotel del Lago', 'Hermoso hotel con vista al lago', 'Panajachel'),
('Hotel de Playa', 'Hotel frente al mar con todas las comodidades', 'Monterrico');

-- Insertar datos en la tabla cliente
INSERT INTO cliente (cliente_dpi, nombres, apellidos, email, telefono) VALUES
(1234567890101, 'Juan', 'P�rez', 'juan.perez@email.com', '55512345'),
(9876543210123, 'Maria', 'Lopez', 'maria.lopez@email.com', '55567890'),
(4567891230456, 'Carlos', 'Ramirez', 'carlos.ramirez@email.com', '55534567');

-- Insertar datos en la tabla rol
INSERT INTO rol (tipo_rol) VALUES
('Root'),
('Cliente'),
('Empleado'),
('Supervisor'),
('Administrado');

-- Insertar datos en la tabla empleado
INSERT INTO empleado (nombres, apellidos, email, telefono, fecha_nacimiento, hotel_id) VALUES
('Ana', 'Gonzalez', 'ana.gonzalez@email.com', '55511111', '1990-05-10', 1),
('Luis', 'Martinez', 'luis.martinez@email.com', '55522222', '1985-08-20', 2),
('Sofia', 'Hernandez', 'sofia.hernandez@email.com', '55533333', '1992-11-15', 3);

-- Insertar datos en la tabla tipo_habitacion
INSERT INTO tipo_habitacion (tipo, descripcion) VALUES
('Sencilla', 'Habitaci�n con una cama individual'),
('Doble', 'Habitaci�n con dos camas'),
('Suite', 'Habitaci�n de lujo con sala y jacuzzi');

-- Insertar datos en la tabla usuario
INSERT INTO usuario (username, password, empleado_id, rol_id) VALUES
('root', 'root', 1, 1),
('ana_admin', '123456', 1, 2),
('luis_recep', '654321', 2, 4),
('sofia_limpieza', 'limpieza123', 3, 3);

-- Insertar datos en la tabla habitacion
INSERT INTO habitacion (disponibilidad, hotel_id, tipo_habitacion_id) VALUES
('S�', 1, 1),
('No', 1, 2),
('S�', 2, 3),
('No', 3, 1),
('S�', 3, 2);

-- Insertar datos en la tabla reservacion
INSERT INTO reservacion (cliente_dpi, empleado_id, hotel_id, fecha_inicio, fecha_fin, costo) VALUES
(1234567890101, 1, 1, '2025-04-01', '2025-04-05', 500.00),
(9876543210123, 2, 2, '2025-05-10', '2025-05-15', 800.00),
(4567891230456, 3, 3, '2025-06-20', '2025-06-25', 1200.00);

ALTER TABLE reservacion DROP CONSTRAINT fk_reservacion_encabezado_cliente;
ALTER TABLE reservacion DROP CONSTRAINT fk_reservacion_encabezado_empleado;
ALTER TABLE reservacion DROP CONSTRAINT fk_reservacion_hotel;
ALTER TABLE habitacion DROP CONSTRAINT fk_habitacion_tipo_habitacion;
ALTER TABLE habitacion DROP CONSTRAINT fk_habitacion_hotel;
ALTER TABLE habitacion DROP CONSTRAINT chk_disponibilidad;
ALTER TABLE usuario DROP CONSTRAINT fk_usuario_empleado;

SELECT * FROM hotel;
SELECT * FROM cliente;
SELECT * FROM rol;
SELECT * FROM empleado;
SELECT * FROM tipo_habitacion;
SELECT * FROM usuario;
SELECT * FROM habitacion;
SELECT * FROM reservacion;



