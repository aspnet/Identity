// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.AspNetCore.Identity
{
    using System;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public class AuthenticatorVerificationService
    {
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        /// <summary>
        /// Generates a new 160-bit security token (size of SHA1 hash).
        /// </summary>
        /// <returns>The new security token.</returns>
        public virtual byte[] GenerateSecret()
        {
            byte[] bytes = new byte[20];
            _rng.GetBytes(bytes);
            return bytes;
        }

        private static long GetUnixTimestamp()
        {
            return Convert.ToInt64(Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual bool VerifyCode(byte[] secret, int code)
        {
            var timestamp = Convert.ToInt64(GetUnixTimestamp() / 30);
            //return ComputeTotp(new HMACSHA1(secret), (ulong)timestamp, null);

            // Allow codes from 90s in each direction
            using (var hash = new HMACSHA1(secret))
            {
                for (long i = -2; i <= 2; i++)
                {
                    var expectedCode = 1;// Rfc6238AuthenticationService.ComputeTotp(hash, (ulong), i);
                    if (expectedCode == code)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="stepModifier"></param>
        /// <returns></returns>
        public int CalculateOneTimePassword(byte[] secret, int stepModifier = 0)
        {
            var timestamp = Convert.ToInt64(GetUnixTimestamp() / 30) + (long)stepModifier;
            return Rfc6238AuthenticationService.ComputeTotp(new HMACSHA1(secret), (ulong)timestamp, null);
        }

    }

    internal static class Base32
    {
        private static readonly string _base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        // See http://tools.ietf.org/html/rfc3548#section-5
        public static string ToBase32(byte[] input)
        {
            if (input == null) { throw new ArgumentNullException("input"); }

            StringBuilder sb = new StringBuilder();
            for (int offset = 0; offset < input.Length;)
            {
                byte a, b, c, d, e, f, g, h;
                int numCharsToOutput = GetNextGroup(input, ref offset, out a, out b, out c, out d, out e, out f, out g, out h);

                sb.Append((numCharsToOutput >= 1) ? _base32Chars[a] : '=');
                sb.Append((numCharsToOutput >= 2) ? _base32Chars[b] : '=');
                sb.Append((numCharsToOutput >= 3) ? _base32Chars[c] : '=');
                sb.Append((numCharsToOutput >= 4) ? _base32Chars[d] : '=');
                sb.Append((numCharsToOutput >= 5) ? _base32Chars[e] : '=');
                sb.Append((numCharsToOutput >= 6) ? _base32Chars[f] : '=');
                sb.Append((numCharsToOutput >= 7) ? _base32Chars[g] : '=');
                sb.Append((numCharsToOutput >= 8) ? _base32Chars[h] : '=');
            }

            return sb.ToString();
        }

        public static byte[] FromBase32(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (input.Length == 0)
            {
                return new byte[0];
            }

            input = input.TrimEnd('=').ToUpperInvariant();
            var output = new byte[input.Length * 5 / 8];

            //byte outputByte = 0;
            //var bitsToGo = 8;
            //var outputIndex = 0;

            //foreach (var c in input)
            //{

            //}

            int bit_buffer;
            int currentCharIndex;
            int bits_in_buffer;

            if (input.Length < 3)
            {
                output[0] = (byte)(_base32Chars.IndexOf(input[0]) | _base32Chars.IndexOf(input[1]) << 5);
                return output;
            }

            bit_buffer = (_base32Chars.IndexOf(input[0]) | _base32Chars.IndexOf(input[1]) << 5);
            bits_in_buffer = 10;
            currentCharIndex = 2;
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = (byte)bit_buffer;
                bit_buffer >>= 8;
                bits_in_buffer -= 8;
                while (bits_in_buffer < 8 && currentCharIndex < input.Length)
                {
                    bit_buffer |= _base32Chars.IndexOf(input[currentCharIndex++]) << bits_in_buffer;
                    bits_in_buffer += 5;
                }
            }

            return output;
        }

            // Check the size
        //    if (outputBytes.Length == 0)
        //    {
        //        throw new ArgumentException("Specified string is not valid Base32 format because it doesn't have enough data to construct a complete byte array");
        //    }

        //    // Position in the string
        //    int base32Position = 0;

        //    // Offset inside the character in the string
        //    int base32SubPosition = 0;

        //    // Position within outputBytes array
        //    int outputBytePosition = 0;

        //    // The number of bits filled in the current output byte
        //    int outputByteSubPosition = 0;

        //    // Normally we would iterate on the input array but in this case we actually iterate on the output array
        //    // We do it because output array doesn't have overflow bits, while input does and it will cause output array overflow if we don't stop in time
        //    while (outputBytePosition < outputBytes.Length)
        //    {
        //        // Look up current character in the dictionary to convert it to byte
        //        int currentBase32Byte = Base32Alphabet.IndexOf(base32StringUpperCase[base32Position]);

        //        // Check if found
        //        if (currentBase32Byte < 0)
        //        {
        //            throw new ArgumentException(string.Format("Specified string is not valid Base32 format because character \"{0}\" does not exist in Base32 alphabet", base32String[base32Position]));
        //        }

        //        // Calculate the number of bits we can extract out of current input character to fill missing bits in the output byte
        //        int bitsAvailableInByte = Math.Min(OutByteSize - base32SubPosition, InByteSize - outputByteSubPosition);

        //        // Make space in the output byte
        //        outputBytes[outputBytePosition] <<= bitsAvailableInByte;

        //        // Extract the part of the input character and move it to the output byte
        //        outputBytes[outputBytePosition] |= (byte)(currentBase32Byte >> (OutByteSize - (base32SubPosition + bitsAvailableInByte)));

        //        // Update current sub-byte position
        //        outputByteSubPosition += bitsAvailableInByte;

        //        // Check overflow
        //        if (outputByteSubPosition >= InByteSize)
        //        {
        //            // Move to the next byte
        //            outputBytePosition++;
        //            outputByteSubPosition = 0;
        //        }

        //        // Update current base32 byte completion
        //        base32SubPosition += bitsAvailableInByte;

        //        // Check overflow or end of input array
        //        if (base32SubPosition >= OutByteSize)
        //        {
        //            // Move to the next character
        //            base32Position++;
        //            base32SubPosition = 0;
        //        }
        //    }

        //    return outputBytes;



        //}

        // returns the number of bytes that were output
        private static int GetNextGroup(byte[] input, ref int offset, out byte a, out byte b, out byte c, out byte d, out byte e, out byte f, out byte g, out byte h)
        {
            Debug.Assert(offset < input.Length);
            uint b1, b2, b3, b4, b5;

            int retVal;
            switch (offset - input.Length)
            {
                case 1: retVal = 2; break;
                case 2: retVal = 4; break;
                case 3: retVal = 5; break;
                case 4: retVal = 7; break;
                default: retVal = 8; break;
            }

            b1 = (offset < input.Length) ? input[offset++] : 0U;
            b2 = (offset < input.Length) ? input[offset++] : 0U;
            b3 = (offset < input.Length) ? input[offset++] : 0U;
            b4 = (offset < input.Length) ? input[offset++] : 0U;
            b5 = (offset < input.Length) ? input[offset++] : 0U;

            a = (byte)(b1 >> 3);
            b = (byte)(((b1 & 0x07) << 2) | (b2 >> 6));
            c = (byte)((b2 >> 1) & 0x1f);
            d = (byte)(((b2 & 0x01) << 4) | (b3 >> 4));
            e = (byte)(((b3 & 0x0f) << 1) | (b4 >> 7));
            f = (byte)((b4 >> 2) & 0x1f);
            g = (byte)(((b4 & 0x3) << 3) | (b5 >> 5));
            h = (byte)(b5 & 0x1f);

            return retVal;
        }
    }

    // TAKEN from: http://scottless.com/blog/archive/2014/02/15/base32-encoder-and-decoder-in-c.aspx
    // which has a free to use/modify license https://creativecommons.org/licenses/by/2.0/


    /// <summary>
    /// Class used for conversion between byte array and Base32 notation
    /// </summary>
    internal sealed class Base32A
    {
        /// <summary>
        /// Size of the regular byte in bits
        /// </summary>
        private const int InByteSize = 8;

        /// <summary>
        /// Size of converted byte in bits
        /// </summary>
        private const int OutByteSize = 5;

        /// <summary>
        /// Alphabet
        /// </summary>
        private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        /// <summary>
        /// Convert byte array to Base32 format
        /// </summary>
        /// <param name="bytes">An array of bytes to convert to Base32 format</param>
        /// <returns>Returns a string representing byte array</returns>
        internal static string ToBase32(byte[] bytes)
        {
            // Check if byte array is null
            if (bytes == null)
            {
                return null;
            }
            // Check if empty
            else if (bytes.Length == 0)
            {
                return string.Empty;
            }

            // Prepare container for the final value
            StringBuilder builder = new StringBuilder(bytes.Length * InByteSize / OutByteSize);

            // Position in the input buffer
            int bytesPosition = 0;

            // Offset inside a single byte that <bytesPosition> points to (from left to right)
            // 0 - highest bit, 7 - lowest bit
            int bytesSubPosition = 0;

            // Byte to look up in the dictionary
            byte outputBase32Byte = 0;

            // The number of bits filled in the current output byte
            int outputBase32BytePosition = 0;

            // Iterate through input buffer until we reach past the end of it
            while (bytesPosition < bytes.Length)
            {
                // Calculate the number of bits we can extract out of current input byte to fill missing bits in the output byte
                int bitsAvailableInByte = Math.Min(InByteSize - bytesSubPosition, OutByteSize - outputBase32BytePosition);

                // Make space in the output byte
                outputBase32Byte <<= bitsAvailableInByte;

                // Extract the part of the input byte and move it to the output byte
                outputBase32Byte |= (byte)(bytes[bytesPosition] >> (InByteSize - (bytesSubPosition + bitsAvailableInByte)));

                // Update current sub-byte position
                bytesSubPosition += bitsAvailableInByte;

                // Check overflow
                if (bytesSubPosition >= InByteSize)
                {
                    // Move to the next byte
                    bytesPosition++;
                    bytesSubPosition = 0;
                }

                // Update current base32 byte completion
                outputBase32BytePosition += bitsAvailableInByte;

                // Check overflow or end of input array
                if (outputBase32BytePosition >= OutByteSize)
                {
                    // Drop the overflow bits
                    outputBase32Byte &= 0x1F;  // 0x1F = 00011111 in binary

                    // Add current Base32 byte and convert it to character
                    builder.Append(Base32Alphabet[outputBase32Byte]);

                    // Move to the next byte
                    outputBase32BytePosition = 0;
                }
            }

            // Check if we have a remainder
            if (outputBase32BytePosition > 0)
            {
                // Move to the right bits
                outputBase32Byte <<= (OutByteSize - outputBase32BytePosition);

                // Drop the overflow bits
                outputBase32Byte &= 0x1F;  // 0x1F = 00011111 in binary

                // Add current Base32 byte and convert it to character
                builder.Append(Base32Alphabet[outputBase32Byte]);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Convert base32 string to array of bytes
        /// </summary>
        /// <param name="base32String">Base32 string to convert</param>
        /// <returns>Returns a byte array converted from the string</returns>
        internal static byte[] FromBase32(string base32String)
        {
            // Check if string is null
            if (base32String == null)
            {
                return null;
            }
            // Check if empty
            else if (base32String == string.Empty)
            {
                return new byte[0];
            }

            // Convert to upper-case
            string base32StringUpperCase = base32String.ToUpperInvariant();

            // Prepare output byte array
            byte[] outputBytes = new byte[base32StringUpperCase.Length * OutByteSize / InByteSize];

            // Check the size
            if (outputBytes.Length == 0)
            {
                throw new ArgumentException("Specified string is not valid Base32 format because it doesn't have enough data to construct a complete byte array");
            }

            // Position in the string
            int base32Position = 0;

            // Offset inside the character in the string
            int base32SubPosition = 0;

            // Position within outputBytes array
            int outputBytePosition = 0;

            // The number of bits filled in the current output byte
            int outputByteSubPosition = 0;

            // Normally we would iterate on the input array but in this case we actually iterate on the output array
            // We do it because output array doesn't have overflow bits, while input does and it will cause output array overflow if we don't stop in time
            while (outputBytePosition < outputBytes.Length)
            {
                // Look up current character in the dictionary to convert it to byte
                int currentBase32Byte = Base32Alphabet.IndexOf(base32StringUpperCase[base32Position]);

                // Check if found
                if (currentBase32Byte < 0)
                {
                    throw new ArgumentException(string.Format("Specified string is not valid Base32 format because character \"{0}\" does not exist in Base32 alphabet", base32String[base32Position]));
                }

                // Calculate the number of bits we can extract out of current input character to fill missing bits in the output byte
                int bitsAvailableInByte = Math.Min(OutByteSize - base32SubPosition, InByteSize - outputByteSubPosition);

                // Make space in the output byte
                outputBytes[outputBytePosition] <<= bitsAvailableInByte;

                // Extract the part of the input character and move it to the output byte
                outputBytes[outputBytePosition] |= (byte)(currentBase32Byte >> (OutByteSize - (base32SubPosition + bitsAvailableInByte)));

                // Update current sub-byte position
                outputByteSubPosition += bitsAvailableInByte;

                // Check overflow
                if (outputByteSubPosition >= InByteSize)
                {
                    // Move to the next byte
                    outputBytePosition++;
                    outputByteSubPosition = 0;
                }

                // Update current base32 byte completion
                base32SubPosition += bitsAvailableInByte;

                // Check overflow or end of input array
                if (base32SubPosition >= OutByteSize)
                {
                    // Move to the next character
                    base32Position++;
                    base32SubPosition = 0;
                }
            }

            return outputBytes;
        }
    }

    internal static class Rfc6238AuthenticationService
    {
        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly TimeSpan _timestep = TimeSpan.FromMinutes(3);
        private static readonly Encoding _encoding = new UTF8Encoding(false, true);
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        // Generates a new 80-bit security token
        public static byte[] GenerateRandomKey()
        {
            byte[] bytes = new byte[20];
            _rng.GetBytes(bytes);
            return bytes;
        }

        public static int CalculateOneTimePassword(byte[] secret, int stepModifier = 0)
        {
            var timestamp = Convert.ToInt64(GetUnixTimestamp() / 30) + (long)stepModifier;
            return ComputeTotp(new HMACSHA1(secret), (ulong)timestamp, null);
        }

        private static long GetUnixTimestamp()
        {
            return Convert.ToInt64(Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds));
        }

        internal static int ComputeTotp(HashAlgorithm hashAlgorithm, ulong timestepNumber, string modifier)
        {
            // # of 0's = length of pin
            const int Mod = 1000000;

            // See https://tools.ietf.org/html/rfc4226
            // We can add an optional modifier
            var timestepAsBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((long)timestepNumber));
            var hash = hashAlgorithm.ComputeHash(ApplyModifier(timestepAsBytes, modifier));

            // Generate DT string
            var offset = hash[hash.Length - 1] & 0xf;
            Debug.Assert(offset + 4 < hash.Length);
            var binaryCode = (hash[offset] & 0x7f) << 24
                             | (hash[offset + 1] & 0xff) << 16
                             | (hash[offset + 2] & 0xff) << 8
                             | (hash[offset + 3] & 0xff);

            return binaryCode % Mod;
        }

        private static byte[] ApplyModifier(byte[] input, string modifier)
        {
            if (String.IsNullOrEmpty(modifier))
            {
                return input;
            }

            var modifierBytes = _encoding.GetBytes(modifier);
            var combined = new byte[checked(input.Length + modifierBytes.Length)];
            Buffer.BlockCopy(input, 0, combined, 0, input.Length);
            Buffer.BlockCopy(modifierBytes, 0, combined, input.Length, modifierBytes.Length);
            return combined;
        }

        // More info: https://tools.ietf.org/html/rfc6238#section-4
        private static ulong GetCurrentTimeStepNumber()
        {
            var delta = DateTime.UtcNow - _unixEpoch;
            return (ulong)(delta.Ticks / _timestep.Ticks);
        }

        public static int GenerateCode(byte[] securityToken, string modifier = null)
        {
            if (securityToken == null)
            {
                throw new ArgumentNullException(nameof(securityToken));
            }

            // Allow a variance of no greater than 90 seconds in either direction
            var currentTimeStep = GetCurrentTimeStepNumber();
            using (var hashAlgorithm = new HMACSHA1(securityToken))
            {
                return ComputeTotp(hashAlgorithm, currentTimeStep, modifier);
            }
        }

        public static bool ValidateCode(byte[] securityToken, int code, string modifier = null)
        {
            if (securityToken == null)
            {
                throw new ArgumentNullException(nameof(securityToken));
            }

            // Allow a variance of no greater than 90 seconds in either direction
            var currentTimeStep = GetCurrentTimeStepNumber();
            using (var hashAlgorithm = new HMACSHA1(securityToken))
            {
                for (var i = -2; i <= 2; i++)
                {
                    var computedTotp = ComputeTotp(hashAlgorithm, (ulong)((long)currentTimeStep + i), modifier);
                    if (computedTotp == code)
                    {
                        return true;
                    }
                }
            }

            // No match
            return false;
        }
    }
}
