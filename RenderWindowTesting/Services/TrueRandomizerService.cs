using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace RenderWindowTesting.Services
{
    /// <summary>
    /// Provides methods for randomizing numbers.
    /// </summary>
    public class TrueRandomizerService : IRandomizerService
    {
        #region Private Fields
        private readonly RNGCryptoServiceProvider _provider = new RNGCryptoServiceProvider();
        private readonly byte[] _uint32Buffer = new byte[4];
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="PseudoRandomizerService"/>.
        /// </summary>
        public TrueRandomizerService() { }
        #endregion


        #region Public Methods
        /// <summary>
        /// Returns a true/false value that represents the flip of a coin.
        /// </summary>
        /// <returns></returns>
        public bool FlipCoin() => GetValue(0f, 1f) <= 0.5f;


        /// <summary>
        /// Gets a random number between the given <paramref name="minValue"/> and <paramref name="maxValue"/>s.
        /// A random value will be chosen between the min and max values no matter which value is less than 
        /// or greater than the other.
        /// </summary>
        /// <param name="minValue">The inclusive minimum value of the range to randomly choose from.</param>
        /// <param name="maxValue">The inclusive maximum value of the range to randomly choose from.</param>
        /// <returns></returns>
        public float GetValue(float minValue, float maxValue)
        {
            var minValueAsInt = (int)((minValue + 0.001f) * 1000);
            var maxValueAsInt = (int)((maxValue + 0.001f) * 1000);

            if (minValueAsInt > maxValueAsInt)
            {
                return (float)Math.Round(GetValue(maxValueAsInt, minValueAsInt) / 1000f, 3);
            }
            else
            {
                return (float)Math.Round(GetValue(minValueAsInt, maxValueAsInt) / 1000f, 3);
            }
        }


        /// <summary>
        /// Gets a random number between the given <paramref name="minValue"/> and <paramref name="maxValue"/>s.
        /// A random value will be chosen between the min and max values no matter which value is less than 
        /// or greater than the other.
        /// </summary>
        /// <param name="minValue">The inclusive minimum value of the range to randomly choose from.</param>
        /// <param name="maxValue">The inclusive maximum value of the range to randomly choose from.</param>
        /// <returns></returns>
        public double GetValue(double minValue, double maxValue) =>
            //Add 0.001 so that way the max value is inclusive.
            GetValue((float)minValue, (float)maxValue);


        /// <summary>
        /// Gets a random number between the given <paramref name="minValue"/> and <paramref name="maxValue"/>s.
        /// A random value will be chosen between the min and max values no matter which value is less than 
        /// or greater than the other.
        /// </summary>
        /// <param name="minValue">The inclusive minimum value of the range to randomly choose from.</param>
        /// <param name="maxValue">The inclusive maximum value of the range to randomly choose from.</param>
        /// <returns></returns>
        public int GetValue(int minValue, int maxValue)
        {
            //If the min value is greater than the max,
            //swap the values.
            if (minValue > maxValue)
            {
                var valueTemp = minValue;
                minValue = maxValue;
                maxValue = valueTemp;
            }

            if (minValue == maxValue) return minValue;

            maxValue += 1;

            var diff = (long)(maxValue - minValue);

            while (true)
            {
                _provider.GetBytes(_uint32Buffer);

                var rand = Math.Abs((int)BitConverter.ToUInt32(_uint32Buffer, 0));
                var max = 1 + (long)int.MaxValue;
                var remainder = max % diff;

                if (rand < max - remainder)
                    return (int)(minValue + (rand % diff));
            }
        }
        #endregion
    }

}
