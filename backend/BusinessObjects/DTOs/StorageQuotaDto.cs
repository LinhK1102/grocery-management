using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs
{
    public class StorageQuotaDto
    {
        public long Limit { get; set; }
        public long Usage { get; set; }
        public long Remaining { get; set; }
        public long UsageInDrive { get; set; }
        public long UsageInDriveTrash { get; set; }

        public double Limit_GB { get; set; }
        public double Usage_GB { get; set; }
        public double Remaining_GB { get; set; }
        public double UsageInDrive_GB { get; set; }
        public double UsageInTrash_GB { get; set; }
    }

}
