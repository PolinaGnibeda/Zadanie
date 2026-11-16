using HelpDesk.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Core.Services
{
    public class StatusTransitionValidator
    {
        public bool IsTransitionAllowed(Status from, Status to)
        {
            if (from == to) return false;

            return from switch
            {
                Status.New => to == Status.InProgress || to == Status.Cancelled,
                Status.InProgress => to == Status.Resolved || to == Status.Cancelled,
                Status.Resolved => to == Status.Closed,
                Status.Closed => false, // Из Closed никуда нельзя
                Status.Cancelled => false, // Из Cancelled никуда нельзя
                _ => false
            };
        }
    }
}
