//
//
//  Copryright 2011 @ OPSoft INC.All rights reseved.
//
//  Project : OPS.Data
//  File Name : DataBaseType.cs
//  Date : 8/19/2011
//  Author : ����
//
//

namespace AtNet.DevFw.Data
{
    /// <summary>
    /// ���ݿ�����
    /// </summary>
    public enum DataBaseType
    {
        SQLServer,

        /// <summary>
        /// SQLite���ݿ�
        /// </summary>
        SQLite,

        /// <summary>
        ///Mono SQLite 
        /// </summary>
        MonoSQLite,

        /// <summary>
        /// mysql���ݿ�
        /// </summary>
        MySQL,

        /// <summary>
        /// Access�����ݿ�
        /// </summary>
        OLEDB
    }
}