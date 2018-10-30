using Script.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Script
{
    public class Result
    {
        public string TCM_ID { get; set; }
        public string Title { get; set; }
        // public string Item_Type { get; set; }
        // public string Publication_Id { get; set; }
        //public string Using_Publication_id { get; set; }
        public string Using_Publication_name_With_Local { get; set; }
        public string Using_Publication_name_Without_Local { get; set; }
        // public string Publish_Status { get; set; }

        public string Using_Component_Tcm_ID { get; set; }

        public string using_Page_tcm_id { get; set; }


        //public string LocalCopy { get; set; }
    }

    static class Program
    {
        static void Main(string[] args)
        {
            CoreServiceClient Client = new CoreServiceClient("basicHttp");
            // bluePrintInfo(Client);
            string str = string.Empty;
            // str = lookUpPublication("300 Multi-brand Content ES");

            if (listComponent(Client, "test"))
            {
                Console.WriteLine("Connected");
                Console.ReadLine();

            }
            else
            {

                Console.WriteLine("not Connected");
                Console.ReadLine();
            }
        }


        public static bool bluePrintInfo(CoreServiceClient client)
        {
            client.ClientCredentials.Windows.ClientCredential.Domain = ConfigurationSettings.AppSettings["Domain"];
            client.ClientCredentials.Windows.ClientCredential.UserName = ConfigurationSettings.AppSettings["User"]; 
            client.ClientCredentials.Windows.ClientCredential.Password = ConfigurationSettings.AppSettings["PWD"];
            UsingItemsFilterData usingItemsFilterData = new UsingItemsFilterData
            {

                ItemTypes = new[] { ItemType.PublicationTarget, ItemType.Publication, ItemType.Component, ItemType.Page, ItemType.Keyword }
             ,
                IncludeLocalCopies = true,
                BaseColumns = ListBaseColumns.Extended,


            };

            PublicationsFilterData filter = new PublicationsFilterData
            {
                BaseColumns = ListBaseColumns.IdAndTitle
            };
            XElement publications = client.GetSystemWideListXml(filter);


            XElement elist = client.GetListXml("tcm:3-1973", usingItemsFilterData);

            List<Result> lstRS = new List<Result>();

            IEnumerable<XElement> usingXML1 = (from el in elist.Elements()
                                                   //where (string)el.Attribute("IsPublished").Value == "true"
                                               select el);


            return true;
        }
        public static bool listComponent(CoreServiceClient client, string path)
        {
            try
            {

                client.ClientCredentials.Windows.ClientCredential.Domain = ConfigurationSettings.AppSettings["Domain"];
                client.ClientCredentials.Windows.ClientCredential.UserName = ConfigurationSettings.AppSettings["User"]; 
                client.ClientCredentials.Windows.ClientCredential.Password = ConfigurationSettings.AppSettings["PWD"];// 


                XElement elist = client.GetListXml(ConfigurationSettings.AppSettings["FolderTCM_ID"].ToString(), new OrganizationalItemItemsFilterData
                {
                    ItemTypes = new[] { ItemType.Component },
                    Recursive = true,
                    BaseColumns = ListBaseColumns.Default
                });

                List<Result> lstRS = new List<Result>();
                System.Data.DataTable dt = new System.Data.DataTable();
                Stopwatch watch = new Stopwatch();
                TimeSpan timeSpan = new TimeSpan();

                watch.Start();
                IEnumerable<XElement> elist1 = (from el in elist.Elements()
                                                    //where (string)el.Attribute("IsPublished").Value == "true"
                                                select el);
                foreach (var x in elist1.ToArray())
                {
                    Result rs = new Result();
                    try
                    {





                        UsingItemsFilterData usingItemsFilterData = new UsingItemsFilterData
                        {

                            ItemTypes = new[] { ItemType.PublicationTarget, ItemType.Publication, ItemType.Component, ItemType.Page, ItemType.Keyword }
                              ,
                            IncludeLocalCopies = true
                        };

                        XElement usingXML = client.GetListXml(x.FirstAttribute.Value, usingItemsFilterData);
                        #region Comment
                        //BluePrintChainFilterData filter = new BluePrintChainFilterData();
                        //filter.Direction = BluePrintChainDirection.Down;
                        //XElement result = client.GetListXml(x.FirstAttribute.Value, filter);
                        //IEnumerable<XElement> result1 = (from el in result.Elements()
                        //                                     //where (string)el.Attribute("IsPublished").Value == "true"
                        //                                 select el);
                        //StringBuilder sbBlueChain = new StringBuilder();
                        //StringBuilder sbLocal = new StringBuilder();
                        //foreach (var z in result1.ToArray())
                        //{
                        //    sbBlueChain.Append(z.FirstAttribute.Value + " ; ");
                        //    if (z.Attribute("FromPub") != null)
                        //    {
                        //        sbLocal.Append(z.Attribute("FromPub").Value + " ; ");
                        //    }

                        //}
                        //rs.Blueprint_Chain_TCM_ID = sbBlueChain.ToString();
                        // rs.LocalCopy = sbLocal.ToString();
                        #endregion
                        IEnumerable<XElement> usingXML1 = (from el in usingXML.Elements()
                                                           select el);
                        StringBuilder sbComponent = new StringBuilder();
                        StringBuilder sbKeyword = new StringBuilder();
                        StringBuilder sbPage = new StringBuilder();
                        StringBuilder sbPublicationNameWL = new StringBuilder();
                        StringBuilder sbPublicationNameWOL = new StringBuilder();
                        if (usingXML1.ToArray().Count() < 2 && usingXML1.ToArray().Count() != 0)
                        {
                            rs.TCM_ID = x.FirstAttribute.Value;
                            rs.Title = x.Attribute("Title").Value;
                            foreach (var y in usingXML1.ToArray())
                            {

                                if (y.Attribute("Type").Value == "16" && y.Attribute("CommentToken") != null)
                                {
                                    sbComponent.Append(y.FirstAttribute.Value + " ; ");
                                    sbPublicationNameWL.Append(y.Attribute("Publication").Value + " ; ");
                                }
                                if (y.Attribute("Type").Value == "16" && y.Attribute("CommentToken") == null)
                                {
                                    sbComponent.Append(y.FirstAttribute.Value + " ; ");
                                    sbPublicationNameWOL.Append(y.Attribute("Publication").Value + " ; ");
                                    #region Demote
                                    //Demote Logic
                                    //string strPubID = string.Empty;
                                    //strPubID = lookUpPublication(y.Attribute("Publication").Value);
                                    //OperationInstruction instruction = new OperationInstruction { Mode = OperationMode.FailOnError };
                                    //OperationResultDataOfRepositoryLocalObjectData result = client.Promote(x.FirstAttribute.Value, strPubID, instruction, new ReadOptions());

                                    //if (result.ValidationWarnings.Length > 0)
                                    //{
                                    //    Console.WriteLine("Validation warnings:");
                                    //    foreach (ValidationWarningData warning in result.ValidationWarnings)
                                    //    {
                                    //        Console.WriteLine("\tMessage:{0} | Source:{1} | Location:{2}",
                                    //            warning.Message, warning.Source, warning.Location);
                                    //    }
                                    //}

                                    #endregion

                                }

                                //if (y.Attribute("Type").Value == "64")
                                //{
                                //    sbPage.Append(y.FirstAttribute.Value + " ; ");
                                //    sbPublicationNameWL.Append(y.Attribute("Publication").Value + " ; ");
                                //}

                            }
                            rs.Using_Component_Tcm_ID = (string.IsNullOrEmpty(sbComponent.ToString())) ? "NA" : sbComponent.ToString();
                            rs.using_Page_tcm_id = (string.IsNullOrEmpty(sbPage.ToString())) ? "NA" : sbPage.ToString();
                            // rs.using_keyword_tcm_id = (string.IsNullOrEmpty(sbKeyword.ToString())) ? "NA" : sbKeyword.ToString();
                            rs.Using_Publication_name_With_Local = (string.IsNullOrEmpty(sbPublicationNameWL.ToString())) ? "NA" : sbPublicationNameWL.ToString();
                            rs.Using_Publication_name_Without_Local = (string.IsNullOrEmpty(sbPublicationNameWOL.ToString())) ? "NA" : sbPublicationNameWOL.ToString();
                            if (rs.TCM_ID != null)
                            {
                                lstRS.Add(rs);
                                rs = null;

                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        ExcelUtility.WriteLog(ex, "elist1");
                        rs = null;
                    }
                    timeSpan = TimeSpan.FromSeconds(Convert.ToInt32(watch.Elapsed.TotalSeconds));
                    Console.Write(timeSpan.ToString("c"));
                    Console.Write('\r');

                }

                lstRS = lstRS.AsEnumerable().ToList();
                dt = ToDataTable(lstRS);
                dt.TableName = "REPORT";
             


                ExcelUtility.WriteDataTableToExcel(dt, @"Report.xlsx");
                Console.WriteLine("Total Records Are " + lstRS.Count() + System.Environment.NewLine);
                Console.WriteLine("Total Time Taken " + timeSpan.Hours + ":" + timeSpan.Minutes + ":" + timeSpan.Seconds + ":" + timeSpan.Milliseconds);
                //var x= elist.Descendants().Count();
                return true;
            }
            catch (Exception ex)
            {
                ExcelUtility.WriteLog(ex, "MainFunction");
                return false;
            }

        }


        public static string lookUpPublication(string pubName)
        {
            string publication_id = string.Empty;
            XElement Publication = XElement.Load(Environment.CurrentDirectory + "\\" + "PublicationLookUp.xml");
            var x = (from el in Publication.Elements()
                     where (string)el.Attribute("Title").Value == pubName
                     select el.FirstAttribute).ToArray()[0].Value;

            return x.ToString();
        }

        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

    }

}
