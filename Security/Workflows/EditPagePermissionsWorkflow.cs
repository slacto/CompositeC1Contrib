using System;
using System.Linq;

using Composite.C1Console.Workflow;
using Composite.Data;
using Composite.Data.Types;

using CompositeC1Contrib.Security.Data.Types;

namespace CompositeC1Contrib.Security.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public sealed class EditPagePermissionsWorkflow : BaseEditPermissionsWorkflow<IPagePermissions, IPage>
    {
        protected override EvaluatedPermissions GetEvaluatedPermissions()
        {
            return EvaluatedPagePermissions.GetEvaluatedPermissionsForPage(DataEntity);
        }

        protected override IPagePermissions GetPermissions()
        {
            using (var data = new DataConnection())
            {
                return data.Get<IPagePermissions>().SingleOrDefault(p => p.PageId == DataEntity.Id);
            }
        }

        protected override void SavePermissions(IPagePermissions permissions)
        {
            using (var data = new DataConnection())
            {
                if (permissions.PageId == Guid.Empty)
                {
                    permissions.PageId = DataEntity.Id;

                    data.Add(permissions);
                }
                else
                {
                    data.Update(permissions);
                }
            }
        }
    }
}
