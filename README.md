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

```bash
erd-dotnet INPUTFILE OUTPUTFILE [OPTION]
```

Where:
 * *INPUTFILE* is the path of a text file (.er format)
 * *OUTPUTFILE* is the path of the output file (.png or .txt)

Options:
  -f, --format TXT  The output is a text file (dot format)

By default, the output will be a png file. This requires Dot to be installed.

Alternatively you can use the text option and use an online tool:
Copy and paste the contents of the .dot file into an [online graphviz viewer](https://edotor.net)

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
