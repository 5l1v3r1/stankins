﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Stankins.Interfaces;
using Stankins.Office;
using Stankins.Razor;
using Stankins.SqlServer;
using StankinsCommon;
using StankinsObjects;

namespace Stankins.SimpleRecipes
{
    public class ExportTableToExcelSql : BaseObjectInSerial<ReceiveTableDatabaseSql, SenderExcel>, IReceive, ISender
    {
        public ExportTableToExcelSql(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ExportTableToExcelSql);
        }
        //sql injection?
        public ExportTableToExcelSql(string connectionString, string nameTable, string fileName) : this(new CtorDictionary()
        {
            {"sql","select * from "+ nameTable },
            {nameof(fileName),Path.GetFileName(fileName)},
            {nameof(connectionString),connectionString },
            {"folderToSave", Path.GetDirectoryName(fileName)},
            {"connectionType",typeof(SqlConnection).AssemblyQualifiedName },
            {"addKey",false },
            {nameof(nameTable),nameTable}

        })
        {

        }
    }
}
