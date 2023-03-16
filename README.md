# erd-dotnet

This utility translates a plain text description of a relational database schema to a graphical entity-relationship diagram. The visualization is produced by using Dot with GraphViz. The output graph is a png image.

Here is an example of the output:

![Simple erd example](./examples/simple.png?raw=true)

### Installation

erd-dotnet requires:
* [GraphViz](http://www.graphviz.org/download/)
* [.Net 7 Runtime](https://dotnet.microsoft.com/download/dotnet/7.0)

All of these are available for Windows, Mac and Linux.
Dot must be in your path.

### Usage

erd-dotnet <path-to-input-file> <path-to-output-file>
  
Where:
 * <path-to-input-file> is the path of a text file (.er format)
 * <path-to-output-file> is the path of the output file (.png)

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

You need .net 7 SDK.

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
