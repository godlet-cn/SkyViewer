using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ShareObject : MarshalByRefObject
    {
        public ShareObject()
        {
            State = WinState.NORMAL;
        }

        public  WinState State { get; set; }

        /// <summary>
        /// 主页地址
        /// </summary>
        public string HomeUrl { get; set; }

        /// <summary>
        /// X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 客户端宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 客户端高度
        /// </summary>
        public double Height { get; set; }
    }


    public enum WinState
    {
        NORMAL,
        MINIMAZE,
        MAXIMAZE,
        CLOSE
    }
}
