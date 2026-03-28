using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentIL
{
    public delegate Cut PointCut(in Cut cut);

    public static class CutEvents
    {
        public static Action<MethodBody> OnModify { get; set; } = m => { };
    }

    public readonly struct Cut
    {
        private readonly bool _entry;
        private readonly bool _exit;
        private readonly Instruction _refInst;
        private readonly MethodBody _body;

        public MethodDefinition Method => this._body.Method;
        public TypeSystem TypeSystem => this._body.Method.Module.TypeSystem;

        private Collection<Instruction> Instructions => this._body.Instructions;

        public Cut(MethodBody body, bool entry, bool exit)
        {
            if (!entry && !exit) throw new ArgumentException("Should be either entry or exit");

            this._body = body;
            this._entry = entry;
            this._exit = exit;
            this._refInst = null;
        }

        public Cut(MethodBody body, Instruction instruction)
        {
	        this._refInst = instruction ?? throw new ArgumentNullException(nameof(instruction));
	        this._body = body ?? throw new ArgumentNullException(nameof(body));

	        this._entry = false;
	        this._exit = false;
        }

        public Cut Next()
        {
            if (this._entry) return this;
            if (this.Instructions[this.Instructions.Count - 1] == this._refInst) return new Cut(this._body, false, true);
            return new Cut(this._body, this._refInst.Next);
        }

        public Cut Prev()
        {
            if (this._exit) return this;
            if (this.Instructions.Count != 0 && this.Instructions[0] == this._refInst) return new Cut(this._body, true, false);
            return new Cut(this._body, this._refInst.Previous);
        }

        public Cut SkipNops()
        {
            if (this._exit) return this;
            var i = this._entry ? this._body.Instructions[0] : this._refInst;
            while (i.OpCode == OpCodes.Nop)
                i = i.Next;
            return new Cut(this._body, i);
        }

        public Cut Here(PointCut pc)
        {
            if (pc == null) return this;
            return pc(this);
        }

        public Cut Write(Instruction instruction)
        {
            CutEvents.OnModify(this._body);

            if (this._entry)
            {
	            this.Instructions.Insert(0, instruction);

                foreach (var handler in this._body.ExceptionHandlers.Where(h => h.HandlerStart == null).ToList())
                    handler.HandlerStart = this._refInst;
            }
            else if (this._exit || this._refInst == this.Instructions[this.Instructions.Count - 1])
            {
	            this.Instructions.Add(instruction);

                if (!this._exit)
                    foreach (var handler in this._body.ExceptionHandlers.Where(h => h.HandlerEnd == null).ToList())
                        handler.HandlerEnd = this._refInst;
            }
            else
            {
                var index = this.Instructions.IndexOf(this._refInst) + 1;
                this.Instructions.Insert(index, instruction);
            }

            return new Cut(this._body, instruction);
        }

        public Instruction Emit(OpCode opCode, object operand)
        {
            switch (operand)
            {
                case Cut pc: return Instruction.Create(opCode, pc._refInst ?? throw new InvalidOperationException());
                case TypeReference tr: return Instruction.Create(opCode, this.Method.Module.ImportReference(tr));
                case MethodReference mr: return Instruction.Create(opCode, this.Method.Module.ImportReference(mr));
                case CallSite cs: return Instruction.Create(opCode, cs);
                case FieldReference fr: return Instruction.Create(opCode, this.Method.Module.ImportReference(fr));
                case string str: return Instruction.Create(opCode, str);
                case char c: return Instruction.Create(opCode, c);
                case byte b: return Instruction.Create(opCode, b);
                case sbyte sb: return Instruction.Create(opCode, sb);
                case int i: return Instruction.Create(opCode, i);
                case short i: return Instruction.Create(opCode, i);
                case ushort i: return Instruction.Create(opCode, i);
                case long l: return Instruction.Create(opCode, l);
                case float f: return Instruction.Create(opCode, f);
                case double d: return Instruction.Create(opCode, d);
                case Instruction inst: return Instruction.Create(opCode, inst);
                case Instruction[] insts: return Instruction.Create(opCode, insts);
                case VariableDefinition vd: return Instruction.Create(opCode, vd);
                case ParameterDefinition pd: return Instruction.Create(opCode, pd);

                default: throw new NotSupportedException($"Not supported operand type '{operand.GetType()}'");
            }
        }

        public Instruction Emit(OpCode opCode)
        {
            return Instruction.Create(opCode);
        }

        public Cut Replace(Instruction instruction)
        {
            CutEvents.OnModify(this._body);

            if (this._exit || this._entry) return this.Write(instruction);

            this.Redirect(this._refInst, instruction, instruction);
            this.Instructions[this.Instructions.IndexOf(this._refInst)] = instruction;

            return new Cut(this._body, instruction);
        }

        public Cut Remove()
        {
            CutEvents.OnModify(this._body);

            var prevCut = this.Prev();

            var next = this._refInst.Next;
            var prev = this._refInst.Previous;

            this.Redirect(this._refInst, next, prev);
            this.Instructions.Remove(this._refInst);

            return prevCut;
        }

        private void Redirect(Instruction source, Instruction next, Instruction prev)
        {
            var refs = this.Instructions.Where(i => i.Operand == source).ToList();

            if (refs.Any())
            {
                if (next == null)
                    throw new InvalidOperationException("Cannot redirect to non existing instruction");

                foreach (var rref in refs)
                    rref.Operand = next;
            }

            foreach (var handler in this._body.ExceptionHandlers)
            {
                if (handler.FilterStart == source)
                    handler.FilterStart = prev ?? throw new InvalidOperationException();

                if (handler.HandlerEnd == source)
                    handler.HandlerEnd = next ?? throw new InvalidOperationException();

                if (handler.HandlerStart == source)
                    handler.HandlerStart = prev ?? throw new InvalidOperationException();

                if (handler.TryEnd == source)
                    handler.TryEnd = next ?? throw new InvalidOperationException();

                if (handler.TryStart == source)
                    handler.TryStart = prev ?? throw new InvalidOperationException();
            }
        }

    }
}