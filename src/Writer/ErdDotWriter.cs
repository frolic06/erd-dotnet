namespace erd_dotnet;

class ErdDotWriter
{
    private List<string> textLines;
    private Erd erd;
    private WriterOption option;

    public ErdDotWriter(Erd erd)
    {
        option = new WriterOption();
        textLines = new List<string>();
        this.erd = erd;
    }

    public void WriteFile(string path)
    {
        Header();
        Relationships();
        Entities();
        Tail();
        File.WriteAllLines(path, textLines);
    }

    private void Header()
    {
        textLines.Add(@"
            graph {
                graph [nodesep=0.5,
                    ranksep=0.5,
                    pad=""0.2,0.2"",
                    margin=""0.0"",
                    cencentrate=true,
                    splines=""spline"",
                    rankdir=LR
                ];
                node [
                    label=""\N"",
                    fontsize=14,
                    margin=""0.07,0.05"",
                    penwidth=1.0,
                    shape=Mrecord
                ];
                edge [
                    color=gray50,
                    minlen=2,
                    style=dashed
                ];
            ");
    }
    private void Tail()
    {
        textLines.Add(@"
            }");
    }

    private void Relationships()
    {
        foreach (var relationship in erd.Relationships)
        {
            textLines.AddRange(RelationshipWriter.BuildString(relationship));
        }
    }

    private void Entities()
    {
        foreach (var entity in erd.Entities)
        {
            textLines.AddRange(EntityWriter.BuildString(entity, option));
        }
    }
}
