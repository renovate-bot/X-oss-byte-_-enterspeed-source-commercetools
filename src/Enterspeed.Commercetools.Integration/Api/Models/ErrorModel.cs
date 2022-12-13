using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterspeed.Commercetools.Integration.Api.Models
{
    public class ErrorModel
    {
        public string? Id { get; set; }
        public Exception? Exception { get; set; }
    }
}
