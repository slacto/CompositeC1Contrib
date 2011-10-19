﻿using System;

using Composite.C1Console.Security;
using Composite.Functions;

namespace CompositeC1Contrib.RazorFunctions
{
    [SecurityAncestorProvider(typeof(StandardFunctionSecurityAncestorProvider))]
    public class FunctionsProviderEntityToken : EntityToken
    {
        private string _id;
        public override string Id
        {
            get { return this._id; ; }
        }

        private string _source;
        public override string Source
        {
            get { return this._source; }
        }

        public override string Type
        {
            get { return String.Empty; ; }
        }

        public FunctionsProviderEntityToken(string source, string id)
        {
            _source = source;
            _id = id;
        }

        public override string Serialize()
        {
            return DoSerialize();
        }

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            string type;
            string source;
            string id;

            EntityToken.DoDeserialize(serializedEntityToken, out type, out source, out id);

            return new FunctionsProviderEntityToken(source, id);
        }
    }
}