namespace Microsoft.VisualStudio.Project
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Tvl;
    using Interlocked = System.Threading.Interlocked;
    using LockRecursionPolicy = System.Threading.LockRecursionPolicy;
    using ReaderWriterLockSlim = System.Threading.ReaderWriterLockSlim;

    public class HierarchyNodeCollection
    {
        private readonly ProjectNode _projectManager;
        private readonly IEqualityComparer<string> _canonicalNameComparer;
        private readonly ReaderWriterLockSlim _syncObject = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly Dictionary<uint, HierarchyNode> _nodes = new Dictionary<uint, HierarchyNode>();
        private readonly Dictionary<HierarchyNode, uint> _itemIds = new Dictionary<HierarchyNode, uint>(ObjectReferenceEqualityComparer<HierarchyNode>.Default);

        private readonly HashSet<HierarchyNode> _nonCacheableCanonicalNameNodes = new HashSet<HierarchyNode>(ObjectReferenceEqualityComparer<HierarchyNode>.Default);
        private readonly Dictionary<HierarchyNode, string> _nodeToCanonicalNameMap = new Dictionary<HierarchyNode, string>(ObjectReferenceEqualityComparer<HierarchyNode>.Default);
        // TODO: create a dictionary for the common case of only having a single value for a particular canonical name
        private readonly Dictionary<string, List<HierarchyNode>> _canonicalNameToNodesMap;

        private int _nextNode;

        public HierarchyNodeCollection(ProjectNode projectManager, IEqualityComparer<string> canonicalNameComparer)
        {
            Contract.Requires<ArgumentNullException>(projectManager != null, "projectManager");

            _projectManager = projectManager;
            _canonicalNameComparer = canonicalNameComparer ?? EqualityComparer<string>.Default;
            _canonicalNameToNodesMap = new Dictionary<string, List<HierarchyNode>>(_canonicalNameComparer);
        }

        public ProjectNode ProjectManager
        {
            get
            {
                Contract.Ensures(Contract.Result<ProjectNode>() != null);

                return _projectManager;
            }
        }

        public int Count
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return _nodes.Count;
            }
        }

        public HierarchyNode this[uint itemId]
        {
            get
            {
                using (_syncObject.ReadLock())
                {
                    HierarchyNode node;
                    if (!_nodes.TryGetValue(itemId, out node))
                        return null;

                    return node;
                }
            }
        }

        public uint Add(HierarchyNode node)
        {
            Contract.Requires<ArgumentNullException>(node != null, "node");

            string canonicalName = null;
            if (node.CanCacheCanonicalName)
                canonicalName = node.CanonicalName;

            using (_syncObject.WriteLock())
            {
                uint itemId = (uint)Interlocked.Increment(ref _nextNode);
                _itemIds.Add(node, itemId);
                _nodes.Add(itemId, node);
                // always add the node as non-cacheable since the canonical name may not be initialized when this method is called.
                _nonCacheableCanonicalNameNodes.Add(node);
                return itemId;
            }
        }

        public void Remove(HierarchyNode node)
        {
            Contract.Requires<ArgumentNullException>(node != null, "node");

            using (_syncObject.WriteLock())
            {
                uint itemId;
                if (!_itemIds.TryGetValue(node, out itemId))
                    return;

                _itemIds.Remove(node);
                _nodes.Remove(itemId);

                // remove any existing copy of this name
                if (!_nonCacheableCanonicalNameNodes.Remove(node))
                {
                    string previousName;
                    if (_nodeToCanonicalNameMap.TryGetValue(node, out previousName))
                    {
                        List<HierarchyNode> previousList;
                        if (_canonicalNameToNodesMap.TryGetValue(previousName, out previousList))
                        {
                            previousList.Remove(node);
                            if (previousList.Count == 0)
                                _canonicalNameToNodesMap.Remove(previousName);
                        }

                        _nodeToCanonicalNameMap.Remove(node);
                    }
                }
            }
        }

        public List<HierarchyNode> GetNodesByName(string canonicalName)
        {
            List<HierarchyNode> nodes = new List<HierarchyNode>();

            using (_syncObject.ReadLock())
            {
                List<HierarchyNode> cachedNodes;
                if (_canonicalNameToNodesMap.TryGetValue(canonicalName, out cachedNodes))
                    nodes.AddRange(cachedNodes);

                nodes.AddRange(_nonCacheableCanonicalNameNodes.Where(i => _canonicalNameComparer.Equals(canonicalName, i.CanonicalName)));
            }

            return nodes;
        }

        public void UpdateAllCanonicalNames()
        {
            KeyValuePair<HierarchyNode, string>[] itemsToCheck = _nodeToCanonicalNameMap.ToArray();
            foreach (var item in itemsToCheck)
            {
                if (!item.Key.CanCacheCanonicalName || !_canonicalNameComparer.Equals(item.Value, item.Key.CanonicalName))
                    UpdateCanonicalName(item.Key);
            }
        }

        public void UpdateCanonicalName(HierarchyNode node)
        {
            Contract.Requires<ArgumentNullException>(node != null, "node");

            if (!node.CanCacheCanonicalName)
            {
                using (_syncObject.WriteLock())
                {
                    if (_nonCacheableCanonicalNameNodes.Add(node))
                    {
                        string previousName;
                        if (_nodeToCanonicalNameMap.TryGetValue(node, out previousName))
                        {
                            List<HierarchyNode> previousList;
                            if (_canonicalNameToNodesMap.TryGetValue(previousName, out previousList))
                            {
                                previousList.Remove(node);
                                if (previousList.Count == 0)
                                    _canonicalNameToNodesMap.Remove(previousName);
                            }

                            _nodeToCanonicalNameMap.Remove(node);
                        }
                    }
                }

                return;
            }
            else
            {
                string canonicalName = node.CanonicalName;

                using (_syncObject.WriteLock())
                {
                    // remove any existing copy of this name
                    if (!_nonCacheableCanonicalNameNodes.Remove(node))
                    {
                        string previousName;
                        if (_nodeToCanonicalNameMap.TryGetValue(node, out previousName))
                        {
                            List<HierarchyNode> previousList;
                            if (_canonicalNameToNodesMap.TryGetValue(previousName, out previousList))
                            {
                                previousList.Remove(node);
                                if (previousList.Count == 0)
                                    _canonicalNameToNodesMap.Remove(previousName);
                            }

                            _nodeToCanonicalNameMap.Remove(node);
                        }
                    }

                    _nodeToCanonicalNameMap.Add(node, canonicalName);
                    List<HierarchyNode> currentList;
                    if (!_canonicalNameToNodesMap.TryGetValue(canonicalName, out currentList))
                    {
                        currentList = new List<HierarchyNode>();
                        _canonicalNameToNodesMap.Add(canonicalName, currentList);
                    }

                    currentList.Add(node);
                }

                return;
            }
        }
    }
}
