using System.Diagnostics;

namespace Composer.Server
{
    public class Activities
    {
        private static ActivitySource ActivitySource = new ActivitySource("Composer.Server");

        public static Activity GetProjectName()
        {
            return ActivitySource.StartActivity("GetProjectNameAsync", ActivityKind.Consumer);
        }

        public static Activity GetProjectRoles()
        {
            return ActivitySource.StartActivity("GetProjectRolesAsync", ActivityKind.Consumer);
        }
    }
}
