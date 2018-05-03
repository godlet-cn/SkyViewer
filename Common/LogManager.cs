
using System;
using System.IO;
using System.Threading;

namespace Common
{
    public class LogManager
    {
        private static string LogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "SkyViewer", "logs");

        public static void WriteFileLog(Exception ex)
        {
            WriteFileLog("", "", "", ex);
        }

        public static void WiteLog(string logs)
        {
            WriteNomarlLog("", "", "", logs);
        }
        
        public static void WriteFileLog(string windowName, string className, string functionName, Exception ex)
        {
            try
            {
                // Get the Inner Exception
                if (null != ex.InnerException)
                {
                    WriteFileLog(windowName, className, functionName, ex.InnerException);
                    return;
                }

                //日志路径：地址配置参数
                string LogNow = DateTime.Now.ToString("yyyyMMdd");
                string ErrorLogPath = Path.Combine(LogPath, string.Format("LogCreate{0}.txt", LogNow));
                if (!Directory.Exists(LogPath))
                {
                    Directory.CreateDirectory(LogPath);
                }
                //string errorInfo = string.Format("{0} {1}", ex.Message, ex.StackTrace.Substring(ex.StackTrace.Length - 5 >= 0 ? ex.StackTrace.Length - 5 : 0));
                string errorInfo = string.Format("{0} {1}", ex.Message, ex.StackTrace);
                CreateFileLog(ErrorLogPath, string.Format("{0}　#{1}##{2}###{3}: {4}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), (string.IsNullOrWhiteSpace(windowName) ? "NoWindowForm" : windowName), className, functionName, errorInfo));
            }
            catch
            {
            }
        }

        /// <summary>
        /// By Suwmlee [ 2016 - 7 - 8 ]
        /// Edit Verion 6.1
        /// 正常文件日志
        /// </summary>
        /// <param name="windowName">窗体名称</param>
        /// <param name="className">类名称</param>
        /// <param name="functionName">函数名</param>
        /// <param name="logs">日志内容</param>
        public static void WriteNomarlLog(string windowName, string className, string functionName, string logs)
        {
            WriteNomarlLog("", windowName, className, functionName, logs);
        }


        /// <summary>
        /// By Suwmlee [ 2016 - 7 - 8 ]
        /// Edit Verion 6.1
        /// 正常文件日志
        /// </summary>
        /// <param name="windowName">窗体名称</param>
        /// <param name="className">类名称</param>
        /// <param name="functionName">函数名</param>
        /// <param name="logs">日志内容</param>
        public static void WriteNomarlLog(string fileTag, string windowName, string className, string functionName, string logs)
        {
            try
            {
                //日志路径：地址配置参数
                string LogNow = DateTime.Now.ToString("yyyyMMdd");
                string fileName = string.IsNullOrEmpty(fileTag) ? string.Format("NormalLogs{0}.txt", LogNow) : string.Format("NormalLogs{0}_{1}.txt", LogNow, fileTag);
                string ErrorLogPath = Path.Combine(LogPath, fileName);

                if (!Directory.Exists(LogPath))
                {
                    Directory.CreateDirectory(LogPath);
                }
                //string errorInfo = string.Format("{0} {1}", ex.Message, ex.StackTrace.Substring(ex.StackTrace.Length - 5 >= 0 ? ex.StackTrace.Length - 5 : 0));
                string normalInfo = string.Format("{0}", logs);
                CreateFileLog(ErrorLogPath, string.Format("{0}　#{1}##{2}###{3}: {4}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), (string.IsNullOrWhiteSpace(windowName) ? "NoWindowForm" : windowName), className, functionName, normalInfo));
            }
            catch
            {

            }
        }

        /// <summary>
        /// 直接向文件中写入日志
        /// </summary>
        /// <param name="file"></param>
        /// <param name="message"></param>
        public static void CreateFileLog(string file, string message)
        {
            lock (file)
            {
                using (StreamWriter sw = new StreamWriter(file, true, System.Text.Encoding.UTF8))
                {
                    sw.Write(message);
                    sw.Flush();
                }
                return;

                byte[] fileContent = System.Text.Encoding.GetEncoding("gb2312").GetBytes(message);
                Mutex exlock = new Mutex();
                exlock.WaitOne();
                FileStream fs = new FileStream(file, FileMode.Append, FileAccess.Write, FileShare.Write, 2048, true);

                try
                {
                    AutoResetEvent manualEvent = new AutoResetEvent(false);
                    IAsyncResult asyncResult = fs.BeginWrite(fileContent, 0, fileContent.Length,
                                                            new AsyncCallback(EndWriteCallback),
                                                            new WriteState(fs, manualEvent));
                }
                catch
                {
                }
                finally
                {
                    fs.Close();
                    exlock.Close();
                }
            }
        }

        // 异步写
        private static void EndWriteCallback(IAsyncResult asyncResult)
        {
            WriteState stateInfo = (WriteState)asyncResult.AsyncState;
            int workerThreads;
            int portThreads;
            try
            {
                ThreadPool.GetAvailableThreads(out workerThreads, out portThreads);
                stateInfo.fStream.EndWrite(asyncResult);
            }
            finally
            {
                stateInfo.autoEvent.Set();
            }
        }
    }

    #region 异步处理

    /// <summary>
    /// 异步请求区别类
    /// </summary>
    internal sealed class WriteState
    {
        /// <summary>
        /// 文件流
        /// </summary>
        public FileStream fStream;

        /// <summary>
        /// 事件
        /// </summary>
        public AutoResetEvent autoEvent;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fStream">文件流</param>
        /// <param name="autoEvent">事件</param>
        public WriteState(FileStream fStream, AutoResetEvent autoEvent)
        {
            this.fStream = fStream;
            this.autoEvent = autoEvent;
        }
    }
    #endregion
}
