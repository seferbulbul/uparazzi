using Quartz;
using Quartz.Impl;
using UpArazzi2.Tasks.Jobs;

namespace UpArazzi2.Tasks.Triggers
{
    public class HatirlatmaTrigger
    {
        public static void Baslat()
        {
            IScheduler t = StdSchedulerFactory.GetDefaultScheduler();

            if (!t.IsStarted)
            {
                t.Start();
            }

            IJobDetail gorev = JobBuilder.Create<HatirlatmaJob>().Build();

            ICronTrigger tetikleyici = (ICronTrigger)TriggerBuilder.Create().WithIdentity("HatirlatmaJob10", "null").WithCronSchedule("0 0 10 * * ? *").Build();

            t.ScheduleJob(gorev, tetikleyici);
        }

        public static void Baslat2()
        {
            IScheduler t = StdSchedulerFactory.GetDefaultScheduler();

            if (!t.IsStarted)
            {
                t.Start();
            }

            IJobDetail gorev = JobBuilder.Create<HatirlatmaJob>().Build();

            ICronTrigger tetikleyici = (ICronTrigger)TriggerBuilder.Create().WithIdentity("HatirlatmaJob19", "null").WithCronSchedule("0 0 19 * * ? *").Build();

            t.ScheduleJob(gorev, tetikleyici);
        }
    }
}