using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace System.Extract.Stock
{
    /// <summary>
    /// A lock-free FIFO shared memory circular buffer (or ring buffer) utilising a <see cref="MemoryMappedFile"/>.
    /// </summary>
    [SecurityPermission(SecurityAction.LinkDemand)]
    [SecurityPermission(SecurityAction.InheritanceDemand)]
    public unsafe class ConcurrentStock : Stock
    {
        #region Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct NodeHeader
        {
            public volatile int ReadEnd;
            public volatile int ReadStart;
            public volatile int WriteEnd;
            public volatile int WriteStart;

            public int NodeCount;
            public long NodeBufferSize;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct Node
        {
            public int Next;
            public int Prev;
            public int Index;
            public long AmountWritten;

            public long Offset;

            public volatile int DoneRead;
            public volatile int DoneWrite;
        }
        #endregion

        public int NodeCount
        { get; private set; }
        public long NodeBufferSize
        { get; private set; }
        
        protected EventWaitHandle DataExists
        { get; set; }
        protected EventWaitHandle NodeAvailable
        { get; set; }

        protected virtual long NodeHeaderOffset
        {
            get
            {
                return 0;
            }
        }
        protected virtual long NodeOffset
        {
            get
            {
                return NodeHeaderOffset + Marshal.SizeOf(typeof(NodeHeader));
            }
        }
        protected virtual long NodeBufferOffset
        {
            get
            {
                return NodeOffset + (Marshal.SizeOf(typeof(Node)) * NodeCount);
            }
        }

        protected virtual Node* this[int i]
        {
            get
            {
                if (i < 0 || i >= NodeCount)
                    throw new ArgumentOutOfRangeException();

                return ((Node*)(BufferStartPtr + NodeOffset)) + i;
            }
        }

        private NodeHeader* _nodeHeader = null;

        public ConcurrentStock(string file, string name, int nodeCount, long nodeBufferSize) : 
                             this(file, name, nodeCount, nodeBufferSize, true)
        {
            Open();
            CheckArchive();
        }
        public ConcurrentStock(string file, string name) : 
                             this(file, name, 0, 0, false)
        {
            Open();
        }

        private ConcurrentStock(string file, string name, int nodeCount, long nodeBufferSize, bool ownsSharedMemory) : 
                              base(file, name, Marshal.SizeOf(typeof(NodeHeader)) + 
                               (Marshal.SizeOf(typeof(Node)) * nodeCount) + 
                               (nodeCount * nodeBufferSize), 
                               ownsSharedMemory)
        {
            #region Argument validation
            if (ownsSharedMemory && nodeCount < 2)
                throw new ArgumentOutOfRangeException("nodeCount", nodeCount, "The node count must be a minimum of 2.");
            #if DEBUG
            else if (!ownsSharedMemory && (nodeCount != 0 || nodeBufferSize > 0))
                System.Diagnostics.Debug.Write("Node count and nodeBufferSize are ignored when opening an existing shared memory circular buffer.", "Warning");
            #endif
            #endregion

            if (IsOwnerOfSharedMemory)
            {
                NodeCount = nodeCount;
                NodeBufferSize = nodeBufferSize;
            }
        }

        public bool CheckArchive(int timeout = 1000)
        {
            if(Exists)
            {
                if(FixSize)
                {
                    NodeBufferSize = (int)((BufferSize - (Marshal.SizeOf(typeof(NodeHeader)) + 
                                     (Marshal.SizeOf(typeof(Node)) * NodeCount))) / NodeCount);
                }
                //Prepare first node for reading
                Node* firstnode = GetNodeForWriting(timeout);
                if (firstnode == null) return false;
                long firstamount = Math.Min(NodeBufferSize, NodeBufferSize);
                firstnode->AmountWritten = firstamount;
                PostNode(firstnode);
                return true;
            }
            return false;
        }

        #region Open / Close
        protected override bool DoOpen()
        {
            // Create signal events
            DataExists = new EventWaitHandle(false, EventResetMode.AutoReset, Name + "_evt_dataexists");
            NodeAvailable = new EventWaitHandle(false, EventResetMode.AutoReset, Name + "_evt_nodeavail");

            if (IsOwnerOfSharedMemory)
            {
                // Retrieve pointer to node header
                _nodeHeader = (NodeHeader*)(BufferStartPtr + NodeHeaderOffset);

                // Initialise the node header
                InitialiseNodeHeader();

                // Initialise nodes entries
                InitialiseLinkedListNodes();
            }
            else
            {
                // Load the NodeHeader
                _nodeHeader = (NodeHeader*)(BufferStartPtr + NodeHeaderOffset);
                NodeCount = _nodeHeader->NodeCount;
                NodeBufferSize = _nodeHeader->NodeBufferSize;
            }

            return true;
        }

        private void InitialiseNodeHeader()
        {
            if (!IsOwnerOfSharedMemory)
                return;

            NodeHeader header = new NodeHeader();           
            header.ReadStart = 0;
            header.ReadEnd = 0;
            header.WriteEnd = 0;
            header.WriteStart = 0;
            header.NodeBufferSize = NodeBufferSize;
            header.NodeCount = NodeCount;
            object head = header;
            base.Write(head, NodeHeaderOffset);
        }
        private void InitialiseLinkedListNodes()
        {
            if (!IsOwnerOfSharedMemory)
                return;

            int RtStruct = 0;

            Node[] nodes = new Node[NodeCount];

            // First node
            nodes[RtStruct].Next = 1;
            nodes[RtStruct].Prev = NodeCount - 1;
            nodes[RtStruct].Offset = NodeBufferOffset;
            nodes[RtStruct].Index = RtStruct;
            // Middle nodes
            for (RtStruct = 1; RtStruct < NodeCount - 1; RtStruct++)
            {
                nodes[RtStruct].Next = RtStruct + 1;
                nodes[RtStruct].Prev = RtStruct - 1;
                nodes[RtStruct].Offset = NodeBufferOffset + (NodeBufferSize * RtStruct);
                nodes[RtStruct].Index = RtStruct;
            }
            // Last node
            nodes[RtStruct].Next = 0;
            nodes[RtStruct].Prev = NodeCount - 2;
            nodes[RtStruct].Offset = NodeBufferOffset + (NodeBufferSize * RtStruct);
            nodes[RtStruct].Index = RtStruct;

            // Write the nodes to the shared memory
            base.Write(nodes.SelectMany(o => new object[] { o }).ToArray(), 0, nodes.Length, NodeOffset, typeof(Node));
        }

        protected override void DoClose()
        {
            if (DataExists != null)
            {
                (DataExists as IDisposable).Dispose();
                DataExists = null;
                (NodeAvailable as IDisposable).Dispose();
                NodeAvailable = null;
            }

            _nodeHeader = null;
        }
        #endregion

        #region Node Writing
        protected virtual Node* GetNodeForWriting(int timeout)
        {
            for (; ; )
            {
                int blockIndex = _nodeHeader->WriteStart;
                Node* node = this[blockIndex];
                if (node->Next == _nodeHeader->ReadEnd)
                {
                    // No room is available, wait for room to become available
                    if (NodeAvailable.WaitOne(timeout))
                        continue;

                    // Timeout
                    return null;
                }

                #pragma warning disable 0420 // ignore ref to volatile warning - Interlocked API
                if (Interlocked.CompareExchange(ref _nodeHeader->WriteStart, node->Next, blockIndex) == blockIndex)
                    return node;
                #pragma warning restore 0420

                // Another thread has already acquired this node for writing, try again.
                continue;
            }
        }

        protected virtual void PostNode(Node* node)
        {
            // Set the write flag for this node (the node is reserved so no need for locks)
            node->DoneWrite = 1;

            // Move the write pointer as far forward as we can
            // always starting from WriteEnd to make all contiguous
            // completed nodes available for reading.
            for (; ; )
            {
                int blockIndex = _nodeHeader->WriteEnd;
                node = this[blockIndex];
                #pragma warning disable 0420 // ignore ref to volatile warning - Interlocked API
                if (Interlocked.CompareExchange(ref node->DoneWrite, 0, 1) != 1)
                {
                    // If we get here then another thread either another thread
                    // has already moved the write index or we have moved forward 
                    // as far as we can
                    return;
                }

                // Move the pointer one forward
                Interlocked.CompareExchange(ref _nodeHeader->WriteEnd, node->Next, blockIndex);
                #pragma warning restore 0420

                // Signal the "data exists" event if read threads are waiting
                if (blockIndex == _nodeHeader->ReadStart)
                    DataExists.Set();
            }
        }

        public virtual int Write(byte[] source, int startIndex = 0, int timeout = 1000)
        {
            // Grab a node for writing
            Node* node = GetNodeForWriting(timeout);
            if (node == null) return 0;

            // Copy the data
            long amount = Math.Min(source.Length - startIndex, NodeBufferSize);
            
            Marshal.Copy(source, startIndex, new IntPtr(BufferStartPtr + node->Offset), (int)amount);
            node->AmountWritten = amount;
            

            // Writing is complete, make readable
            PostNode(node);

            return (int)amount;
        }

        public virtual int Write(object source, int startIndex = 0, Type t = null, int timeout = 1000)
        {
            int structSize = Marshal.SizeOf(source.GetType());
            if (structSize > NodeBufferSize)
                throw new ArgumentOutOfRangeException("T", "The size of structure " + source.GetType().Name + " is larger than NodeBufferSize");

            // Attempt to retrieve a node for writing
            Node* node = GetNodeForWriting(timeout);
            if (node == null) return 0;

            // Copy the data using the MemoryMappedViewAccessor
            base.Write(source, node->Offset);
            node->AmountWritten = structSize;

            // Return the node for further writing
            PostNode(node);

            return structSize;
        }
        public virtual int Write(object[] source, int startIndex = 0, Type t = null, int timeout = 1000)
        {
            // Grab a node for writing
            Node* node = GetNodeForWriting(timeout);
            if (node == null) return 0;

            // Write the data using the RawStock class (much faster than the MemoryMappedViewAccessor WriteArray<T> method)
            long count = Math.Min(source.Length - startIndex, NodeBufferSize / Marshal.SizeOf(source.GetType()));
            base.Write(source, startIndex, (int)count, node->Offset);
            node->AmountWritten = count * Marshal.SizeOf(source.GetType());

            // Writing is complete, make node readable
            PostNode(node);

            return (int)count;
        }

        public virtual int Write(IntPtr source, int length, Type t = null, int timeout = 1000)
        {
            // Grab a node for writing
            Node* node = GetNodeForWriting(timeout);
            if (node == null) return 0;

            // Copy the data
            long amount = Math.Min(length, NodeBufferSize);
            base.Write(source, amount, node->Offset);
            node->AmountWritten = amount;

            // Writing is complete, make readable
            PostNode(node);

            return (int)amount;
        }
        public virtual int Write(Func<IntPtr, int> writeFunc, int timeout = 1000)
        {
            // Grab a node for writing
            Node* node = GetNodeForWriting(timeout);
            if (node == null) return 0;

            int amount = 0;
            try
            {
                // Pass destination IntPtr to custom write function
                amount = writeFunc(new IntPtr(BufferStartPtr + node->Offset));
                node->AmountWritten = amount;
            }
            finally
            {
                // Writing is complete, make readable
                PostNode(node);
            }

            return amount;
        }
        #endregion

        #region Node Reading
        public NodeHeader ReadNodeHeader()
        {
            return (NodeHeader)Marshal.PtrToStructure(new IntPtr(_nodeHeader), typeof(NodeHeader));
        }

        protected virtual Node* GetNodeForReading(int timeout)
        {
            for (; ; )
            {
                int blockIndex = _nodeHeader->ReadStart;
                Node* node = this[blockIndex];
                if (blockIndex == _nodeHeader->WriteEnd)
                {
                    // No data is available, wait for it
                    if (DataExists.WaitOne(timeout))
                        continue;

                    // Timeout
                    return null;
                }

                #pragma warning disable 0420 // ignore ref to volatile warning - Interlocked API
                if (Interlocked.CompareExchange(ref _nodeHeader->ReadStart, node->Next, blockIndex) == blockIndex)
                    return node;
                #pragma warning restore 0420

                // Another thread has already acquired this node for reading, try again
                continue;
            }
        }
        protected virtual void ReturnNode(Node* node)
        {
            // Set the finished reading flag for this node (the node is reserved so no need for locks)
            node->DoneRead = 1;

            // Keep it clean and reset AmountWritten to prepare it for next Write
            node->AmountWritten = 0;

            // Move the read pointer forward as far as possible
            // always starting from ReadEnd to make all contiguous
            // read nodes available for writing.
            for (; ; )
            {
                int blockIndex = _nodeHeader->ReadEnd;
                node = this[blockIndex];
                #pragma warning disable 0420 // ignore ref to volatile warning - Interlocked API
                if (Interlocked.CompareExchange(ref node->DoneRead, 0, 1) != 1)
                {
                    // If we get here then another read thread has already moved the pointer
                    // or we have moved ReadEnd as far forward as we can
                    return;
                }

                // Move the pointer forward one node
                Interlocked.CompareExchange(ref _nodeHeader->ReadEnd, node->Next, blockIndex);
                #pragma warning restore 0420

               // If a writer thread is waiting on "node available" signal the event
                if (node->Prev == _nodeHeader->WriteStart)
                        NodeAvailable.Set();
            }
        }
      
        public virtual int Read(byte[] destination, int startIndex = 0, Type t = null, int timeout = 1000)
        {
            Node* node = GetNodeForReading(timeout);
            if (node == null) return 0;

            //int amount = Math.Min(buffer.Length, NodeBufferSize);
            int amount = (int)Math.Min(destination.Length - startIndex, node->AmountWritten);

            // Copy the data
            Marshal.Copy(new IntPtr(BufferStartPtr + node->Offset), destination, startIndex, amount);

            // Return the node for further writing
            ReturnNode(node);

            return amount;
        }      

        public virtual int Read(object destination, int startIndex = 0, Type t = null, int timeout = 1000)
        {
            int structSize = Marshal.SizeOf(t);
            if (structSize > NodeBufferSize)
                throw new ArgumentOutOfRangeException("T", "The size of structure " + t.Name + " is larger than NodeBufferSize");

            // Attempt to retrieve a node
            Node* node = GetNodeForReading(timeout);
            if (node == null && t != null)
            {
                destination = GetDefault(t);
                return 0;
            }

            // Copy the data using the MemoryMappedViewAccessor
            base.Read(destination, node->Offset);

            // Return the node for further writing
            ReturnNode(node);

            return structSize;
        }
        public virtual int Read(object[] destination, int startIndex = 0, Type t = null, int timeout = 1000)
        {
            Node* node = GetNodeForReading(timeout);
            if (node == null) return 0;

            // Copy the data using the RawStock class (much faster than the MemoryMappedViewAccessor ReadArray<T> method)
            int count = (int)Math.Min(destination.Length - startIndex, node->AmountWritten / Marshal.SizeOf(t));
            base.Read(destination, startIndex, count, node->Offset);

            // Return the node for further writing
            ReturnNode(node);

            return count;
        }

        public virtual int Read(IntPtr destination, int length, Type t = null, int timeout = 1000)
        {
            Node* node = GetNodeForReading(timeout);
            if (node == null) return 0;

            //int amount = Math.Min(length, NodeBufferSize);
            int amount = (int)Math.Min(length, node->AmountWritten);

            // Copy the data
            base.Read(destination, amount, node->Offset);

            // Return node for further writing
            ReturnNode(node);

            return amount;
        }
        public virtual int Read(Func<IntPtr, int> readFunc, int timeout = 1000)
        {
            Node* node = GetNodeForReading(timeout);
            if (node == null) return 0;

            int amount = 0;
            try
            {
                // Pass pointer to buffer directly to custom read function
                amount = readFunc(new IntPtr(BufferStartPtr + node->Offset));
            }
            finally
            {
                // Return the node for further writing
                ReturnNode(node);
            }
            return amount;
        }

        public static object GetDefault(Type t)
        {
            Func<object> f = GetDefault<object>;
            return f.Method.GetGenericMethodDefinition().MakeGenericMethod(t).Invoke(null, null);
        }
        private static T GetDefault<T>()
        {
            return default(T);
        }
        #endregion
    }
}
