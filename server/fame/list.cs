using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using db;
using MySql.Data.MySqlClient;

namespace server.fame
{
    internal class list : IRequestHandler
    {
        public void HandleRequest(HttpListenerContext context)
        {
            NameValueCollection query;
            using (var rdr = new StreamReader(context.Request.InputStream))
                query = HttpUtility.ParseQueryString(rdr.ReadToEnd());

            byte[] status = null;

            string span = "";
            switch (query["timespan"])
            {
                case "week":
                    span = "(time >= DATE_SUB(NOW(), INTERVAL 1 WEEK))";
                    break;
                case "month":
                    span = "(time >= DATE_SUB(NOW(), INTERVAL 1 MONTH))";
                    break;
                case "all":
                    span = "TRUE";
                    break;
                default:
                    status = Encoding.UTF8.GetBytes("<Error>Invalid fame list</Error>");
                    break;
            }
            string ac = "FALSE";
            if (query["accountId"] != null)
                ac = "(accId=@accId AND chrId=@charId)";

            if (status == null)
            {
                var doc = new XmlDocument();
                XmlElement root = doc.CreateElement("FameList");

                XmlAttribute spanAttr = doc.CreateAttribute("timespan");
                spanAttr.Value = query["timespan"];
                root.Attributes.Append(spanAttr);

                doc.AppendChild(root);

                using (var db = new Database())
                {
                    MySqlCommand cmd = db.CreateQuery();
                    cmd.CommandText = @"SELECT * FROM death WHERE " + span + @" OR " + ac +
                                      @" ORDER BY totalFame DESC LIMIT 20;";
                    if (query["accountId"] != null)
                    {
                        cmd.Parameters.AddWithValue("@accId", query["accountId"]);
                        cmd.Parameters.AddWithValue("@charId", query["charId"]);
                    }
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            XmlElement elem = doc.CreateElement("FameListElem");

                            XmlAttribute accIdAttr = doc.CreateAttribute("accountId");
                            accIdAttr.Value = rdr.GetInt32("accId").ToString();
                            elem.Attributes.Append(accIdAttr);
                            XmlAttribute chrIdAttr = doc.CreateAttribute("charId");
                            chrIdAttr.Value = rdr.GetInt32("chrId").ToString();
                            elem.Attributes.Append(chrIdAttr);

                            root.AppendChild(elem);

                            XmlElement nameElem = doc.CreateElement("Name");
                            nameElem.InnerText = rdr.GetString("name");
                            elem.AppendChild(nameElem);
                            XmlElement objTypeElem = doc.CreateElement("ObjectType");
                            objTypeElem.InnerText = rdr.GetString("charType");
                            elem.AppendChild(objTypeElem);
                            XmlElement tex1Elem = doc.CreateElement("Tex1");
                            tex1Elem.InnerText = rdr.GetString("tex1");
                            elem.AppendChild(tex1Elem);
                            XmlElement tex2Elem = doc.CreateElement("Tex2");
                            tex2Elem.InnerText = rdr.GetString("tex2");
                            elem.AppendChild(tex2Elem);
                            XmlElement equElem = doc.CreateElement("Equipment");
                            equElem.InnerText = rdr.GetString("items");
                            elem.AppendChild(equElem);
                            XmlElement fameElem = doc.CreateElement("TotalFame");
                            fameElem.InnerText = rdr.GetString("totalFame");
                            elem.AppendChild(fameElem);
                        }
                    }
                    db.Dispose();
                }

                var settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                using (XmlWriter wtr = XmlWriter.Create(context.Response.OutputStream))
                    doc.Save(wtr);
            }
        }
    }
}