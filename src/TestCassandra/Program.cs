using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cassandra;

namespace TestCassandra
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Cluster cluster = Cluster.Builder().AddContactPoints("134.220.228.101").Build();

            var session = cluster.Connect("mykeyspace");
            var rs = session.Execute("SELECT * FROM dictionary_entries");
            foreach (var row in rs)
            {
                var value = row.GetValue<string>(0);
                System.Console.WriteLine(value);

                
            }

        }
    }
}
