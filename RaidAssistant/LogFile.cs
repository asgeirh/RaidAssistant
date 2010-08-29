using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RaidAssistant
{
    public class LogFile
    {
        string logfilename;
        StreamReader logReader;

        public LogFile(string logfile)
        {
            logfilename = logfile;

            Open();
        }

        private void Open()
        {
            try
            {
                FileStream stream = new FileStream(logfilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                logReader = new StreamReader(stream);
                logReader.BaseStream.Seek(0, SeekOrigin.End);
            }
            catch (Exception E)
            {
                System.Diagnostics.Debug.WriteLine(E.Message);
            }
        }

        public string ReadLine()
        {
            return logReader.ReadLine();
        }
    }
}
