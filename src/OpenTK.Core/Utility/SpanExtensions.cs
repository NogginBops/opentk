using System;

namespace OpenTK.Core.Utility
{
    /// <summary>
    /// A collection of useful extension methods for <see cref="Span{T}"/> and <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public static class SpanExtensions
    {
        /// <summary>
        /// Returns a new span that ends at the first null char in the input span.
        /// This is useful for fixed length c strings that contain a null terminator.
        /// </summary>
        /// <param name="span">The span to null-terminate.</param>
        /// <returns>The null-terminated span.</returns>
        public static Span<char> SliceAtFirstNull(this Span<char> span)
        {
            int index = span.IndexOf("\0");
            return index == -1 ? span : span.Slice(0, index);
        }

        /// <summary>
        /// Returns a new span that ends at the first null char in the input span.
        /// This is useful for fixed length c strings that contain a null terminator.
        /// </summary>
        /// <param name="span">The span to null-terminate.</param>
        /// <returns>The null-terminated span.</returns>
        public static ReadOnlySpan<char> SliceAtFirstNull(this ReadOnlySpan<char> span)
        {
            int index = span.IndexOf("\0");
            return index == -1 ? span : span.Slice(0, index);
        }

        /// <summary>
        /// Returns a new span that ends at the first null byte in the input span.
        /// This is useful for fixed length c strings that contain a null terminator.
        /// </summary>
        /// <param name="span">The span to null-terminate.</param>
        /// <returns>The null-terminated span.</returns>
        public static Span<byte> SliceAtFirstNull(this Span<byte> span)
        {
            int index = span.IndexOf((byte)0);
            return index == -1 ? span : span.Slice(0, index);
        }

        /// <summary>
        /// Returns a new span that ends at the first null byte in the input span.
        /// This is useful for fixed length c strings that contain a null terminator.
        /// </summary>
        /// <param name="span">The span to null-terminate.</param>
        /// <returns>The null-terminated span.</returns>
        public static ReadOnlySpan<byte> SliceAtFirstNull(this ReadOnlySpan<byte> span)
        {
            int index = span.IndexOf((byte)0);
            return index == -1 ? span : span.Slice(0, index);
        }
    }
}