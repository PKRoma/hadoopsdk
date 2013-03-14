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
using System.Linq.Expressions;
using System.Reflection;

namespace IQToolkit
{
    public interface IEntityProvider : IQueryProvider
    {
        IEntityTable<T> GetTable<T>(string tableId);
        IEntityTable GetTable(Type type, string tableId);
        bool CanBeEvaluatedLocally(Expression expression);
        bool CanBeParameter(Expression expression);
    }

    public interface IEntityTable : IQueryable, IUpdatable
    {
        new IEntityProvider Provider { get; }
        string TableId { get; }
        object GetById(object id);
        int Insert(object instance);
        int Update(object instance);
        int Delete(object instance);
        int InsertOrUpdate(object instance);
    }

    public interface IEntityTable<T> : IQueryable<T>, IEntityTable, IUpdatable<T>
    {
        new T GetById(object id);
        int Insert(T instance);
        int Update(T instance);
        int Delete(T instance);
        int InsertOrUpdate(T instance);
    }
}
