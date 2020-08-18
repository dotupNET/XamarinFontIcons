using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotup.Binding.Collections
{
	public static class BindingCollectionExtensions
	{
		public static async Task Fill<T>(this BindingCollection<T> self, IEnumerable<T> items, int itemsPerIteration = 100)
		{
			self.AllItems = items;

			self.Clear();

			if (itemsPerIteration == -1)
			{
				self.AddRange(items);
			}
			else
			{
				var groupCount = items.Count() / itemsPerIteration;
				for (int index = 0; index < groupCount; index++)
				{
					var part = items.Skip(index * itemsPerIteration).Take(itemsPerIteration);
					self.AddRange(part);
					await Task.Delay(1).ConfigureAwait(true);
				}
			}
		}

	}
}