# erd-dotnet tool

This utility translates a plain text description of a relational database schema to a graphical entity-relationship diagram. The visualization is produced by using Dot with GraphViz. The output graph is a png image.

Here is an example of the output:

![Simple erd example](./examples/simple.png?raw=true)

🎉 **New**: *erd-dotnet* can now automatically create the .er and png files when connecting to your postgress database.


### Installation

erd-dotnet requires:
* [GraphViz](http://www.graphviz.org/download/)
* [.Net 8 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

All of these are available for Windows, Mac and Linux.
Dot must be in your path.

### Usage

```bash
erd-dotnet ERD_FILE OUTPUT_FILE [OPTION]
```

Where:
 * *ERD_FILE* is the path of the erd file (.er format)
 * *OUTPUT_FILE* is the path of the output file (.png or .txt)

Options:
 * -f, --format TXT  The output is a text file (dot format)
 * -d, --database DB_CONNECTION Obtaining the schema from a database (Postgress only)
 DB_CONNECTION is like: "Host=192.168.XXX.XXX:5432;Username=user;Password=pwd;Database=my_database;", see [Npgsql doc](https://www.npgsql.org/doc/connection-string-parameters.html)


🚨 By default, the output will be a png file. This requires *Dot* to be installed.

🌐 Alternatively you can use the text option and use an online tool:
Copy and paste the contents of the .dot file into an [online graphviz viewer](https://edotor.net)

### The .er file format

This is a text file that can easily be updated.
The name of the table is enclosed in [ ], followed by the names of the columns in the following rows. A primary key is preceded by *, a foreign key by +.

The relationships between the tables are expressed as follows:
```
table1 *--1 table2
```
The cardinality is expressed as:
| Cardinality  | Syntax |
| ------------ | -------|
| 0 or 1       |    ?   |
| exactly 1    |    1   |
| 0 or more    |    *   |
| 1 or more    |    +   |

### Quick example of a .er file

```
[Person]
*name
height
weight
birth
+location_id
```

### Build instruction

You need .net 8 SDK.

Build:
```
dotnet build
```
Run:
```
dotnet run
```


### Credit

This work is based on the work of:
 * [https://github.com/BurntSushi/erd](https://github.com/BurntSushi/erd)
 * [https://github.com/kaishuu0123/erd-go](https://github.com/kaishuu0123/erd-go)
