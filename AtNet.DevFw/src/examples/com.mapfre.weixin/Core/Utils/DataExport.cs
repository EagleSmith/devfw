﻿/**
 * Copyright (C) 2007-2015 OPSoft INC,All rights reseved.
 * Get more infromation of this software,please visit site http://cms.ops.cc
 * 
 * name : ExportItem.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */

using System.Collections;
using System.Collections.Generic;
using System.Data;
using AtNet.DevFw.Toolkit.Data.Export;

namespace Com.Plugin.Core.Utils
{
    public class ExportItem:BaseDataExportPortal
    {
        public ExportItem(string queryName, DataColumnMapping[] columns):base(columns)
        {
            this.PortalKey = queryName;
        }

        public override DataTable GetShemalAndData( Hashtable hash,out int totalCount)
        {
            return IocObject.Data.GetQueryView(
                this.PortalKey,
                hash,
                hash.ContainsKey("pageSize") ? int.Parse(hash["pageSize"].ToString()) : 100000,
                hash.ContainsKey("pageIndex") ? int.Parse(hash["pageIndex"].ToString()) : 1, out totalCount);
        }

        public override DataRow GetTotalView(Hashtable hash)
        {
            return IocObject.Data.GetTotalView(
               this.PortalKey,
                hash);
        }


        public override sealed string PortalKey { get; set; }
    }


    public class ExportItemManager
    {
        private static readonly IDictionary<string, IDataExportPortal> exportPortals;

        static ExportItemManager()
        {
            exportPortals = new Dictionary<string, IDataExportPortal>();
        }

        public static IDataExportPortal GetPortal(string queryName)
        {
            if (!exportPortals.Keys.Contains(queryName))
            {
                DataColumnMapping[] columns =
                    ExportUtil.GetColumnMappings(IocObject.Data.GetColumnMappingString(queryName));

                exportPortals.Add(queryName,new ExportItem(queryName,columns));
            }
            return exportPortals[queryName];
        }
    }
}