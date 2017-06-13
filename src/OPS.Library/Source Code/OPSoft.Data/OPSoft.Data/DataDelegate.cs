//
//
//  Copryright 2011 @ OPSoft INC.All rights reseved.
//
//  Project : OPS.Data
//  File Name : ReadDataFunc.cs
//  Date : 8/19/2011
//  Author : ����
//  Modify:
//  2013-05-26  18:00   newmin [+]: SqlEntityHandler
//
//

namespace Ops.Data
{
    using System.Data.Common;

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