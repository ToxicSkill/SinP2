using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Management;
using System.Diagnostics;

namespace SinP2
{
    class SystemInfo
    {

        public string MachineName = "";
        public string OSName = "";
        public string CPUName = "";
        public string GPUName = "";
        public string RAMName = "";
        public string DriveName = "";
        public List<string> ProcessorInfo = new List<string>();
        public List<string> VisualInfo = new List<string>();
        public List<string> RAMInfo = new List<string>();
        public List<List<string>> DrivesInfo = new List<List<string>>();

        public SystemInfo()
        {
            var os = Environment.OSVersion;
            OSName = os.VersionString + " Platform " + os.Platform + " V " + os.Version;
            MachineName = Environment.MachineName;
            getProcessor();
            if (ProcessorInfo.Count != 0)
                CPUName = ProcessorInfo[0];
            else
                CPUName = "N/A";

            getVisual();
            if (VisualInfo.Count != 0)
                GPUName = VisualInfo[0];
            else
                GPUName = "N/A";

            getRAM();

            getLogicalDrivers();

        }
        private void getProcessor()
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
            {
                ProcessorInfo.Add((string)mo["Name"]);
            }
        }

        private void getVisual()
        {
            using (var searcher = new ManagementObjectSearcher("select * from Win32_VideoController"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    VisualInfo.Add((string)obj["Name"]);
                }
            }
        }

        private void getRAM()
        {
            string Query = "SELECT Capacity FROM Win32_PhysicalMemory";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(Query);

            UInt64 Capacity = 0,
                temp = 0;

            foreach (ManagementObject WniPART in searcher.Get())
            {
                temp = Convert.ToUInt64(WniPART.Properties["Capacity"].Value);
                temp /= (ulong)Math.Pow(1024, 3);
                RAMInfo.Add(temp.ToString());
                Capacity += temp;
            }

            // TEST
            //RAMInfo[0] = "5";
            //RAMInfo[1] = "2";
            //RAMInfo[2] = "3";
            //RAMInfo[3] = "2";

            if (RAMInfo.Count != 0)
            {
                string checkSameValues = RAMInfo[0];
                bool identical = true;
                foreach (var single in RAMInfo)
                {
                    if (single != checkSameValues)
                        identical = false;
                }
                if (identical)
                    RAMName = "Total " + Capacity.ToString() + "GB  | " + RAMInfo.Count.ToString() + " x " + RAMInfo[0].ToString() + "GB |";
                else
                {
                    RAMName = "Total " + Capacity.ToString() + "GB  |";

                    for (var i = 0; i < RAMInfo.Count; ++i)
                    {
                        RAMName += RAMInfo[i].ToString() + "GB | ";
                    }
                }
            }
            else
                RAMName = "N/A";
        }
        private void getLogicalDrivers()
        {
            foreach (var drive in System.IO.DriveInfo.GetDrives())
            {
                double freeSpace = drive.TotalFreeSpace;
                double totalSpace = drive.TotalSize;
                double percentFree = (freeSpace / totalSpace) * 100;
                float num = (float)percentFree;
                string percent = percentFree.ToString(),
                    template = drive.ToString(),
                    totalSize = (drive.TotalSize / (Math.Pow(1024, 3))).ToString();
                string[] split = percent.Split(',');
                percent = split[0] + "% ";
                split = null;
                split = totalSize.Split(',');
                totalSize = split[0] + "GB ";

                List<string> disc = new List<string>();

                disc.Add(template.Remove(1,2) +":");
                disc.Add(percent);
                disc.Add(totalSize);

                DrivesInfo.Add(new List<string>(disc));
            }
            if (DrivesInfo.Count != 0)
            {
                for (var i = 0; i < DrivesInfo.Count; ++i)
                {
                    for (var l = 0; l < DrivesInfo[i].Count; ++l)
                        DriveName += DrivesInfo[i][l] + " ";
                    DriveName += "| ";
                }
                DriveName.Remove(DriveName.Length - 1, 1);
            }
            else
                DriveName = "N/A";
        }

        public string getCurrentCpuUsage()
        {
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            return cpuCounter.NextValue() + "%";
        }

        public string getAvailableRAM()
        {
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            return ramCounter.NextValue() + "MB";
        }
    }
}
