using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppStudio.DataProviders;

namespace Windows10News.Config
{
    public abstract class SectionConfigBase<TSchema> where TSchema : SchemaBase
    {
        public abstract Func<Task<IEnumerable<TSchema>>> LoadDataAsyncFunc { get; }
        public virtual bool NeedsNetwork
        {
            get
            {
                return true;
            }
        }

        public abstract ListPageConfig<TSchema> ListPage { get; }
        public abstract DetailPageConfig<TSchema> DetailPage { get; }

        public virtual int MaxRecords
        {
            get
            {
                //override in children sections if you want to get more records from data provider
                return 40;
            }
        }

        public string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }
    }

    public abstract class SectionConfigBase<TSchema, TRelatedSchema> : SectionConfigBase<TSchema> where TSchema : SchemaBase where TRelatedSchema : SchemaBase
    {
        public abstract RelatedContentConfig<TRelatedSchema, TSchema> RelatedContent { get; }
    }
}
