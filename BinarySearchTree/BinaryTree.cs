using System;
using System.Threading;

namespace BinarySearchTree
{
    /// <summary>
    /// What thread-safety strategy does it use and why was it picked?
    /// 
    /// The thread-safety strategy being used is Read/Write locking.
    /// This strategy was choosen because reads should not lock out other reads, 
    /// but should prevent writing during reads.  Adding/Deleting to/from the tree 
    /// should prevent not only writing, but also reading the nodes being modified.
    /// 
    /// What levels of concurrency can you expect on your implementation?
    /// 
    /// This implementation supports an unlimited number of simultaneous tree reads
    /// when no writes are happening, and writes should only block reading on the 
    /// nodes below it on the tre.
    /// </summary>
	public class BinaryTree {

        private Node _root;
        ReaderWriterLockSlim _nodeLock = new ReaderWriterLockSlim();

        public bool Insert (int key) {
            //returns true if insert succeeded
            Node newValue = new Node (key);
            try {
                _nodeLock.EnterWriteLock();
                if (_root == null) {
                    _root = newValue;
                    return true;
                }
            } finally {
                _nodeLock.ExitWriteLock ();
            }

            Node current = _root;

            while (true) {
                try {
                    _nodeLock.EnterWriteLock();
                    if (key > current.Value) {
                        if (current.RightChild == null) {
                            current.RightChild = newValue;
                            return true;
                        } else {
                            current = current.RightChild;
                        }
                    } else if (key < current.Value) {
                        if (current.LeftChild == null) {
                            current.LeftChild = newValue;
                            return true;
                        } else {
                            current = current.LeftChild;
                        }
                    } else {
                        return false;
                    }
                } finally {
                    _nodeLock.ExitWriteLock ();
                }
            }
        }

        public bool Contains (int key) {
            //returns true if key is in the tree
            try {
                _nodeLock.EnterReadLock();
                if (_root == null)
                        return false;
            } finally {
                _nodeLock.ExitReadLock ();
            }

            Node current = _root;
            while (true) {
                try {
                    _nodeLock.EnterReadLock();
                    if (key > current.Value) {
                        if (current.RightChild == null) {
                            return false;
                        } else {
                            current = current.RightChild;
                        }
                    } else if (key < current.Value) {
                        if (current.LeftChild == null) {
                            return false;
                        } else {
                            current = current.LeftChild;
                        }
                    } else if (key == current.Value) {
                        return true;
                    } else {
                        return false;
                    }
                } finally {
                    _nodeLock.ExitReadLock ();
                }
            }
        }

        public bool Remove (int key) {
            //returns true if key was removed
            try {
                _nodeLock.EnterWriteLock();
                if (_root == null)
                    return false;
            } finally {
                _nodeLock.ExitWriteLock ();
            }

            Node current = _root;
            while (true) {
                try {
                    _nodeLock.EnterWriteLock();
                    if (key > current.Value) {
                        if (current.RightChild == null) {
                            return false;
                        } else {
                            if (current.RightChild.Value == key) {
                                current.RightChild = current.RightChild.RightChild;
                                return true;
                            } else {
                                current = current.RightChild;
                            }
                        }
                    } else if (key < current.Value) {
                        if (current.LeftChild == null) {
                            return false;
                        } else {
                            if (current.LeftChild.Value == key) {
                                current.LeftChild = current.LeftChild.RightChild;
                                return true;
                            } else {
                                current = current.RightChild;
                            }
                        }
                    } else {
                        return false;
                    }
                } finally {
                    _nodeLock.ExitWriteLock ();
                }
            }
        }

        private class Node {

            internal Node(int value){
                _value = value;
            }
            private int _value;
            internal int Value {
                get {
                    return _value;
                }
            }
            internal Node LeftChild { get; set; }
            internal Node RightChild{ get; set; }
        }
    }
}
