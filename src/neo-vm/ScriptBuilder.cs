﻿using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Neo.VM
{
    public class ScriptBuilder : IDisposable
    {
        private readonly MemoryStream ms = new MemoryStream();
        private readonly BinaryWriter writer;

        public int Offset => (int)ms.Position;

        public ScriptBuilder()
        {
            this.writer = new BinaryWriter(ms);
        }

        public void Dispose()
        {
            writer.Dispose();
            ms.Dispose();
        }

        public ScriptBuilder Emit(OpCode op, byte[] arg = null)
        {
            writer.Write((byte)op);
            if (arg != null)
                writer.Write(arg);
            return this;
        }

        public ScriptBuilder EmitCall(short offset, byte rvcount, byte pcount)
        {
            byte[] operand = new byte[4];
            operand[0] = rvcount;
            operand[1] = pcount;
            operand[2] = (byte)(offset & 0xff);
            operand[3] = (byte)(offset >> 8);
            return Emit(OpCode.CALL_I, operand);
        }

        public ScriptBuilder EmitJump(OpCode op, short offset)
        {
            if (op != OpCode.JMP && op != OpCode.JMPIF && op != OpCode.JMPIFNOT)
                throw new ArgumentException();
            return Emit(op, BitConverter.GetBytes(offset));
        }

        public ScriptBuilder EmitPush(BigInteger number)
        {
            if (number == -1) return Emit(OpCode.PUSHM1);
            if (number == 0) return Emit(OpCode.PUSH0);
            if (number > 0 && number <= 16) return Emit(OpCode.PUSH1 - 1 + (byte)number);
            return EmitPush(number.ToByteArray());
        }

        public ScriptBuilder EmitPush(bool data)
        {
            return Emit(data ? OpCode.PUSHT : OpCode.PUSHF);
        }

        public ScriptBuilder EmitPush(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException();
            if (data.Length <= (int)OpCode.PUSHBYTES75)
            {
                writer.Write((byte)data.Length);
                writer.Write(data);
            }
            else if (data.Length < 0x100)
            {
                Emit(OpCode.PUSHDATA1);
                writer.Write((byte)data.Length);
                writer.Write(data);
            }
            else if (data.Length < 0x10000)
            {
                Emit(OpCode.PUSHDATA2);
                writer.Write((ushort)data.Length);
                writer.Write(data);
            }
            else// if (data.Length < 0x100000000L)
            {
                Emit(OpCode.PUSHDATA4);
                writer.Write(data.Length);
                writer.Write(data);
            }
            return this;
        }

        public ScriptBuilder EmitPush(string data)
        {
            return EmitPush(Encoding.UTF8.GetBytes(data));
        }

        public ScriptBuilder EmitSysCall(string api, bool compress = true)
        {
            if (api == null) throw new ArgumentNullException(nameof(api));
            if (api.Length == 0) throw new ArgumentException(nameof(api));

            byte[] api_bytes = Encoding.ASCII.GetBytes(api);

            if (compress)
            {
                using (var sha = SHA256.Create())
                    api_bytes = sha.ComputeHash(api_bytes);
                Array.Resize(ref api_bytes, 4);
            }
            else
            {
                if (api_bytes.Length > 252)
                    throw new ArgumentException(nameof(api));
            }

            byte[] arg = new byte[api_bytes.Length + 1];
            arg[0] = (byte)api_bytes.Length;
            Unsafe.MemoryCopy(api_bytes, 0, arg, 1, api_bytes.Length);
            return Emit(OpCode.SYSCALL, arg);
        }

        public byte[] ToArray()
        {
            writer.Flush();
            return ms.ToArray();
        }
    }
}
