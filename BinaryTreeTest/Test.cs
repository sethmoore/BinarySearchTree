using NUnit.Framework;
using System;
using BinarySearchTree;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BinaryTreeTest
{
	[TestFixture ()]
	public class Test
	{
		[Test ()]
		public void BasicTreeTest()
		{
			BinaryTree tree = new BinaryTree ();
			for (int i = 0; i < 20; i++) {
				tree.Insert (i);
			}

			for (int i = 0; i < 20; i++) {
				Assert.True (tree.Contains (i));
			}

			for (int i = 20; i < 25; i++) {
				Assert.False (tree.Contains (i));
			}
		}

		[Test ()]
		public void TestConcurrencyRead()
		{
			BinaryTree tree = new BinaryTree ();
			for (int i = 0; i < 20; i++) {
				tree.Insert (i);
			}

			List<Thread> threads = new List<Thread> ();
			for (int j = 0; j < 30; j++) {
				threads.Add (
					new Thread (() => {
						for (int i = 0; i < 20; i++) {
							Assert.True (tree.Contains (i));
						}

						for (int i = 20; i < 25; i++) {
							Assert.False (tree.Contains (i));
						}
					}));
			}

			Parallel.ForEach (threads, (thread) => {
				thread.Start ();
				thread.Join ();
			});

		}

		[Test()]
		public void TestConcurrencyWrite()
		{
			BinaryTree tree = new BinaryTree ();
			int multiplier = 1;

			List<Thread> threads = new List<Thread> ();
			for (int j = 0; j < 30; j++) {
				threads.Add (
					new Thread (() => {
						for (int i = 0; i < 20; i++) {
							tree.Insert(i+(10^multiplier));
						}

						for (int i = 0; i < 20; i++) {
							Assert.True (tree.Contains (i+(10^multiplier)));
						}
					}));
				multiplier++;
			}

			Parallel.ForEach (threads, (thread) => {
				thread.Start ();
				thread.Join ();
			});

		}
	}
}

