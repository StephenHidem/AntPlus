using System;
using System.Collections.Generic;

namespace AntPlus.UnitTests
{
    internal class Utils
    {
        /// <summary>Shuffles the data pages. Modern implementation of the Fisher-Yates Shuffle algorithm.</summary>
        /// <param name="listToShuffle">The list to shuffle.</param>
        /// <returns>A new list of pages in randomized order.</returns>
        /// <remarks>This is extremely useful testing data that spans multiple pages. Note there is no
        /// guaratee that ANT data pages will be received in order.</remarks>
        public static List<byte[]> ShuffleDataPages(List<byte[]> listToShuffle)
        {
            Random rand = new();
            for (int i = listToShuffle.Count - 1; i > 0; i--)
            {
                var k = rand.Next(i + 1);
                (listToShuffle[i], listToShuffle[k]) = (listToShuffle[k], listToShuffle[i]);
            }
            return listToShuffle;
        }
    }
}
