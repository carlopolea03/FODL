using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FODLSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FODLSystem.Interface
{
    public class Globalnterface : IGlobalnterface
    {
        private readonly FODLSystemContext _context;
        public Globalnterface(FODLSystemContext context)
        {
            _context = context;
        }
        public async Task<string> NextNoSeries(string Module)
        {
            string LastNoSeries = "";
            var nSeries = await _context.NoSeries.FirstOrDefaultAsync(r=>r.Code == Module);
            if (nSeries != null)
            {
                var NoFromString = Regex.Match(nSeries.LastNoUsed, @"\d+").Value;
                var stringFromNo = Regex.Replace(nSeries.LastNoUsed, @"[0-9]", string.Empty);

                int new_last_no = int.Parse(NoFromString) + 1;
                LastNoSeries = stringFromNo + new_last_no.ToString().PadLeft(NoFromString.Length, '0');

            }


            return LastNoSeries;
        }
    }
}
