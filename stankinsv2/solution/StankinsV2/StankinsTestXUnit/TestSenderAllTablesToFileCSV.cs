﻿using FluentAssertions;
using Stankins.FileOps;
using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xbehave;
using Xunit;
using static System.Environment;
namespace StankinsTestXUnit
{
    [Trait("SenderAllTablesToFileCSV", "")]
    [Trait("ExternalDependency", "0")]
    public class TestSenderAllTablesToFileCSV
    {
        [Scenario]
        [Example("Year, Car{NewLine}Ford, 2000{NewLine}Rolls Royce, 2003",2)]
        public void TestSimpleCSV(string fileContents,int NumberRows)
        {
            IReceive receiver = null;
            IDataToSent data=null;
            var nl = Environment.NewLine;
            fileContents = fileContents.Replace("{NewLine}", nl);
            $"When I create the receiver csv for the content {fileContents}".w(() => receiver = new ReceiverCSVText(fileContents));
            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of rows should be {NumberRows}".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRows));
            var filesCSV = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csv");
            foreach (var f in filesCSV)
            {
                File.Delete(f);
            }
            $" and no csv files existing".w(() => Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csv").Length.Should().Be(0));
            $"and now I export to csv to the current folder".w(async () => data =await new SenderAllTablesToFileCSV("").TransformData(data));
            $" and now 1 csv files existing".w(() => Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csv").Length.Should().Be(1));



        } 
    }
}
