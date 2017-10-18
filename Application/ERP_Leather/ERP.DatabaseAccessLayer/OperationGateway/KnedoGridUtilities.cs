using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class KendoGridFilter
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }


    public class KendoGridFilterCollection
    {
        public List<KendoGridFilter> Filters { get; private set; }
        private KendoGridFilterCollection()
        {
            Filters = new List<KendoGridFilter>();
        }

        public static KendoGridFilterCollection BuildEmptyCollection()
        {
            return new KendoGridFilterCollection();
        }

        public static KendoGridFilterCollection BuildCollection(HttpRequestBase request)
        {
            var collection = BuildEmptyCollection();

            var idex = 0;
            while (true)
            {
                var filter = new KendoGridFilter()
                {
                    Field = request.Params["filter[filters][" + idex + "][field]"],
                    Operator = request.Params["filter[filters][" + idex + "][operator]"],
                    Value = request.Params["filter[filters][" + idex + "][value]"]
                };

                if (filter.Field == null) { break; }
                collection.Filters.Add(filter);
                idex++;
            }

            return collection;
        }
    }

    public static class LinqFilteringUtility
    {
        public static IEnumerable<T> MultipleFilter<T>(this IEnumerable<T> data,
          List<KendoGridFilter> filterExpressions)
        {
            if ((filterExpressions == null) || (filterExpressions.Count <= 0))
            {
                return data;
            }

            IEnumerable<T> filteredquery = from item in data select item;

            for (int i = 0; i < filterExpressions.Count; i++)
            {
                var index = i;

                Func<T, bool> expression = item =>
                {
                    var filter = filterExpressions[index];
                    var itemValue = item.GetType()
                        .GetProperty(filter.Field)
                        .GetValue(item, null);

                    if (itemValue == null)
                    {
                        return false;
                    }

                    var value = filter.Value;
                    switch (filter.Operator)
                    {
                        case "eq":
                            return itemValue.ToString() == value;
                        case "startswith":
                            return itemValue.ToString().StartsWith(value);
                        case "contains":
                            return itemValue.ToString().Contains(value);
                        case "endswith":
                            return itemValue.ToString().EndsWith(value);
                    }

                    return true;
                };

                filteredquery = filteredquery.Where(expression);
            }

            return filteredquery;
        }
    }
}
