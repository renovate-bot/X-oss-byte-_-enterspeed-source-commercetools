using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterspeed.Commercetools.Integration.Api.Models
{
    public class ImportAllResultModel
    {
        public List<ErrorModel> Errors = new ();
        public bool IsSuccess => Errors.Count == 0;
    }
}
