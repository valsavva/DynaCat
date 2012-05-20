using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Lunohod.Objects;

namespace Lunohod
{
	public class PerfMon
	{
		private static Dictionary<string, Stopwatch> sws = new Dictionary<string, Stopwatch>();
		private static Dictionary<string, int> counts = new Dictionary<string, int>();
		private static StringBuilder sb = new StringBuilder();
		
		[Conditional("PERFMON")]
		public static void Start(string c)
		{
			if (!sws.ContainsKey(c))
			{
				Stopwatch sw = new Stopwatch();
				sw.Start();
				sws.Add(c, sw);
				counts.Add(c, 1);
			}
			else
			{
				sws[c].Start();
				counts[c] += 1;
			}
		}
		
		[Conditional("PERFMON")]
		public static void Stop(string c)
		{
			Stopwatch sw;
			sw = sws[c];
			sw.Stop();
		}

		[Conditional("PERFMON")]
		public static void Reset()
		{
			sws.Clear();
			counts.Clear();
		}

		[Conditional("PERFMON")]
		public static void Reset(string c)
		{
			if (sws.ContainsKey(c))
			{
				sws.Remove(c);
				counts.Remove(c);
			}
		}

		[Conditional("PERFMON")]
		public static void Dump()
		{
			sb.Clear();

			sb.Append("*** Dump ***\r\n");

			var keys = new List<string>(sws.Keys);
			keys.Sort();

			foreach(var key in keys)
			{
				sb.Append(string.Format("{0} : Time: {1} Count: {2}\r\n", key, sws[key].Elapsed, counts[key]));
			}
			
			sb.Append("***      ***\r\n");
			
			Console.WriteLine(sb);
		}
	}
}

