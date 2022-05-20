using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace PrimalEditor.Utilities
{
    public static class Serializer
    {
        /* used to write to file */
        public static void ToFile<T>(T Instance, string Path)
        {
            try
            {
                using var fs = new FileStream(Path, FileMode.Create);
                var Serializer = new DataContractSerializer(typeof(T));
                Serializer.WriteObject(fs, Instance);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // TODOL: update log
            }
        }

        internal static T FromFile<T>(string Path)
        {
            try
            {
                using var fs = new FileStream(Path, FileMode.Open);
                var Serializer = new DataContractSerializer(typeof(T));
                T instance = (T)Serializer.ReadObject(fs);
                return instance;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // TODOL: update log
                return default(T);
            }
        }
    }
}