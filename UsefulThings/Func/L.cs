using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;
using System.IO.Compression;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Windows;

namespace UsefulThings
{
    public class L
    {
        public static void AppDomain_CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            SendDataAndShowMessage((Exception)e.ExceptionObject);
            System.Windows.Forms.Application.Exit();
        }

        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            SendDataAndShowMessage(e.Exception);
            //на этом месте можно предложить "попытать продолжить работу" (ну-ну) или закрыть приложение
            System.Windows.Forms.Application.Exit();
        }

        public static void SendDataAndShowMessage(System.Exception ex)
        {
            string text = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "\n\r\n\r" + ex.ToString();// тут хорошо бы его отформатировать, скажем, в XML, добавить данные о времени, дате, железе и софте... 
            text += "\n\r\n\rСтек вызовов:\n\r " + ex.StackTrace + "\n\r\n\rМетод:\n\r\n\r" + ex.TargetSite;
            try
            {
                System.Windows.Forms.MessageBox.Show("Произошла ошибка. Информация об ошибке отправлена разработчику.\n\n" + text, "Ошибка программы");
            }
            catch (System.Exception ex2)
            {
                System.Windows.Forms.MessageBox.Show("Произошла ошибка. Информацию об ошибке отправить разработчику не удалось.\n\n" + ex2.ToString(), "\nОшибка программы");
            }
        }

        public static void LW(string log_string, Exception exc = null)
        {
            DateTime date = DateTime.Now;
            string time = String.Format("[{0:t}] ", date);
            string text = time + " ";
            if (exc != null)
            {
                text += log_string + "\n" + exc.ToString() + "\n" + exc.StackTrace;
            }
            else
            {
                text += log_string + "\n";
            }

            try
            {
                using (StreamWriter sw = File.AppendText(@"log.txt"))
                {
                    //sw.WriteLine(text + "\n");
                    sw.WriteLine(text);
                    Console.WriteLine(text);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Исключение при доступе к логу\n" + ex.ToString());
            }
        }

        public static void LW(Exception exc)
        {
            string text = "Словили исключение\n" + exc.ToString() + "\n" + exc.StackTrace;
            try
            {
                using (StreamWriter sw = File.AppendText(@"log.txt"))
                {
                    sw.WriteLine(text + "\n");
                    Console.WriteLine(text);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Исключение при доступе к логу\n" + ex.ToString());
            }
        }

        public static void Trace(
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                LW("member name: " + memberName + "\n" + "file path: " + sourceFilePath + "\n" + "line number: " + sourceLineNumber);
            }
            catch { }
        }
    }
}
