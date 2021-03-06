﻿using StankinsInterfaces;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace SenderUnzipFiles
{
    /// <summary>
    /// assuming valuesToBeSent has Values["FullName"]
    /// </summary>
    public class SenderUnzipFile : ISend
    {
        public SenderUnzipFile(string destinationFolder)
        {
            DestinationFolder = destinationFolder;
        }
        public IRow[] valuesToBeSent { get; set; }
        public string Name { get ; set; }
        public string DestinationFolder { get; set; }

        public async Task Send()
        {
            if (!Directory.Exists(DestinationFolder))
                Directory.CreateDirectory(DestinationFolder);

            foreach(var item in valuesToBeSent)
            {
                if (!item.Values.ContainsKey("FullName"))
                {
                    string message = $"item does not contain FullName";
                    //@class.Log(LogLevel.Information, 0, $"sender unzip files: {message}", null, null);                        
                    message += "";
                    continue;
                }
                var zip = item.Values["FullName"]?.ToString();
                if (!File.Exists(zip))
                {
                    string message = $"{zip} file does not exists";
                    //@class.Log(LogLevel.Information, 0, $"sender unzip files: {message}", null, null);                        
                    message += "";
                    continue;
                }
                ZipFile.ExtractToDirectory(zip, DestinationFolder);
                await Task.CompletedTask;
            }
        }
    }
}
