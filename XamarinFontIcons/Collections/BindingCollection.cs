using dotup.Binding.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotup.Binding
{
	public class BindingCollection<T> : ObservableCollection<T>
	{
		/// <summary> 
		/// Initializes a new instance of the dotup.Binding.BindingCollection(Of T) class. 
		/// </summary> 
		public BindingCollection() :
			base()
		{ }

		/// <summary> 
		/// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class that contains elements copied from the specified collection. 
		/// </summary> 
		/// <param name="collection">collection: The collection from which the elements are copied.</param> 
		/// <exception cref="System.ArgumentNullException">The collection parameter cannot be null.</exception> 
		public BindingCollection(IEnumerable<T> list) :
			base(list)
		{
			this.AllItems = list;
		}

		public IEnumerable<T> AllItems { get; internal set; }

		/// <summary> 
		/// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
		/// </summary> 
		public void AddRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Add)
		{
			if (notificationMode != NotifyCollectionChangedAction.Add && notificationMode != NotifyCollectionChangedAction.Reset)
				throw new ArgumentException("Mode must be either Add or Reset for AddRange.", nameof(notificationMode));
			if (collection == null)
				throw new ArgumentNullException(nameof(collection));

			if (collection.Count() < 1)
				return;

			var startIndex = Count;

			CheckReentrancy();
			AddArrangeCore(collection);

			if (notificationMode == NotifyCollectionChangedAction.Reset)
			{
				RaiseChangeNotificationEvents(action: NotifyCollectionChangedAction.Reset);

				return;
			}

			var changedItems = collection is List<T> ? (List<T>)collection : new List<T>(collection);

			RaiseChangeNotificationEvents(
				action: NotifyCollectionChangedAction.Add,
				changedItems: changedItems,
				startingIndex: startIndex);
		}

		public void Filter(Func<T, bool> predicate)
		{
			this.ClearAndAddRange(this.AllItems.Where(predicate));
		}

		public void RemoveFilter()
		{
			this.ClearAndAddRange(this.AllItems);
		}

		/// <summary> 
		/// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T). NOTE: with notificationMode = Remove, removed items starting index is not set because items are not guaranteed to be consecutive.
		/// </summary> 
		public void RemoveRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Reset)
		{
			if (notificationMode != NotifyCollectionChangedAction.Remove && notificationMode != NotifyCollectionChangedAction.Reset)
				throw new ArgumentException("Mode must be either Remove or Reset for RemoveRange.", nameof(notificationMode));
			if (collection == null)
				throw new ArgumentNullException(nameof(collection));

			CheckReentrancy();

			if (notificationMode == NotifyCollectionChangedAction.Reset)
			{
				var raiseEvents = false;
				foreach (var item in collection)
				{
					Items.Remove(item);
					raiseEvents = true;
				}

				if (raiseEvents)
					RaiseChangeNotificationEvents(action: NotifyCollectionChangedAction.Reset);

				return;
			}

			var changedItems = collection is List<T> ? (List<T>)collection : new List<T>(collection);
			for (var i = 0; i < changedItems.Count; i++)
			{
				if (!Items.Remove(changedItems[i]))
				{
					changedItems.RemoveAt(i); //Can't use a foreach because changedItems is intended to be (carefully) modified
					i--;
				}
			}

			if (changedItems.Count == 0)
				return;

			RaiseChangeNotificationEvents(
					action: NotifyCollectionChangedAction.Remove,
					changedItems: changedItems);
		}

		/// <summary> 
		/// Clears the current collection and replaces it with the specified item. 
		/// </summary> 
		public void ClearAndAdd(T item) => ClearAndAddRange(new T[] { item });

		/// <summary> 
		/// Clears the current collection and replaces it with the specified collection. 
		/// </summary> 
		public void ClearAndAddRange(IEnumerable<T> collection)
		{
			if (collection == null)
				throw new ArgumentNullException(nameof(collection));

			CheckReentrancy();

			var previouslyEmpty = Items.Count == 0;

			Items.Clear();

			AddArrangeCore(collection);

			var currentlyEmpty = Items.Count == 0;

			if (previouslyEmpty && currentlyEmpty)
				return;

			RaiseChangeNotificationEvents(action: NotifyCollectionChangedAction.Reset);
		}

		private void AddArrangeCore(IEnumerable<T> collection)
		{
			//var itemAdded = false;
			foreach (var item in collection)
			{
				Items.Add(item);
				//itemAdded = true;
			}
			//return itemAdded;
		}

		protected void RaiseChangeNotificationEvents(NotifyCollectionChangedAction action, List<T> changedItems = null, int startingIndex = -1)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));

			if (changedItems is null)
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
			else
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItems: changedItems, startingIndex: startingIndex));
		}

	}
}
