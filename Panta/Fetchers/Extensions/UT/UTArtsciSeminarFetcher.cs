﻿using Panta.DataModels.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panta.Fetchers.Extensions.UT
{
    public class UTArtsciSeminarFetcher : IItemFetcher<UTCourse>
    {
        public static string[] Addresses = WebUrlConstants.ArtsciSeminars;
        public IEnumerable<UTCourse> FetchItems()
        {
            IEnumerable<UTCourse> courseInfo = new UTArtsciFirstYearSeminarInfoFetcher().FetchItems();
            List<UTCourse> courseDetails = new List<UTCourse>();

            Parallel.ForEach<string>(Addresses, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, delegate(string address)
            //foreach(var address in Addresses)
            {
                courseDetails.AddRange(new UTArtsciFirstYearSeminarDetailFetcher(address).FetchItems());
            }
            );

            return courseInfo.GroupJoin(courseDetails,
                (x => x.Abbr + x.Sections[0].Name),
                (x => x.Abbr + x.Corequisites),
                ((x, y) => this.CombineInfoDetail(x, y.FirstOrDefault())));
        }


        private UTCourse CombineInfoDetail(UTCourse info, UTCourse detail)
        {
            if (detail != null)
            {
                info.Description = detail.Description;
                info.BreadthRequirement = detail.BreadthRequirement;
            }
            return info;
        }
    }
}
