using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Web;

namespace MyCollectionUmb.ContentDeliveryApi
{
    public class AuthorFilter : IFilterHandler, IContentIndexHandler
    {
        private const string AuthorSpecifier = "author:";
        private const string FieldName = "author";
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public AuthorFilter(IUmbracoContextFactory umbracoContextFactory)
        {
            _umbracoContextFactory = umbracoContextFactory;
        }

        // Querying
        public bool CanHandle(string query)
            => query.StartsWith(AuthorSpecifier, StringComparison.OrdinalIgnoreCase);

        public FilterOption BuildFilterOption(string filter)
        {
            var fieldValue = filter.Substring(AuthorSpecifier.Length);

            // There might be several values for the filter
            var values = fieldValue.Split(',');

            return new FilterOption
            {
                FieldName = FieldName,
                Values = values,
                Operator = FilterOperation.Is
            };
        }

        // Indexing
        public IEnumerable<IndexFieldValue> GetFieldValues(IContent content, string? culture)
        {
            var authorId = content.GetValue<GuidUdi>(FieldName);

            if (authorId == null)
            {
                return Enumerable.Empty<IndexFieldValue>();
            }

            //    return new[]
            //    {
            //    new IndexFieldValue
            //    {
            //        FieldName = FieldName,
            //        Values = new object[] { authorUdi.Guid }
            //    }
            //};

            var ctxRef = _umbracoContextFactory.EnsureUmbracoContext();
            if (ctxRef != null)
            {
                var pickedAuthorNode = ctxRef?.UmbracoContext?.Content?.GetById(authorId);
                if (pickedAuthorNode != null)
                {
                    return new[]
                    {
                        new IndexFieldValue
                        {
                            FieldName=FieldName,
                            Values=new object[] {pickedAuthorNode.Name }
                    }
                    };
                }
            }

            return Enumerable.Empty<IndexFieldValue>();
        }

        public IEnumerable<IndexField> GetFields() => new[]
        {
        new IndexField
        {
            FieldName = FieldName,
            FieldType = FieldType.StringSortable,
            VariesByCulture = false
        }
    };
    }
}
