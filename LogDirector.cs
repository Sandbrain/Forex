using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Windows.Forms;

namespace Forex
{
    /// <summary>
    /// Contains Readonly log Manager (Log4Net), and a method to log
    /// the error based on the Error level.
    /// </summary>
    class LogDirector
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
     
        public static bool GlobalError = false;

        /// <summary>
        /// Logs the error based on the ErrorLevel, also passed 
        /// the exception from the method the error occurs.
        /// </summary>
        /// <param name="ErrorLevel">int</param>
        /// <param name="e">Exception</param>
        public static void DoAction(int ErrorLevel, Exception e)
        {            
            //LogSender LS = new LogSender();

            switch (ErrorLevel)
            {
                case 0:
                    log.Debug("Debug", e);
                    break;
                case 1:
                    log.Info("Info", e);
                    GlobalError = true;
                    break;
                case 2:
                    log.Warn("Warn", e);
                    GlobalError = true;
                    break;
                case 3:
                    log.Error("Error", e);
                    MessageBox.Show("An error has been detected, the process will now stop.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                    //LS.SendLogger();                   
                    break;
                case 4:
                    log.Fatal("Fatal", e);
                    MessageBox.Show("An error has been detected, the process will now stop.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                    //LS.SendLogger();                   
                    break;
                default:
                    log.Info("Info", e);
                    break;
            }
        }        
    }
}

