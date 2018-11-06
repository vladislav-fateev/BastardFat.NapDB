﻿using BastardFat.NapDB.Abstractions;
using System.Reflection;

namespace BastardFat.NapDB.Config
{
    internal class ReferenceConfiguration<TDb, TKey>
        where TDb : INapDb<TKey>
    {
        public ReferenceKind Kind { get; set; }
        public IDataSet<TKey> SourceDataSet { get; set; }
        public PropertyInfo ForeignKeyProperty { get; set; }
    }
}
