using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.VM;
using Neo.VM.Types;
using System;

namespace Neo.Test
{
    [TestClass]
    public class UtStruct
    {
        private readonly Struct @struct;

        public UtStruct()
        {
            @struct = new Struct { 1 };
            for (int i = 0; i < 20000; i++)
                @struct = new Struct { @struct };
        }

        [TestMethod]
        public void Clone()
        {
            Struct s1 = new() { 1, new Struct { 2 } };
            Struct s2 = s1.Clone(ExecutionEngineLimits.Default);
            s1[0] = 3;
            Assert.AreEqual(1, s2[0]);
            ((Struct)s1[1])[0] = 3;
            Assert.AreEqual(2, ((Struct)s2[1])[0]);
            Assert.ThrowsException<ArgumentException>(() => @struct.Clone(ExecutionEngineLimits.Default));
        }

        [TestMethod]
        public void Equals()
        {
            Struct s1 = new() { 1, new Struct { 2 } };
            Struct s2 = new() { 1, new Struct { 2 } };
            Assert.IsTrue(s1.Equals(s2));
            Struct s3 = new() { 1, new Struct { 3 } };
            Assert.IsFalse(s1.Equals(s3));
            Assert.ThrowsException<ArgumentException>(() => @struct.Equals(@struct.Clone(ExecutionEngineLimits.Default)));
        }
    }
}
