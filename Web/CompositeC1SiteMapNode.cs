﻿using System;
using System.Linq;

using Composite.Data;
using Composite.Data.Types;

namespace CompositeC1Contrib.Web
{
    public class CompositeC1SiteMapNode : BaseSiteMapNode
    {
        public PageNode PageNode { get; protected set; }

        public CompositeC1SiteMapNode(CompositeC1SiteMapProvider provider, PageNode node, DataConnection data)
            : base(provider, node.Id.ToString(), data.CurrentLocale)
        {
            Title = node.MenuTitle;
            Description = node.Description;
            Url = fixUrl(node.Url, data);

            DocumentTitle = node.Title;
            Depth = node.Level;
            LastModified = data.Get<IPage>().Single(p => p.Id == node.Id).ChangeDate;
            Priority = 5;

            PageNode = node;
        }

        private string fixUrl(string url, DataConnection data)
        {
            url = url.Replace(".aspx", String.Empty);

            var websites = data.Get<IPageStructure>().Where(p => p.ParentId == Guid.Empty);
            if (websites.Count() == 1)
            {
                var numberOfLocales = data.Get<ISystemActiveLocale>().Count();
                if (numberOfLocales > 1)
                {
                    url = url.Remove(0, url.IndexOf("/", 1));
                }
                else
                {
                    int secondSlash = url.IndexOf("/", 1);
                    url = url.Remove(0, secondSlash == -1 ? url.Length : secondSlash);
                }
            }

            int index = url.IndexOf("?");
            if (index == -1)
            {
                url = UrlUtils.GetCleanUrl(url);
            }
            else
            {
                string query = url.Substring(index, url.Length - index);
                url = url.Substring(0, index);

                url = UrlUtils.GetCleanUrl(url);
                url = url + query;
            }

            if (String.IsNullOrEmpty(url))
            {
                url = "/";
            }

            return url;
        }
    }
}
