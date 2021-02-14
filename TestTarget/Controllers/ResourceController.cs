using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Threading;

namespace TestTarget.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResourceController : ControllerBase
    {
        public static int reqstCounter = 0;
        [HttpGet]
        [Route("ShortMem")]
        //e.g. https://localhost:<port>/resource/ShortMem/?count=200
        public List<Employee> ShortMem(int count = 100)
        {
            reqstCounter += 1;
               DoRandomStuff(reqstCounter);
         var ret = new List<Employee>(count);
            while (count-- >= 0)
            {
                ret.Add(new Employee());
            }
            return ret;
        }

        [HttpGet]
        [Route("LargeMem")]
        public StringObj LargeMem(int count = 100)
        {
            reqstCounter += 1;
            DoRandomStuff(reqstCounter);
            var ret = new StringObj { Value = string.Join(' ', LoremNET.Lorem.Paragraphs(300, 20, 30)) };
            return ret;
        }
        [HttpGet]
        [Route("LargeCpu")]
        public StringObj LargeCpu(int count = 100)
        {
            reqstCounter += 1;
            DoRandomStuff(reqstCounter);
            var ret = new StringObj { Value = new PrimeCalculator().FindPrimeNumber(count).ToString() };
            return ret;
        }

        private void DoRandomStuff(int number)
        {
            switch (number/(new Random().Next(11,37)))
            {
                case 3: Thread.Sleep(40); break;
                case 7: throw new Exception($"Exception when request counter = {reqstCounter}");
            }
        }
    }

    public class PrimeCalculator
    {
        public long FindPrimeNumber(int n)
        {
            int count = 0;
            long a = 2;
            while (count < n)
            {
                long b = 2;
                int prime = 1;
                while (b * b <= a)
                {
                    if (a % b == 0)
                    {
                        prime = 0;
                        break;
                    }
                    b++;
                }
                if (prime > 0)
                {
                    count++;
                }
                a++;
            }
            return (--a);
        }
    }
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public int Experience { get; set; }
        public Employee()
        {
            var rand = new Random(DateTime.Now.Millisecond);
            EmployeeId = rand.Next(100000);
            Name = LoremNET.Lorem.Sentence(30);
            Designation = LoremNET.Lorem.Words(3);
            Experience = rand.Next(50);
        }
    }
    public class StringObj
    {
        public string Value { get; set; }
    }


}
