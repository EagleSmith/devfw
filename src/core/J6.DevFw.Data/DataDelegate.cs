//
//
//  Copryright 2011 @ S1N1.COM.All rights reseved.
//
//  Project : OPS.Data
//  File Name : ReadDataFunc.cs
//  Date : 8/19/2011
//  Author : ����
//  Modify:
//  2013-05-26  18:00   newmin [+]: SqlEntityHandler
//
//

using System.Data.Common;

namespace JR.DevFw.Data
{
    /// <summary>
    /// ���ݶ�ȡ������
    /// </summary>
    /// <param name="reader"></param>
    public delegate void DataReaderFunc(DbDataReader reader);

    /// <summary>
    /// SQLʵ�����
    /// </summary>
    /// <param name="sql"></param>
    public delegate void SqlEntityHandler(SqlQuery sql);
}