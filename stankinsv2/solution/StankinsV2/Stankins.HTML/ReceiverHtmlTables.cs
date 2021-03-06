﻿using HtmlAgilityPack;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.HTML
{
    public class ReceiverHtmlTables : ReceiverHtml
    {
        public ReceiverHtmlTables(CtorDictionary dataNeeded) :base(dataNeeded)
        {
            this.Name = nameof(ReceiverHtmlTables);
           
        }
        public ReceiverHtmlTables(string file, Encoding encoding) : base(file,encoding)
        {
            this.Name = nameof(ReceiverHtmlTables);
        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var tables = await base.Find("//table");

            if ((tables?.Count ?? 0) == 0)
                throw new ArgumentException("not found tables");

            var ret = new DataToSentTable();
           
            int nrTable = 0;
            foreach (var table in tables)
            {
                nrTable++;
                var dt = new DataTable
                {
                    TableName = $"Table{nrTable}"
                };

                string[] columnsNames = null;
                var head = table.SelectSingleNode("thead");
                if (head != null)
                {
                    var columns = head.SelectNodes("tr/th");
                    if ((columns?.Count ?? 0) == 0)
                    {
                        columns = head.SelectNodes("tr/td");
                    }
                    if (columns != null)
                    {

                        columnsNames = new string[columns.Count];
                        for (int i = 0; i < columns.Count; i++)
                        {
                            var colName = columns[i].InnerText;
                            columnsNames[i] = MakePretty(colName);
                        }
                    }
                }
                
                if ((columnsNames?.Length ?? 0) > 0)
                {
                    foreach (var item in columnsNames)
                    {
                        
                        dt.Columns.Add(new DataColumn(item, typeof(string)));                        
                    }
                    foreach (var item in columnsNames)
                    {
                        dt.Columns.Add(new DataColumn(item + "_html", typeof(string)));
                    }
                }
                HtmlNodeCollection rows;

                var body = table.SelectSingleNode("tbody");
                if (body != null)
                {
                    rows = body.SelectNodes("tr");
                }
                else
                {
                    rows = table.SelectNodes("tr");
                }

                foreach (var row in rows)
                {
                    var cells = row.SelectNodes("td");
                    if ((cells?.Count ?? 0) == 0)
                    {
                        cells = row.SelectNodes("th");
                    }
                    if ((cells?.Count ?? 0) == 0)
                        continue;

                    if ((columnsNames?.Length ?? 0) == 0)
                    {
                        columnsNames = new string[cells.Count];
                        for (int i = 0; i < columnsNames.Length; i++)
                        {
                            var colName = cells[i].InnerText;
                            columnsNames[i] = MakePretty(cells[i].InnerText);
                        }
                        foreach (var item in columnsNames)
                        {
                            
                            dt.Columns.Add(new DataColumn(item, typeof(string)));
                            
                        }
                        foreach (var item in columnsNames)
                        {
                            dt.Columns.Add(new DataColumn(item + "_html", typeof(string)));
                        }
                            continue;
                    }
                    if ((cells?.Count ?? 0) == 0)
                        continue;
                    var arr = cells.Select(it => it.InnerText).ToList();
                    arr.AddRange(cells.Select(it => it.InnerHtml).ToArray());
                    dt.Rows.Add(arr.ToArray());
                }
                var id= ret.AddNewTable(dt);
                ret.Metadata.AddTable(dt,id);
            }
            return ret;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }

        
    }
}
