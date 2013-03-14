// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License.  You may obtain a copy
// of the License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
// WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.




using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IQToolkit
{
    public interface IEntitySession
    {
        IEntityProvider Provider { get; }
        ISessionTable<T> GetTable<T>(string tableId);
        ISessionTable GetTable(Type elementType, string tableId);
        void SubmitChanges();
    }

    public interface ISessionTable : IQueryable
    {
        IEntitySession Session { get; }
        IEntityTable ProviderTable { get; }
        object GetById(object id);
        void SetSubmitAction(object instance, SubmitAction action);
        SubmitAction GetSubmitAction(object instance);
    }

    public interface ISessionTable<T> : IQueryable<T>, ISessionTable
    {
        new IEntityTable<T> ProviderTable { get; }
        new T GetById(object id);
        void SetSubmitAction(T instance, SubmitAction action);
        SubmitAction GetSubmitAction(T instance);
    }

    public enum SubmitAction
    {
        None,
        Update,
        PossibleUpdate,
        Insert,
        InsertOrUpdate,
        Delete
    }

    public static class SessionTableExtensions
    {
        public static void InsertOnSubmit<T>(this ISessionTable<T> table, T instance)
        {
            table.SetSubmitAction(instance, SubmitAction.Insert);
        }

        public static void InsertOnSubmit(this ISessionTable table, object instance)
        {
            table.SetSubmitAction(instance, SubmitAction.Insert);
        }

        public static void InsertOrUpdateOnSubmit<T>(this ISessionTable<T> table, T instance)
        {
            table.SetSubmitAction(instance, SubmitAction.InsertOrUpdate);
        }

        public static void InsertOrUpdateOnSubmit(this ISessionTable table, object instance)
        {
            table.SetSubmitAction(instance, SubmitAction.InsertOrUpdate);
        }

        public static void UpdateOnSubmit<T>(this ISessionTable<T> table, T instance)
        {
            table.SetSubmitAction(instance, SubmitAction.Update);
        }

        public static void UpdateOnSubmit(this ISessionTable table, object instance)
        {
            table.SetSubmitAction(instance, SubmitAction.Update);
        }

        public static void DeleteOnSubmit<T>(this ISessionTable<T> table, T instance)
        {
            table.SetSubmitAction(instance, SubmitAction.Delete);
        }

        public static void DeleteOnSubmit(this ISessionTable table, object instance)
        {
            table.SetSubmitAction(instance, SubmitAction.Delete);
        }
    }
}
