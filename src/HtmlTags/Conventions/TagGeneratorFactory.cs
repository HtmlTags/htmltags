using System;
using System.Collections.Generic;
using System.Threading;

namespace HtmlTags.Conventions
{
    public interface ITagGeneratorFactory
    {
        ITagGenerator GeneratorFor();
    }

    public class TagGeneratorFactory : ITagGeneratorFactory
    {
        private readonly ActiveProfile _profile;
        private readonly HtmlConventionLibrary _library;
        // TODO: Make this only one generator
        private readonly IDictionary<Type, object> _generators = new Dictionary<Type, object>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public TagGeneratorFactory(ActiveProfile profile, HtmlConventionLibrary library)
        {
            _profile = profile;
            _library = library;
        }

        public ITagGenerator GeneratorFor()
        {
            var generator = _lock.MaybeWrite(answer: () => (TagGenerator) _generators[typeof (ElementRequest)],
                                             missingTest: () => !_generators.ContainsKey(typeof (ElementRequest)),
                                             write: buildNew);

            return generator;
        }

        private void buildNew()
        {
            var generator = new TagGenerator(_library.TagLibrary, _profile);
            _generators.Add(typeof(ElementRequest), generator);
        }
    }



    // Copied from FubuCore
    public static class ReaderWriterLockExtensions
    {
        public static void Write(this ReaderWriterLockSlim rwLock, Action action)
        {
            rwLock.EnterWriteLock();
            try
            {
                action();
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public static T Read<T>(this ReaderWriterLockSlim rwLock, Func<T> func)
        {
            rwLock.EnterReadLock();
            try
            {
                return func();
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public static void MaybeWrite(this ReaderWriterLockSlim theLock, Action action)
        {
            try
            {
                theLock.EnterUpgradeableReadLock();
                action();
            }
            finally
            {
                theLock.ExitUpgradeableReadLock();
            }
        }

        public static T MaybeWrite<T>(this ReaderWriterLockSlim theLock, Func<T> answer, Func<bool> missingTest,
                                      Action write)
        {
            try
            {
                theLock.EnterUpgradeableReadLock();
                if (missingTest())
                {
                    theLock.Write(() =>
                    {
                        if (missingTest())
                        {
                            write();
                        }
                    });
                }

                return answer();
            }
            finally
            {
                theLock.ExitUpgradeableReadLock();
            }
        }
    }
}