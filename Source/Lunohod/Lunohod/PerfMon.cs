using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Lunohod.Objects;

namespace Lunohod
{
	public class PerfMon
	{
		private static Dictionary<string, TimeSpan> times = new Dictionary<string, TimeSpan>();
		
		private static Dictionary<string, Stopwatch> sws = new Dictionary<string, Stopwatch>();
		
		public static void Start(string c)
		{
			if (!sws.ContainsKey(c))
			{
				Stopwatch sw = new Stopwatch();
				sw.Start();
				sws.Add(c, sw);
			}
		}
		
		public static void Stop(string c)
		{
			Stopwatch sw;
			sw = sws[c];
			sw.Stop();
			
			if (times.ContainsKey(c))
				times[c] += sw.Elapsed;
			else
				times[c] = sw.Elapsed;
		}
		
		public static string Dump()
		{
			
			StringBuilder sb = new StringBuilder();

			sb.Append("*** Dump ***\r\n");
			
			foreach(var item in times)
			{
				sb.Append(string.Format("{0} : {1}\r\n", item.Key, item.Value));
			}
			
			sb.Append("***      ***\r\n");
			
			return sb.ToString();
		}
	}
}

