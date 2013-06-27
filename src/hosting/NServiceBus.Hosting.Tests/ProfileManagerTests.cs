namespace NServiceBus.Hosting.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Profiles;

    [TestFixture]
    public class ProfileManagerTests
    {
        public interface MyProfile : IProfile, AlsoThisInterface
        {
        }

        public interface AlsoThisInterface : IProfile
        {
        }

        [Test]
        public void ActiveProfileInMyAssembly()
        {
            var allAssemblies = AssemblyPathHelper.GetAllAssemblies();
            var profileManager = new ProfileManager(allAssemblies, null, new[] { typeof(MyProfile).FullName }, null);
            Assert.IsTrue(profileManager.activeProfiles.Any(x => x == typeof(MyProfile)));
            Assert.IsTrue(profileManager.activeProfiles.Any(x => x == typeof(AlsoThisInterface)));
            Assert.AreEqual(2, profileManager.activeProfiles.Count);
        }

        [Test]
        public void Should_use_most_specific_profile()
        {
            var allAssemblies = AssemblyPathHelper.GetAllAssemblies();
            var args = new[] { typeof(CustomProductionProfile).FullName };
            var profileManager = new ProfileManager(allAssemblies, null, args, new List<Type> { typeof(Production) });
            var configureLogging = profileManager.GetImplementor<IConfigureLogging>(typeof(IConfigureLoggingForProfile<>));
            Assert.AreEqual(configureLogging.GetType(), typeof(CustomLoggingProfile));
        }

        public interface CustomProductionProfile : Production
        {
        }

        public class CustomLoggingProfile : IConfigureLoggingForProfile<CustomProductionProfile>
        {
            public void Configure(IConfigureThisEndpoint specifier)
            {
            }
        }

        public class ProductionLoggingHandler : IConfigureLoggingForProfile<Production>
        {
            public void Configure(IConfigureThisEndpoint specifier)
            {

            }
        }
    }
}