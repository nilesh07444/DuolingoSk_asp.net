using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuolingoSk.Helper
{
    public enum AdminRoles
    {
        AdminUser = 1,
        Agent = 2
    }

    public enum ExamResultStatus
    {
        Pending = 1,
        Complete = 2
    }

    public enum MaterialFileTypes
    {
        Document = 1,
        Video = 2
    }

    public enum MaterialTypes
    {
        Material = 1,
        Tips = 2
    }

}