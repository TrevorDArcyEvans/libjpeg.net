using System;
using System.Diagnostics.CodeAnalysis;

namespace BitMiracle.LibJpeg
{
    /// <summary>
    /// Represents a row of image - collection of samples.
    /// </summary>
    [SuppressMessage("Reliability", "CA2022:Avoid inexact read with \'Stream.Read\'")]
    public class SampleRow
    {
        private Sample[] m_samples;

        /// <summary>
        /// Creates a row from raw samples data.
        /// </summary>
        /// <param name="row">Raw description of samples.<br/>
        /// You can pass collection with more than sampleCount samples - only sampleCount samples 
        /// will be parsed and all remaining bytes will be ignored.</param>
        /// <param name="sampleCount">The number of samples in row.</param>
        /// <param name="bitsPerComponent">The number of bits per component.</param>
        /// <param name="componentsPerSample">The number of components per sample.</param>
        public SampleRow(byte[] row, int sampleCount, byte bitsPerComponent, byte componentsPerSample)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            if (row.Length == 0)
                throw new ArgumentException("row is empty");

            if (sampleCount <= 0)
                throw new ArgumentOutOfRangeException("sampleCount");

            if (bitsPerComponent <= 0 || bitsPerComponent > 16)
                throw new ArgumentOutOfRangeException("bitsPerComponent");

            if (componentsPerSample <= 0 || componentsPerSample > 5)
                throw new ArgumentOutOfRangeException("componentsPerSample");
            
            using (BitStream bitStream = new BitStream(row))
            {
                m_samples = new Sample[sampleCount];
                for (int i = 0; i < sampleCount; ++i)
                    m_samples[i] = new Sample(bitStream, bitsPerComponent, componentsPerSample);
            }
        }

        /// <summary>
        /// Gets the number of samples in this row.
        /// </summary>
        /// <value>The number of samples.</value>
        public int Length
        {
            get
            {
                return m_samples.Length;
            }
        }


        /// <summary>
        /// Gets the sample at the specified index.
        /// </summary>
        /// <param name="sampleNumber">The number of sample.</param>
        /// <returns>The required sample.</returns>
        public Sample this[int sampleNumber]
        {
            get
            {
                return GetAt(sampleNumber);
            }
        }

        /// <summary>
        /// Gets the sample at the specified index.
        /// </summary>
        /// <param name="sampleNumber">The number of sample.</param>
        /// <returns>The required sample.</returns>
        public Sample GetAt(int sampleNumber)
        {
            return m_samples[sampleNumber];
        }
    }
}
