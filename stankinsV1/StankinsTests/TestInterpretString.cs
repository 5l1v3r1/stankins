﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StringInterpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using StanskinsImplementation;
using StankinsInterfaces;
using SenderToFile;
using System.Diagnostics;
using Shouldly;

namespace StankinsTests
{
    [TestClass]
    public class TestInterpretString
    {
        
        public static string RandomString(int length)
        {
            var random = new Random(DateTime.Now.Second);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        [TestMethod]
        public void InterpretTextNone()
        {
            #region arrange
            string textToInterpret = RandomString(22);
            #endregion
            #region act

            var i = new Interpret();
            var str = i.InterpretText(textToInterpret);
            #endregion
            #region assert
            Assert.AreEqual(textToInterpret, str);
            #endregion
        }
        [TestMethod]
        public void InterpretSettingsFile()
        {
            #region arrange
            string textToInterpret = "this is from #file:SqlServerConnectionString#";
            #endregion
            #region act

            var i = new Interpret();
            var str = i.InterpretText(textToInterpret);
            #endregion
            #region assert
            Console.WriteLine("interpreted: " + str);
            Assert.IsFalse(str.Contains("#"),"should be interpreted");
            Assert.IsTrue(str.Contains("this is from"),"should contain first chars");
            //Assert.IsTrue(str.Contains("atabase"),"should contain database");
            Assert.IsTrue(str.Contains("rusted"),"should containt trusted connection");
            #endregion
            

        }
        [TestMethod]
        public void InterpretEnv()
        {
            #region arrange
            string textToInterpret = "";
            string textInterpreted = "";
            var var = Environment.GetEnvironmentVariables();
            foreach (var item in var.Keys)
            {
                var randomString = RandomString(10);
                textToInterpret += randomString + "#env:" + item + "#" + Environment.NewLine;
                textInterpreted += randomString + var[item]  + Environment.NewLine;
                continue;
            }
            #endregion
            #region act
            var i = new Interpret();
            i.TwoSlashes = false;
            var str = i.InterpretText(textToInterpret);
            #endregion
            #region assert
            Assert.AreEqual(textInterpreted, str);
            
            #endregion
            

        }
        [TestMethod]
        public void InterpretDateTime()
        {
            #region arrange
            string textToInterpret = "this is from #now:yyyyMMddHHmmss#";
            #endregion
            #region act

            var i = new Interpret();
            i.TwoSlashes = false;
            var str = i.InterpretText(textToInterpret);
            #endregion
            #region assert
            Console.WriteLine("interpreted: " + str);
            str.ShouldContain($"this is from {DateTime.Now.ToString("yyyyMMdd")}");            
            #endregion
        }
        [TestMethod]
        public void InterpretStaticOneParameter()
        {
            #region arrange
            string textToInterpret = "this is from #static:DateTime.Today#";
            textToInterpret += " and #static:Directory.GetCurrentDirectory()#";
                
            #endregion
            #region act

            var i = new Interpret();
            i.TwoSlashes = false;
            var str = i.InterpretText(textToInterpret);
            #endregion
            #region assert
            Console.WriteLine("interpreted: " + str);
            str.ShouldBe($"this is from {DateTime.Today.ToString()} and {Directory.GetCurrentDirectory()}");
            #endregion
        }
        [TestMethod]
        public void InterpretStaticParameterString()
        {
            #region arrange
            string textToInterpret = "this is @static:System.IO.Path.GetFileNameWithoutExtension(#env:solutionPath#)@";
            Environment.SetEnvironmentVariable("solutionPath", "a.sln");
            
            #endregion
            #region act

            var i = new Interpret();
            i.TwoSlashes = false;
            var str = i.InterpretText(textToInterpret);
            #endregion
            #region assert
            
            str.ShouldBe($"this is a");
            #endregion
        }

        [TestMethod]
        public void InterpretStaticTwoParameterString()
        {
            #region arrange
            
            string textToInterpret = "this is @static:Path.GetPathRoot(#static:Directory.GetCurrentDirectory()#)@";
            

            #endregion
            #region act

            var i = new Interpret();
            i.TwoSlashes = false;
            var str = i.InterpretText(textToInterpret);
            #endregion
            #region assert
            
            string s = Path.GetPathRoot(Directory.GetCurrentDirectory());
            str.ShouldBe($"this is {s}");
            #endregion
        }
        [TestMethod]
        public async Task RunJobInterpreted()
        {
            #region arrange
            var dir = AppContext.BaseDirectory;
            foreach (var item in Directory.EnumerateFiles(dir, "*.csv"))
            {
                File.Delete(item);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("1,model,Track_number");
            sb.AppendLine("2,Ford,B325ROS");
            sb.AppendLine("3,Audi,PL654CSM");
            sb.AppendLine("4,BMW,B325DFH");
            sb.AppendLine("5,Ford,B325IYS");
            File.WriteAllText(Path.Combine(dir, "cars.csv"), sb.ToString());
            var textJob = File.ReadAllText(Path.Combine(dir, "InterpreterJobDateTime.txt"));
            //var j1 = new SimpleJob();
            //j1.Senders.Add(0, new Sender_CSV(":ASDAS"));
            //File.WriteAllText("aaa.txt", j1.SerializeMe());
            //Process.Start("notepad.exe", "aaa.txt");
            #endregion
            #region act
            IJob j = new SimpleJob();
            j.UnSerialize(textJob);
            await j.Execute();
            #endregion
            #region assert
            Assert.IsTrue(File.Exists("SendTo" + DateTime.Now.ToString("yyyyMMdd") + ".csv"));
               
            #endregion

        }
    }
}

