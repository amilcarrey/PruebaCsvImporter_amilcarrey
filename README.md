[![Action Status](https://github.com/amilcarrey/PruebaCsvImporter_amilcarrey/workflows/Build&Test/badge.svg)](https://github.com/amilcarrey/PruebaCsvImporter_amilcarrey/actions)
# CsvImporter for Acme Corporation


Este proyecto es una aplicación desarrollada como requisito de una entrevista tecnica. Consta de consumir un archivo csv desde una cuenta de almacenamiento de Azure e insertar el contenido en una BD SQL Server local. 

## Run on dotnet 

Con el proyecto descargado y situado en el directorio raiz del mismo, ejecuta los siguiente comandos.  

```bash

dotnet restore

dotnet build

dotnet run

```
# Why Clean?

Uno de las cosas requeridas en el enunciado es desarrollar el proyecto utilizando las mejores practicas y los beneficios que provee el framework. La arquitectura que elegí usar es [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html). De esta manera puedo, de manera más fácil, escribir codigo con bajo acoplamiento y que sea independiente de la implementación de la infraestructura. Otra ventaja de esta arquitectura es la mantenibilidad, su flexibilidad a cambios y, gracias a como está estructurada, simple de testear. 

![Clean Architecture Diagram](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/media/image5-8.png)

## Infraestructure 
### Entity Framework Core
Para insertar los datos que el enunciado pide debo acceder de alguna forma a la base de datos desde el código. Estaba entre tres alternativas: 

 1. EF Core
 2. Dapper
 3. SqlClient 

Leyendo un poco de pro's y contras de cada uno me tope con un [artículo en Medium](https://medium.com/@engr.mmohsin/entity-framework-core-2-dapper-performance-benchmark-c29e8cce9e1b#:~:text=Dapper%20is%20faster%20than%20the,performance,%20use%20entity%20framework%20instead.) que después de realizar distintas pruebas concluye: 

> I am currently using both Entity Framework Core and Dapper in my current project. **Entity Framework for insert, update, and get**. **And Dapper for complex queries like reports, joins (where the data is fetched from multiple tables).** Entity Framework is reducing my development time and dapper is fetching complex data with more speed.

Esto me predispuso a utilizar EF Core. Entendiendo que SqlClient probablemente me de una mayor performance y teniendo en cuenta que el tiempo apremia decidí, en primera instancia, utilizar *Entity Framework Core* con su provider de SqlServer para tener una primera iteración en menos tiempo.   

## Get CSV
La fuente de los datos a insertar en la BD es un archivo CSV alojado en una cuenta de almacenamiento de azure. El primer paso era obtener esos datos y estos son los approach que manejé.

### Vía Url
El enunciado me provee la url del blobstorage que contiene el archivo. Con ella genero un request a dicha url, obtengo la respuesta y a partir de ella el stream de datos a consumir. Este es el enfoque que tomé para este proyecto.

### Vía librería de Azure
Otra de las opciones es tener una clase que, con las credenciales adecuadas, cree un cliente que se conecta con azure para descargar el stream del archivo. La ventaja de esto es que se pueden dejar los datos privados. Los datos necesarios para este approach son los siguientes: 
 - [ ] Connection string: *~~key obtenida de azure~~*
 - [x] Cuenta de almacenamiento: **storage10082020**
 - [x] Container: **y9ne9ilzmfld**
 - [x] Nombre de archivo: **Stock.csv**

### Vía ubicación del archivo (local)
Esta es la opción más conocida pero, aunque experimenté e hice pruebas con este método, la fuente de los datos está en la nube y descargar el archivo para recién procesarlo, para volumenes muy grandes, no es viable. 

# Inserción de datos
Esta fué la parte del proyecto a la que mas tiempo le dediqué. Teniendo una fuente de datos externa, los datos se van leyendo y parseando a medida que se van descargando. 

Como la conexión o velocidad de bajada no es algo que pueda modificar a través del código, el consumo de memoria depende de como se realice la inserción de estos datos.

El primer método que utilicé fué el típico  `Add()` + `SaveChanges()` de EntityFramework con un archivo local acotado. Claramente funcionaba pero al escalar en cantidad de datos la cantidad de memoria utilizada escalaba linealmente. 

Para mejorar la perfomance lo siguiente que hice fue agrupar los datos en grupos de 10k y utilizar  `AddRange()` + `SaveChanges()` , insertar varios datos de una sola vez y liberar memoria al terminar la inserción. Esto haría que la memoria se mantenga en un valor constante. Para mi sorpresa, después de hacer el cambio la memoria seguía escalando de la misma manera que antes porque Entity Framework seguía trackeando el estado de esas entidades de la aplicación. Esto me lleva al siguiente cambio, después de cada inserción detachaba el estado de las entidades modificadas lo que hacía que la memoria baje a medida que se iban modificando. 

Aún con estos cambios el tiempo para insertar un millón de registros era demasiado alto, alrededor de 15 minutos. 

Luego pensé en como lo haría directamente en la base datos y me puse en busqueda de una implementación de `BULK INSERT` sobre Entity Framework y encontré una librería que extendía funcionalidades a EF brindandome un BulkInsert que claramente es más rapido que el SaveChanges.

| Operaciones| 1,000 Entities | 2,000 Entities | 5,000 Entities
|--|--|--|--|
| SaveChanges | 1,200 ms | 2,400 ms | 6,000 ms |
| BulkSaveChanges | 150 ms | 225 ms | 500 ms |

Luego de implementar la librería probé con un archivo (local) de 1 millón de registros insertando distintos batch sizes

| Batch Size| Tiempo (m) | RAM max|
|--|--|--|
| 10.000 | 1:59 m | 66,1 Mb
| 50.000 | 1:58 m | 82,6 Mb
| 100.000 | 1:56 m | 108,3 Mb 
| 200.000 | 2:01 m | 167,6 Mb
| 500.000 | 2:00 m | 317,2 Mb

Tomando el último valor puedo inferir que: 
Si `1.000.000 registros = ~36MB` toma 1:51 minutos en procesarse (111 segundos), 
`([tamaño del archivo] / 36) * 111 = tiempo de procesamiento en segundos.`

### SqlBulkCopy
Un método antiguo pero no por eso ineficiente es crear las conexiones a "mano" y dejar atrás las abstracciones hermosas que nos brinda EF Core. Utilicé `Microsoft.Data.SqlClient` para probar si el uso de memoria y el tiempo de procesamiento mejoraba al no tener a Entity Framework de por medio. Y así fue, estos son los resultados: 

| Batch Size| Tiempo (m) | RAM max|
|--|--|--|
| 10.000 | 1:53 m | 47,6 Mb
| 50.000 | 1:50 m | 52,2 Mb
| 100.000 | 1:43 m | 59,4 Mb 
| 200.000 | 1:58 m | 72,9 Mb
| 500.000 | 2:00 m | 109,5 Mb


## Posibles mejoras
### Performance
Claramente la performance se puede mejorar si sacamos a la conexión como cuello de botella. En mi caso, por lo inestabilidad del mi proveedor de servicio, realicé todas las pruebas primeramente con un archivo local con 1 millon de registros que pesa 36 mb. Si planteamos un supuesto de una conexión de 300 mbps, un archivo de 700MB puede descargarse en ~19 segundos. Asumiendo ese tiempo y el espacio de almacenamiento para el archivo, se podría mejorar en gran medida la performance aplicando multithreading al parseo de los datos del archivo reduciendolo drasticamente. Hice unas pruebas  con la muestra de 36 MB y, sin tener en cuenta optimizaciones de memoriaun archivo de 1 millón de registros se procesó (ineficientemente) en 23 segundos, aproximadamente 75% más rápido y consumió 410 MB de memoria. El método está en `StockServices.cs` y se llama `AddByEfCoreWithParallelismAsync()`.

Otra enfoque que me hubiera gustado probar es la implementación de [Apache Spark con su librería para .Net](https://dotnet.microsoft.com/apps/data/spark). Inclusive tienen un [connector para realizar BulkInserts](https://github.com/microsoft/sql-spark-connector) dentro de SqlServer. Aunque ya no entre para la primera iteración, de seguro la voy a probar para tener la comparación completa. En mi humilde opinión, este debería haber sido mi primer enfoque y el foco para invertir mi tiempo y llegar al deadline.   

### Testing
El proyecto podría mejorar muchisimo en la parte de testing. Desde explotar más casos de prueba en los Unit Test hasta aplicar tests de integración. 

## Librerias
### CsvHelper 
La opción "tradicional" para trabajar con archivos csv sería iterar sobre los datos separando los datos a partir de un carácter, dando como resultado un array que deberíamos, en el mejor de los casos, validar sus datos y convertirlos a un objeto de la aplicación pero ¿para qué reinventar la rueda? Para parsear los datos del archivo csv, decidí utilizar [esta librería](https://bartsimons.me/reading-csv-files-in-csharp-with-csvparser) que es simple ampliamente utilizada en la comunidad. 

### EfExtensions
Como mencioné anteriormente, la operación `Add()` + `SaveChanges()` de Entity Framework no es performante para grandes volumenes de datos. Para solucionar este problema y poder tener la funcionalidad de Bulk implemente esta [librería](https://entityframework-extensions.net/bulk-savechanges). 

### XUnit y Moq
Existen diversas librerías para realizar testing en el ecosistema .Net pero  XUnit y Moq son las mas simples de utilizar, a mi gusto. Cumplen su función y dejan relativamente semantico el programa. 

### FastMember
Como mencioné anteriormente, uno de los métodos que utilicé fue *SqlBulkCopy*, para utilizarlo necesitaba mapear mi objeto con un DataTable para así poder obtener un DataReader. Para evitarme esta parte aburrida y engorrosa utilicé [esta librería](https://github.com/mgravell/fast-member).
