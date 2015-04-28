using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chargify
{
    public interface IChargifyEntity
    {
        /// <summary>
        /// The Id of the entity
        /// </summary>
        int id { get; }
    }
}
