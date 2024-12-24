using System.Collections.Generic;
using System.Linq;

public class HtmlElement
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    public List<string> Classes { get; set; } = new List<string>();
    public string InnerHtml { get; set; }
    public HtmlElement Parent { get; set; }
    public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();

    public IEnumerable<HtmlElement> Descendants()
    {
        var queue = new Queue<HtmlElement>();
        queue.Enqueue(this);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            yield return current;

            foreach (var child in current.Children)
            {
                queue.Enqueue(child);
            }
        }
    }

    public IEnumerable<HtmlElement> Ancestors()
    {
        HtmlElement current = this.Parent;
        while (current != null)
        {
            yield return current;
            current = current.Parent;
        }
    }

    public List<HtmlElement> FindElements(Selector selector)
    {
        var results = new HashSet<HtmlElement>();
        FindElementsRecursive(this, selector, results);
        return results.ToList();
    }

    private void FindElementsRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> results)
    {
        if (selector == null) return;

        var descendants = element.Descendants();
        var matchingElements = descendants.Where(e => MatchesSelector(e, selector));

        if (selector.Child == null)
        {
            foreach (var match in matchingElements)
            {
                results.Add(match);
            }
            return;
        }

        foreach (var match in matchingElements)
        {
            FindElementsRecursive(match, selector.Child, results);
        }
    }

    private bool MatchesSelector(HtmlElement element, Selector selector)
    {
        return MatchesTag(element, selector) &&
               MatchesId(element, selector) &&
               MatchesClasses(element, selector);
    }

    private bool MatchesTag(HtmlElement element, Selector selector)
    {
        return string.IsNullOrEmpty(selector.TagName) ||
               string.Equals(element.Name, selector.TagName, System.StringComparison.OrdinalIgnoreCase);
    }

    private bool MatchesId(HtmlElement element, Selector selector)
    {
        return string.IsNullOrEmpty(selector.Id) ||
               string.Equals(element.Id, selector.Id, System.StringComparison.OrdinalIgnoreCase);
    }

    private bool MatchesClasses(HtmlElement element, Selector selector)
    {
        return !selector.Classes.Any() ||
               selector.Classes.All(cls => element.Classes.Contains(cls));
    }
}