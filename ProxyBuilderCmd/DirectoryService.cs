﻿using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.CDS.ProxyBuilderCmd
{
    public class DirectoryService : IDirectoryService
    {
        public string GetApplicationDirectory()
        {
            string path = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;

            return path;
        }

        public string SimpleSearch(string path, string search)
        {
            try
            {
                foreach (string f in Directory.GetFiles(path, search, SearchOption.AllDirectories))
                {
                    return f;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public List<string> Search(string path, string search)
        {
            var matcher = new Matcher(StringComparison.InvariantCultureIgnoreCase);
            if (search.StartsWith("..") || search.StartsWith("**") || search.StartsWith("\\"))
            {
                matcher.AddInclude(search);
            }
            else
            {
                matcher.AddInclude("**\\" + search);
            }

            var globbMatch = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(path)));

            var matchList = new List<string>();
            foreach (var file in globbMatch.Files)
            {
                var fullFilePath = Path.Combine(path, file.Path.Replace("/", "\\"));
                var fileInfo = new FileInfo(fullFilePath);
                matchList.Add(fileInfo.FullName);
            }
            return matchList;

        }
       
    }
}
