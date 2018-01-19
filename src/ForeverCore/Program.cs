using System;
using Microsoft.AspNetCore.Hosting;

namespace ForeverCore
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new ForeverApplication();
            return app.Execute(args);          
        }
    }
}
