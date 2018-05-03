using System;

namespace Common
{
    [Serializable]
    public class ClientLocation
    {
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

    [Serializable]
    public class ClientSetting
    {
        public ClientSetting()
        {
            HomeUrl = "www.skyworth.com";
            Location = new ClientLocation() {
                X=0,
                Y=0,
                Width=1920,
                Height=1080
            };
        }

        /// <summary>
        /// 主页地址
        /// </summary>
        public string HomeUrl { get; set; }


        /// <summary>
        /// 显示位置
        /// </summary>
        public ClientLocation Location { get; set; }
    }

}
