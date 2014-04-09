using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfOrganizingNetworks
{
    class SelfOrganizingTaskV2 : SelfOrganizingTaskV1
    {

        public SelfOrganizingTaskV2()
        {
            NetworkWidth = 10;
            NetworkHeight = 10;
            LearningRadius = 1;
        }
        
    }
}
