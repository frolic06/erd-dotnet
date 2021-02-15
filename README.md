# erd-dotnet

This utility translates a plain text description of a relational database schema to a graphical entity-relationship diagram. The visualization is produced by using Dot with GraphViz. The output graph is a png image.

Here is an example of the output:

![Simple erd example](https://github.com/frolic06/erd-dotnet/raw/main/examples/simple.png)

### Installation

erd-dotnet requires:
* [GraphViz](http://www.graphviz.org/download/), and one of:
* [.Net 5 Runtime](https://dotnet.microsoft.com/download/dotnet/5.0)

All of these are available for Windows, Mac and Linux.

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
