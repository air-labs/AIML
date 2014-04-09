using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfOrganizingNetworks
{
    class SelfOrganizingTaskV3 : SelfOrganizingTaskV2
    {

        public SelfOrganizingTaskV3()
        {
            NetworkWidth = 10;
            NetworkHeight = 10;
            LearningRadius = 3;
        }
        
    }
}
