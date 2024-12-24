using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class HtmlSerializer
{
    public async Task<string> Load(string url)
    {
        HttpClient client = new HttpClient();
        var response = await client.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }

    public List<HtmlElement> ParseHtml(string html)
    {
        var cleanHtml = Regex.Replace(html, "\\s+", " ");
        var matches = Regex.Matches(cleanHtml, "<[^>]+>|([^<>]+)");
        var elements = new List<HtmlElement>();
        var stack = new Stack<HtmlElement>();

        foreach (Match match in matches)
        {
            var content = match.Value.Trim();
            if (string.IsNullOrWhiteSpace(content)) continue;

            if (!content.StartsWith("<"))
            {
                if (stack.Count > 0)
                {
                    stack.Peek().InnerHtml = content;
                }
                continue;
            }

            if (content.StartsWith("</"))
            {
                if (stack.Count > 0) stack.Pop();
                continue;
            }

            var element = new HtmlElement
            {
                Name = Regex.Match(content, "<([\\w-]+)").Groups[1].Value
            };

            var attributes = Regex.Matches(content, "(\\w+)=\"(.*?)\"");
            foreach (Match attribute in attributes)
            {
                var attributeName = attribute.Groups[1].Value;
                var attributeValue = attribute.Groups[2].Value;

                if (attributeName == "class")
                {
                    element.Classes.AddRange(attributeValue.Split(' '));
                }
                else if (attributeName == "id")
                {
                    element.Id = attributeValue;
                    element.Attributes.Add(attributeName, attributeValue);
                }
                else
                {
                    element.Attributes.Add(attributeName, attributeValue);
                }
            }

            if (stack.Count > 0)
            {
                element.Parent = stack.Peek();
                element.Parent.Children.Add(element);
            }

            if (!content.EndsWith("/>") && !HtmlHelper.Instance.VoidTags.Contains(element.Name))
            {
                stack.Push(element);
            }

            elements.Add(element);
        }

        return elements;
    }
}
