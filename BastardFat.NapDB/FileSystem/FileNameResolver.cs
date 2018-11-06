using BastardFat.NapDB.Abstractions;
using BastardFat.NapDB.Abstractions.DataStructs;
using BastardFat.NapDB.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.FileSystem
{
    public class FileNameResolver<TKey> : IFileNameResolver<TKey>
    {
        private const string NullStringReplacer = "_null_";

        public virtual string GetFilename(FileNameSegments segments)
        {
            var allSegments = new[] { segments.Key ?? NullStringReplacer, segments.Signature ?? NullStringReplacer }.Concat(segments.AdditionalSegments.Select(x => x.Value));
            return String.Join(".", allSegments.Select(x => x ?? NullStringReplacer));
        }

        public virtual string GetSearchPattern(FileNameSegments segments)
        {
            var allSegments = new[] { segments.Key ?? "*", segments.Signature ?? "*" }.Concat(segments.AdditionalSegments.Select(x => x.Value));
            return String.Join(".", allSegments.Select(x => x ?? "*"));
        }

        public virtual FileNameSegments GetSegments(string filename, string[] additionalSegmentsNames)
        {
            var segments = filename
                .Split('.')
                .Select(x => x == NullStringReplacer ? null : x)
                .ToArray();

            if (segments.Length != additionalSegmentsNames.Length + 2)
                throw new ArgumentException($"Filename '{filename}' must contains {additionalSegmentsNames.Length + 2} segments");

            return new FileNameSegments
            {
                Key = segments[0],
                Signature = segments[1],
                AdditionalSegments = segments
                    .Skip(2)
                    .Select((s, i) => new KeyValuePair<string, string>(additionalSegmentsNames[i], s))
                    .ToDictionary(x => x.Key, x => x.Value)
            };

        }

        public virtual TKey ParseKey(string keyString)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(TKey));
                if (converter != null)
                    return (TKey)converter.ConvertFromString(keyString);
                throw NotSupported();
            }
            catch (NotSupportedException ex)
            {
                throw NotSupported(ex);
            }
        }
        public virtual string StringifyKey(TKey key)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(TKey));
                if (converter != null)
                    return converter.ConvertToString(key);
                throw NotSupported();
            }
            catch (NotSupportedException ex)
            {
                throw NotSupported(ex);
            }
        }


        private static NapDbException NotSupported(Exception inner = null)
        {
            return new NapDbException($"Type {typeof(TKey).FullName} is not supported as key. " +
                $"You must implement your own IFileNameResolver<TKey> or override ParseKey and StringifyKey", inner);
        }
    }
}

