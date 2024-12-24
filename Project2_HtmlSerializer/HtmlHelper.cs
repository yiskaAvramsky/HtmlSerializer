using System;
using System.IO;
using System.Text.Json;

public class HtmlHelper
{
    private readonly static HtmlHelper _instance = new HtmlHelper();
    public static HtmlHelper Instance => _instance;

    public string[] HtmlTags { get; set; }
    public string[] VoidTags { get; set; }

    private HtmlHelper()
    {
        HtmlTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("HtmlTags.json"));
        VoidTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("HtmlVoidTags.json"));
    }
}
