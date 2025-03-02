-- Comando para poder ver si la base de datos existe
SELECT name 
FROM sys.databases 
WHERE name = 'hoteleriaGrupo3';

-- Eliminar base de datos si es que ya existe
use master; -- Utilizamos otra base de datos en caso de que ya usemos la que vamos a eliminar, de lo contrario dará error
drop database if exists hoteleriaGrupo3;

-- Crear base de datos
create database hoteleriaGrupo3;

-- Usar la base de datos
use hoteleriaGrupo3;

-- Sección de creación de tablas

-- Hotel
create table hotel(
	hotel_id int identity(1,1) not null,
	nombre varchar(50) not null,
	descripcion varchar(150),
	ubicacion varchar(150) not null,
	primary key (hotel_id)
);

select * from hotel;

-- Cliente
create table cliente(
	cliente_dpi bigint not null,
	nombres varchar(50) not null,
	apellidos varchar(50) not null,
	email varchar(100) not null,
	telefono varchar(8) not null,
	primary key (cliente_dpi)
);

select * from cliente;

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
			references hotel(hote_id)
);

insert into empleado (nombres, apellidos, email, telefono, fecha_nacimiento, hotel_id) values ('Julio', 'Jaramillo', 'jjaramillo@gmail.com', '12345678','2025-02-22',1);
select * from empleado;

-- Tipo Habitación
create table tipo_habitacion(
	tipo_habitacion_id int identity(1,1) not null,
	tipo varchar(50) not null,
	descripcion varchar(150),
	primary key (tipo_habitacion_id)
);

insert into tipo_habitacion(tipo, descripcion) values('Individual', 'Habitación espaciosa con 3 cuartos y 2 baños completos');
insert into tipo_habitacion(tipo, descripcion) values('Doble', 'Habitación espaciosa con 2 cuartos y 1 baños completos');
insert into tipo_habitacion(tipo, descripcion) values('Suite', 'Habitación espaciosa con 1 cuarto y 2 baños completos');

select * from tipo_habitacion;

-- Usuario
create table usuario(
	usuario_id int identity(1,1) not null,
	username varchar(25) not null,
	password varbinary(256) not null,
	empleado_id int not null,
	primary key  (usuario_id),
	constraint fk_usuario_empleado 
		foreign key (empleado_id) 
			references empleado(empleado_id) 
);

insert into usuario(username, password,empleado_id) values ('jjaramillo', HASHBYTES('SHA2_256', 'julio12'), 1);

select * from usuario;

-- Habitación
create table habitacion(
	habitacion_id int identity(1,1) not null,
	disponibilidad varchar(2) not null,
	constraint chk_disponibilidad CHECK (disponibilidad IN ('Sí', 'No')),
	hotel_id int not null,
	tipo_habitacion_id int not null,
	tipo_habitacion varchar (50),
	descripcion varchar(150),
	primary key (habitacion_id),
	constraint fk_habitacion_tipo_habitacion
		foreign key (tipo_habitacion_id)
			references tipo_habitacion(tipo_habitacion_id),
	constraint fk_habitacion_hotel
		foreign key (hotel_id)
			references hotel(hotel_id)
);

insert into habitacion(disponibilidad, hotel_id, tipo_habitacion_id) values ('Sí',1,1);

select * from habitacion;

-- Reservaciones
create table reservacion_encabezado(
	reservacion_encabezado_id int identity(1,1) not null,
	cliente_id int not null,
	empleado_id int not null,
	hotel_id int not null,
	costo decimal(10,2) not null
);

select * from reservacion_encabezado;


-- Reservaciones Detalle
create table reservacion_detalle(
	reservacion_detalle_id int identity(1,1) not null,
	reservacion_encabezado_id int not null,
	fecha_inicio date not null,
	fecha_fin date not null,
	primary key (reservacion_detalle_id),
	constraint fk_reservacion_detalle_reservacion_encabezado
		foreign key (reservacion_encabezado_id)
			references reservacion_encabezado(reservacion_encabezado_id)
);


-- Sección de funciones tipo "Trigger"

-- Trigger en la tabla habitación luego de insertar un registro
create trigger trigger_habitacion_after_insert
on habitacion
after insert
as
begin
		update a
		set a.tipo_habitacion = b.tipo, a.descripcion = b.descripcion
		from habitacion a
		inner join tipo_habitacion b on b.tipo_habitacion_id = a.tipo_habitacion_id
		inner join inserted i ON i.habitacion_id = a.habitacion_id -- "inserted" para aplicar solo en las nuevos registros, de lo contrario, actualiza todos los registros de la tabla
end;


