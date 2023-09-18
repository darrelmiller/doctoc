// See https://aka.ms/new-console-template for more information
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Services;

var filename = args.Length > 0 ? args[0] : "openapi.yaml";
var outfilename = args.Length > 1 ? args[1] : "toc.yaml";

var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
var reader = new OpenApiStreamReader();
var result = await reader.ReadAsync(stream);
var doc = result.OpenApiDocument;
var root = OpenApiUrlTreeNode.Create(doc, "v1.0");

using var outstream = new FileStream(outfilename, FileMode.Create, FileAccess.Write, FileShare.None);
var builder = new TocBuilder(root,"v1.0",outstream);
builder.BuildToc();

