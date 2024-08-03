using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;

namespace GetInfoFiles
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        public string outtext;

        private void button1_Click(object sender, EventArgs e)
        {
            string dir = textBox1.Text;
            string[] array = Directory.GetDirectories(dir);
            textBox2.Text += ("Количество директорий в корневой папке = " + array.Length + " \r\n");
            outtext += ("Количество директорий в корневой папке = " + array.Length + " \r\n");
            for (int i = 0; i < array.Length; i++)
            {

                //textBox2.Text += (array[i] + " \r\n");
                outtext += ("Папка №"+i+" "+array[i] + " \r\n\r\n");
                CreateCrcForDirectory(array[i]);
            }
        }

        private void CreateCrcForDirectory(string dir)
        {
            //throw new NotImplementedException();
            if (!Directory.Exists(dir))
            {
                MessageBox.Show("Директория не существует - " + dir);
            }
            try
            {
                string[] array = Directory.GetFiles(dir);
                textBox2.Text += (dir + ". Количество файлов в директории " + array.Length + " \r\n");
                outtext += (dir + ". Количество файлов в директории " + array.Length + " \r\n");
                int i = 0;
                while (i < array.Length)
                {
                    string text = array[i];
                    //textBox2.Text += "Файл - " +text + "\r\n";
                    try
                    {
                        FileInfo fileObj = new FileInfo(text);
                        /*List<string> vuFileExtList = new List<string> { ".txt",".log",".bak",".tgd",".dat",".xml",".evt",};
                        if (vuFileExtList.Any((string str) => string.Equals(fileObj.Extension, str, StringComparison.CurrentCultureIgnoreCase)))
                        {}*/
                        List<string> FormatList = new List<string> { ".dll", ".exe" };
                        /*string formatfile = fileObj.Extension;
                        textBox2.Text += "Расширение файла: " + formatfile + "\r\n";*/
                        if (FormatList.Any((string fl) => string.Equals(fileObj.Extension, fl, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            outtext += "Файл - " +text + "\r\n";
                            string ProductVersion = FileVersionInfo.GetVersionInfo(text).ProductVersion ?? "Нет версии";
                            //textBox2.Text += "Версия продукта: " + ProductVersion + "\r\n";
                            outtext += "Версия продукта: " + ProductVersion + "\r\n";
                            string text2 = FileVersionInfo.GetVersionInfo(text).FileVersion ?? "Нет версии";
                            outtext += "Версия файла: " + ProductVersion + "\r\n";
                            //textBox2.Text += "Версия файла: " + ProductVersion + "\r\n";
                            byte[] array2 = CreateHash(text);
                            if (array2 == null)
                            {
                                //textBox2.Text += "Хэш не вычислен. \r\n";
                                outtext += "Хэш не вычислен. \r\n";
                            }
                            else
                            {
                                //textBox2.Text += "Хэш: "+ BitConverter.ToString(array2) +"\r\n";
                                outtext += "Хэш: " + BitConverter.ToString(array2) + "\r\n\r\n";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    i++;
                }
                string[] arraydirs = Directory.GetDirectories(dir);
                for (i = 0; i < arraydirs.Length; i++)
                {
                    CreateCrcForDirectory(arraydirs[i]);
                }
                WriteLogToFile(outtext);
                outtext = "";
            }
            catch (Exception ex2)
            {
                MessageBox.Show("Общая ошибка создания контрольных сумм - " + ex2.Message);
            }
        }
        public void WriteLogToFile(string message)
        {
            try
            {
                string currentdir = Directory.GetCurrentDirectory();
                StreamWriter streamWriter = new StreamWriter(currentdir + "LogFile_" + DateTime.Now.ToString("yyyy-M-d_hh-mm") + ".log", true);
                streamWriter.WriteLine(string.Concat(new object[]
                {
                    /*"[",DateTime.Now,"] ",*/
                    message
                }));
                streamWriter.Close();
            }
            catch (Exception)
            {
            }
        }
        private static byte[] CreateHash(string filename)
        {
            byte[] result;
            try
            {
                using (MD5 md = MD5.Create())
                {
                    using (FileStream fileStream = File.OpenRead(filename))
                    {
                        result = md.ComputeHash(fileStream);
                    }
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
    }
}
