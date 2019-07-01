using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebAPI.Models
{
    public class XMLCostAnalyzer
    {
        private Stack<String> expectedClosingXmlTags;
        private List<CostCenter> costList;
        private int numberOfTextAnalyzed;
        private int numberOfTextAnalyzedWithError;
        public XMLCostAnalyzer()
        {
            expectedClosingXmlTags = new Stack<String> { };
            costList = new List<CostCenter> { };
            numberOfTextAnalyzed = 0;
            numberOfTextAnalyzedWithError = 0;
        }

        /* 
         * tmp = { text |XML }*
         * text = {letter | number | spaces}*
         * letter = a-z|A-Z
         * number = digits {digits]*
         * digit = 0-9
         * spaces = space | \n|\r|\t ...
         * XML = <TAG>[text | XML | ] </TAG>
         * TAG = letter {letter | number}*
         * 
         * Algorithm:
         * - skip all text
         * - Check if XML has the correct formats: 
         *    -- not starting with </..>
         *    -- <start> ...</start>
         * - if <total> float </total> => float is the total cost (GST included) -> compute GST and  total exclusing GST
         * - <cost_centre> COST_CENTER </cost_center>: not <cost_center> => COST_CENTER = UNKNOWN
         */

        private string Parser(string data)
        {
            bool closingTag = false;
            string tmp;
            string errorMessage = "";
            string costCenterName = "UNKNOWN";
            List<CostCenter> currentCostList = new List<CostCenter> { };
            if (data == null) return "";

            /* read text until < or end of file */
            tmp = data;
            while (tmp.Length > 0)
            {
                int index = tmp.IndexOf('<');
                if (index < 0) break;
                if (tmp[index + 1] == '/')
                {
                    // closing tag  
                    index++;
                    closingTag = true;
                }
                else closingTag = false;

                string tag = "";
                for (index++; (index < tmp.Length) && (tmp[index] != '>'); index++)
                    tag = tag + tmp[index];
                if (!string.IsNullOrWhiteSpace(tag))
                {
                    index++; // go to next character
                    if (closingTag)
                    {
                        string expected = expectedClosingXmlTags.Pop();
                        if (tag != expected)
                        {
                            errorMessage = $"[Error] Expected </{expected}>, receving {tag}";
                            break;
                        }
                    }
                    else
                    {
                        expectedClosingXmlTags.Push(tag);
                        if (tag == "total" || tag == "cost_center" || tag == "cost_centre")
                        {
                            string value = "";
                            for (; (index < tmp.Length) && (tmp[index] != '<'); index++)
                                value = value + tmp[index];
                            if (tag == "total")
                                currentCostList.Add(new CostCenter(Convert.ToDouble(value), costCenterName));
                            else
                                costCenterName = value;
                            //Console.WriteLine("{0} = {1}", tag, value);
                        }
                    }
                }
                tmp = tmp.Substring(index); ;
            }
            if (errorMessage == "")
            {
                if (expectedClosingXmlTags.Count > 0)
                {
                    errorMessage = "[Error] Expected </" + expectedClosingXmlTags.Pop() + ">, receving end of text";
                }
                else
                {
                    foreach (CostCenter cc in currentCostList) costList.Add(cc);
                }
            }

            return errorMessage;
        }

        public string ExtractCostAndCenter(string data)
        {
            string errorMessage = "";

            numberOfTextAnalyzed++;
            errorMessage = Parser(data);
            if (errorMessage != "")
            {
                //Console.WriteLine(errorMessage);
                numberOfTextAnalyzedWithError++;
            }
            return errorMessage;
        }

        public string GetCosts()
        {
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(costList);
            return str;
        }
    }
}
