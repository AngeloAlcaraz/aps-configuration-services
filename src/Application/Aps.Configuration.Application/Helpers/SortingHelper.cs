namespace Aps.Configuration.Application.Helpers;

public static class SortingHelper
{
    /// <summary>
    /// Sorts a collection of items based on a specified property and sort direction.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <param name="source">The collection of items to be sorted.</param>
    /// <param name="sortBy">The name of the property to sort by.</param>
    /// <param name="sortDescending">A boolean value indicating whether to sort in descending order.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> sorted by the specified property and sort direction.</returns>
    /// <exception cref="ArgumentException">Thrown when the specified property is not found in the type.</exception>
    public static IEnumerable<T> SortByProperty<T>(this IEnumerable<T> source, string sortBy, bool sortDescending)
    {
        if (string.IsNullOrEmpty(sortBy))
            return source;

        var propertyInfo = typeof(T).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        return propertyInfo switch
        {
            null => throw new ArgumentException($"No such property: {sortBy}", nameof(sortBy)),
            _ => sortDescending
                            ? source.OrderByDescending(x => propertyInfo.GetValue(x, null))
                            : source.OrderBy(x => propertyInfo.GetValue(x, null)),
        };
    }
}