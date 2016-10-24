using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asg2A_rxs161630
{
    class FileManager
    {
        //Function to read and analyze the input file
        public static TimeModel doOperations(FileStream rebateEntry)
        {
            List<DateTime[]> time = new List<DateTime[]>();
            List<TimeSpan> timesSE = new List<TimeSpan>();
            List<TimeSpan> idle = new List<TimeSpan>();
            TimeSpan minTimeEntry = DateTime.Now.Subtract(DateTime.MinValue), maxTimeEntry = DateTime.Now.Subtract(DateTime.Now);
            TimeSpan minIdle = DateTime.Now.Subtract(DateTime.MinValue), maxIdle = DateTime.Now.Subtract(DateTime.Now);
            DateTime earliestStart = DateTime.Now, latestFinish = DateTime.MinValue;
            TextReader reader = new StreamReader(rebateEntry);
            String entrys;
            try
            {
                //Read all relevant DateTime fields from the input file
                while ((entrys = reader.ReadLine()) != null)
                {
                    String[] entryItems = entrys.Split('\t');
                    DateTime[] dt = new DateTime[4];
                    bool entryStart = DateTime.TryParse(entryItems[10], out dt[0]);
                    bool entryEnd = DateTime.TryParse(entryItems[11], out dt[1]);
                    bool onIdle = DateTime.TryParse(entryItems[14], out dt[2]);
                    bool onStart = DateTime.TryParse(entryItems[15], out dt[3]);
                    time.Add(dt);
                }
                TimeModel model = new TimeModel();
                model.filePath = Path.GetFullPath("CS6326Asg2.txt");
                reader.Close();
                rebateEntry.Close();
                foreach (DateTime[] entry in time)
                {
                    //Extracting Individual Values for Entries in Record
                    DateTime startTime = entry[0];
                    DateTime endTime = entry[1];
                    DateTime onIdle = entry[2];
                    DateTime onStart = entry[3];
                    TimeSpan eMinusS = endTime.Subtract(startTime);
                    TimeSpan iMinusS = onStart.Subtract(onIdle);

                    //Determining values via comparison
                    if (earliestStart > startTime)
                        earliestStart = startTime;
                    if (latestFinish < endTime)
                        latestFinish = endTime;
                    if (minTimeEntry > eMinusS)
                        minTimeEntry = eMinusS;
                    if (maxTimeEntry < eMinusS)
                        maxTimeEntry = eMinusS;
                    if (minIdle > iMinusS)
                        minIdle = iMinusS;
                    if (maxIdle < iMinusS)
                        maxIdle = iMinusS;

                    timesSE.Add(eMinusS);
                    idle.Add(iMinusS);
                }
                model.totalNumberOfRecords = time.Count();

                double doubleAverageTicks = timesSE.Average(timeSpan => timeSpan.Ticks);
                long longAverageTicks = Convert.ToInt64(doubleAverageTicks);
                TimeSpan avgTicks = new TimeSpan(longAverageTicks);

                double doubleAverageTicksIdle = idle.Average(timeSpan => timeSpan.Ticks);
                long longAverageTicksIdle = Convert.ToInt64(doubleAverageTicksIdle);
                TimeSpan avgTicksIdle = new TimeSpan(longAverageTicksIdle);

                model.minTimeEntry = minTimeEntry.Hours + "h " + minTimeEntry.Minutes + "m " + minTimeEntry.Seconds + "s " + minTimeEntry.Milliseconds + "ms";

                model.maxTimeEntry = maxTimeEntry.Hours + "h " + maxTimeEntry.Minutes + "m " + maxTimeEntry.Seconds + "s " + maxTimeEntry.Milliseconds + "ms";

                model.avgTimeEntry = avgTicks.Hours + "h " + avgTicks.Minutes + "m " + avgTicks.Seconds + "s " + avgTicks.Milliseconds + "ms";

                model.minIdle = minIdle.Hours + "h " + minIdle.Minutes + "m " + minIdle.Seconds + "s " + minIdle.Milliseconds + "ms";

                model.maxIdle = maxIdle.Hours + "h " + maxIdle.Minutes + "m " + maxIdle.Seconds + "s " + maxIdle.Milliseconds + "ms";

                model.avgIdle = avgTicksIdle.Hours + "h " + avgTicksIdle.Minutes + "m " + avgTicksIdle.Seconds + "s " + avgTicksIdle.Milliseconds + "ms";

                TimeSpan difFirstLast = latestFinish.Subtract(earliestStart);
                model.difFirstLast = difFirstLast.Hours + "h " + difFirstLast.Minutes + "m " + difFirstLast.Seconds + "s " + difFirstLast.Milliseconds + "ms";

                model.timeOfAnalysis = DateTime.Now;

                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        //Function to write analysis into a file
        public static void saveAnalysis(String FileName, TimeModel model)
        {
            FileStream file = File.Create(FileName);
            StreamWriter writer = new StreamWriter(file);
            writer.WriteLine("Number of Records Analysed:\t" + model.totalNumberOfRecords);
            writer.WriteLine("Minimum Entry Time of Record:\t" + model.minTimeEntry);
            writer.WriteLine("Maximum Entry Time of Record:\t" + model.maxTimeEntry);
            writer.WriteLine("Minimum Time b/w Records:\t" + model.minIdle);
            writer.WriteLine("Maximum Time b/w Records:\t" + model.maxIdle);
            writer.WriteLine("Average Entry Time of Record:\t" + model.avgTimeEntry);
            writer.WriteLine("Average Waiting b/w Entering Records:\t" + model.avgIdle);
            writer.WriteLine("Time from First to Last Entry:\t" + model.difFirstLast);
            writer.WriteLine("Time of Analysis:\t" + model.timeOfAnalysis.ToLongTimeString() + " " + model.timeOfAnalysis.ToLongDateString());
            writer.Close();
            file.Close();
        }
    }
}
