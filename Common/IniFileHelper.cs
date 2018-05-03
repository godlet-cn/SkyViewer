using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Common
{
    public class IniFileHelper
    {

        public static string Class = typeof(IniFileHelper).ToString();

        public string Path;
        public IniFileHelper(string path)
        {
            this.Path = path;
        }

        #region 声明读写INI文件的API函数
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, Byte[] retVal, int size, string filePath);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringW", CharSet = CharSet.Unicode)]
        private static extern uint GetPrivateProfileStringByByteArray(string lpAppName, string lpKeyName, string lpDefault, byte[] lpReturnedString, uint nSize, string lpFileName);

        #endregion

        /**/
        /// <summary>
        /// 写INI文件
        /// </summary>
        /// <param name="section">段落</param>
        /// <param name="key">键</param>
        /// <param name="iValue">值</param>
        public void IniWriteValue(string section, string key, string iValue)
        {
            try
            {
                WritePrivateProfileString(section, key, iValue, this.Path);
            }
            catch (Exception ex)
            {
                LogManager.WriteFileLog("", Class, "InterfaceAttribute", ex);
            }
        }

        /**/
        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="section">段落</param>
        /// <param name="key">键</param>
        /// <returns>返回的键值</returns>
        public string IniReadValue(string section, string key)
        {
            string readValue = string.Empty;
            try
            {
                byte[] byteAr = new byte[1024];
                uint resultSize = GetPrivateProfileStringByByteArray(section, key, "", byteAr, (uint)byteAr.Length, this.Path);
                readValue = Encoding.Unicode.GetString(byteAr, 0, (int)resultSize * 2);

                // Old solution
                //int i = GetPrivateProfileString(section, key, "", temp, 255, this.Path);
            }
            catch (Exception ex)
            {
                LogManager.WriteFileLog("", Class, "string IniReadValue(string section, string key)", ex);
            }
            return readValue;
        }

        /**/
        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="Section">段，格式[]</param>
        /// <param name="Key">键</param>
        /// <returns>返回byte类型的section组或键值组</returns>
        public byte[] IniReadValues(string section, string key)
        {
            byte[] temp = new byte[255];
            try
            {
                int i = GetPrivateProfileString(section, key, "", temp, 255, this.Path);
            }
            catch (Exception ex)
            {
                LogManager.WriteFileLog("", Class, "byte[] IniReadValues(string section, string key)", ex);
            }
            return temp;
        }
    }
}
