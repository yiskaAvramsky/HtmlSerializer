using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class Selector
{
    public string TagName { get; set; }
    public string Id { get; set; }
    public List<string> Classes { get; set; }
    public Selector Parent { get; set; }
    public Selector Child { get; set; }

    public Selector()
    {
        Classes = new List<string>();
    }

    public static Selector FromQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Query string cannot be null or empty.");

        var parts = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        Selector root = null;
        Selector current = null;

        foreach (var part in parts)
        {
            var segments = Regex.Split(part, @"(?=[#\.])");

            var selector = new Selector();

            foreach (var segment in segments)
            {
                if (string.IsNullOrWhiteSpace(segment))
                    continue;

                if (segment.StartsWith("#"))
                {
                    selector.Id = segment.Substring(1);
                }
                else if (segment.StartsWith("."))
                {
                    selector.Classes.Add(segment.Substring(1));
                }
                else
                {
                    if (HtmlHelper.Instance.HtmlTags.Contains(segment))
                    {
                        selector.TagName = segment;
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid tag name: {segment}");
                    }
                }
            }

            if (root == null)
            {
                root = selector;
            }

            if (current != null)
            {
                current.Child = selector;
                selector.Parent = current;
            }

            current = selector;
        }

        return root;
    }
}
