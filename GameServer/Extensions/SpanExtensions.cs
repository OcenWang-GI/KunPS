namespace GameServer.Extensions
{
    internal static class SpanExtensions
    {
        /// <summary>
        /// Writes a 24-bit integer to a span in little-endian format.
        /// </summary>
        /// <param name="span">The span to write the integer to.</param>
        /// <param name="value">The integer value to write.</param>
        public static void WriteInt24LittleEndian(this Span<byte> span, int value)
        {
            span[0] = (byte)value;
            span[1] = (byte)(value >> 8);
            span[2] = (byte)(value >> 16);
        }

        /// <summary>
        /// Reads a 24-bit integer from a read-only span in little-endian format.
        /// </summary>
        /// <param name="span">The read-only span to read the integer from.</param>
        /// <returns>The integer value read from the span.</returns>
        public static int ReadInt24LittleEndian(this ReadOnlySpan<byte> span)
        {
            return span[0] | span[1] << 8 | span[2] << 16;
        }
    }
}