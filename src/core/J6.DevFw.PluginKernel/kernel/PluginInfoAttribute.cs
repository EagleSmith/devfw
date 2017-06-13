using System;

namespace JR.DevFw.PluginKernel
{
    /// <summary>
    /// �����Ϣ
    /// </summary>
    [Obsolete]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PluginInfoAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public PluginInfoAttribute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="author"></param>
        /// <param name="webpage"></param>
        /// <param name="descript"></param>
        [Obsolete]
        public PluginInfoAttribute(string name, string author, string webpage, string descript)
        {
        }

        private string id;

        /// <summary>
        /// ������
        /// </summary>
        public string ID
        {
            get
            {
                if (id == null)
                {
                    id = String.Format("{0}{1}", GetType().Assembly.GetName().Name,
                        IndexNum == 0 ? string.Empty : "_" + IndexNum);
                }
                return id;
            }
        }

        /// <summary>
        /// ������,�������ļ���������������������
        /// </summary>
        public int IndexNum { get; set; }

        /// <summary>
        /// ���������Ϣ
        /// </summary>
        public object Tag { get; set; }
    }
}