using ChoNongSan.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Responses.TinDang
{
    public class ListPostByAccountIdVm
    {
        public PageResult<PostVmTongQuat> ListOfWeb { get; set; }
        public List<PostVmTongQuat> ListOfApp { get; set; }
    }
}