using System.Text;
using Microsoft.OpenApi.Services;
using System.IO;

public class TocBuilder {
    private readonly OpenApiUrlTreeNode root;
    private readonly string label;
    private readonly Stream stream;
    private readonly StreamWriter writer;
    
    public TocBuilder(OpenApiUrlTreeNode root, string label, Stream stream)
{
        this.root = root;
        this.label = label;
        this.stream = stream;
        writer = new StreamWriter(stream);
    }

public void BuildToc()
{
    writer.WriteLine($"name: Reference");
    writer.WriteLine($"items:");
    foreach (var child in root.Children.Values)
    {
        GenerateTocItem(child);
    }
    writer.Flush();
}
private void GenerateTocItem(OpenApiUrlTreeNode node, int level = 0)
{
    if (node.Segment == "$ref") return;

    var indent = new string(' ', level * 2);
    var segment = node.Segment.Replace("{","").Replace("}","");
    writer.WriteLine($"{indent}- name: {segment}");
    
    if (node.PathItems.Any()) {
        writer.WriteLine($"{indent}  items:");
        var ops = node.PathItems[label].Operations;
        foreach (var op in ops)
        {
            if (op.Value.ExternalDocs?.Url.OriginalString != null) {
            writer.WriteLine($"{indent}  - name: \"{op.Value.Summary}\"");
            //writer.WriteLine($"{indent}    displayName: \"{op.Value.Summary}\"");
            writer.WriteLine($"{indent}    href: {op.Value.ExternalDocs?.Url}");
            }
        }
    }

    if (node.Children.Any())
    {
        if (!node.PathItems.Any()) {
            writer.WriteLine($"{indent}  items:");
        }
        foreach (var child in node.Children.Values)
        {
            GenerateTocItem(child, level + 1);
        }
    }
    else
    {
        var href = node.Path.Replace("{version}", "v1.0");
        writer.WriteLine($"{indent}  href: {href}");
    }
}

}