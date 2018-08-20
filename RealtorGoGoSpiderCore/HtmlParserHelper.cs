using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorGoGoSpider
{
    public class HtmlParserHelper
    {
        public static IEnumerable<HtmlNode> GetElementsByID(HtmlNode doc, String id)
        {
            return doc
                .Descendants()
                .Where(n => n.NodeType == HtmlNodeType.Element)
                .Where(e => e.Id == id);
        }

        public static IEnumerable<HtmlNode> GetElementsWithClass(HtmlNode doc, String className)
        {

            return doc
                .Descendants()
                .Where(n => n.NodeType == HtmlNodeType.Element)
                .Where(e =>
                   CheapClassListContains(
                       e.GetAttributeValue("class", ""),
                       className,
                       StringComparison.Ordinal
                   )
                );
        }

        public static IEnumerable<HtmlNode> GetElementsByElementType(HtmlNode doc, String nodeType)
        {
            return doc
                .Descendants(nodeType);
        }

        public static Boolean CheapClassListContains(String haystack, String needle, StringComparison comparison)
        {
            if (String.Equals(haystack, needle, comparison)) return true;
            Int32 idx = 0;
            while (idx + needle.Length <= haystack.Length)
            {
                idx = haystack.IndexOf(needle, idx, comparison);
                if (idx == -1) return false;

                Int32 end = idx + needle.Length;

                // Needle must be enclosed in whitespace or be at the start/end of string
                Boolean validStart = idx == 0 || Char.IsWhiteSpace(haystack[idx - 1]);
                Boolean validEnd = end == haystack.Length || Char.IsWhiteSpace(haystack[end]);
                if (validStart && validEnd) return true;

                idx++;
            }
            return false;
        }
    }
}
