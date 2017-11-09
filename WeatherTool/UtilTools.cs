using Spire.Doc;
using Spire.Doc.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Data.OleDb;

namespace WeatherTool
{
    class UtilTools
    {
        #region 文档读取处理
        /// <summary>
        /// 根据当前时间构造文件名
        /// </summary>
        /// <returns></returns>
        public static string initFileName()
        {
            DateTime now = DateTime.Now;
            string strDate = now.Date.ToString("yyyy年M月d日");
            int h = now.Hour;
            string strTime = h > 12 ? "下午" : "上午";
            string docname = ConfigurationManager.AppSettings["docname"].ToString();
            string file = string.Format(docname, strDate, strTime);

            return file;
        }
        /// <summary>
        /// 根据日期匹配文档并解析
        /// </summary>
        /// <returns></returns>
        public static List<string> SpireDocProcess()
        {
            string keyword = ConfigurationManager.AppSettings["keyword"].ToString();
            string path = Environment.CurrentDirectory + "\\files\\";
            string file = initFileName() + ".doc";
            Document document = new Document(path + file);
            List<string> ls = new List<string>();

            int f = 0;
            foreach (Section section in document.Sections)
            {
                //读取表格
                if (f == 0)
                {
                    string forecast = "";
                    Table tab = (Table)section.Tables[0];
                    foreach (TableRow tr in tab.Rows)
                    {
                        for (int j = 1; j < tr.Cells.Count; j++)
                        {
                            forecast += tr.Cells[j].Paragraphs[0].Text + ",";
                        }
                    }
                    ls.Add(forecast);
                    f++;
                }

                //读取文本
                int i = 0;
                foreach (Paragraph paragraph in section.Paragraphs)
                {
                    string txt = paragraph.Text;
                    if (keyword.Equals(txt))
                    {
                        i = 3;
                    }
                    else if (i > 0)
                    {
                        ls.Add(txt);
                        i--;
                    }
                }
            }
            return ls;
        }
        #endregion

        #region 文件下载
        /// <summary>
        /// WebClient上传文件至服务器
        /// </summary>
        /// <param name="fileNamePath">文件名，全路径格式</param>
        /// <param name="uriString">服务器文件夹路径</param>
        private static void UpLoadFile(string fileNamePath, string uriString)
        {
            string fileName = fileNamePath.Substring(fileNamePath.LastIndexOf("//") + 1);
            string NewFileName = DateTime.Now.ToString("yyMMddhhmmss") + DateTime.Now.Millisecond.ToString() + fileNamePath.Substring(fileNamePath.LastIndexOf("."));
            string fileNameExt = fileName.Substring(fileName.LastIndexOf(".") + 1);
            if (uriString.EndsWith("/") == false) uriString = uriString + "/";
            uriString = uriString + NewFileName;
            /**//// 创建WebClient实例
            WebClient myWebClient = new WebClient();
            myWebClient.Credentials = CredentialCache.DefaultCredentials;
            // 要上传的文件
            FileStream fs = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
            //FileStream fs = OpenFile();
            BinaryReader r = new BinaryReader(fs);
            try
            {
                //使用UploadFile方法可以用下面的格式
                //myWebClient.UploadFile(uriString,"PUT",fileNamePath);
                byte[] postArray = r.ReadBytes((int)fs.Length);
                Stream postStream = myWebClient.OpenWrite(uriString, "PUT");
                if (postStream.CanWrite)
                {
                    postStream.Write(postArray, 0, postArray.Length);
                }
                else
                {
                    MessageBox.Show("文件目前不可写！");
                }
                postStream.Close();
            }
            catch
            {
                MessageBox.Show("文件上传失败，请稍候重试~");
            }
        }

        /// <summary>
        /// 下载服务器文件至客户端
        /// </summary>
        /// <param name="URL">被下载的文件地址，绝对路径</param>
        /// <param name="Dir">另存放的目录</param>
        public static void DownloadWord(string URL, string Dir)
        {
            WebClient client = new WebClient();
            string fileName = URL.Substring(URL.LastIndexOf("/") + 1); //被下载的文件名
            string Path = Dir + fileName;   //另存为的绝对路径＋文件名

            try
            {
                WebRequest myre = WebRequest.Create(URL);
            }
            catch
            {
                //MessageBox.Show(exp.Message,"Error"); 
            }

            try
            {
                client.DownloadFile(URL, Path);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(exp.Message,"Error");
            }
        }
        #endregion

        #region 导入excel数据
        /// <summary>
        /// 导入excel数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool ImportExcel(string filePath)
        {
            try
            {
                //Excel就好比一个数据源一般使用
                //这里可以根据判断excel文件是03的还是07的，然后写相应的连接字符串
                string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + "Extended Properties=Excel 8.0;";
                OleDbConnection con = new OleDbConnection(strConn);
                con.Open();
                string[] names = GetExcelSheetNames(con);
                if (names.Length > 0)
                {
                    foreach (string name in names)
                    {
                        OleDbCommand cmd = con.CreateCommand();
                        cmd.CommandText = string.Format(" select * from [{0}]", name);//[sheetName]要如此格式
                        OleDbDataReader odr = cmd.ExecuteReader();
                        while (odr.Read())
                        {
                            if (odr[0].ToString() == "日期")//过滤列头  按你的实际Excel文件
                                continue;
                            //数据库添加操作
                            /*进行非法值的判断
                             * 添加数据到数据表中
                             * 添加数据时引用事物机制，避免部分数据提交
                             * Add(odr[1].ToString(), odr[2].ToString(), odr[3].ToString());//数据库添加操作，Add方法自己写的
                             * */

                        }
                        odr.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 查询表名
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public static string[] GetExcelSheetNames(OleDbConnection con)
        {
            try
            {
                System.Data.DataTable dt = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new[] { null, null, null, "Table" });//检索Excel的架构信息
                var sheet = new string[dt.Rows.Count];
                for (int i = 0, j = dt.Rows.Count; i < j; i++)
                {
                    //获取的SheetName是带了$的
                    sheet[i] = dt.Rows[i]["TABLE_NAME"].ToString();
                }
                return sheet;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}