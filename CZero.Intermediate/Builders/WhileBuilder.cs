using Ardalis.GuardClauses;
using CZero.Intermediate.Instructions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Intermediate.Builders
{
    class WhileBuilder
    {
        public WhileBuilder ParentWhile { get; }
        public Instruction StartLabel { get; }
        public Instruction DoneLabel { get; }

        public WhileBuilder(WhileBuilder parent, Instruction startLabel, Instruction doneLabel)
        {
            ParentWhile = parent;

            Guard.Against.Null(startLabel, nameof(startLabel));
            Guard.Against.Null(doneLabel, nameof(doneLabel));

            StartLabel = startLabel;
            DoneLabel = doneLabel;
        }


    }
}
