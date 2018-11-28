using System;
using System.Linq;
using System.Reflection;

namespace TestAOP
{
    public static class Helper
    {
        public static PropertyInfo GetAttributedProperty(this object obj, Type attrib)
        {
            return 
                obj.GetType().GetProperties()
                    .FirstOrDefault(
                        p => p.GetCustomAttributes(true).Cast<Attribute>().Any(a => a.GetType() == attrib));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            #region entity setup

            var poco1 = new MyPoco
            {
                Key = 1,
                Id = 100,
                Name = "1000"
            };

            var poco2 = new YourPoco
            {
                Key = 2,
                Id = 200,
                Name = "2000"
            };

            var poco3 = new OurPoco
            {
                Key = 4,
                Id = 400,
                Name = "4000"
            };

            var poco4 = new TheirPoco
            {
                Key = 3,
                Id = 300,
                Name = "3000"
            };

            #endregion

            Verbose(poco1);
            Verbose(poco2);
            Verbose(poco3);
            Verbose(poco4);
            Console.ReadKey();
        }

        private static PropertyInfo GetAttributeProperty(IPoco entity)
        {
            var obj = entity.GetType();
            foreach (PropertyInfo p in obj.GetProperties())
            {
                foreach (Attribute a in p.GetCustomAttributes(true))
                {
                    if (a is TestAttribute) return p;

                    //TestAttribute k = (TestAttribute)a;
                }
            }
            return null;
        }

        private static void Verbose(IPoco poco)
        {
            //var prop = GetAttributeProperty(poco);
            var prop = poco.GetAttributedProperty(typeof(TestAttribute));


            var obj = poco.GetType();
            if (prop == null)
            {
                Console.WriteLine($"The POCO `{obj.Name}` does not have the `TestAttribute`.");
                return;
            }
            Console.WriteLine($"The POCO `{obj.Name}` has the `TestAttribute` on property `{prop.Name}` and it's value is \"{prop.GetValue(poco)}\"" );
        }
    }

    public interface IPoco
    {

    }

    public class YourPoco : IPoco
    {
        [Test]
        public int Key { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class OurPoco : IPoco
    {
        public int Key { get; set; }
        [Test]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class MyPoco : IPoco
    {
        public int Key { get; set; }
        public int Id { get; set; }
        [Test]
        public string Name { get; set; }

    }

    public class TheirPoco : IPoco
    {
        public int Key { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class TestAttribute : Attribute
    {
        public TestAttribute() { }
    }
}
