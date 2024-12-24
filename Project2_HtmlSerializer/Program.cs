
class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            HtmlSerializer loader = new HtmlSerializer();
            string url = "https://www.example.com";
            string htmlContent = await loader.Load(url);
            HtmlSerializer htmlSerializer = new HtmlSerializer();
            List<HtmlElement> parsedElements = htmlSerializer.ParseHtml(htmlContent);
            Selector selector = Selector.FromQuery("body div h1");
            List<HtmlElement> lst = parsedElements[0].FindElements(selector);

            Console.WriteLine(htmlContent);

            //foreach (var element in parsedElements)
            //{
            //    Console.WriteLine(element.InnerHtml);
            //}
            Console.WriteLine("------------");

            foreach (var element in lst)
            {
                Console.WriteLine(element.InnerHtml);
            }
            //while ( selector!=null)
            //{
            //    Console.WriteLine(selector.TagName);
            //    selector = selector.Child;
            //}
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}