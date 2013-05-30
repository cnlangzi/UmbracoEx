using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using EBA.Helpers;

namespace UmbracoEx
{
    //http://weblogs.asp.net/scottgu/archive/2005/12/10/432854.aspx
    public static class MailHelper
    {
        public static void SendEmail(string from, string[] toList, string subject, string bodyText, Dictionary<string, object> tokens = null, string[] ccList = null, bool isBodyHtml = true)
        {
            MailMessage message = new MailMessage();
            if (from.HasValue())
            {
                message.From = new MailAddress(from);
            }

            foreach (var to in toList)
            {
                message.To.Add(new MailAddress(to));
            }

            if (ccList != null && ccList.Length > 0)
            {
                foreach (var cc in ccList)
                {
                    message.CC.Add(new MailAddress(cc));
                }
            }

            message.IsBodyHtml = isBodyHtml;

            if (tokens != null && tokens.Count > 0)
            {
                var keys = tokens.Keys.ToList();
                var values = tokens.Values.ToArray();
                message.Subject = string.Format(ReplaceTokens(isBodyHtml ? HttpUtility.HtmlDecode(subject) : subject, keys), values);
                message.Body = string.Format(ReplaceTokens(isBodyHtml ? HttpUtility.HtmlDecode(bodyText) : bodyText, keys), values);
            }
            else
            {
                message.Subject = isBodyHtml ? HttpUtility.HtmlDecode(subject) : subject;
                message.Body = isBodyHtml ? HttpUtility.HtmlDecode(bodyText) : bodyText;
            }

            SmtpClient client = new SmtpClient();
            client.Send(message);
        }

        static string ReplaceTokens(string source, List<string> tokens)
        {

            var regexText = string.Join("|", tokens.Select(i => "\\{" + i + "\\}").ToArray());

            regexText += @"|\{.*?\}";

            var regex = new Regex(regexText, RegexOptions.IgnoreCase);

            var expression = regex.Replace(source, match =>
            {
               
                for (var i = 0; i < tokens.Count; i++)
                {
                    var token = tokens[i];

                    if (match.Value.IndexOf(":") > -1)
                    {
                        if (match.Value.StartsWith("{" + token + ":", StringComparison.InvariantCultureIgnoreCase))
                        {
                        
                            var replaced = "{" + i.ToString() + match.Value.Remove(0, token.Length + 1);

                            return replaced;
                        }
                    }
                    else
                    {
                        if (string.Compare(match.Value, "{" + token + "}", StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            //log.Write("-> " + token + " [" + i.ToString() + "]");
                            var replaced = "{" + i.ToString() + match.Value.Remove(0, token.Length + 1);

                            //log.WriteLine("convert ->{" + i.ToString() + match.Value.Remove(0, token.Length + 1));
                            return replaced;
                        }
                    }
                }

                return match.Value.Replace("{", "{{").Replace("}", "}}");

            });

            return expression;

            //}

        }
    }

}
