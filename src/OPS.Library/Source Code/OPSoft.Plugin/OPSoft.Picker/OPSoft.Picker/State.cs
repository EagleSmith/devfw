//
//
//  Copyright 2011 @ OPSoft Inc.all rights reseved.
//
//  Project : Untitled
//  File Name : State.cs
//  Date : 2011/8/25
//  Author : 
//
//


namespace OPSoft.Plugin.NetCrawl
{
    /// <summary>
    /// �ɼ�״̬
    /// </summary>
    public class State
    {
        /// <summary>
        /// ����
        /// </summary>
        public int TotalCount { get; internal set; }

        /// <summary>
        /// ʧ����
        /// </summary>
        public int FailCount { get; internal set; }

        /// <summary>
        /// �ɹ���
        /// </summary>
        public int SuccessCount { get; internal set; }
    }
}