﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;

namespace Bugsnag
{
    internal class OfflineStore
    {
        private static object _lock = new object();

        private const string CrashReportDirectory = "crash_reports";

        internal IEnumerable<string> ReadStoredJson()
        {
            using (var store = IsolatedStorageFile())
            {
                store.CreateDirectory(CrashReportDirectory);
                foreach (var filePath in store.GetFileNames(string.Format("{0}\\*", CrashReportDirectory)))
                {
                    string fileData = ReadAndRemoveCrashReport(store, filePath);

                    if (fileData != null)
                    {
                        yield return fileData;
                    }
                }
            }
        }

        internal void StoreJson(string json)
        {
            using (var store = IsolatedStorageFile())
            {
                store.CreateDirectory(CrashReportDirectory);
                using (var storageStream = new IsolatedStorageFileStream(FullCrashReportFilePath(Guid.NewGuid()), FileMode.CreateNew, store))
                {
                    var writer = new StreamWriter(storageStream);
                    writer.Write(json);
                    writer.Flush();
                }
            }
        }

        private string ReadAndRemoveCrashReport(IsolatedStorageFile store, string fileName)
        {
            string fullCrashReportFilePath = FullCrashReportFilePath(fileName);
            string fileData = null;

            lock (_lock)
            {
                try
                {
                    using (var storageStream = new IsolatedStorageFileStream(fullCrashReportFilePath, FileMode.Open, store))
                    {
                        var reader = new StreamReader(storageStream);
                        fileData = reader.ReadToEnd();
                    }
                }
                catch (FileNotFoundException)
                {
                    // we can assume here that this crash report has already been sent in another thread
                }
                store.DeleteFile(fullCrashReportFilePath);
            }

            return fileData;
        }

        private static string FullCrashReportFilePath(object fileName)
        {
            return string.Format("{0}\\{1}", CrashReportDirectory, fileName);
        }

        private static IsolatedStorageFile IsolatedStorageFile()
        {
            if (AppDomain.CurrentDomain != null && AppDomain.CurrentDomain.ActivationContext != null)
            {
                return System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
            }
            else
            {
                return System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForAssembly();
            }
        }
    }
}
