﻿using System;
using System.Collections.Generic;
using System.Text;

using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.Serialization;

using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib
{
    public class ConfirmWorkflowActionToken : WorkflowActionToken
    {
        public ConfirmWorkflowActionToken(string confirmMessage, Type type) : this(confirmMessage, type, new[] { PermissionType.Administrate }) { }

        public ConfirmWorkflowActionToken(string confirmMessage, Type type, IEnumerable<PermissionType> permmissionType) :
            base(typeof(ConfirmWorkflow), permmissionType)
        {
            var sb = new StringBuilder();

            StringConversionServices.SerializeKeyValuePair(sb, "ConfirmMessage", confirmMessage);
            StringConversionServices.SerializeKeyValuePair<Type>(sb, "Type", type);

            Payload = sb.ToString();
        }

        public new static ActionToken Deserialize(string serialiedWorkflowActionToken)
        {
            return WorkflowActionToken.Deserialize(serialiedWorkflowActionToken);
        }
    }
}
