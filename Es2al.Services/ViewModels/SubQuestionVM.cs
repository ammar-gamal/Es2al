using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es2al.Services.ViewModels
{
    public class SubQuestionVM
    {
        [MinLength(2, ErrorMessage = "Minimum Length For Text Is 2")]
        public string Text { get; set; }
        public bool IsAnonymous { get; set; }
        public int ThreadId { get; set; }
        public int ParentQuestionId { get; set; }
        public int  ReceiverId{ get; set; }
    }
}
