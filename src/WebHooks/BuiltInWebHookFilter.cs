﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SenseNet.ContentRepository;
using SenseNet.ContentRepository.Search;
using SenseNet.Events;
using Task = System.Threading.Tasks.Task;

namespace SenseNet.WebHooks
{
    public class BuiltInWebHookFilter : IWebHookFilter
    {
        public Task<IEnumerable<WebHookSubscription>> GetRelevantSubscriptionsAsync(ISnEvent snEvent)
        {
            //UNDONE: implement a subscription cache that is invalidated when a subscription changes
            // Do NOT cache nodes, their data is already cached. Cache only ids, paths, or trees.
            var allSubs = Content.All.DisableAutofilters().Where(c =>
                c.InTree("/Root/System/WebHooks") &&
                c.ContentHandler is WebHookSubscription)
                .AsEnumerable()
                .Select(c => c.ContentHandler as WebHookSubscription)
                .ToList();

            //UNDONE: use the Content available on GenericContent instances
            var pe = new PredicationEngine(Content.Create(snEvent.NodeEventArgs.SourceNode));
            var filteredSubs = allSubs.Where(sub => pe.IsTrue(sub.Filter)).ToList();

            return Task.FromResult((IEnumerable<WebHookSubscription>)filteredSubs);
        }
    }
}
