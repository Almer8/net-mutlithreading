using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;


namespace NetPract1
{
    internal class Program
    {
        static List<List<int>> result = new List<List<int>>();

        static List<List<int>> divideList(List<int> list, int n)
        {
            int baseSize = list.Count/n;
            int reminder = list.Count%n;
            List<List<int>> res = new List<List<int>>();
            for (int i = 0; i < n-1; i ++)
            {
                res.Add(list.GetRange(i*baseSize,baseSize));
            }
            res.Add(list.GetRange(list.Count-baseSize-reminder,baseSize+reminder));
            return res;
        }

        static List<int> sort(List<int> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list.Count - 1; j++)
                {
                    if (list[j] > list[j + 1])
                    {
                        int temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
            }
            return list;
        }
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            Stopwatch multiStopWatch = new Stopwatch();

            Console.WriteLine("Please input number of elements to sort:");
            int arrLen = Convert.ToInt32(Console.ReadLine());
            if (arrLen < 1)
            {
                Console.WriteLine("Array size cant be less than 1");
                return;
            }
            Console.WriteLine("Please input number of threads to run:");
            int threadsNum = Convert.ToInt32(Console.ReadLine());
            if (threadsNum < 1)
            {
                Console.WriteLine("Number of threads cant be less than 1");
                return;
            }
            if (threadsNum > arrLen)
            {
                Console.WriteLine("Number of threads cant be less than array size");
                return;
            }
            List<int> list = new List<int>();
            Random random = new Random();
            for (int i = 0; i < arrLen; i++)
            {
                list.Add(random.Next());
            }
            stopWatch.Start();
            List<int> sortedList = new List<int>(sort(list));
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                       ts.Hours, ts.Minutes, ts.Seconds,
                       ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            List<Thread> threads = new List<Thread>();
            object lockObject = new object();
            multiStopWatch.Start();
            List<List<int>> dividedList = new List<List<int>>(divideList(list,threadsNum));
            for (int i = 0; i < threadsNum; i++)
            {
                int index = i;
                Thread thread = new Thread(()=> {
                    List<int> sortedPart = sort(list);
                    lock (lockObject)
                    {
                        result.Add(sortedPart);
                    }
                    });
                threads.Add(thread);
                thread.Start();
            }
            for(int i = 0;i < threadsNum; i++)
            {
                threads[i].Join();
            }
            List<List<int>> multSortedLists = result.OrderBy(x=>x.First()).ToList();
            List<int> finalList = multSortedLists.SelectMany(x => x).ToList();
            multiStopWatch.Stop();
            TimeSpan multiTs = multiStopWatch.Elapsed;
            string multiElapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                       multiTs.Hours, multiTs.Minutes, multiTs.Seconds,
                       multiTs.Milliseconds / 10);
            Console.WriteLine("Multithread RunTime " + multiElapsedTime);
        }
    }
}