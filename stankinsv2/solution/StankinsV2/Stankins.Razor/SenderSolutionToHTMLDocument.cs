﻿using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stankins.Razor
{
    public class SenderSolutionToHTMLDocument : SenderToRazor, ISenderToOutput
    {
        public SenderSolutionToHTMLDocument(string inputTemplate = null) : this(new CtorDictionary() {
                { nameof(InputTemplate), inputTemplate}

            })
        {
            
        }

        public SenderSolutionToHTMLDocument(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(SenderSolutionToHTMLDocument);
        }

        public override string DefaultText()
        {
            return base.ReadFile($"{nameof(SenderSolutionToHTMLDocument)}.cshtml");
        }
    }
}