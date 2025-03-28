using System.Runtime.InteropServices;

namespace erd_dotnet;

class EntityWriter
{
    public static List<string> BuildString(Entity entity, WriterOption option)
    {
        var res = new List<string>
        {
            $@"
            {entity.Title} [label=<
        <TABLE 
            BORDER=""0"" 
            CELLPADDING=""0"" 
            ALIGN=""CENTER"" 
            CELLSPACING=""0.5"">
            <TR>
                <TD ALIGN=""CENTER"" VALIGN=""BOTTOM""><B><FONT FACE=""Helvetica"" COLOR=""{option.TitleColor}"" POINT-SIZE=""16"">{entity.Title}</FONT></B></TD>
            </TR>
        </TABLE>| 
            <TABLE
                BORDER=""0""
                ALIGN=""LEFT""
                CELLPADDING=""0""
                CELLSPACING=""4""
                >
        "
        };
        bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        foreach (var field in entity.Fields)
        {
            string key = field.IsPK ? "🔑" : "";
            if (isWindows && field.IsPK)
            {
                key = @"<FONT FACE=""Segoe UI Emoji"">🔑</FONT>";
            }
            if (isLinux && field.IsPK)
            {
                key = @"<FONT FACE=""Noto Color Emoji"">🔑</FONT>";
            }
            string prefix = field.IsFK ? "<I>" : "";
            if (field.IsPK) { prefix += "<U>"; }
            string suffix = field.IsPK ? "</U>" : "";
            if (field.IsFK) { suffix += "</I>"; }

            res.Add($@"
                <TR>
                    <TD ALIGN=""LEFT"">{key}</TD>
                    <TD ALIGN=""LEFT"">{prefix}<FONT FACE=""Helvetica"">{field.Name}</FONT>{suffix}</TD>
                </TR>
            ");
        }
        var fill = string.IsNullOrEmpty(option.BgColor) ? "" : @$",fillcolor=""{option.BgColor}"", style=filled ";
        res.Add($"</TABLE>>{fill}]; ");
        return res;
    }
}
