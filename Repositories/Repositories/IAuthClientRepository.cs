﻿using Business.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public interface IAuthClientRepository
    {
        Client FindClient(string clientId);
    }
}
