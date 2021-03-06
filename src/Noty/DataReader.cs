﻿using FastMember;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noty
{
    public static class DataReader
    {
        public static T MapDataToObject<T>(this object objectValue)
        {
            if (objectValue == null)
                return default(T);

            return (T)objectValue;
        }

        public static async Task<T> MapDataToObject<T, T2>(this T2 dataReader) where T2 : DbDataReader
        {
            var objectMemberAccessor = FastMember.TypeAccessor.Create(typeof(T));

            var propertiesHashSet =
                    objectMemberAccessor
                    .GetMembers()
                    .Select(mp => mp.Name)
                    .ToHashSet();

            T newObject = Activator.CreateInstance<T>();

            string objectPropertyName = null;

            await dataReader.ReadAsync();
            
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                objectPropertyName = propertiesHashSet.FirstOrDefault(x => x.Equals(dataReader.GetName(i), StringComparison.OrdinalIgnoreCase));

                if (objectPropertyName != null)
                {

                    objectMemberAccessor[newObject, objectPropertyName]
                        = dataReader.IsDBNull(i) ? null : dataReader.GetValue(i);
                }
            }

            return newObject;
        }

        public static async Task<DataTable> MapDataToDataTable<T, T2>(this T2 dataReader) where T2 : DbDataReader
        {
            var objectMemberAccessor = FastMember.TypeAccessor.Create(typeof(T));

            var members =
                    objectMemberAccessor
                    .GetMembers().ToList();

            var membersNameHashSet = members.Select(x => x.Name).ToHashSet();

            DataTable dt = new DataTable();

            members.ForEach(x => { dt.Columns.Add(x.Name, x.Type); });

            string objectPropertyName = null;
            DataRow newRow = null;

            while (await dataReader.ReadAsync())
            {
                newRow = dt.NewRow();

                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    objectPropertyName = membersNameHashSet.FirstOrDefault(x => x.Equals(dataReader.GetName(i), StringComparison.OrdinalIgnoreCase));

                    if (objectPropertyName != null)
                    {
                        newRow[objectPropertyName]
                            = dataReader.IsDBNull(i) ? null : dataReader.GetValue(i);
                    }
                }
            }

            return dt;
        }

        public static async Task<IEnumerable<T>> MapDataToObjectCollection<T, T2>(this T2 dataReader) where T2 : DbDataReader
        {
            var objectMemberAccessor = FastMember.TypeAccessor.Create(typeof(T));

            var propertiesHashSet =
                    objectMemberAccessor
                    .GetMembers()
                    .Select(mp => mp.Name)
                    .ToHashSet();


            T newObject = default(T);

            var newCollection = new Collection<T>();

            string objectPropertyName = null;

            while (await dataReader.ReadAsync())
            {
                newObject = Activator.CreateInstance<T>();

                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    objectPropertyName = propertiesHashSet.FirstOrDefault(x => x.Equals(dataReader.GetName(i), StringComparison.OrdinalIgnoreCase));

                    if (objectPropertyName != null)
                    {

                        objectMemberAccessor[newObject, objectPropertyName]
                            = dataReader.IsDBNull(i) ? null : dataReader.GetValue(i);
                    }
                }

                newCollection.Add(newObject);
            }

            return newCollection;
        }
    }
}
