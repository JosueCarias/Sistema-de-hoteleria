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

-- Tipo Habitación
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
    --password varbinary(64) not null,  -- Se cambió a 64 para SHA-256
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

-- Habitación
create table habitacion(
    habitacion_id int identity(1,1) not null,
    disponibilidad varchar(2) not null,
    constraint chk_disponibilidad CHECK (disponibilidad IN ('Sí', 'No')),
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

-- Insertar datos de prueba
