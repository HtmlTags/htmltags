using System;
using System.Collections.Generic;
using System.Threading;

namespace HtmlTags.Conventions
{
    public interface ITagGeneratorFactory
    {
        ITagGenerator<T> GeneratorFor<T>() where T : TagRequest;
    }

    public class TagGeneratorFactory : ITagGeneratorFactory
    {
        private readonly ActiveProfile _profile;
        private readonly HtmlConventionLibrary _library;
        private readonly IEnumerable<ITagRequestActivator> _activators;
        private readonly IDictionary<Type, object> _generators = new Dictionary<Type, object>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public TagGeneratorFactory(ActiveProfile profile, HtmlConventionLibrary library, IEnumerable<ITagRequestActivator> activators)
        {
            _profile = profile;
            _library = library;
            _activators = activators;
        }

        public ITagGenerator<T> GeneratorFor<T>() where T : TagRequest
        {
            var generator = _lock.MaybeWrite<TagGenerator<T>>(answer: () => (TagGenerator<T>) _generators[typeof (T)],
                                             missingTest: () => !_generators.ContainsKey(typeof (T)),
                                             write: buildNew<T>);

            return generator;
        }

        private void buildNew<T>() where T : TagRequest
        {
            var generator = new TagGenerator<T>(_library.For<T>(), _activators, _profile);
            _generators.Add(typeof(T), generator);
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